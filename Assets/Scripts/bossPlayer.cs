using System;
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
    public Sprite[] Sprite;
    public Animation[] Animation;

    // Start is called before the first frame update
    void Awake()
    {
        ui = FindObjectOfType<ui>();

        if (PlayerResource.Instance.isLoaded == false)
        {
            ui.LifeBarBackground.SetActive(false);
            StartCoroutine(MoveToStart());
        }
        else if (PlayerResource.Instance.isLoaded == true)
        {
            transform.position = new Vector3(4.92f, 13f, transform.position.z);
            ui.LifeBarBackground.SetActive(true);
            PlayerResource.Instance.bossMove = false;
        }
    }

    public void ChangeBoss(int level)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Sprite[level];
    }

    public IEnumerator MoveToStart()
    {
        startPosition = new Vector3(12.59f, 13f, transform.position.z);
        endPosition = new Vector3(4.92f, 13f, transform.position.z);

        PlayerResource.Instance.bossMove = true;
        float step;
        float moveTime = 0;
        float speed = 1f;


        step = (speed / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime;
        while (moveTime <= 1.0f)
        {
            moveTime += step;
            transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = endPosition;
        PlayerResource.Instance.bossMove = false;
        ui.LifeBarBackground.SetActive(true);
    }
}
