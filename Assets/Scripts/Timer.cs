using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour

{
    //Скрипт для отображения собранных монет, прикреплён к префабу timertext
    public GameObject timerText; //таймер 
    public double timeMin; //минуты в таймере 
    public double timeSec; //секунды в таймере
    public GameObject NoTimeLayer;

    void Start()
    {
        //timerText = GetComponent<Text>(); //при старте присваиваем элемент UI
        if(PlayerResource.Instance.gameMode == "timetrial")
        {
            timerText.SetActive(true);
        }
        else if(PlayerResource.Instance.gameMode == "normal")
        {
            timerText.SetActive(false);
        }

    }



    void Update()
    {
        if (PlayerResource.Instance.GameIsPaused == false && PlayerResource.Instance.gameMode == "timetrial") //таймер отсчитывает назад на всех уровнях кроме пиццерии шопа и не отсчитывает когда умерли
        {
            PlayerResource.Instance.time -= Time.deltaTime; //отнимаем секунду

            timeMin = Math.Floor(PlayerResource.Instance.time / 60); //получаем целые минуты, округленные вниз до целого
            timeSec = Math.Floor(PlayerResource.Instance.time - (timeMin * 60)); //целые секунды, округленные вниз до целого
            if (timeSec > 9)
            {
                timerText.GetComponent<Text>().text = timeMin + ":" + timeSec; //при каждом обновлении кадра присваиваем тексту в UI значение из общего списка ресурсов
            }
            else
            {
                timerText.GetComponent<Text>().text = timeMin + ":0" + timeSec; //при каждом обновлении кадра присваиваем тексту в UI значение из общего списка ресурсов
            }
            if (PlayerResource.Instance.time <= 0) //если время вышло
            {
                timerText.SetActive(false);
                Time.timeScale = 0f;
                NoTimeLayer.SetActive(true);
                PlayerResource.Instance.GameIsPaused = true;
                PlayerResource.Instance.EndGameT = true;




                PlayServicesGoogle.Instance.CollectData(); //собираем данные
                PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON
                PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако

                AdMob_baner.Instance.Show();
            }
        }

    }
}

