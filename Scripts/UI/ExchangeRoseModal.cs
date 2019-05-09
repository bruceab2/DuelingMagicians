using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeRoseModal : MonoBehaviour
{
    public Board game;
    public UIScript ui;
    public Text goldText;
    public InputField input;
    public Button exchangeRoses, plus, minus, cancel, submit;


    // HELPERS

    /*
     * Converts the roses shown into gold
     */
    public int convertRoses()
    {

        // get value from input string
        int ret = int.Parse(input.text);

        // convert it
        return ret * Constants.roseRate;
    }

    public void placeInScreen()
    {
        transform.localPosition = new Vector2(0, 0);
    }

    public void placeOutOfScreen()
    {
        transform.localPosition = new Vector2(1700, 0);
    }

    /*
     * Updates the UI
     */
    public void updateGoldOutput()
    {
        goldText.text = convertRoses().ToString();

    }


    // LISTENERS

    /*
     * added to the plus button adds 1 rose to the 
     */
    public void addListener()
    {
        input.text = (int.Parse(input.text) < ((Magician)game.players[0]).getRoses()) ? (int.Parse(input.text) + 1).ToString() : "0";
        updateGoldOutput();
    }

    public void minusListener()
    {
        input.text = (int.Parse(input.text ) > 0 ) ? (int.Parse(input.text) - 1).ToString() : "0";
        updateGoldOutput();
    }

    public void submitListener()
    {


        ((Magician)game.players[0]).exchangeRoses(int.Parse(input.text));
        ui.updateGameState();

        placeOutOfScreen();
    }

    public void cancelListner()
    {
        placeOutOfScreen();
    }

    public void initExchangeListener()
    {
        placeInScreen();
    }

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Board>();
        ui = FindObjectOfType<UIScript>();

        
        


        plus.onClick.AddListener(addListener);
        minus.onClick.AddListener(minusListener);
        submit.onClick.AddListener(submitListener);
        cancel.onClick.AddListener(cancelListner);
        exchangeRoses.onClick.AddListener(initExchangeListener);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
