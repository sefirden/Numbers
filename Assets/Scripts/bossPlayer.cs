using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossPlayer : MonoBehaviour
{
    public GameObject boss;
    public GameObject zero;
    private Board board;
    private ui ui;
    private Vector3 startPosition, endPosition;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        ui = FindObjectOfType<ui>();

       // Vector2 posTop = ui.PlayerAnchor.transform.position;  // get the game object position
      //  Vector2 viewportPointTop = Camera.main.WorldToViewportPoint(posTop);  //convert game object position to VievportPoint

      //  boss.gameObject.GetComponent<RectTransform>().anchorMin = viewportPointTop;
       // boss.gameObject.GetComponent<RectTransform>().anchorMax = viewportPointTop;

        StartCoroutine(MoveToStart());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator MoveToStart()
    {
        float step;
        float moveTime = 0;
        float speed = board.width / 5f;

       // startPosition = new Vector3(Camera.main.orthographicSize + 1f, ui.PlayerAnchor.transform.position.y, 1f);
       // endPosition = new Vector3(Camera.main.transform.position.x + 1f, ui.PlayerAnchor.transform.position.y, 1f);
        step = (speed / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime;
        while (moveTime <= 1.0f)
        {
            moveTime += step;
            transform.position = Vector2.Lerp(startPosition, endPosition, moveTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = endPosition;
    }
}
