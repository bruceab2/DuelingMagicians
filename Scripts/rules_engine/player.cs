using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected int gold;
    protected List<Card> hand = new List<Card>();
    protected int cardsInHand = 0;
    protected int maxCards = 10; // might need a function to modify this if we implement Mulligan later
    protected bool isfinished; //ready to end turn
    public bool warningSent;
    //public Game board

    public bool isFinished()
    {
        return isfinished;
    }

    public int getGold()
    {
        return gold;
    }

    /*
     * will keep gold from going below zero if adding negative gold
     */
    public void addGold(int x)
    {
        gold = Mathf.Max(0, gold+x);
    }

    public void deductGold(int x)
    {
        gold = Mathf.Max(0, gold - x);
    }

    public int getCardsInHand()
    {
        return cardsInHand;
    }

    public List<Card> getHand()
    {
        return hand;
    }

    public int getMaxCards()
    {
        return maxCards;
    }

    /*
     * Adds that card to hand
     * Return false if not added
     */
    public bool addToHand(Card c)
    {
        // Need to handle errors of beyond max number of cards
        if (cardsInHand == maxCards)
        {
            return false;
        }
        hand.Add(c);
        cardsInHand++;
        return true;
    }

    public void removeFromHand(int index)
    {
        hand.RemoveAt(index);
        cardsInHand--;
    }

    /*
     * places card from hand into boards action slot
     * 
     */
    public void playCard(Card c)
    {

    }
    
    

    /*
     * Sells card for gold
     * it will round down
     */
     public void sellCard (int index)
    {
        gold += (int)(hand[index].getCost()*Constants.cardRate);
        removeFromHand(index);
    }

    /*
     * ends that players turn by setting isFinished to true
     */
     public void endTurn()
    {
        isfinished = true;
    }

    /*
     * 
     */
     public void beginTurn()
    {
        isfinished = false;
    }

    
}

public class Magician : Player
{
    private int rose;

    public int getRoses()
    {
        return rose;
    }

    /*
     * will prevent roses from going below 0 if adding negative
     */
    public void addRoses(int x)
    {
        rose = Mathf.Max(0, rose+x);
    }

    /*
     * Exchanges roses for gold
     */
    public void exchangeRoses(int r)
    {
        gold += r * Constants.roseRate;
        rose -= r;
    }
}
