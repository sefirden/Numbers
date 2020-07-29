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


    public int score; //количество здоровья, пока 4
    public int hiScore; //количество ячеек здоровья
    public int hint; //количество монеток у игрока, нужны для магазина поверапов
    public int refill; //количество спасенных кусков пиццы, за каждый кусок противники сложнее

    public float time; //

    public bool GameIsPaused; //если игра на паузе

    public bool EndGame; //true если конец игры

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
