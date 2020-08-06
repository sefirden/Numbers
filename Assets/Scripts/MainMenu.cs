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
    public GameObject ResumeN; //прикрепляем кнопку продолжить
    public GameObject ResumeT; //прикрепляем кнопку новая игра


    //слои меню и настроек
    public GameObject MainLayer;
    public GameObject SettingsLayer;
    public GameObject LanguageLayer;
    public GameObject TimeLayer;
    public GameObject NormalLayer;
    public Dropdown size_dropN;
    public Dropdown size_dropT;

    [SerializeField]
    int[] BoardSize;
    int index;

    int width;
    int height;


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
    }

    private void Update() //при старте сцены с меню
    {
        /*if (PlayerResource.Instance.scoreN != 0 && PlayerResource.Instance.EndGameN == false) //если файл сохранения есть
        {
            ResumeN.SetActive(true); //включаем кнопку продолжить
        }
        else if(PlayerResource.Instance.scoreT != 0 && PlayerResource.Instance.EndGameT == false) //если файла сохранения не найдено
        {
            ResumeT.SetActive(true); //выключаем кнопку продолжить
        }*/

        width = 5;
        height = 5;

        int v = Array.IndexOf(BoardSize, width);
        size_dropN.value = v;

        size_dropN.onValueChanged.AddListener(delegate {
            index = size_dropN.value;
            height = BoardSize[index];
            width = BoardSize[index];
        });

        int w = Array.IndexOf(BoardSize, width);
        size_dropT.value = w;

        size_dropT.onValueChanged.AddListener(delegate {
            index = size_dropN.value;
            height = BoardSize[index];
            width = BoardSize[index];
        });


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


    public void NormalModeResume()
    {
        PlayerResource.Instance.isLoaded = true;
        PlayerResource.Instance.GameIsPaused = false;
        PlayerResource.Instance.gameMode = "normal";
        Time.timeScale = 1f;
        Debug.LogWarning("NormalModeResume");
        SceneManager.LoadScene("Main"); //тупо загружаем первый уровень, потом добавить сюда туториал
    }

    public void TimeModeResume()
    {
        PlayerResource.Instance.isLoaded = true;
        PlayerResource.Instance.GameIsPaused = false;
        PlayerResource.Instance.gameMode = "timetrial";
        Time.timeScale = 1f;
        Debug.LogWarning("TimeModeResume");
        SceneManager.LoadScene("Main"); //тупо загружаем первый уровень, потом добавить сюда туториал

    }

    public void NormalMode()
    {
        MainLayer.SetActive(false);
        NormalLayer.SetActive(true);
        PlayServicesGoogle.Instance.ReadFromJson(); //читаем из JSON
        PlayServicesGoogle.Instance.LoadFromJson(); //применяем в игре
        if (PlayerResource.Instance.scoreN != 0 && PlayerResource.Instance.EndGameN == false) //если файл сохранения есть
        {
            ResumeN.SetActive(true); //включаем кнопку продолжить
        }
    }

    public void TimeMode()
    {
        MainLayer.SetActive(false);
        TimeLayer.SetActive(true);
        PlayServicesGoogle.Instance.ReadFromJson(); //читаем из JSON
        PlayServicesGoogle.Instance.LoadFromJson(); //применяем в игре
        if (PlayerResource.Instance.scoreT != 0 && PlayerResource.Instance.EndGameT == false) //если файл сохранения есть
        {
            ResumeT.SetActive(true); //включаем кнопку продолжить
        }
    }

    public void Menu()
    {
        TimeLayer.SetActive(false);
        NormalLayer.SetActive(false);
        MainLayer.SetActive(true);
    }
       
    public void NormalModeStart()
    {
        PlayerResource.Instance.GameIsPaused = false; //убираем паузу
        Time.timeScale = 1f;        //убираем паузу
        PlayerResource.Instance.EndGameN = false; //ставим что конец игры не тру
        PlayerResource.Instance.hintN = 3;
        PlayerResource.Instance.refillN = 1;
        PlayerResource.Instance.gameMode = "normal";
        PlayerResource.Instance.scoreN = 0;
        PlayerResource.Instance.heightN = height;
        PlayerResource.Instance.widthN = width;
        SceneManager.LoadScene("Main"); //тупо загружаем первый уровень, потом добавить сюда туториал
    }

    public void TimeTrialModeStart()
    {
        
        PlayerResource.Instance.GameIsPaused = false; //убираем паузу
        Time.timeScale = 1f;        //убираем паузу
        PlayerResource.Instance.EndGameT = false; //ставим что конец игры не тру
        PlayerResource.Instance.hintT = 3;
        PlayerResource.Instance.refillT = 1;
        PlayerResource.Instance.gameMode = "timetrial";
        PlayerResource.Instance.time = 120f;
        PlayerResource.Instance.scoreT = 0;
        PlayerResource.Instance.heightT = height;
        PlayerResource.Instance.widthT = width;
        SceneManager.LoadScene("Main"); //тупо загружаем первый уровень, потом добавить сюда туториал
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
        PlayServicesGoogle.ShowLeaderboardsUI();
    }

    public void ShowAchievement()
    {
        PlayServicesGoogle.ShowAchievementUI();
    }

    public void CloudLoad()
    {
        PlayServicesGoogle.Instance.LoadFromCloud(); //было false
    }

}
