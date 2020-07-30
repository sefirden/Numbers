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


    public void PauseClick()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        PlayerResource.Instance.GameIsPaused = true;

        // PlayServicesGoogle.Instance.CollectData(); //собираем данные
        // PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON
        // PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако

        //   AdMob_baner.Instance.Show(Settings.Instance.ad_top_down);

    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Invoke("ResumeToGame", 0.1f);
        
       //  AdMob_baner.Instance.Hide(Settings.Instance.ad_top_down);

    }

    public void Restart()
    {
        SceneManager.LoadScene("Main"); //тупо загружаем первый уровень, потом добавить сюда туториал
        PlayerResource.Instance.GameIsPaused = false; //убираем паузу
        Time.timeScale = 1f;        //убираем паузу
        PlayerResource.Instance.EndGame = false; //ставим что конец игры не тру
        PlayerResource.Instance.hint = 3;
        PlayerResource.Instance.refill = 1;
        PlayerResource.Instance.score = 0;

        if (PlayerResource.Instance.gameMode == "normal")
        {
            PlayerResource.Instance.time = 0f;
        }
        else if (PlayerResource.Instance.gameMode == "timetrial")
        {
            PlayerResource.Instance.time = 120f;
        }

    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
        // AdMob_baner.Instance.Hide(Settings.Instance.ad_top_down);
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
