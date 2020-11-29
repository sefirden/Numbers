﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using UnityEngine.SceneManagement;


public class PlayServicesGoogle : MonoBehaviour
{

    public static PlayServicesGoogle Instance { get; private set; } //определяем

    public FullSaves fullsave = new FullSaves(); //сюда сохраняем все данные
    private string path; //путь к сейву
    public Button LeaderBoard; //кнопка лидерборд
    public Button Achievement; //кнопка ачивки
    public Button CloudLoad; //кнопка загрузка из облака

    public DateTime SaveTime; //текущая дата и время, нужно для облачного сейва

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



    #region Auth
    private void Start()
    {
        //ниже по гайду гугл плей сервисы

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        OnConnectionResponse(PlayGamesPlatform.Instance.localUser.authenticated);

        SignIn();
    }

    void SignIn() //логинимся
    {

        Social.localUser.Authenticate((bool success) =>
        {
            OnConnectionResponse(success);
            Debug.LogWarning(success);
        });
    }


    private void OnConnectionResponse(bool authenticated)
    {
        if (authenticated) //если залогинились
        {
            //включаем кнопки
            LeaderBoard.interactable = true; 
            Achievement.interactable = true;
            CloudLoad.interactable = true;

            Debug.Log("authenticated"); //показываем кнопки ачив и лидерборд
        }
        else //если не залогинились
        {
            //кнопки выключены
            LeaderBoard.interactable = false;
            Achievement.interactable = false;
            CloudLoad.interactable = false;

            Debug.Log("not authenticated"); //прячем кнопки ачив и лидерборд
        }
    }
    #endregion Auth

    #region Collect data
    [Serializable]
    public class FullSaves //класс со всеми данными которые надо сохранить
    {   //описание переменных в скрипте плеерресоурсес
        public bool GameIsPaused; //если игра на паузе

        //переменные для Normal режима
        public string scoreN; 
        public string hiScoreN;
        public string hintN;
        public string refillN;
        public int heightN;
        public int widthN;

        public bool EndGameN;
        public string loadedBoardN;
        public bool AdRewardN;
        public int levelN;
        public int damageN;

        //переменные для Time Limit режима
        public float time;
        public float playedTime;
        public string scoreT;
        public string hiScoreT;
        public string hintT;
        public string refillT;
        public int heightT;
        public int widthT;
        public int levelT;

        public bool EndGameT;
        public string loadedBoardT;
        public bool AdRewardT;
        public int damageT;

        public string SaveTime;
    }

    public void CollectData() //собираем дынные из игры
    {
        if (SceneManager.GetActiveScene().name != "Menu") //если мы не в меню
        {
            fullsave.scoreN = PlayerResource.Instance.scoreN;
            fullsave.hiScoreN = PlayerResource.Instance.hiScoreN;
            fullsave.hintN = PlayerResource.Instance.hintN;
            fullsave.refillN = PlayerResource.Instance.refillN;
            fullsave.heightN = PlayerResource.Instance.heightN;
            fullsave.widthN = PlayerResource.Instance.widthN;
            fullsave.EndGameN = PlayerResource.Instance.EndGameN;
            fullsave.loadedBoardN = PlayerResource.Instance.loadedBoardN;
            fullsave.AdRewardN = PlayerResource.Instance.AdRewardN;
            fullsave.levelN = PlayerResource.Instance.levelN;
            fullsave.damageN = PlayerResource.Instance.damageN;

            fullsave.scoreT = PlayerResource.Instance.scoreT;
            fullsave.hiScoreT = PlayerResource.Instance.hiScoreT;
            fullsave.hintT = PlayerResource.Instance.hintT;
            fullsave.refillT = PlayerResource.Instance.refillT;
            fullsave.heightT = PlayerResource.Instance.heightT;
            fullsave.widthT = PlayerResource.Instance.widthT;
            fullsave.EndGameT = PlayerResource.Instance.EndGameT;
            fullsave.loadedBoardT = PlayerResource.Instance.loadedBoardT;
            fullsave.AdRewardT = PlayerResource.Instance.AdRewardT;
            fullsave.time = PlayerResource.Instance.time;
            fullsave.playedTime = PlayerResource.Instance.playedTime;
            fullsave.levelT = PlayerResource.Instance.levelT;
            fullsave.damageT = PlayerResource.Instance.damageT;

            fullsave.SaveTime = Convert.ToString(DateTime.UtcNow);
        }
    }

    public void SaveToJson() //сохраняем в JSON
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "FullSave.json");
#else
        path = Path.Combine(Application.dataPath, "FullSave.json");
#endif
        if (SceneManager.GetActiveScene().name != "Menu") //если мы не в меню
        {
            File.WriteAllText(path, SaveSystem.Encrypt(JsonUtility.ToJson(fullsave))); //кодируем сейвы и пишем закодированное в жсон
            Debug.LogWarning("SaveToJson Done!!!");
        }
    }


    public void ReadFromJson() //читаем из JSON
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "FullSave.json");
#else
        path = Path.Combine(Application.dataPath, "FullSave.json");
#endif
        if (File.Exists(path))
        {
            fullsave = JsonUtility.FromJson<FullSaves>(SaveSystem.Decrypt(File.ReadAllText(path))); //читаем из жсон и декодируем
            Debug.LogWarning("ReadFromJson Done!!!");
        }
    }

    public void LoadFromJson() //применяем данные из JSON
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "FullSave.json");
#else
        path = Path.Combine(Application.dataPath, "FullSave.json");
