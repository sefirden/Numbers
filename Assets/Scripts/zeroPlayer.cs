using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zeroPlayer : MonoBehaviour
{
    public GameObject zero;
    private Board board;
    private Vector2 startPosition, endPosition;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        zero.transform.position = new Vector2(Camera.main.orthographicSize - (Camera.main.orthographicSize+1), board.height + 2);

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
        float speed = 0.66f;

        startPosition = zero.transform.position;
        endPosition = new Vector2((board.width / 2) - 1, zero.transform.position.y);
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
