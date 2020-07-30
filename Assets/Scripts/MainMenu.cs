using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{

    //Скрипт прикреплён к кнопке продолжить игру

    private string path; //переменная для пути сохранения
    public GameObject resume; //прикрепляем кнопку продолжить
    public GameObject newGame; //прикрепляем кнопку новая игра
    public GameObject settings; //прикрепляем кнопку продолжить
    public GameObject quit; //прикрепляем кнопку новая игра

    //слои меню и настроек
    public GameObject MainLayer;
    public GameObject SettingsLayer;
    public GameObject LanguageLayer;
    public GameObject SelectMode;
    public Dropdown size_drop;

    [SerializeField]
    int[] BoardSize;
    int index;


    private void Start() //при старте сцены с меню
    {
#if UNITY_ANDROID && !UNITY_EDITOR //если скрипт запущен в редакторе или на андроиде
        path = Path.Combine(Application.persistentDataPath, "Settings.json"); //путь к сохранению
#else
        path = Path.Combine(Application.dataPath, "Settings.json"); //путь к сохранению если не редактор и не андроид
#endif
        if (File.Exists(path)) //если файл сохранения есть
        {
            MainLayer.SetActive(true);
            LanguageLayer.SetActive(false);

        }
        else //если файла сохранения не найдено
        {
            MainLayer.SetActive(false);
            LanguageLayer.SetActive(true);

        }
        
        int size = PlayerResource.Instance.width;

        int v = Array.IndexOf(BoardSize, size);
        size_drop.value = v;

        size_drop.onValueChanged.AddListener(delegate {
            index = size_drop.value;
            PlayerResource.Instance.width = BoardSize[index];
            PlayerResource.Instance.height = BoardSize[index];
        });

    }

    private void Update() //при старте сцены с меню
    {
#if UNITY_ANDROID && !UNITY_EDITOR //если скрипт запущен в редакторе или на андроиде
        path = Path.Combine(Application.persistentDataPath, "FullSave.json"); //путь к сохранению
#else
        path = Path.Combine(Application.dataPath, "FullSave.json"); //путь к сохранению если не редактор и не андроид
#endif
        if (File.Exists(path)) //если файл сохранения есть
        {
            resume.SetActive(true); //включаем кнопку продолжить
            newGame.transform.position = new Vector2(0, -1f); //кнопку новая игра двигаем ниже
            settings.transform.position = new Vector2(0, -2f); //кнопку настройки двигаем ниже
            quit.transform.position = new Vector2(0, -3f); //кнопку выход двигаем ниже
        }
        else //если файла сохранения не найдено
        {
            resume.SetActive(false); //выключаем кнопку продолжить
            newGame.transform.position = new Vector2(0, 0); //кнопку новая игра поднимаем выше
            settings.transform.position = new Vector2(0, -1f); //кнопку настройки двигаем выше
            quit.transform.position = new Vector2(0, -2f); //кнопку выход двигаем выше

        }
    }

    public void Ru()
    {
        Settings.Instance.language = "ru";
        SaveSystem.Instance.SettingsSave();
        string lvl = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(lvl);
    }

    public void En()
    {
        Settings.Instance.language = "en";
        SaveSystem.Instance.SettingsSave();
        string lvl = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(lvl);
    }

    public void Ua()
    {
        Settings.Instance.language = "ua";
        SaveSystem.Instance.SettingsSave();
        string lvl = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(lvl);
    }


    public void Resume()
    {
       // PlayServicesGoogle.Instance.ReadFromJson(); //читаем из JSON
       // PlayServicesGoogle.Instance.LoadFromJson(); //применяем в игре
        PlayerResource.Instance.GameIsPaused = false;
        Time.timeScale = 1f;
    }

    public void NewGame()
    {
        MainLayer.SetActive(false);
        SelectMode.SetActive(true);
    }

    public void Menu()
    {
        SelectMode.SetActive(false);
        MainLayer.SetActive(true);
    }

    public void NormalMode()
    {
        SceneManager.LoadScene("Main"); //тупо загружаем первый уровень, потом добавить сюда туториал
        PlayerResource.Instance.GameIsPaused = false; //убираем паузу
        Time.timeScale = 1f;        //убираем паузу
        PlayerResource.Instance.EndGame = false; //ставим что конец игры не тру
        PlayerResource.Instance.hint = 3;
        PlayerResource.Instance.refill = 1;
        PlayerResource.Instance.gameMode = "normal";
        PlayerResource.Instance.time = 0f;
        PlayerResource.Instance.score = 0;
    }

    public void TimeTrialMode()
    {
        SceneManager.LoadScene("Main"); //тупо загружаем первый уровень, потом добавить сюда туториал
        PlayerResource.Instance.GameIsPaused = false; //убираем паузу
        Time.timeScale = 1f;        //убираем паузу
        PlayerResource.Instance.EndGame = false; //ставим что конец игры не тру
        PlayerResource.Instance.hint = 3;
        PlayerResource.Instance.refill = 1;
        PlayerResource.Instance.gameMode = "timetrial";
        PlayerResource.Instance.time = 120f;
        PlayerResource.Instance.score = 0;
    }

    public void Setting()
    {
        MainLayer.SetActive(false);
        SettingsLayer.SetActive(true);
    }

    public void Quit()
    {
        FindObjectOfType<AudioManager>().Stop("Theme");
        Application.Quit();
    }

    public void ShowLeaderBoard()
    {
        //PlayServicesGoogle.ShowLeaderboardsUI();
    }

    public void ShowAchievement()
    {
       // PlayServicesGoogle.ShowAchievementUI();
    }

    public void CloudLoad()
    {
       // PlayServicesGoogle.Instance.LoadFromCloud(); //было false
    }

}