#endif
        if (File.Exists(path))
        {
            PlayerResource.Instance.scoreN = fullsave.scoreN;
            PlayerResource.Instance.hiScoreN = fullsave.hiScoreN;
            PlayerResource.Instance.hintN = fullsave.hintN;
            PlayerResource.Instance.refillN = fullsave.refillN;
            PlayerResource.Instance.heightN = fullsave.heightN;
            PlayerResource.Instance.widthN = fullsave.widthN;
            PlayerResource.Instance.EndGameN = fullsave.EndGameN;
            PlayerResource.Instance.loadedBoardN = fullsave.loadedBoardN;
            PlayerResource.Instance.AdRewardN = fullsave.AdRewardN;
            PlayerResource.Instance.levelN = fullsave.levelN;
            PlayerResource.Instance.damageN = fullsave.damageN;

            PlayerResource.Instance.scoreT = fullsave.scoreT;
            PlayerResource.Instance.hiScoreT = fullsave.hiScoreT;
            PlayerResource.Instance.hintT = fullsave.hintT;
            PlayerResource.Instance.refillT = fullsave.refillT;
            PlayerResource.Instance.heightT = fullsave.heightT;
            PlayerResource.Instance.widthT = fullsave.widthT;
            PlayerResource.Instance.EndGameT = fullsave.EndGameT;
            PlayerResource.Instance.loadedBoardT = fullsave.loadedBoardT;
            PlayerResource.Instance.AdRewardT = fullsave.AdRewardT;
            PlayerResource.Instance.time = fullsave.time;
            PlayerResource.Instance.playedTime = fullsave.playedTime;
            PlayerResource.Instance.levelT = fullsave.levelT;
            PlayerResource.Instance.damageT = fullsave.damageT;
            SaveTime = Convert.ToDateTime(fullsave.SaveTime);
        }
        else
        {
            int zeroInt = 0;

            PlayerResource.Instance.hiScoreN = SaveSystem.Encrypt(Convert.ToString(zeroInt));//иначе не начинает новую игру
            
            PlayerResource.Instance.hiScoreT = SaveSystem.Encrypt(Convert.ToString(zeroInt));
        }

        Debug.LogWarning("LoadFromJson Done!!!");
    }

    #endregion /Collect data


    #region Saved Games
    //многое из гайда, не трогать пока работает

    public void SaveToCloud() //сохраням в облако, вызываем везде этот метод
    {
        Debug.Log("SaveToCloud");

        if (Social.localUser.authenticated) //если залогинены
        {

            ((PlayGamesPlatform)Social.Active).SavedGame
                .OpenWithAutomaticConflictResolution(
                "Numbers",
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, DataToCloud);
        }
    }

    private void DataToCloud(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        Debug.Log("DataToCloud");

        if (status == SavedGameRequestStatus.Success)
        {
            CollectData(); //собираем данные
            string json = JsonUtility.ToJson(fullsave); //записываем в жсон

            byte[] data = Encoding.UTF8.GetBytes(json); //из жсона в дату, а ее отправляем в облако
            SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("Saved at " + DateTime.Now.ToString()).Build();

            ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, update, data, SaveToCloudStatus);

        }
    }

    //статус сохранения
    private void SaveToCloudStatus(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        Debug.LogWarning("SaveToCloudStatus " + status);
    }



    //грузим из облака
    public void LoadFromCloud() //для загрузки вызываем этот метод
    {
        Debug.Log("LoadFromCloud");

        if (Social.localUser.authenticated)
        {

            ((PlayGamesPlatform)Social.Active).SavedGame
            .OpenWithAutomaticConflictResolution(
             "Numbers",
             DataSource.ReadCacheOrNetwork,
             ConflictResolutionStrategy.UseLongestPlaytime, DataFromCloud);


        }
    }

    private void DataFromCloud(SavedGameRequestStatus status, ISavedGameMetadata meta) //загружаем данные из облака
    {
        Debug.Log("DataToCloud");

        if (status == SavedGameRequestStatus.Success)
        {
            ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, ReadFromCloud);

        }
    }
    private void ReadFromCloud(SavedGameRequestStatus status, byte[] data) //читаем загруженные данные 
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string saveData = Encoding.UTF8.GetString(data); //декодируем загруженное
            fullsave = JsonUtility.FromJson<FullSaves>(saveData); //применяем


#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "FullSave.json");
#else
            path = Path.Combine(Application.dataPath, "FullSave.json");
#endif

            File.WriteAllText(path, SaveSystem.Encrypt(JsonUtility.ToJson(fullsave))); //кодируем и пишем в жсон, не помню зачем оно тут, но лучше не трогать

            Debug.LogWarning("saveData " + saveData);
        }
    }

    #endregion /Saved Games


    #region Achievements
    public static void UnlockAchievement(string id) //запускаем этот метод с ид ачивки, когда ее полуает игрок
    {
        Social.ReportProgress(id, 100, success => { });
    }

    public static void IncrementAchievement(string id, int stepsToIncrement) //ачивка из нескольких частей, типа просмотрите 3 рекламы, передаем ид и количество шагов в ачивке
    {
        PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
    }

    public static void ShowAchievementUI() //показать уи ачивок
    {
        Social.ShowAchievementsUI();
    }
    #endregion /Achievements

    #region Leaderboards
    public static void AddScoreToLeaderboard(string leaderboardId, long score) //запускаем этот метод когда переадем данные в лидерборд, передаем ид лидерборда и данные в типе лонг
    {
        Social.ReportScore(score, leaderboardId, success => { });
    }

    public static void ShowLeaderboardsUI() //показать уи лидербордов
    {
        Social.ShowLeaderboardUI();
    }
    #endregion /Leaderboards

}

