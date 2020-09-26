using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject[] Levels; //список уровней
    private bossPlayer boss; //скрипт босса
    public Board board; //объект поля

    private void Awake()
    {
        boss = FindObjectOfType<bossPlayer>(); ////присваиваем скрипт к переменной
        board = FindObjectOfType<Board>();
    }

    public void LoadLevel(int level) //загрузка уровня, перебираем все уровни
    {
        for (int i = 0; i < Levels.Length; i++)
        {
            if(i == level)
            {
                Levels[i].SetActive(true); //если нашли нужный включаем объект
            }
            else
            {
                Levels[i].SetActive(false); //остальные выключаем
            }
        }
    }

    public void ChangeLevel(int level) //смена уровня
    {
        boss.gameObject.SetActive(false); //выключаем босса

        Levels[level].SetActive(true); //включаем следующий уровень
        
        StartCoroutine(MoveNewLevel(level)); //двигаем новый уровень
        StartCoroutine(MoveOldLevel(level)); //двигаем старый уровень
    }

    public void StartNewGameLevel(int level)
    {
        Levels[level].SetActive(true); //включаем первый уровень

        StartCoroutine(MoveNewLevel(level)); //двигаем новый уровень
        StartCoroutine(MoveOldLevel(11)); //двигаем стартовый уровень

    }

    private IEnumerator MoveNewLevel(int level) 
    {
        //стандартная схема с движением, можно посмотреть в скрипте босса или ноля
        Vector3 startPositionNewLevel = new Vector3(14f, Levels[level].transform.position.y, Levels[level].transform.position.z);
        Vector3 endPositionNewLevel = new Vector3(0f, Levels[level].transform.position.y, Levels[level].transform.position.z);

        float step;
        float moveTime = 0;
        float speed = 1f;


        step = (speed / (startPositionNewLevel - endPositionNewLevel).magnitude) * Time.fixedDeltaTime;
        while (moveTime <= 1.0f)
        {
            moveTime += step;
            Levels[level].transform.position = Vector3.Lerp(startPositionNewLevel, endPositionNewLevel, moveTime);
            yield return new WaitForFixedUpdate();
        }
        Levels[level].transform.position = endPositionNewLevel;

        PlayerResource.Instance.zeroMove = false; //говорим что босс не двигается

        boss.gameObject.SetActive(true); //включаем босса
        board.gameObject.SetActive(true); //когда вышел ноль включаем поле
        StartCoroutine(boss.MoveToStart()); //двигаем босса к стартовой точке
    }

    private IEnumerator MoveOldLevel(int level)
    {
        //стандартная схема с движением, можно посмотреть в скрипте босса или ноля

        level--; //что бы не менять весь скрипт с предыдущего метода, смысл тот же

        Vector3 startPositionNewLevel = new Vector3(0, Levels[level].transform.position.y, Levels[level].transform.position.z);
        Vector3 endPositionNewLevel = new Vector3(-14f, Levels[level].transform.position.y, Levels[level].transform.position.z);

        float step;
        float moveTime = 0;
        float speed = 1f;


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
