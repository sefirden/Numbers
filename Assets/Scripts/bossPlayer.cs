using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossPlayer : MonoBehaviour
{
    public GameObject boss; //объект босса
    public GameObject zero; //объект ноля

    private ui ui; //скрипт всего УИ
    private Vector3 startPosition, endPosition; //вектор3 стартовой и конечной позиции босса
    public Sprite[] Sprite; //список со спрайтами боссов
    public Animation[] Animation; //список с анимациями боссов

    void Awake()
    {
        ui = FindObjectOfType<ui>(); //присваиваем скрипт к переменной

        if (PlayerResource.Instance.isLoaded == false) //если игре НЕ была загружена, новая игра
        {
            ui.LifeBarBackground.SetActive(false); //выключаем лайфбар
        }
        else if (PlayerResource.Instance.isLoaded == true) //если игру БЫЛА загружена, то
        {
            transform.position = new Vector3(4.92f, 13f, transform.position.z); //сразу ставим нужну позицию

            ui.LifeBarBackground.SetActive(true); //включаем лайфбар
            PlayerResource.Instance.bossMove = false; //говорим что босс не двигается
        }
    }

    public void ChangeBoss(int level) //смена босса
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Sprite[level]; //меняем боссу спрайт в зависимости от уровня, из списка
    }

    public IEnumerator MoveToStart()
    {
        startPosition = new Vector3(12.59f, 13f, transform.position.z); //стартовая позиция босса
        endPosition = new Vector3(4.92f, 13f, transform.position.z); //конечная позиция босса

        PlayerResource.Instance.bossMove = true; //говорим что босс двигается
        float step; //количество шагов, зависит от растояния
        float moveTime = 0; //не помню зачем, но нужно
        float speed = 1f; //скорость движения


        step = (speed / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime; //считаем количество шагов
        //ниже формула для плавного движения
        while (moveTime <= 1.0f)
        {
            moveTime += step;
            transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = endPosition;

        PlayerResource.Instance.bossMove = false; //говорим что босс не двигается
        ui.LifeBarBackground.SetActive(true); //включаем лайфбар
    }
}
