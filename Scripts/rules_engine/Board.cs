using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Threading;

static class Constants
{
    // Enums to make it uniformly accross all cards
    public enum Types:int { sleight, favor, trick, sabatoge, audience }
    public enum Mods:int { gold, rose, hand }
    public enum Target:int { self, magician, audience, coinflip, magicians, trick } // coinflip means coinflip for random magician,  magicians means select target magician
    public enum Counter:int { sleight, favor, trick, sabatoge, audience }

    // For index of each player
    public enum player:int { magician1, magician2, audience }

    // in game constants
    public const int audienceGold = 20, magicianGold = 10, roseRate = 5, startingHand = 6, magicianStartingGold = 100;
    public const double cardRate = .25;
}

public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}

public class Board : MonoBehaviour
{
    public Player[] players; // three players, initialized in Start()
    private int audienceIdx = (int)Constants.player.audience;
    private int magician1Idx = (int)Constants.player.magician1;
    private int magician2Idx = (int)Constants.player.magician2;
    public AudioSource inGame;
    private bool isAudienceTurn = false;
    private List<Card>[] deck;
    private List<Card>[] activeCard; // This is the slots where user drags card to play
    private int currPlayerTurn = 0; // Start with magician1
    private static System.Random rand = new System.Random();
    public int winner = -1;
    public bool isDone = false;
    public Forfeit ff;
    public int selectedPlayer = -1;
    public SelectPlayerModal sel;
    private TextAsset cardInv;

    private int trickModCost = 0;



    public UIScript uiscr; // To access the UI


    enum Phases { Draw, Gold, Main, Cost, Effect, End }

    /*
     * 
     */
    public void reduceCounter(Card c, List<int> typesPlayed)
    {
        for (int i = 0; i < typesPlayed.Count; i++)
        {
            if (typesPlayed[i] == c.getCounterReduction())
            {
                c.reduceCounter();
            }
        }
    }

    /*
     * Allowing respective player to place card in active slots on board
     */
    public void addToActiveCard(Card c, int player, int index)
    {
        activeCard[player].Add(c);
        players[player].removeFromHand(index);
    }
    
    /*
     * Removing a card from the active slot by card name
     * might need to implement way to find that card when button is clicked to remove
     */
    public void removeFromActive(int player, Card c)
    {
        activeCard[player].Remove(c);
        players[player].addToHand(c);
    }

    /*
     * checks to see if player can afford with the card added to the active card list
     */
     public bool canAfford(Card c, int player)
    {
        int currCost = 0;
        foreach (Card currCard in activeCard[player])
        {
            currCost += currCard.getCost();
        }

        return (currCost + c.getCost()) <= players[player].getGold();
    }

    //PHASES

    /*
     * give player(s) card
     * add to hand from their respective deck
     * set isfinished to false
     */
    public void drawPhase()
    {
        if (isAudienceTurn)
        {
            players[audienceIdx].beginTurn();
            players[audienceIdx].addToHand(deck[audienceIdx].ElementAt(0));
            deck[audienceIdx].RemoveAt(0);
            uiscr.displayHand(players[(int)Constants.player.audience].getHand());
        }
        else
        {
            players[magician1Idx].beginTurn();
            players[magician2Idx].beginTurn();
            players[magician1Idx].addToHand(deck[magician1Idx].ElementAt(0));
            deck[magician1Idx].RemoveAt(0);
            players[magician2Idx].addToHand(deck[magician2Idx].ElementAt(0));
            deck[magician2Idx].RemoveAt(0);
            uiscr.displayHand(players[magician1Idx].getHand());

        }
        
        

    }

    /*
     * adds gold to the player 
     */
    public void goldPhase()
    {
        if (isAudienceTurn)
        {
            players[audienceIdx].addGold(Constants.audienceGold);
        }
        else
        {
            players[magician1Idx].addGold(Constants.magicianGold);
            players[magician2Idx].addGold(Constants.magicianGold);
        }

        uiscr.updateGameState();
    }

