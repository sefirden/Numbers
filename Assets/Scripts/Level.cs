using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject[] Levels;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void LoadLevel(int level)
    {
        for (int i = 0; i < Levels.Length; i++)
        {
            if(i == level)
            {
                Levels[i].SetActive(true);
            }
            else
            {
                Levels[i].SetActive(false);
            }
        }
    }

    public void ChangeLevel(int level)
    {
        Levels[level].SetActive(true);
        
        StartCoroutine(MoveNewLevel(level));
        StartCoroutine(MoveOldLevel(level));
    }

    private IEnumerator MoveNewLevel(int level)
    {
        Vector3 startPositionNewLevel = new Vector3(14f, Levels[level].transform.position.y, Levels[level].transform.position.z);
        Vector3 endPositionNewLevel = new Vector3(0f, Levels[level].transform.position.y, Levels[level].transform.position.z);

        float step;
        float moveTime = 0;
        float speed = 0.66f;


        step = (speed / (startPositionNewLevel - endPositionNewLevel).magnitude) * Time.fixedDeltaTime;
        while (moveTime <= 1.0f)
        {
            moveTime += step;
            Levels[level].transform.position = Vector3.Lerp(startPositionNewLevel, endPositionNewLevel, moveTime);
            yield return new WaitForFixedUpdate();
        }
        Levels[level].transform.position = endPositionNewLevel;
    }

    private IEnumerator MoveOldLevel(int level)
    {
        level--;
        Vector3 startPositionNewLevel = new Vector3(0, Levels[level].transform.position.y, Levels[level].transform.position.z);
        Vector3 endPositionNewLevel = new Vector3(-14f, Levels[level].transform.position.y, Levels[level].transform.position.z);

        float step;
        float moveTime = 0;
        float speed = 0.66f;


        step = (speed / (startPositionNewLevel - endPositionNewLevel).magnitude) * Time.fixedDeltaTime;
        while (moveTime <= 1.0f)
        {
            moveTime += step;
            Levels[level].transform.position = Vector3.Lerp(startPositionNewLevel, endPositionNewLevel, moveTime);
            yield return new WaitForFixedUpdate();
        }
        Levels[level].transform.position = endPositionNewLevel;
        Levels[level].SetActive(false);
    }
}
