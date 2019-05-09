using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIScript : MonoBehaviour
{
    public Button sellButton, endTurnButton, exitButton, exchangeButton, closeButton;
    public GameObject pausePanel;
    public GameObject audience, mag1, mag2, msgModal;
    public Text goldText, roseText, turnText, playerText, msgText, leftPlayer, rightPlayer;
    public bool sell = false;
    public GameObject cardPlaceholder;
    public Transform activeSlots;
    public Transform handArea;
    public Board game;
    public Dictionary<int,Vector2[]> posDict = new Dictionary<int, Vector2[]>();


    public void showTurn(int turn, string phase)
    {
        turnText.text = "Turn " + turn + ": " + phase;
    }

    public void showPlaying()
    {
        switch (game.getActivePlayer())
        {
            case 0:
                playerText.text = "Player: Magician 1";
                leftPlayer.text = "Magician 2";
                rightPlayer.text = "Audience";
                break;

            case 1:
                playerText.text = "Player: Magician 2";
                leftPlayer.text = "Audience";
                rightPlayer.text = "Magician 1";
                break;
            case 2:
                playerText.text = "Player: Audience";
                leftPlayer.text = "Magician 1";
                rightPlayer.text = "Magician 2";
                break;
            default:
                Debug.Log("no player");
                break;
        }
    }

    public IEnumerator displayMessage(string msg)
    {
        msgModal.transform.localPosition = new Vector2(0, 0);
        msgText.text = msg;
        yield return new WaitForSeconds(5);
    }


    public void closeListener()
    {
        msgModal.transform.localPosition = new Vector2(1700, 0);
    }

    /*
     * update UI Functions
     */
    public void updateGameState()
    {
        // Get all of the texts
        Text mag1Gold, mag1Rose, mag2Gold, mag2Rose, audGold;
        mag1Gold = mag1.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>();
        mag1Rose = mag1.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>();
        mag2Gold = mag2.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>();
        mag2Rose = mag2.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>();
        audGold = audience.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>();

        // game not ready yet bug
        if (!game)
        {
            game = FindObjectOfType<Board>();
        }
        // Get new values
        // Gold
        var golds = game.getGameStateGold();
        var roses = game.getGameStateRose();

        // Update all of the UI
        mag1Gold.text = "Gold: " + golds.Item1.ToString();
        mag1Rose.text = "Rose: " + roses.Item1.ToString();

        mag2Gold.text = "Gold: " + golds.Item2.ToString();
        mag2Rose.text = "Rose: " + roses.Item2.ToString();

        audGold.text = "Gold: " + golds.Item3.ToString();

    }
    
    //move stuff based on active player
    //0,1,2 for mag1,mag2,audience
    public void drawActivePlayer(int player)
    {
        // Get all of the texts
        RectTransform mag1Gold, mag1Rose, mag2Gold, mag2Rose, audGold;
        mag1Gold = (RectTransform)(mag1.transform.GetChild(0).gameObject.transform);
        mag1Rose = (RectTransform)(mag1.transform.GetChild(1).gameObject.transform);
        mag2Gold = (RectTransform)(mag2.transform.GetChild(0).gameObject.transform);
        mag2Rose = (RectTransform)(mag2.transform.GetChild(1).gameObject.transform);
        audGold = (RectTransform)(audience.transform.GetChild(0).gameObject.transform);

        //Debug.Log(mag1Gold);
        switch (player)
        {
            case 0:
                mag1Gold.anchorMin = posDict[0][0];
                mag1Gold.anchorMax = posDict[0][1];
                mag2Gold.anchorMin = posDict[1][0];
                mag2Gold.anchorMax = posDict[1][1];
                audGold.anchorMin = posDict[2][0];
                audGold.anchorMax = posDict[2][1];

                mag1Rose.anchorMin = posDict[3][0];
                mag1Rose.anchorMax = posDict[3][1];
                mag2Rose.anchorMin = posDict[4][0];
                mag2Rose.anchorMax = posDict[4][1];
                Debug.Log("0");

                sellButton.gameObject.SetActive(true);
                exchangeButton.gameObject.SetActive(true);
                break;
            case 1:
                mag1Gold.anchorMin = posDict[2][0];
                mag1Gold.anchorMax = posDict[2][1];
                mag2Gold.anchorMin = posDict[0][0];
                mag2Gold.anchorMax = posDict[0][1];
                audGold.anchorMin = posDict[1][0];
                audGold.anchorMax = posDict[1][1];

                mag1Rose.anchorMin = posDict[5][0];
                mag1Rose.anchorMax = posDict[5][1];
                mag2Rose.anchorMin = posDict[3][0];
                mag2Rose.anchorMax = posDict[3][1];
                Debug.Log("1");

                sellButton.gameObject.SetActive(true);
                exchangeButton.gameObject.SetActive(true);
                break;
            case 2:
                mag1Gold.anchorMin = posDict[1][0];
                mag1Gold.anchorMax = posDict[1][1];
                mag2Gold.anchorMin = posDict[2][0];
                mag2Gold.anchorMax = posDict[2][1];
                audGold.anchorMin = posDict[0][0];
                audGold.anchorMax = posDict[0][1];

                mag1Rose.anchorMin = posDict[4][0];
                mag1Rose.anchorMax = posDict[4][1];
                mag2Rose.anchorMin = posDict[5][0];
                mag2Rose.anchorMax = posDict[5][1];
                Debug.Log("2");

                sellButton.gameObject.SetActive(false);
                exchangeButton.gameObject.SetActive(false);
                break;
            default:
                Debug.LogError("invalid argument to drawActivePlayer");
                break;
        }
    }


        // Start is called before the first frame update
        void Start()
    {
        game = FindObjectOfType<Board>();
        sellButton.onClick.AddListener(sellToggle);
        exitButton.onClick.AddListener(exitGame);
        endTurnButton.onClick.AddListener(endOnclick);
        closeButton.onClick.AddListener(closeListener);

        posDict.Add(0,new Vector2[2] { new Vector2(0.05f, 0.27f), new Vector2(0.15f, 0.35f) });
        posDict.Add(1,new Vector2[2] {new Vector2(0.05f, 0.87f), new Vector2(0.15f, 0.95f)});
        posDict.Add(2,new Vector2[2] {new Vector2(0.85f, 0.87f), new Vector2(0.95f, 0.95f)});
        posDict.Add(3,new Vector2[2] {new Vector2(0.85f, 0.27f), new Vector2(0.95f, 0.35f)});
        posDict.Add(4,new Vector2[2] {new Vector2(0.05f, 0.77f), new Vector2(0.15f, 0.85f)});
        posDict.Add(5,new Vector2[2] {new Vector2(0.85f, 0.77f), new Vector2(0.95f, 0.85f)});
    }

    // Update is called once per frame
    void Update()
    {
        showPlaying();
        updateGameState();
        if (Input.GetKeyDown("escape"))
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            drawActivePlayer(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            drawActivePlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            drawActivePlayer(2);
        }
    }

    // LISTENERS

    /*
     * end turn listener
     */
    public void endOnclick()
    {
        game.endTurn();

        // Destroy active cards
        foreach (Transform child in activeSlots)
        {
            Destroy(child.gameObject);
        }
        

    }

    

    void exitGame()
    {
        SceneManager.LoadScene(sceneName: "mainMenu");
    }

    void sellCardOnclick(int index)
    {
        game.players[(int)Constants.player.magician1].sellCard(index);
        updateGameState();
        displayHand(game.players[(int)Constants.player.magician1].getHand());
        foreach (Transform child in handArea)
        {
            child.gameObject.GetComponent<Button>().onClick.AddListener(() => sellCardOnclick(int.Parse(child.gameObject.name)));
        }

    }

    void sellToggle()
    {
        if (sell)
        {
            sell = false;
            sellButton.GetComponentInChildren<Text>().text = "Sell";

            // Remove listener on cards
            foreach (Transform child in handArea)
            {
                child.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
        else
        {
            sell = true;
            sellButton.GetComponentInChildren<Text>().text = "Stop Sell";
            Component[] buttons = handArea.GetComponentsInChildren<Button>();

            foreach (Button b in buttons)
            {
                Debug.Log(b.gameObject.name);
                b.onClick.AddListener(() => sellCardOnclick(int.Parse(b.gameObject.name)));
            }

            
            
        }
    }
    

    void updateGold(int gold)
    {
        goldText.text = "Gold: " + gold;
    }

    void updateRoses(int roses)
    {
        roseText.text = "Rose: " + roses;
    }

    // Displays active card slot
    public void displayActive(List<Card> hand)
    {
        if (hand.Count == 0)
        {
            return;
        }

        

        foreach (Transform child in activeSlots)
        {
            Destroy(child.gameObject);
        }
        int cards = 0;
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] is Card)
            {
                //Debug.Log(i);
                cards++;
            }
        }
        double totalWidth = (double)(cards) * 0.09 + 0.1;
        int right = 180;
        int offset = Mathf.Min((int)1150 / hand.Count, 150);
        int startingX = (-offset / 2) + offset * (hand.Count / 2);
        if (hand.Count % 2 == 1)
        {
            startingX += (int)(offset * 0.5);
        }
        //Debug.Log("Actual hand size: " + hand.Count);
        int realCard = 0;

        // find 

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] is Card)
            {
                GameObject newCard = Instantiate(cardPlaceholder, activeSlots);
                Debug.Log(newCard.transform.GetChild(0).GetComponent<CardDragHandler>());
                newCard.transform.GetChild(0).GetComponent<CardDragHandler>().enabled = false;
                // setting first position
                newCard.transform.localPosition = new Vector2(startingX - i * offset, 0);

                newCard.name = i.ToString();

                // Get objects to fill in (title, description, gold, type)
                GameObject title, desc, gold, type;
                title = newCard.transform.GetChild(0).transform.GetChild(3).gameObject;
                desc = newCard.transform.GetChild(0).transform.GetChild(4).gameObject;
                gold = newCard.transform.GetChild(0).transform.GetChild(5).gameObject.transform.GetChild(0).gameObject;
                type = newCard.transform.GetChild(0).transform.GetChild(6).gameObject;

                title.GetComponent<Text>().text = hand[i].getName();
                desc.GetComponent<Text>().text = hand[i].getDescription();
                gold.GetComponent<Text>().text = hand[i].getCost().ToString();
                type.GetComponent<Text>().text = hand[i].getStringType();

                realCard++;
            }
        }
    }


    // Displays that players hand
    public void displayHand(List<Card> hand)
    {
        
        foreach (Transform child in handArea)
        {
            Destroy(child.gameObject);
        }
        if (hand.Count == 0)
        {
            return;
        }
        int cards = 0;
        for(int i = 0; i < hand.Count; i++)
        {
            if(hand[i] is Card)
            {
                //Debug.Log(i);
                cards++;
            }
        }
        double totalWidth = (double)(cards) * 0.09 + 0.1;
        int right = 180;
        int offset = Mathf.Min((int)1150/hand.Count, 150);
        int startingX = (-offset/2) + offset * (hand.Count/2);
        if(hand.Count % 2 == 1)
        {
            startingX += (int)(offset * 0.5);
        }
        //Debug.Log("Actual hand size: " + hand.Count);
        int realCard = 0;

        // find 
        
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] is Card)
            {
                GameObject newCard = Instantiate(cardPlaceholder, handArea);

                // setting first position
                newCard.transform.localPosition = new Vector2(startingX - i * offset, 0);

                
                

                newCard.name = i.ToString();

                // Get objects to fill in (title, description, gold, type)
                GameObject title, desc, gold, type;
                title = newCard.transform.GetChild(0).transform.GetChild(3).gameObject;
                desc = newCard.transform.GetChild(0).transform.GetChild(4).gameObject;
                gold = newCard.transform.GetChild(0).transform.GetChild(5).gameObject.transform.GetChild(0).gameObject;
                type = newCard.transform.GetChild(0).transform.GetChild(6).gameObject;

                title.GetComponent<Text>().text = hand[i].getName();
                desc.GetComponent<Text>().text = hand[i].getDescription();
                gold.GetComponent<Text>().text = hand[i].getCost().ToString();
                type.GetComponent<Text>().text = hand[i].getStringType();

                realCard++;
            }
        }
    }   
}