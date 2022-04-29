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

    private string path; //переменная для пути сохранения
    public GameObject ResumeN; //прикрепляем кнопку продолжить норм режим игры
    public GameObject ResumeT; //прикрепляем кнопку продолжить режим на время


    //слои меню и настроек
    public GameObject MainLayer; //слой основного меню
    public GameObject SettingsLayer; //слой настроек
    public GameObject LanguageLayer; //слой выбора языка
    public GameObject TimeLayer; //слой режима на время
    public GameObject NormalLayer; //слой нормального режима
    public GameObject NormalLayerNewGame; //слой вопроса запускать ли новую игру
    public GameObject TimeLayerNewGame; //слой вопроса запускать ли новую игру на время
    public Dropdown size_dropN; //выбор размера уровня норм режима
    public Dropdown size_dropT; //выбор размера режима на время
    public GameObject ExitLabel; //всплывающее окно про выход из игры
    public GameObject QuestionCloudGame; //слой вопроса переписать ли сохранение
    public Text DebugCloudLog;
   // public GameObject Loading; //лого загрузки из облака сейвов
   // public GameObject Loading_failed; //лого загрузки из облака сейвов

    [SerializeField]
    int[] BoardSize; //варианты размеров поля

    int width;
    int height;

    void Update()
    {
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(ExitLabel.gameObject.activeSelf == true)
            {
                Quit();
            }
            else
            {
                StartCoroutine(BackButtonExit());
            }

        }

       /* if (Loading_failed.activeSelf)
        {
            StartCoroutine(LoadingFailed());
        }*/
    }

    private IEnumerator BackButtonExit()
    {
        ExitLabel.SetActive(true);
        yield return new WaitForSeconds(2f);
        ExitLabel.SetActive(false);
    }

   /* private IEnumerator LoadingFailed()
    {
        yield return new WaitForSeconds(2f);
        Loading_failed.SetActive(false);
    }*/

    private void Start() //при старте игры
    {
#if UNITY_ANDROID && !UNITY_EDITOR //если скрипт запущен в редакторе или на андроиде
        path = Path.Combine(Application.persistentDataPath, "Settings.json"); //путь к сохранению
#else
        path = Path.Combine(Application.dataPath, "Settings.json"); //путь к сохранению если не редактор и не андроид
#endif
        if (File.Exists(path)) //если файл сохранения настроек есть, то прячем выбор зыка и показываем меню
        {
            MainLayer.SetActive(true);

            LanguageLayer.SetActive(false);

        }
        else //если файла сохранения настроек не найдено, показываем слой выбора языка
        {
            MainLayer.SetActive(false);
            LanguageLayer.SetActive(true);

        }

        width = 5; //размер поля по умолчанию
        height = 5;

        //при изменении размера поля в выпадающем меню
        size_dropT.onValueChanged.AddListener(delegate {
                DropdownValueChangedT(size_dropT);
        });

        //см выше, но другой режим
        size_dropN.onValueChanged.AddListener(delegate {
            DropdownValueChangedN(size_dropN);
        });

        FindObjectOfType<AudioManager>().Play("music_menu");
    }

    //от выбранного пункта выпадающего меня меняем ширину и высоту
    void DropdownValueChangedT(Dropdown change)
    {
        width = BoardSize[change.value];
        height = BoardSize[change.value];
    }

    //см выше
    void DropdownValueChangedN(Dropdown change)
    {
        width = BoardSize[change.value];
        height = BoardSize[change.value];
    }

    //ниже для каждого языка свой метод, больше языков, больше методов
    public void Ru()
    {
        Settings.Instance.language = "ru"; //говорим что это выбранный язык
        SaveSystem.Instance.SettingsSave(); //сохраняем настройки
        string lvl = SceneManager.GetActiveScene().name; //берем название текущей сцены
        SceneManager.LoadScene(lvl); //и перезагружаем его, чтобы язык поменялся
    }

    public void En() //см выше
    {
        Settings.Instance.language = "en";
        SaveSystem.Instance.SettingsSave();
        string lvl = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(lvl);
    }

    public void Ua() //см выше
    {
        Settings.Instance.language = "ua";
        SaveSystem.Instance.SettingsSave();
        string lvl = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(lvl);
    }

    public void NormalMode() //включаем вспл окно норм режима
    {
        MainLayer.SetActive(false);
        NormalLayer.SetActive(true);
        PlayServicesGoogle.Instance.ReadFromJson(); //читаем из JSON сохранения
        PlayServicesGoogle.Instance.LoadFromJson(); //применяем в игре
        if (Convert.ToInt32(SaveSystem.Decrypt(PlayerResource.Instance.scoreN)) != 0 && PlayerResource.Instance.EndGameN == false) //если очков в сейве было больше 0 и там не проиграли
        {
            ResumeN.SetActive(true); //включаем кнопку продолжить
        }
        else
        {
            ResumeN.SetActive(false); //выключаем кнопку продолжить
        }
    }
    
    public void NormalModeResume() //кнопка продолжить обычный режим игры
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "NormalModeResume");
        PlayerResource.Instance.isLoaded = true; //говорим что игра зугружена, нужно при старте сцены с доской и для уровней и прочего
        PlayerResource.Instance.GameIsPaused = false; //говорим что уже не на паузе, надо когда вышли из игры из меню паузы
        PlayerResource.Instance.gameMode = "normal"; //говорим что режим нормальный
        Time.timeScale = 1f; //это для отключения паузы в игре
        SceneManager.LoadScene("Main"); //тупо загружаем основной уровень
    }

    public void NormalModeNewGame() //если есть кнопка продолжить игру и мы нажали новая игра то задать вопрос, если кнопки продолжить нет, то просто начать новую игру
    {
            if (ResumeN.activeSelf == true)
            {
                NormalLayer.SetActive(false);
                NormalLayerNewGame.SetActive(true);
            }
            else
            {
                NormalModeStart();
            }
    }

    public void NormalModeNewGameNO()
    {
        NormalLayer.SetActive(true);
        NormalLayerNewGame.SetActive(false);
    }

    public void NormalModeStart() //кнопка старт обычного ержима новой игры
    {


        int zeroInt = 0;
        PlayerResource.Instance.GameIsPaused = false; //убираем паузу
        Time.timeScale = 1f;        //убираем паузу
        PlayerResource.Instance.EndGameN = false; //ставим что конец игры не тру
        PlayerResource.Instance.AdRewardN = false; //ставим что рекламу мы не смотрели ради перемешивания
        PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__normal_mode, Convert.ToInt32(SaveSystem.Decrypt(PlayerResource.Instance.hiScoreN))); //отправляем лучшее время прошлой игры в Google Play
        PlayerResource.Instance.hintN = SaveSystem.Encrypt(Convert.ToString(3)); //количество подсказок
        PlayerResource.Instance.refillN = SaveSystem.Encrypt(Convert.ToString(1)); //количество перемешиваний поля
        PlayerResource.Instance.turnx2N = SaveSystem.Encrypt(Convert.ToString(0)); //количество ходов с двойным уроном
        PlayerResource.Instance.gameMode = "normal"; //говорим что это норм режим
        PlayerResource.Instance.scoreN = SaveSystem.Encrypt(Convert.ToString(zeroInt)); //обнуляем очки
        PlayerResource.Instance.levelN = zeroInt; //стартуем с 0 левела
        PlayerResource.Instance.damageN = zeroInt; //обнуляем урон
        PlayerResource.Instance.heightN = height; //высота поля
        PlayerResource.Instance.widthN = width; //ширина поля

        Firebase.Analytics.Parameter[] StartGame =
    {
            new Firebase.Analytics.Parameter("width", Convert.ToString(width)),
            new Firebase.Analytics.Parameter("language", Settings.Instance.language),
            new Firebase.Analytics.Parameter("GameMode", PlayerResource.Instance.gameMode),
            };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("StartGame", StartGame);

        SceneManager.LoadScene("Main"); //тупо загружаем основной уровень, потом добавить сюда туториал
    }

    public void TimeMode() //см выше нормал мод
    {
        MainLayer.SetActive(false);
        TimeLayer.SetActive(true);
        PlayServicesGoogle.Instance.ReadFromJson(); //читаем из JSON
        PlayServicesGoogle.Instance.LoadFromJson(); //применяем в игре
        if (Convert.ToInt32(SaveSystem.Decrypt(PlayerResource.Instance.scoreT)) != 0 && PlayerResource.Instance.EndGameT == false)
        {
            ResumeT.SetActive(true); //включаем кнопку продолжить
        }
        else
        {
            ResumeT.SetActive(false); //включаем кнопку продолжить
        }
    }

    public void TimeModeResume() //см выше нормал мод
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "TimeModeResume");
        PlayerResource.Instance.isLoaded = true;
        PlayerResource.Instance.GameIsPaused = false;
        PlayerResource.Instance.gameMode = "timetrial";
        Time.timeScale = 1f;
        Debug.LogWarning("TimeModeResume");
        SceneManager.LoadScene("Main");

    }

    public void TimeModeNewGame() //если есть кнопка продолжить игру и мы нажали новая игра то задать вопрос, если кнопки продолжить нет, то просто начать новую игру
    {
        if (ResumeT.activeSelf == true)
        {
            TimeLayer.SetActive(false);
            TimeLayerNewGame.SetActive(true);
        }
        else
        {
            TimeTrialModeStart();
        }
    }

    public void TimeModeNewGameNO()
    {
        TimeLayer.SetActive(true);
        TimeLayerNewGame.SetActive(false);
    }


    public void TimeTrialModeStart() //см выше нормал мод
    {
        PlayerResource.Instance.GameIsPaused = false; //убираем паузу
        PlayerResource.Instance.starttimer = false;
        Time.timeScale = 1f;        //убираем паузу
        PlayerResource.Instance.EndGameT = false; //ставим что конец игры не тру
        PlayerResource.Instance.AdRewardT = false;
        PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_play_time_time_limit_mode, Convert.ToInt64(PlayerResource.Instance.playedTime * 1000)); //отправляем лучшее время в Google Play
        PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__time_limit_mode, Convert.ToInt32(SaveSystem.Decrypt(PlayerResource.Instance.hiScoreT))); //отправляем лучшие очки в Google Play
        PlayerResource.Instance.hintT = SaveSystem.Encrypt(Convert.ToString(3));
        PlayerResource.Instance.refillT = SaveSystem.Encrypt(Convert.ToString(1));
        PlayerResource.Instance.turnx2T = SaveSystem.Encrypt(Convert.ToString(0)); //количество ходов с двойным уроном
        PlayerResource.Instance.turnTime = SaveSystem.Encrypt(Convert.ToString(0)); //количество ходов до отката +1 минута
        PlayerResource.Instance.gameMode = "timetrial";
        PlayerResource.Instance.time = 120f;
        PlayerResource.Instance.playedTime = 0f;
        PlayerResource.Instance.scoreT = SaveSystem.Encrypt(Convert.ToString(0)); //обнуляем очки
        PlayerResource.Instance.levelT = 0;
        PlayerResource.Instance.damageT = 0;
        PlayerResource.Instance.heightT = height;
        PlayerResource.Instance.widthT = width;

        Firebase.Analytics.Parameter[] StartGame =
    {
            new Firebase.Analytics.Parameter("width", Convert.ToString(width)),
            new Firebase.Analytics.Parameter("language", Settings.Instance.language),
            new Firebase.Analytics.Parameter("GameMode", PlayerResource.Instance.gameMode),
            };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("StartGame", StartGame);

        SceneManager.LoadScene("Main"); //тупо загружаем основной уровень, потом добавить сюда туториал
    }

    public void Menu() //возврат меню
    {
        TimeLayer.SetActive(false);
        NormalLayer.SetActive(false);
        MainLayer.SetActive(true);
    }

    public void Setting() //включаем слой настроек
    {
        MainLayer.SetActive(false);
        SettingsLayer.SetActive(true);
    }

    public void Quit() //выход из игры
    {
        FindObjectOfType<AudioManager>().Stop("music_menu");
        Application.Quit();
    }

    public void ShowLeaderBoard() //показать лидерборд
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "ShowLeaderBoard");
        PlayServicesGoogle.ShowLeaderboardsUI();
    }

    public void ShowAchievement() //показать ачивки
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "ShowAchievement");

        PlayServicesGoogle.ShowAchievementUI();
    }

    public void CloudLoad() //загрузить данные из облака
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "CloudLoad");


#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "FullSave.json");
#else
        path = Path.Combine(Application.dataPath, "FullSave.json");
#endif
        if (File.Exists(path))
        {
            QuestionCloudGame.SetActive(true);
        }
        else
        {
            //Loading.SetActive(true);
            PlayServicesGoogle.Instance.LoadFromCloud(); //было false
        }
    }

    public void ReWrite(bool rewrite)
    {
        if(rewrite == true)
        {
            QuestionCloudGame.SetActive(false);
            //Loading.SetActive(true);
            PlayServicesGoogle.Instance.LoadFromCloud(); //было false
        }
        else
        {
            QuestionCloudGame.SetActive(false);
        }
    }

}
