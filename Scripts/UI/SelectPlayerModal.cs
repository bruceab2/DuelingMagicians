using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPlayerModal : MonoBehaviour
{
    public Board game;
    public UIScript ui;
    public Button Magician1Button, Magician2Button;
    public Text messageText;


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

    public void Magician1Listener()
    {
        game.selectedPlayer = (int)Constants.player.magician1;

        placeOutOfScreen();

    }

    public void Magician2Listener()
    {
        game.selectedPlayer = (int)Constants.player.magician2;

        placeOutOfScreen();
    }



    public void init(string msg)
    {
        messageText.text = msg;
        placeInScreen();
    }

    public IEnumerator WaitForPlayerSelected()
    {
        while (game.selectedPlayer == -1)
        {
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Board>();
        ui = FindObjectOfType<UIScript>();

        Magician1Button.onClick.AddListener(Magician1Listener);
        Magician2Button.onClick.AddListener(Magician2Listener);


    }

    // Update is called once per frame
    void Update()
    {



    }
}
