using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Forfeit : MonoBehaviour
{
    public Board game;
    public UIScript ui;
    public Button YesButton, NoButton;

    // HELPERS



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



    // LISTENERS

    public void yesListener()
    {
        game.winner = 2;
        game.gameWon();

        placeOutOfScreen();

    }

    public void noListener()
    {
        placeOutOfScreen();
    }

    

    public void init()
    {
        placeInScreen();
    }

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Board>();
        ui = FindObjectOfType<UIScript>();

        YesButton.onClick.AddListener(yesListener);
        NoButton.onClick.AddListener(noListener);


    }

    // Update is called once per frame
    void Update()
    {
        
        
        
    }
}
