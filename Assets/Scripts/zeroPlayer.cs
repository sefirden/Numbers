﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zeroPlayer : MonoBehaviour
{
    public bossPlayer boss; //скрипт босса
    private Level Level; //скрипт уровней
    public GameObject[] weapon;
    public RuntimeAnimatorController[] Animation; //список с анимациями боссов
    public Vector3[] kill_boss_point;
    public float[] timing;
    public float[] speed;
    float timer = 0f;

    private Vector3 startPosition, endPosition; //вектор3 стартовой и конечной позиции ноля
   
    private void Update()
    {
        timer += Time.deltaTime;
    }
    void Awake()
    {
        Level = FindObjectOfType<Level>(); //присваиваем скрипт к переменной
        boss = FindObjectOfType<bossPlayer>(); //присваиваем скрипт к переменной

        if (PlayerResource.Instance.isLoaded == false) //если игре НЕ была загружена, новая игра
        {
            startPosition = transform.position; //ставим стартовую позицию ноля как текущую
            endPosition = new Vector3(1f, 13.6f, transform.position.z); //конечная позиция ноля, подправить потом под уровень
            StartCoroutine(MoveToStart()); //запускаем движения ноля к стартовой позиции
        }
        else if (PlayerResource.Instance.isLoaded == true) //если игру БЫЛА загружена, то
        {
            transform.position = new Vector3(1f, 13.6f, transform.position.z); //сразу ставим нужну позицию
            PlayerResource.Instance.zeroMove = false; //ну и тут говорим что ноль не двигается, не помню но где-то было нужно

        }
    }

    private IEnumerator MoveToStart() //метод плавного движения ноля к старту
    {
        PlayerResource.Instance.zeroMove = true; //говорим что ноль двигается
        gameObject.GetComponent<Animator>().SetBool("run", true);
        float step; //количество шагов, зависит от растояния
        float moveTime = 0; //не помню зачем, но нужно
        float speed = 1;  //скорость движения


        step = (speed / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime; //считаем количество шагов
        //ниже формула для плавного движения
        while (moveTime <= 1.0f)
        {
            moveTime += step;
            transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = endPosition;

        PlayerResource.Instance.zeroMove = false; //ну и тут говорим что ноль не двигается
        Level.StartNewGameLevel(0); //меняем стартовую локацию на первый уровень при старте игры
    }

    public void Attack(int level) //создание объекта с летящими ножами
    {
        gameObject.GetComponent<Animator>().SetTrigger("attack"); //анимация атаки ноля
        GameObject weapon_temp = Instantiate(weapon[level], weapon[level].transform.position, Quaternion.identity); //создаем объект цифры, которая берет префаб из списка дотс и нужными координатами
        weapon_temp.transform.parent = this.transform; //присваиваем позицию
        weapon_temp.name = "weapon_temp"; //присваиваем имя

        //StartCoroutine(KillTheBoss());
    }

    public void ChangeZero(int level)
    {
        gameObject.GetComponent<Animator>().runtimeAnimatorController = Animation[level];
        gameObject.GetComponent<Animator>().SetBool("run", true); //анимацмя бега
    }


    public IEnumerator KillTheBoss() //метод плавного движения ноля к старту
    {
        PlayerResource.Instance.zeroMove = true; //говорим что ноль двигается
                       
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<Animator>().SetTrigger("kill");

        timer = 0;
        Vector3 startPosition, endPosition;
        int point = 0;

        while (point < 6)
        {
            float step; //количество шагов, зависит от растояния
            float moveTime = 0; //не помню зачем, но нужно

            startPosition = transform.position;
            endPosition = kill_boss_point[point];

            step = (speed[point] / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime; //считаем количество шагов
                                                                                            //ниже формула для плавного движения
            while (moveTime <= 1.0f)
            {
                moveTime += step;
                transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
                yield return new WaitForFixedUpdate();
            }
            transform.position = endPosition;
            Debug.LogError(timer);
            yield return new WaitForSeconds(timing[point]);
            point++;

        }
        timer = 0;
        PlayerResource.Instance.zeroMove = false; //ну и тут говорим что ноль не двигается

    }
}
