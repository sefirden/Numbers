using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject TopAnchor;
    public GameObject ButtomAnchor;
    public GameObject PlayerAnchor;

    public float OneSize;




    // Start is called before the first frame update
    void Start()
    {
       
        board = FindObjectOfType<Board>();

        switch (board.width)
        {
            case 5:
                OneSize = 1.2f;
                break;
            case 6:
                OneSize = 1.33f;
                break;
            case 7:
                OneSize = 1.47f;
                break;
            case 8:
                OneSize = 1.61f;
                break;
            case 9:
                OneSize = 1.75f;
                break;
            default:
                OneSize = board.width + board.width / 5f;
                break;
        }

        TopAnchor.transform.position = new Vector3(Camera.main.transform.position.x, board.width -1f + OneSize, TopAnchor.transform.position.z);

        ButtomAnchor.transform.position = new Vector3(Camera.main.transform.position.x, - OneSize, ButtomAnchor.transform.position.z);

        PlayerAnchor.transform.position = new Vector3(Camera.main.transform.position.x, board.width + OneSize - 1f + OneSize, PlayerAnchor.transform.position.z);


        Vector2 posTop = TopAnchor.transform.position;  // get the game object position
        Vector2 viewportPointTop = Camera.main.WorldToViewportPoint(posTop);  //convert game object position to VievportPoint     

        Vector2 posButtom = ButtomAnchor.transform.position;  // get the game object position
        Vector2 viewportPointButtom = Camera.main.WorldToViewportPoint(posButtom);  //convert game object position to VievportPoint

        HintButton.gameObject.GetComponent<RectTransform>().anchorMin = viewportPointButtom;
        HintButton.gameObject.GetComponent<RectTransform>().anchorMax = viewportPointButtom;

        AdHintButton.gameObject.GetComponent<RectTransform>().anchorMin = viewportPointButtom;
        AdHintButton.gameObject.GetComponent<RectTransform>().anchorMax = viewportPointButtom;

        Pause.gameObject.GetComponent<RectTransform>().anchorMin = viewportPointButtom;
        Pause.gameObject.GetComponent<RectTransform>().anchorMax = viewportPointButtom;

        RefillButton.gameObject.GetComponent<RectTransform>().anchorMin = viewportPointButtom;
        RefillButton.gameObject.GetComponent<RectTransform>().anchorMax = viewportPointButtom;

        AdRefillButton.gameObject.GetComponent<RectTransform>().anchorMin = viewportPointButtom;
        AdRefillButton.gameObject.GetComponent<RectTransform>().anchorMax = viewportPointButtom;

        scoreText.rectTransform.anchorMin = viewportPointTop;
        scoreText.rectTransform.anchorMax = viewportPointTop;

        HighscoreText.rectTransform.anchorMin = viewportPointTop;
        HighscoreText.rectTransform.anchorMax = viewportPointTop;

        time.rectTransform.anchorMin = viewportPointTop;
        time.rectTransform.anchorMax = viewportPointTop;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
