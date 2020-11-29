using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using TMPro;


public class Timer : MonoBehaviour

{
    public GameObject timer;
    public GameObject timerText; //таймер 
    public double timeMin; //минуты в таймере 
    public double timeSec; //секунды в таймере
    public GameObject NoTimeLayer; //слой когда кончилось время
    private ui ui;

    void Start()
    {
        ui = FindObjectOfType<ui>();//прикрепляем крипт ui

        if (PlayerResource.Instance.gameMode == "timetrial") //если режим игры на время, то показываем таймер
        {
            timer.SetActive(true);
        }
        else if(PlayerResource.Instance.gameMode == "normal") //или нет
        {
            timer.SetActive(false);
        }

    }



    void Update()
    {
        if (PlayerResource.Instance.GameIsPaused == false && PlayerResource.Instance.gameMode == "timetrial") //таймер отсчитывает назад в режиме игры на время и когда нет паузы
        {
            PlayerResource.Instance.time -= Time.deltaTime; //отнимаем секунду

            PlayerResource.Instance.playedTime += Time.deltaTime; //сыгранное время, для лидерборда

            timeMin = Math.Floor(PlayerResource.Instance.time / 60); //получаем целые минуты, округленные вниз до целого
            timeSec = Math.Floor(PlayerResource.Instance.time - (timeMin * 60)); //целые секунды, округленные вниз до целого
            if (timeSec > 9)
            {
                timerText.GetComponent<Text>().text = timeMin + ":" + timeSec; //если секунд больше 9
            }
            else
            {
                timerText.GetComponent<Text>().text = timeMin + ":0" + timeSec; //если секунд меньше или равно 9, то добавляем 0 типа 09, 08
            }
            if (PlayerResource.Instance.time <= 0) //если время вышло
            {
                timerText.SetActive(false); //выключаем таймер
                Time.timeScale = 0f;
                NoTimeLayer.SetActive(true); //показываем всплывающее окно, что время вышло

                ui.NoTimeScore.text = SaveSystem.Decrypt(PlayerResource.Instance.scoreT); //присваиваем очки и мак очки для всплывающего окна
                ui.NoTimeHiScore.text = SaveSystem.Decrypt(PlayerResource.Instance.hiScoreT);

                PlayerResource.Instance.GameIsPaused = true; //ставим паузу
                PlayerResource.Instance.EndGameT = true; //ставим конец игры

                PlayServicesGoogle.UnlockAchievement(GPGSIds.achievement_end_game); //ачивка прошел игру получена
                PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_play_time_time_limit_mode, Convert.ToInt64(PlayerResource.Instance.playedTime * 1000)); //отправляем лучшее время в Google Play
                PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__time_limit_mode, Convert.ToInt32(SaveSystem.Decrypt(PlayerResource.Instance.hiScoreT))); //отправляем лучшие очки в Google Play

                PlayServicesGoogle.Instance.CollectData(); //собираем данные
                PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON
                PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако

                AdMob_baner.Instance.Show(); //показываем банер
            }
        }

    }
}

