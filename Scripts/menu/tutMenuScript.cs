using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class tutMenuScript : MonoBehaviour
{
    public Button menuButton;
    // Start is called before the first frame update
    void Start()
    {
        menuButton.onClick.AddListener(retMenu);
    }

    void retMenu()
    {
        Debug.Log("pressed back");
        SceneManager.LoadScene(sceneName: "mainMenu");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