    /*
     * Player will have control of what they are doing
     * just check for when that the appropriate players' turn is done
     */
    IEnumerator mainPhase()
    {
        // Must play at least one card
        if (isAudienceTurn)
        {
            while (!players[audienceIdx].isFinished())
            {
                // audience do things
                yield return null;

            }
        }
        else
        {
            while (!players[magician1Idx].isFinished() || !players[magician2Idx].isFinished())
            {
                // magicians do things
                yield return null;
            }
            if (activeCard[magician1Idx].Count == 0 || activeCard[magician2Idx].Count == 0)
            {
                winner = audienceIdx;
            }
        }
    }

    /*
     * Cost phase this is where all the cards in action get deducted from their wallet
     */
    public void costPhase()
    {
        if (isAudienceTurn)
        {
            foreach (Card c in activeCard[audienceIdx])
            {
                players[audienceIdx].deductGold(c.getCost());
            }
        }
        else
        {
            foreach (Card c in activeCard[magician1Idx])
            {
                int cost = c.getCost();
                if (c.getType() == (int)Constants.Types.trick)
                {
                    cost += trickModCost;
                }
                players[magician1Idx].deductGold(cost);
            }
            foreach (Card c in activeCard[magician2Idx])
            {
                int cost = c.getCost();
                if (c.getType() == (int)Constants.Types.trick)
                {
                    cost += trickModCost;
                }
                players[magician2Idx].deductGold(cost);
            }
        }
    }

