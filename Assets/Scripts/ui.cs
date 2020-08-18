using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ui : MonoBehaviour
{

    private Board board;

    public Text scoreText;
    public Text HighscoreText;
    public Text time;
    public Text hintcount;
    public Text refillcount;
    public Text refillcountLayer;
    public Text Adrefillcount;
    public Text AdrefillcountLayer;

    public GameObject EndGameLayer;
    public GameObject NoMatchLayer;

    public Button HintButton;
    public Button AdHintButton;
    public Button Pause;
    public Button AdRefillButton;
    public Button AdRefillButtonLayer;
    public Button RefillButton;
    public Button RefillButtonLayer;

    public GameObject LifeBar;
    public Text lifeText;

   // private float fillHealth;

    // Start is called before the first frame update
    void Awake()
    {
        LifeBar.transform.position = new Vector3(transform.position.x, 10.5f, transform.position.z);

    }

    // Update is called once per frame
    public void BossHealth(int score, int level)
    {
        int scoreToNextLevel = 0;

        for (int j = 0; j < level; j++)
        {
            scoreToNextLevel += PlayerResource.Instance.scoreToNextLevel[j];
        }

        if (PlayerResource.Instance.gameMode == "normal" && level == 0)
        {
            LifeBar.GetComponent<Image>().fillAmount = 1f - (float)score / (float)PlayerResource.Instance.scoreToNextLevel[level];
        }
        else if (PlayerResource.Instance.gameMode == "timetrial" && level == 0)
        {
            LifeBar.GetComponent<Image>().fillAmount = 1f - (float)score / (float)PlayerResource.Instance.scoreToNextLevel[level];
        }
        else if (PlayerResource.Instance.gameMode == "normal")
        {
            LifeBar.GetComponent<Image>().fillAmount = 1f - (float)(score - scoreToNextLevel) / (float)PlayerResource.Instance.scoreToNextLevel[level];
        }
        else if (PlayerResource.Instance.gameMode == "timetrial")
        {
            LifeBar.GetComponent<Image>().fillAmount = 1f - (float)(score - scoreToNextLevel) / (float)PlayerResource.Instance.scoreToNextLevel[level];
        }

        //lifeText.text = Convert.ToString(PlayerResource.Instance.scoreToNextLevel[level]);
    }
}
