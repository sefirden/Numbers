using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerResource : MonoBehaviour
{

    //Unity singletone по сути, храним тут всю информацию про персонажа при переходе между уровнями
    //Прикреплён к префабу PlayerResources

    public static PlayerResource Instance { get; private set; } //определяем

    public string gameMode; //режим игры
    public bool GameIsPaused; //если игра на паузе
    public bool isLoaded;

    //переменные для Normal режима
    public int scoreN; //количество здоровья, пока 4
    public int hiScoreN; //количество ячеек здоровья
    public int hintN; //количество монеток у игрока, нужны для магазина поверапов
    public int refillN; //количество спасенных кусков пиццы, за каждый кусок противники сложнее
    public int heightN;
    public int widthN;

    public bool EndGameN; //true если конец игры
    public string loadedBoardN;


    //переменные для Time Limit режима
    public float time; //
    public int scoreT; //количество здоровья, пока 4
    public int hiScoreT; //количество ячеек здоровья
    public int hintT; //количество монеток у игрока, нужны для магазина поверапов
    public int refillT; //количество спасенных кусков пиццы, за каждый кусок противники сложнее
    public int heightT;
    public int widthT;

    public bool EndGameT; //true если конец игры
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