    /*
     * Effect phase this is where all the cards get their effects played
     * Keep temporary variables of all players to determine effect (gold gained, gold reduced, roses gained, roses reduced)
     * At end place all active cards in appropriate trash piles
     */
    public IEnumerator effectPhase()
    {
        // Save game state
        ChangeOfGameState[] stateOfPlayers = new ChangeOfGameState[3];
        for (int i = 0; i < stateOfPlayers.Length; i++)
        {
            stateOfPlayers[i] = new ChangeOfGameState();
        }

        List<int> typeHistory = new List<int>();

        // Iter through each active card then adding the effects of each card 
        if (isAudienceTurn)
        {
            foreach (Card c in activeCard[audienceIdx])
            {
                LinkedList<int> e = c.getEffect(), m = c.getModifies(), t = c.getTarget();
                LinkedListNode<int> headE = e.First, headM = m.First, headT = t.First;
                for (int i = 0; headE != null; i++)
                {
                    int target = -1;
                    if (c.getIsCoinFlip() && coinFlip() == 1)
                    {
                        headE = headE.Next;
                        headM = headM.Next;
                        headT = headT.Next;
                    }

                    //Find out who to target
                    switch (headT.Value)
                    {
                        case (int)Constants.Target.self:
                            target = audienceIdx;
                            break;
                        case (int)Constants.Target.magician:
                            // Select target magician

                            string msg = "Select Target Magician to lose " + headE.Value.ToString();
                            switch(headM.Value)
                            {
                                case (int)Constants.Mods.gold:
                                    msg += " Gold.";
                                    break;
                                case (int)Constants.Mods.rose:
                                    msg += " Roses.";
                                    break;
                            }

                            sel.init(msg);
                            while (selectedPlayer == -1)
                            {
                                yield return null;
                            }
                            
                            target = selectedPlayer;
                            selectedPlayer = -1;
                            break;
                        case (int)Constants.Target.audience:
                            target = audienceIdx;
                            break;
                        case (int)Constants.Target.coinflip:
                            target = coinFlip();
                            break;

                        case (int)Constants.Target.trick:
                            target = (int)Constants.Target.trick;
                            break;
                    }
                    // Make sure target is set to something on selected magician
                    Debug.Log("selected magician: " + target.ToString());

                    

                    switch (headM.Value)
                    {
                        case (int)Constants.Mods.gold:
                            if (target == (int)Constants.Target.trick)
                            {
                                trickModCost += headE.Value;
                                break;
                            }
                            if (headE.Value < 0)
                            {
                                stateOfPlayers[target].goldReduce += headE.Value;
                            }
                            else if (headE.Value > 0)
                            {
                                stateOfPlayers[target].goldIncrease += headE.Value;
                            }
                            break;

                        case (int)Constants.Mods.rose:
                            if (headE.Value < 0)
                            {

                                stateOfPlayers[target].roseReduce += headE.Value;
                            }
                            else if (headE.Value > 0)
                            {
                                stateOfPlayers[target].roseIncrease += headE.Value;
                            }
                            break;
                    }
                    headE = headE.Next;
                    headM = headM.Next;
                    headT = headT.Next;
                }
            }
        }
        else
        {
            foreach (Card c in activeCard[magician1Idx])
            {
                LinkedList<int> e = c.getEffect(), m = c.getModifies(), t = c.getTarget();
                bool steal = c.getIsStealing();
                for (int i = 0; i < e.Count; i++)
                {
                    int target = -1;

                    //Find out who to target
                    switch (t.First.Value)
                    {
                        case (int)Constants.Target.self:
                            target = magician1Idx;
                            break;
                        case (int)Constants.Target.magician:
                            target = magician2Idx;
                            break;
                        case (int)Constants.Target.audience:
                            target = audienceIdx;
                            break;
                    }



                    switch (m.First.Value)
                    {
                        case (int)Constants.Mods.gold:
                            if (e.First.Value < 0)
                            {
                                
                                 stateOfPlayers[target].goldReduce += e.First.Value;
                                
                            }
                            else if (e.First.Value > 0)
                            {
                                if (steal)
                                {
                                    int val = Math.Min(Math.Abs(e.First.Value), (players[target].getGold()));
                                    stateOfPlayers[target].goldReduce -= Math.Max(val, 0);
                                    stateOfPlayers[(int)Constants.Target.self].goldIncrease += Math.Max(val, 0);
                                }
                                else
                                {
                                    stateOfPlayers[target].goldIncrease += e.First.Value;
                                }
                                
                            }
                            break;

                        case (int)Constants.Mods.rose:
                            if (e.First.Value < 0)
                            {
                                stateOfPlayers[target].roseReduce += e.First.Value;
                            }
                            else if (e.First.Value > 0)
                            {
                                stateOfPlayers[target].roseIncrease += e.First.Value;
                            }
                            break;
                    }
                }
            }

            foreach (Card c in activeCard[magician2Idx])
            {
                LinkedList<int> e = c.getEffect(), m = c.getModifies(), t = c.getTarget();
                bool steal = c.getIsStealing();
                for (int i = 0; i < e.Count; i++)
                {
                    int target = -1;

                    //Find out who to target
                    switch (t.First.Value)
                    {
                        case (int)Constants.Target.self:
                            target = magician2Idx;
                            break;
                        case (int)Constants.Target.magician:
                            target = magician1Idx;
                            break;
                        case (int)Constants.Target.audience:
                            target = audienceIdx;
                            break;
                    }
                    switch (m.First.Value)
                    {
                        case (int)Constants.Mods.gold:
                            if (e.First.Value < 0)
                            {
                                
                                stateOfPlayers[target].goldReduce += e.First.Value;
                                


                            }
                            else if (e.First.Value > 0)
                            {
                                if (steal)
                                {
                                    int val = Math.Min(Math.Abs(e.First.Value), (players[target].getGold()));
                                    stateOfPlayers[target].goldReduce -= Math.Max(val, 0);
                                    stateOfPlayers[(int)Constants.Target.self].goldIncrease += Math.Max(val, 0);
                                }
                                else
                                {
                                    stateOfPlayers[target].goldIncrease += e.First.Value;
                                }

                            }
                            break;

                        case (int)Constants.Mods.rose:
                            if (e.First.Value < 0)
                            {
                                stateOfPlayers[target].roseReduce += e.First.Value;
                            }
                            else if (e.First.Value > 0)
                            {
                                stateOfPlayers[target].roseIncrease += e.First.Value;
                            }
                            break;
                    }
                }
            }
        }

        

        // updating game state
        for (int i = 0; i < players.Length; i++)
        {
            players[i].addGold(stateOfPlayers[i].goldReduce);
            players[i].addGold(stateOfPlayers[i].goldIncrease);
            if (i != audienceIdx)
            {
                ((Magician)players[i]).addRoses(stateOfPlayers[i].roseReduce);
                ((Magician)players[i]).addRoses(stateOfPlayers[i].roseIncrease);

            }

        }



        // Clearing out active cards
        for (int i = 0; i < activeCard.Length; i++)
        {
            List<Card> currActiveCardList = new List<Card>(activeCard[i]);
            foreach(Card c in currActiveCardList)
            {
                reduceCounter(c, typeHistory);

                if (c.getCounter() <= 0)
                {
                    removePrestiege(c);
                    activeCard[i].Remove(c);
                }
            }
        }

        // Need to update the ui
        uiscr.updateGameState();


    }

