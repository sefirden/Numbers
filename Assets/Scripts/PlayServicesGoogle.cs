using System.Collections;
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

    //public bool authenticated;
    public static PlayServicesGoogle Instance { get; private set; } //определяем

    public FullSaves fullsave = new FullSaves();
    private string path;
    public Button LeaderBoard;
    public Button Achievement;
    public Button CloudLoad;

    public DateTime SaveTime;
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

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        OnConnectionResponse(PlayGamesPlatform.Instance.localUser.authenticated);

        SignIn();
    }

    void SignIn()
    {

        Social.localUser.Authenticate((bool success) =>
        {
            OnConnectionResponse(success);
            Debug.LogWarning(success);
        });
    }


    private void OnConnectionResponse(bool authenticated)
    {
        if (authenticated)
        {
            LeaderBoard.interactable = true;
            Achievement.interactable = true;
            CloudLoad.interactable = true;

            Debug.Log("authenticated"); //показываем кнопки ачив и лидерборд
        }
        else
        {
            LeaderBoard.interactable = false;
            Achievement.interactable = false;
            CloudLoad.interactable = false;

            Debug.Log("not authenticated"); //прячем кнопки ачив и лидерборд
        }
    }
    #endregion Auth

    #region Collect data
    [Serializable]
    public class FullSaves
    {
        public bool GameIsPaused; //если игра на паузе

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

        public string SaveTime;
    }

    public void CollectData() //собираем дынные из игры
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            fullsave.scoreN = PlayerResource.Instance.scoreN;
            fullsave.hiScoreN = PlayerResource.Instance.hiScoreN;
            fullsave.hintN = PlayerResource.Instance.hintN;
            fullsave.refillN = PlayerResource.Instance.refillN;
            fullsave.heightN = PlayerResource.Instance.heightN;
            fullsave.widthN = PlayerResource.Instance.widthN;
            fullsave.EndGameN = PlayerResource.Instance.EndGameN;
            fullsave.loadedBoardN = PlayerResource.Instance.loadedBoardN;

            fullsave.scoreT = PlayerResource.Instance.scoreT;
            fullsave.hiScoreT = PlayerResource.Instance.hiScoreT;
            fullsave.hintT = PlayerResource.Instance.hintT;
            fullsave.refillT = PlayerResource.Instance.refillT;
            fullsave.heightT = PlayerResource.Instance.heightT;
            fullsave.widthT = PlayerResource.Instance.widthT;
            fullsave.EndGameT = PlayerResource.Instance.EndGameT;
            fullsave.loadedBoardT = PlayerResource.Instance.loadedBoardT;
            fullsave.time = PlayerResource.Instance.time;

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
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            File.WriteAllText(path, SaveSystem.Encrypt(JsonUtility.ToJson(fullsave)));
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
            fullsave = JsonUtility.FromJson<FullSaves>(SaveSystem.Decrypt(File.ReadAllText(path)));
            Debug.LogWarning("ReadFromJson Done!!!");
        }
    }

    public void LoadFromJson() //применяем данные из JSON
    {
        PlayerResource.Instance.scoreN = fullsave.scoreN;
        PlayerResource.Instance.hiScoreN = fullsave.hiScoreN;
        PlayerResource.Instance.hintN = fullsave.hintN;
        PlayerResource.Instance.refillN = fullsave.refillN;
        PlayerResource.Instance.heightN = fullsave.heightN;
        PlayerResource.Instance.widthN = fullsave.widthN;
        PlayerResource.Instance.EndGameN = fullsave.EndGameN;
        PlayerResource.Instance.loadedBoardN = fullsave.loadedBoardN;

        PlayerResource.Instance.scoreT = fullsave.scoreT;
        PlayerResource.Instance.hiScoreT = fullsave.hiScoreT;
        PlayerResource.Instance.hintT = fullsave.hintT;
        PlayerResource.Instance.refillT = fullsave.refillT;
        PlayerResource.Instance.heightT = fullsave.heightT;
        PlayerResource.Instance.widthT = fullsave.widthT;
        PlayerResource.Instance.EndGameT = fullsave.EndGameT;
        PlayerResource.Instance.loadedBoardT = fullsave.loadedBoardT;
        PlayerResource.Instance.time = fullsave.time;

        SaveTime = Convert.ToDateTime(fullsave.SaveTime);
        Debug.LogWarning("LoadFromJson Done!!!");
    }

    #endregion /Collect data


    #region Saved Games


    //save
    public void SaveToCloud() //true повесить на кнопку перед кнопкой продолжить
    {
        Debug.Log("SaveToCloud");

        if (Social.localUser.authenticated)
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
            CollectData();
            string json = JsonUtility.ToJson(fullsave); //возможно тут хня написана
            Debug.LogWarning(json);
            byte[] data = Encoding.UTF8.GetBytes(json);
            SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("Saved at " + DateTime.Now.ToString()).Build();

            ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, update, data, SaveToCloudStatus);

        }
    }

    //Success save
    private void SaveToCloudStatus(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        Debug.LogWarning("SaveToCloudStatus " + status);
    }



    //Load
    public void LoadFromCloud()
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

    private void DataFromCloud(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        Debug.Log("DataToCloud");

        if (status == SavedGameRequestStatus.Success)
        {
            ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, ReadFromCloud);

        }
    }
    private void ReadFromCloud(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string saveData = Encoding.UTF8.GetString(data);
            fullsave = JsonUtility.FromJson<FullSaves>(saveData); //пишем в файл, возможно хня


#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "FullSave.json");
#else
            path = Path.Combine(Application.dataPath, "FullSave.json");
#endif

            File.WriteAllText(path, SaveSystem.Encrypt(JsonUtility.ToJson(fullsave)));

            Debug.LogWarning("saveData " + saveData);
        }
    }

    #endregion /Saved Games


    #region Achievements
    public static void UnlockAchievement(string id)
    {
        Social.ReportProgress(id, 100, success => { });
    }

    public static void IncrementAchievement(string id, int stepsToIncrement)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
    }

    public static void ShowAchievementUI()
    {
        Social.ShowAchievementsUI();
    }
    #endregion /Achievements

    #region Leaderboards
    public static void AddScoreToLeaderboard(string leaderboardId, long score)
    {
        Social.ReportScore(score, leaderboardId, success => { });
    }

    public static void ShowLeaderboardsUI()
    {
        Social.ShowLeaderboardUI();
    }
    #endregion /Leaderboards

}

