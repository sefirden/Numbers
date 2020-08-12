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
    public GameObject SettingsLayer;
    private Board board;


    public void PauseClick()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        PlayerResource.Instance.GameIsPaused = true;

        PlayServicesGoogle.Instance.CollectData(); //собираем данные
        PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON
        PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако

        AdMob_baner.Instance.Show();

    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Invoke("ResumeToGame", 0.1f);
        AdMob_baner.Instance.Hide();

    }

    public void Restart()
    {
        AdMob_baner.Instance.Hide();

        board = FindObjectOfType<Board>();

        if (PlayerResource.Instance.gameMode == "normal" && board != null)
        {
            Debug.LogError("restart normal");
            board.endGame = false; //ставим что конец игры не тру
            board.hints = 3;
            board.refill = 1;
            board.score = 0;
            board.AdReward = false;
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__normal_mode, board.hiScore); //отправляем лучшее время в Google Play
        }
        else if (PlayerResource.Instance.gameMode == "timetrial" && board != null)
        {
            PlayerResource.Instance.time = 120f;
            board.endGame = false; //ставим что конец игры не тру
            board.hints = 3;
            board.refill = 1;
            board.score = 0;
            board.AdReward = false;
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_play_time_time_limit_mode, Convert.ToInt64(PlayerResource.Instance.playedTime * 1000)); //отправляем лучшее время в Google Play
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__time_limit_mode, board.hiScore); //отправляем лучшее время в Google Play
        }

        SceneManager.LoadScene("Main"); //тупо загружаем первый уровень, потом добавить сюда туториал
        PlayerResource.Instance.GameIsPaused = false; //убираем паузу
        Time.timeScale = 1f;//убираем паузу

    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
        AdMob_baner.Instance.Hide();
    }

    public void Setting()
    {
        SettingsLayer.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    public void Quit()
    {
        FindObjectOfType<AudioManager>().Stop("Theme");
        Application.Quit();
    }

    //хз что такое снизу
    private void ResumeToGame()
    {
        pauseMenuUI.SetActive(false);
        PlayerResource.Instance.GameIsPaused = false;
    }
}
