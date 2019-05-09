using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharSelect : MonoBehaviour
{
    public Button magButton, audButton;
    public Image[] playerSel = new Image[3];
    private int playIdx = 0;
    private List<Image> mags = new List<Image>();
    private Image audpointer;
    private Color selColor = new Color(0.1012371f, 0.8584906f, 0.1272933f);
    private Color audcolor = new Color(1f, 0.09661572f,0f);
    public AudioSource mainMusic;

    // Start is called before the first frame update
    void Start()
    {
        magButton.onClick.AddListener(addMag);
        audButton.onClick.AddListener(addAud);
        Debug.Log("wat");
        mainMusic.Play();
    }
    
    void addMag()
    {
        Debug.Log("selected magician for player " + (playIdx + 1));
        mags.Add(playerSel[playIdx]);
        playerSel[playIdx].color = selColor;
        playIdx++;
        if (playIdx > 2)
        {
            SceneManager.LoadScene(sceneName: "mainBoard");
        }
    }
    void addAud()
    {
        Debug.Log("selected audience for player " + (playIdx + 1));
        audpointer = playerSel[playIdx];
        playerSel[playIdx].color = audcolor;
        playIdx++;
        if (playIdx > 2)
        {
            SceneManager.LoadScene(sceneName: "mainBoard");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
