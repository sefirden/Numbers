using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossPlayer : MonoBehaviour
{
    public GameObject boss; //объект босса
    public GameObject zero; //объект ноля
    public Board board; //объект поля

    public Tutorial tutorial;


    private ui ui; //скрипт всего УИ
    private Vector3 startPosition, endPosition; //вектор3 стартовой и конечной позиции босса
    public GameObject[] weapon;
    public RuntimeAnimatorController[] Animation; //список с анимациями боссов

    void Awake()
    {
        ui = FindObjectOfType<ui>(); //присваиваем скрипт к переменным
        board = FindObjectOfType<Board>();
        tutorial = FindObjectOfType<Tutorial>();

        if (PlayerResource.Instance.isLoaded == false) //если игре НЕ была загружена, новая игра
        {
            PlayerResource.Instance.bossMove = true; //говорим что босс двигается
            ui.LifeBarBackground.SetActive(false); //выключаем лайфбар
            ui.turnLeft.SetActive(false);
        }
        else if (PlayerResource.Instance.isLoaded == true) //если игру БЫЛА загружена, то
        {
            transform.position = new Vector3(4.92f, 13f, transform.position.z); //сразу ставим нужну позицию
            Invoke("RandomAttackTimer", UnityEngine.Random.Range(5f, 15f)); //запускаем атаку
            ui.LifeBarBackground.SetActive(true); //включаем лайфбар
            ui.turnLeft.SetActive(true);
            PlayerResource.Instance.bossMove = false; //говорим что босс не двигается
        }
    }

    private void RandomAttackTimer()
    {
        if (boss.activeSelf)
        StartCoroutine(Attack(board.level));
    }

    private IEnumerator Attack(int level) //создание объекта с летящими ножами
    {
        if (PlayerResource.Instance.bossMove == false && PlayerResource.Instance.GameIsPaused == false)
        {
            gameObject.GetComponent<Animator>().SetTrigger("attack"); //анимация атаки ноля
            GameObject weapon_temp = Instantiate(weapon[level], weapon[level].transform.position, Quaternion.identity); //создаем объект цифры, которая берет префаб из списка дотс и нужными координатами
            weapon_temp.transform.parent = this.transform; //присваиваем позицию
            weapon_temp.name = "weapon_boss_temp"; //присваиваем имя

            yield return new WaitForSeconds(UnityEngine.Random.Range(10f, 15f));

            StartCoroutine(Attack(board.level));
        }
        else
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(10f, 15f));
            StartCoroutine(Attack(board.level));
        }
    }

    public void ChangeBoss(int level) //смена босса
    {
        if (level != PlayerResource.Instance.scoreToNextLevel.Length) //если у нас не последний уровень
        {
            gameObject.GetComponent<Animator>().runtimeAnimatorController = Animation[level];
        }
        else //если у нас последний уровень
        {
            gameObject.SetActive(false); //выключаем босса
        }
    }

    public IEnumerator MoveToStart()
    {
        startPosition = new Vector3(12.59f, 13f, transform.position.z); //стартовая позиция босса
        endPosition = new Vector3(4.92f, 13f, transform.position.z); //конечная позиция босса

        PlayerResource.Instance.bossMove = true; //говорим что босс двигается
        gameObject.GetComponent<Animator>().SetBool("run", true); //анимацмя бега
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
        Invoke("RandomAttackTimer", UnityEngine.Random.Range(5f, 15f)); //запускаем атаку
        gameObject.GetComponent<Animator>().SetBool("run", false); //анимацмя бега
        ui.LifeBarBackground.SetActive(true); //включаем лайфбар
        ui.Tutorial.interactable = true;
        ui.turnLeft.SetActive(true);


        //запускаем тут подсказки
        if (Settings.Instance.showtutorial == false)
        {
            StartCoroutine(tutorial.ShowTip());
        }
    }
}
