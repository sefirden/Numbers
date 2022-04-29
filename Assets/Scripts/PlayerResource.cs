using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerResource : MonoBehaviour
{
    public static PlayerResource Instance { get; private set; } //определяем


    public bool anim_board_destroy = false;
    public int id; //для борьбы с чтением из оперативки времени и дамага

    public string gameMode; //режим игры
    public bool GameIsPaused; //если игра на паузе
    public bool TurnIsOn; //отмечаем ход
    public bool isLoaded; //тру если игра была загружена
    public int[] scoreToNextLevel; //количество очков до следующего уровня

    public bool bossMove; //если босс двигается
    public bool zeroMove; //если ноль двигается

    public bool starttimer;


    //переменные для Normal режима
    public string scoreN; //количество очков
    public string hiScoreN; //количество макс очков
    public string turnx2N; //количество ходов с двойным бонусом к урону или очкам
    public string hintN; //количество подсказок
    public string refillN; //количество перезаполнений поля
    public int heightN; //высота поля
    public int widthN; //ширина поля
    public bool AdRewardN; //была ли просмотрена реклама для перезаполнения поля
    public int levelN; //текущий уровень
    public int damageN; //количество нанесенного урона

    public bool EndGameN; //true если конец игры
    public string loadedBoardN; //загружаемый уровень в формате текстовой строки


    //переменные для Time Limit режима, половина та же что и выше
    public float time; //оставшееся время
    public float playedTime; //общее время игры
    public string turnTime; //количество ходов до восстановления активной кнопки +1 минута
    public string turnx2T; //количество ходов с двойным бонусом к урону или очкам
    public string scoreT; 
    public string hiScoreT;
    public string hintT;
    public string refillT;
    public int heightT;
    public int widthT;
    public bool AdRewardT;
    public int levelT;
    public int damageT;

    public bool EndGameT;
    public string loadedBoardT;

    private void Awake() //запускается до всех стартов
    {
        if (Instance == null) //если объекта ещё нет
        {
            Instance = this; //говорим что вот кагбе он
            DontDestroyOnLoad(gameObject); //и говорим что его нельзя ломать между уровнями, иначе он нахер не нужен
        }
        else //но, если вдруг на уровне такой уже есть
        {
            Destroy(gameObject); //то ломаем его к херам
        }

        Input.multiTouchEnabled = false;
        id = UnityEngine.Random.Range(10000, 100000);

    }
}
