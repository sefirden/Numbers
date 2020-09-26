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


    public void PauseClick() //клик на паузу в игре
    {
        Time.timeScale = 0f; //ставим паузу
        pauseMenuUI.SetActive(true); //включаем слой с меню паузы
        PlayerResource.Instance.GameIsPaused = true; //говорим переменной что тут пауза

        PlayServicesGoogle.Instance.CollectData(); //собираем данные
        PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON
        PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако

        AdMob_baner.Instance.Show(); //показываем банер внизу экрана

    }

    public void Resume() //вернуться в игру
    {
        Time.timeScale = 1f; //выключаем паузу
        pauseMenuUI.SetActive(false); //выключаем слой паузы
        SettingsLayer.SetActive(false); //выключаем слой настроек

        PlayerResource.Instance.GameIsPaused = false; //говорим что выключили паузу
        AdMob_baner.Instance.Hide(); //выключаем рекламный банер

    }

    public void Restart() //рестарт игры
    {
        AdMob_baner.Instance.Hide(); //выключаем рекламный банер

        board = FindObjectOfType<Board>(); //прикрепляем к переменной скрипт 

        if (PlayerResource.Instance.gameMode == "normal" && board != null) //если режим нормальный
        {
            //если сдесь делать не через board. а сразу в плеерресоурсес то не сработает
            board.endGame = false; //ставим что конец игры не тру
            board.hints = 3; //даем 3 подсказки
            board.refill = 1; //даем 1 перемешивание
            board.score = 0; //очки обнуляем
            board.AdReward = false; //говорим что рекламу не смотрел
            board.level = 0; //уровень ставим 0
            board.damage = 0; //обнуляем урон
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__normal_mode, board.hiScore); //отправляем лучшее время в Google Play
        }
        else if (PlayerResource.Instance.gameMode == "timetrial" && board != null) //см выше но для режима на время
        {
            PlayerResource.Instance.time = 120f; //даем 2 минуты в начале игры
            board.endGame = false;
            board.hints = 3;
            board.refill = 1;
            board.score = 0;
            board.AdReward = false;
            board.level = 0;
            board.damage = 0;
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_play_time_time_limit_mode, Convert.ToInt64(PlayerResource.Instance.playedTime * 1000)); //отправляем лучшее время в Google Play
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__time_limit_mode, board.hiScore); //отправляем лучшие очки в Google Play
        }

        SceneManager.LoadScene("Main"); //тупо загружаем основной уровень
        PlayerResource.Instance.GameIsPaused = false; //убираем паузу
        Time.timeScale = 1f;//убираем паузу

    }

    public void Menu() //кнопка в меню
    {
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
        FindObjectOfType<AudioManager>().Stop("Theme"); //выключаем музыку
        Application.Quit(); //закрываем приложение
    }
}
