using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class buttonTest : MonoBehaviour
{
    public Button exitButton, playButton, tutButton;
    public AudioSource main;
    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(StartGame);
        tutButton.onClick.AddListener(ShowTut);
        exitButton.onClick.AddListener(ExitGame);
        main.Play();
    }

    void StartGame()
    {
        Debug.Log("pressed start");
        //SceneManager.LoadScene(sceneName: "mainBoard");
        SceneManager.LoadScene(sceneName: "charSelect");
    }

    void ShowTut()
    {
        Debug.Log("pressed show tutorial");
        SceneManager.LoadScene(sceneName: "tutPage");
    }

    void ExitGame()
    {
        Debug.Log("pressed exit");
        Application.Quit();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
