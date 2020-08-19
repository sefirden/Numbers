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

    internal void ChangeLevel(int level)
    {
        throw new NotImplementedException();
    }
}
