using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zeroPlayer : MonoBehaviour
{
    public bossPlayer boss; //скрипт босса
    public Board board; //объект поля
    private Level Level; //скрипт уровней
    public GameObject[] weapon;
    public RuntimeAnimatorController[] Animation; //список с анимациями боссов
    public Vector3[] pick_up_weapon; //масив для сохранения всех подсказок и подальшего их сравнения
    public float[] speed;
    public float[] timing;
    float timer = 0f;

    private Vector3 startPosition, endPosition; //вектор3 стартовой и конечной позиции ноля
   
    void Awake()
    {
        Level = FindObjectOfType<Level>(); //присваиваем скрипт к переменной
        board = FindObjectOfType<Board>();
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


    public IEnumerator MoveToStart() //метод плавного движения ноля к старту
    {
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

    public void ChangeZero(int level) //при смене уровня меняем аниматор и говорим что ноль идет
    {
        gameObject.GetComponent<Animator>().runtimeAnimatorController = Animation[level];
    }
    
  /*  public IEnumerator KillTheBoss() //метод плавного движения ноля к старту
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


    }
    */


    public IEnumerator KillTheBoss() //метод плавного движения ноля к старту
    {

           // PlayerResource.Instance.zeroMove = true; //говорим что ноль двигается
            yield return new WaitForSeconds(1.5f);

            gameObject.GetComponent<Animator>().SetTrigger("kill");

            //анимация от старта до приземления 2,32

            Vector3 startPosition, endPosition;

            float step; //количество шагов, зависит от растояния
            float moveTime = 0; //не помню зачем, но нужно

            startPosition = transform.position;
            endPosition = new Vector3(8f,13.6f,1f);

            step = (speed[0] / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime; //считаем количество шагов
                                                                                         //ниже формула для плавного движения
            while (moveTime <= 1.0f)
            {
                moveTime += step;

            Vector3 center = (startPosition + endPosition) * 0.5F;

            center -= new Vector3(0, 0.2f, 0);

            Vector3 riseRelCenter = startPosition - center;
            Vector3 setRelCenter = endPosition - center;

            transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, moveTime);
            transform.position += center;

            yield return new WaitForFixedUpdate();
            }
            transform.position = endPosition; //конец прыжка

            boss.GetComponent<Animator>().SetTrigger("dead"); //после завершения прыжка включаем анимацию убийства босса

            yield return new WaitForSeconds(timing[0]); //ждем конца анимации и идем за оружием
            gameObject.GetComponent<Animator>().SetBool("empy_run", true);

        //ниже двигаем босса к точке с оружием
            moveTime = 0; //не помню зачем, но нужно
            startPosition = transform.position;
            endPosition = pick_up_weapon[board.level];

            step = (speed[1] / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime; //считаем количество шагов
                                                                                               //ниже формула для плавного движения
            while (moveTime <= 1.0f)
            {
                moveTime += step;
                transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
                yield return new WaitForFixedUpdate();
            }
            transform.position = endPosition;
            gameObject.GetComponent<Animator>().SetBool("empy_run", false);
            boss.GetComponent<Animator>().SetTrigger("clear");
            boss.gameObject.SetActive(false); //выключаем босса
            gameObject.GetComponent<Animator>().SetBool("new_weapon_run", true);

        //ниже двигаем к началу карты

        moveTime = 0; //не помню зачем, но нужно
        startPosition = transform.position;
        endPosition = new Vector3(1f, 13.6f, transform.position.z);

        step = (speed[1] / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime; //считаем количество шагов
                                                                                           //ниже формула для плавного движения
        while (moveTime <= 1.0f)
        {
            moveTime += step;
            transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
            yield return new WaitForFixedUpdate();
        }
        gameObject.GetComponent<Animator>().SetBool("new_weapon_run", false);
        gameObject.GetComponent<Animator>().SetTrigger("change_zero");

        yield return new WaitForSeconds(timing[1]);

        ChangeZero(board.level);
        boss.ChangeBoss(board.level); //запускаем смену босса

        gameObject.GetComponent<Animator>().SetBool("run", true);
        Level.ChangeLevel(board.level); //запускаем смену уровня
    }
}
