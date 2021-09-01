using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{

    public GameObject pauseMenuUI;
    public GameObject restartMenuUI;
    public GameObject SettingsLayer;
    private Board board;
    public Tutorial tutorial;
    public Button turorial_button;

    void Update()
    {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                // Quit the application
                Resume();
            }
    }



    public void PauseClick() //клик на паузу в игре
    {
        pauseMenuUI.SetActive(true); //включаем слой с меню паузы
        PlayerResource.Instance.GameIsPaused = true; //говорим переменной что тут пауза
        AdMob_baner.Instance.Show(); //показываем банер внизу экрана

        board = FindObjectOfType<Board>(); //прикрепляем к переменной скрипт 

        if (PlayerResource.Instance.isLoaded == true)
        {
            if(board.level > 8)
            turorial_button.interactable = false;
        }

        StartCoroutine(SaveGame());
    }

    IEnumerator SaveGame()
    {
        while (PlayerResource.Instance.TurnIsOn == true)
        {
            yield return new WaitForFixedUpdate();
        }

        PlayServicesGoogle.Instance.CollectData(); //собираем данные
        PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON
        PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако
        Time.timeScale = 0f; //ставим паузу
    }

    public void Resume() //вернуться в игру
    {
        Time.timeScale = 1f; //выключаем паузу
        pauseMenuUI.SetActive(false); //выключаем слой паузы
        SettingsLayer.SetActive(false); //выключаем слой настроек

        PlayerResource.Instance.GameIsPaused = false; //говорим что выключили паузу
        AdMob_baner.Instance.Hide(); //выключаем рекламный банер

    }

    public void RestartLayer() //срабатывает по клику на кнопку рестарт
    {
        pauseMenuUI.SetActive(false); //прячем паузу
        restartMenuUI.SetActive(true); //показываем рестарт
    }

    public void Restart(bool answer) //рестарт игры, передаем тру или фалс по клику на да и нет соответственно из рестарт меню
    {
        board = FindObjectOfType<Board>(); //прикрепляем к переменной скрипт 

        if (answer == true) //если нажали да рестарт
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "Restart_Yes");

            if (restartMenuUI.activeSelf == true && board != null)
            {
                Debug.LogError("restart with firebase");
                Firebase.Analytics.Parameter[] EndGame =
    {
                    new Firebase.Analytics.Parameter("width", Convert.ToString(board.width)),
                    new Firebase.Analytics.Parameter("Why", "restart"),
                    new Firebase.Analytics.Parameter("level", Convert.ToString(board.level)),
                    new Firebase.Analytics.Parameter("GameMode", PlayerResource.Instance.gameMode),
                    new Firebase.Analytics.Parameter("score", Convert.ToInt32(SaveSystem.Decrypt(board.score))),
                };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("EndGame", EndGame);
            }

            int zeroInt = 0;
            AdMob_baner.Instance.Hide(); //выключаем рекламный банер
                       
            if (PlayerResource.Instance.gameMode == "normal" && board != null) //если режим нормальный
            {
                //если сдесь делать не через board. а сразу в плеерресоурсес то не сработает
                board.endGame = false; //ставим что конец игры не тру
                board.ToPlayerResources("endGame");
                board.hints = SaveSystem.Encrypt(Convert.ToString(3)); //даем 3 подсказки
                board.ToPlayerResources("hints");
                board.refill = SaveSystem.Encrypt(Convert.ToString(1)); //даем 1 перемешивание
                board.ToPlayerResources("refill");
                board.score = SaveSystem.Encrypt(Convert.ToString(zeroInt)); //обнуляем очки
                board.ToPlayerResources("score");
                board.AdReward = false; //говорим что рекламу не смотрел
                board.ToPlayerResources("AdReward");
                board.level = zeroInt; //уровень ставим 0
                board.ToPlayerResources("level");
                board.damage = zeroInt; //обнуляем урон
                board.ToPlayerResources("damage");

                PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__normal_mode, Convert.ToInt32(SaveSystem.Decrypt(board.hiScore))); //отправляем лучшее время в Google Play
            }
            else if (PlayerResource.Instance.gameMode == "timetrial" && board != null) //см выше но для режима на время
            {
                PlayerResource.Instance.time = 120f; //даем 2 минуты в начале игры
                PlayerResource.Instance.starttimer = false;
                board.endGame = false; //ставим что конец игры не тру
                board.ToPlayerResources("endGame");
                board.hints = SaveSystem.Encrypt(Convert.ToString(3)); //даем 3 подсказки
                board.ToPlayerResources("hints");
                board.refill = SaveSystem.Encrypt(Convert.ToString(1)); //даем 1 перемешивание
                board.ToPlayerResources("refill");
                board.score = SaveSystem.Encrypt(Convert.ToString(zeroInt)); //обнуляем очки
                board.ToPlayerResources("score");
                board.AdReward = false; //говорим что рекламу не смотрел
                board.ToPlayerResources("AdReward");
                board.level = zeroInt; //уровень ставим 0
                board.ToPlayerResources("level");
                board.damage = zeroInt; //обнуляем урон
                board.ToPlayerResources("damage");
                PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_play_time_time_limit_mode, Convert.ToInt64(PlayerResource.Instance.playedTime * 1000)); //отправляем лучшее время в Google Play
                PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__time_limit_mode, Convert.ToInt32(SaveSystem.Decrypt(board.hiScore))); //отправляем лучшие очки в Google Play
            }

            SceneManager.LoadScene("Main"); //тупо загружаем основной уровень
            PlayerResource.Instance.GameIsPaused = false; //убираем паузу
            Time.timeScale = 1f;//убираем паузу
        }
        else //если нажали нет рестарту
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "Restart_No");
            pauseMenuUI.SetActive(true);
            restartMenuUI.SetActive(false);
        }
    }


    public void Tutorial()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "Tutorial");
        StartCoroutine(tutorial.ShowTip());
        Time.timeScale = 1f; //выключаем паузу
        pauseMenuUI.SetActive(false); //выключаем слой паузы
        SettingsLayer.SetActive(false); //выключаем слой настроек
        AdMob_baner.Instance.Hide(); //выключаем рекламный банер
    }

    public void Menu() //кнопка в меню
    {
        FindObjectOfType<AudioManager>().StopShuffle();
        FindObjectOfType<AudioManager>().Play("music_menu");
        SceneManager.LoadScene("Menu"); //загружаем сцену меню
        AdMob_baner.Instance.Hide(); //выключаем рекламный банер
    }

    public void Setting() //кнопка настройки
    {
        SettingsLayer.SetActive(true); //включаем слой настройки
        pauseMenuUI.SetActive(false); //выключаем основной слой паузы
    }

    public void Quit() //кнопка выход
    {
        FindObjectOfType<AudioManager>().StopShuffle();
        Application.Quit(); //закрываем приложение
    }
}