    public void removePrestiege(Card c)
    {
        LinkedList<int> e = c.getEffect(), t = c.getTarget(), m = c.getModifies();

        if (t.First.Value == (int)Constants.Target.trick)
        {
            trickModCost -= e.First.Value;
        }

    }

    /*
     * change the active turn
     */
    public void endPhase()
    {

    }

    /*
     * Initiates one full turn i.e. all of the phases in order
     */
    IEnumerator playGame()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                uiscr.showTurn(i + 1, "Draw");
                yield return new WaitForSeconds(1);
                drawPhase();
                uiscr.showTurn(i + 1, "Gold");
                yield return new WaitForSeconds(1);

                goldPhase();
                uiscr.showTurn(i + 1, "Main");
                yield return new WaitForSeconds(1);

                yield return mainPhase();
                if (winner == audienceIdx)
                {
                    gameWon();
                    yield break;
                }
                uiscr.showTurn(i + 1, "Cost");
                yield return new WaitForSeconds(1);

                costPhase();
                uiscr.showTurn(i + 1, "Effect");
                yield return new WaitForSeconds(1);

                yield return effectPhase();
                uiscr.showTurn(i + 1, "End");
                yield return new WaitForSeconds(1);

                endPhase();

                // Change turn bool
                isAudienceTurn = !isAudienceTurn;
                if (isAudienceTurn)
                {
                    uiscr.drawActivePlayer((int)Constants.player.audience);
                    uiscr.displayHand(players[(int)Constants.player.audience].getHand());
                    currPlayerTurn = 2;
                }
                else
                {
                    uiscr.drawActivePlayer((int)Constants.player.magician1);
                    uiscr.displayHand(players[(int)Constants.player.magician1].getHand());
                    currPlayerTurn = 0;
                }
            }
            

        }

        gameWon();
       
        
    }



    

    /*
     * reads in cards from csv file and creates card objects
     */
    public void readAllCards()
    {
        //var cardFile = Resources.Load<TextAsset>("Cards/Card Inventories.csv");
        // load into data structure

        // Multiply cards
        for (int j = 0; j < 3; j++)
        {


            for (int i = 0; i < 3; i++)
            {


                //using (var reader = new StreamReader(cardInv.text))
                    var lines = cardInv.text.Split('\n');
                Card c = null;
                foreach (var line in lines)
                {
                    //old card to keep track if we are finished with it
                    

                    //while (!reader.EndOfStream)
                    //{


                        if (string.Equals(line, "audience,aud,Rotten Tomatoes,30,,-8,pick a magician coinflip: heads = lose 8 roses tails = 16 roses,rose,magician,,0,,2,no,yes\r"))
                    {
                        int x = 0;
                        x++;
                    }
                        var values = line.Split(',');
                        int play = -1, type = -1, modifies = -1, target = -1;

                        // Player
                        if (string.Equals(values[0], "magician"))
                        {
                            play = (int)Constants.Target.magician;
                            if (i == 2)
                            {
                                c = null;
                                continue;
                            }
                        }
                        else if (string.Equals(values[0], "audience"))
                        {

                            if (i != 2)
                            {
                                c = null;
                                continue;
                            }

                            play = (int)Constants.Target.audience;
                        }

                        // Only do test cards
                        if (string.Equals(values[0], "end"))
                        {
                            //deck[i].Add(c);

                            break;
                        }
                        else if (string.Equals(values[0], "player"))
                        {
                            // skip over categories row
                            continue;
                        }

                        // Parsing all the values



                        //type
                        if (string.Equals(values[1], "favor"))
                        {
                            type = (int)Constants.Types.favor;
                        }
                        else if (string.Equals(values[1], "trick"))
                        {
                            type = (int)Constants.Types.trick;
                        }
                        else if (string.Equals(values[1], "sleight"))
                        {
                            type = (int)Constants.Types.sleight;
                        }

                        // Modifies
                        if (string.Equals(values[7], "gold"))
                        {
                            modifies = (int)Constants.Mods.gold;
                        }
                        else if (string.Equals(values[7], "rose"))
                        {
                            modifies = (int)Constants.Mods.rose;
                        }

                        // Target
                        if (string.Equals(values[8], "self"))
                        {
                            target = (int)Constants.Target.self;
                        }
                        else if (string.Equals(values[8], "audience"))
                        {
                            target = (int)Constants.Target.audience;
                        }
                        else if (string.Equals(values[8], "magician"))
                        {
                            target = (int)Constants.Target.magician;
                        }
                        else if (string.Equals(values[8], "coinflip"))
                        {
                            target = (int)Constants.Target.coinflip;
                        }
                        else if (string.Equals(values[8], "trick"))
                        {
                            target = (int)Constants.Target.trick;
                        }

                        //Checking if this is a new card
                        if (string.Equals(values[2], ""))
                        {
                            c.addRule(int.Parse(values[5]), modifies, target);
                            continue;
                        }
                        else
                        {
                            // skip first time
                            if (!Equals(c, null))
                            {
                                deck[i].Add(c);

                            }

                            c = null;
                        }
                        bool isSteal = string.Equals(values[13], "yes");

                        bool isCoinFlip = string.Equals(values[14], "yes") || string.Equals(values[14], "yes\r");




                        int tem_cost = Int32.Parse(values[3]);
                        int tem_eff = Int32.Parse(values[5]);

                        c = new Card(values[2], values[6], values[9], tem_cost, play, type, tem_eff, modifies, target, isSteal, isCoinFlip);

                        deck[i].Add(c);
                    //}
                }
                // checking to see what the vals are
                /*foreach (Card temp in deck[i])
                {
                    Debug.Log(temp.ToString());
                }*/
            }
        }
    }

    /*
     * initiates the coinflip
     * returns 0 for heads and 1 for tails
     */
    public int coinFlip()
    {
        Thread.Sleep(20);

        System.Random rnd = new System.Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        return rnd.Next(0, 2);
    }

    public void shuffleCards(List<Card> deck)
    {


        int n = deck.Count;
        Thread.Sleep(20);

        System.Random rnd = new System.Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            Card value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        cardInv = (TextAsset)Resources.Load("card", typeof(TextAsset));
        inGame.Play();
        uiscr = FindObjectOfType<UIScript>();
        // Set up the game
        players = new Player[3];
        players[magician1Idx] = new Magician();
        players[magician2Idx] = new Magician();
        players[audienceIdx] = new Player();

        // Starting gold audience begins with 0
        for (int i = 0; i < players.Length-1; i++)
        {
            players[i].addGold(Constants.magicianStartingGold);
        }

        deck = new List<Card>[3];
        
        activeCard = new List<Card>[3];
        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = new List<Card>();
            activeCard[i] = new List<Card>();
        }

        // start with magicians
        isAudienceTurn = false;

        // read assets to parse cards into deck
        readAllCards();

        foreach (int p in EnumUtil.GetValues<Constants.player>())
        {
        // Shuffle the list of parsed cards
            shuffleCards(deck[p]);
            Debug.Log(deck[p].Count);
            // Add cards to hand for each player

            for (int i = 0; i < Constants.startingHand; i++)
            {
                players[p].addToHand(deck[p].ElementAt(0));
                deck[p].RemoveAt(0);
            }
        }
        uiscr.displayHand(players[0].getHand());
        // while not end game
        StartCoroutine("playGame");
        


}

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * HELPER FUNCTIONS FOR UI
     */

    /*
     * display hand
     */
     public List<Card> getHand(int p)
    {
        return players[p].getHand();
    }

    /*
     * display active cards
     */
     public List<Card> getActiveCards(int p)
    {
        return activeCard[p];
    }

    /*
     * retrives gold for all players
     * returns in format (magician1, magician2, audience)
     */

     public (int, int, int) getGameStateGold()
    {
        var mag1Gold = players[(int)Constants.player.magician1].getGold();
        var mag2Gold = players[(int)Constants.player.magician2].getGold();
        var audGold = players[(int)Constants.player.audience].getGold();
        return (mag1Gold, mag2Gold, audGold);

    }

    public (int, int) getGameStateRose()
    {
        var mag1Rose = ((Magician)players[(int)Constants.player.magician1]).getRoses();
        var mag2Rose = ((Magician)players[(int)Constants.player.magician2]).getRoses();
        return (mag1Rose, mag2Rose);

    }

    public void endTurn()
    {
        if (activeCard[currPlayerTurn].Count == 0 && currPlayerTurn != 2)
        {
            ff.init();
        }
        else
        {
            players[currPlayerTurn].endTurn();
            if (currPlayerTurn == 0)
            {
                switchMagician();
            }
        }
        
    }

    public int getActivePlayer()
    {
        return currPlayerTurn;
    }

    public bool switchToNextPlayer()
    {
        if (currPlayerTurn == 2)
        {
            return players[(int)Constants.player.audience].isFinished();
        } 
        else
        {
            return (players[(int)Constants.player.magician1].isFinished() && players[(int)Constants.player.magician2].isFinished());

        }
    }

    public void switchMagician()
    {
        if (currPlayerTurn != 0 && currPlayerTurn != 1) {
            Debug.Log("wtf happened with switching");
            return;
        }
        currPlayerTurn = (currPlayerTurn == 0) ? currPlayerTurn = 1 : currPlayerTurn = 0;
        uiscr.displayHand(players[currPlayerTurn].getHand());
        uiscr.drawActivePlayer(currPlayerTurn);

    }

    public void gameWon()
    {
        // Determine winner
        if (winner != audienceIdx)
        {
            
            winner = (((Magician)players[magician1Idx]).getRoses() > ((Magician)players[magician2Idx]).getRoses()) ? magician1Idx : magician2Idx;
        }
        isDone = true;
        Debug.Log("player won: " + winner.ToString());

        //uiscr.displayMessage("player won: " + winner.ToString());
    }

    public string gameIsOverMsg()
    {
        return "player won: " + winner.ToString();
    }

    

    

    /*
     * retrives rose for all players
     * returns in format (magician1, magician2)
     */

    private class ChangeOfGameState
    {
        public int goldReduce = 0, goldIncrease = 0, roseIncrease = 0, roseReduce = 0; 
        
    }
}