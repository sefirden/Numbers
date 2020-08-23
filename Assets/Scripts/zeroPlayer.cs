using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zeroPlayer : MonoBehaviour
{
    public GameObject zero;
    public GameObject boss;

    private ui ui;
    private Vector3 startPosition, endPosition;

    // Start is called before the first frame update

    void Start()
    {
        startPosition = transform.position;
        endPosition = new Vector3(2f, 12.5f, transform.position.z);
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
        float speed = 1;


        step = (speed / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime;
        while (moveTime <= 1.0f)
        {
            moveTime += step;
            transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = endPosition;
    }
}
