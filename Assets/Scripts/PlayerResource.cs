using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerResource : MonoBehaviour
{
    public static PlayerResource Instance { get; private set; } //определяем

    public string gameMode; //режим игры
    public bool GameIsPaused; //если игра на паузе
    public bool isLoaded; //тру если игра была загружена
    public int[] scoreToNextLevel; //количество очков до следующего уровня

    public bool bossMove; //если босс двигается
    public bool zeroMove; //если ноль двигается


    //переменные для Normal режима
    public int scoreN; //количество очков
    public int hiScoreN; //количество макс очков
    public int hintN; //количество подсказок
    public int refillN; //количество перезаполнений поля
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
    public int scoreT; 
    public int hiScoreT;
    public int hintT;
    public int refillT;
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

    }
}
