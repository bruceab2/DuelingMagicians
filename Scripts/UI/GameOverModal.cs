using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverModal : MonoBehaviour
{
    public Board game;
    public UIScript ui;
    public Text msg;
    public Button replay, quit, mainMenu;
    private bool started = false;

    // HELPERS

    

    public void placeInScreen()
    {
        transform.localPosition = new Vector2(0, 0);
    }

    public void placeOutOfScreen()
    {
        transform.localPosition = new Vector2(1700, -50);
    }

    /*
     * Updates the UI
     */
    


    // LISTENERS

    public void quitListener()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void replayListener()
    {
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    public void mainMenuListener()
    {
        SceneManager.LoadScene("mainMenu", LoadSceneMode.Additive);
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





        replay.onClick.AddListener(replayListener);
        quit.onClick.AddListener(quitListener);
        mainMenu.onClick.AddListener(mainMenuListener);
        

    }

    // Update is called once per frame
    void Update()
    {
        if (game.isDone && !started)
        {
            started = true;
            msg.text = game.gameIsOverMsg();
            init();
        }
    }
}
