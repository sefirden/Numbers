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


    public void PauseClick()
    {
        Time.timeScale = 0f;

        pauseMenuUI.SetActive(true);
        PlayerResource.Instance.GameIsPaused = true;

        // PlayServicesGoogle.Instance.CollectData(); //собираем данные
        // PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON

        // PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако

        if (SceneManager.GetActiveScene().name != "pizzeria_shop")
        {
            //   AdMob_baner.Instance.Show(Settings.Instance.ad_top_down);
        }

    }

    public void BackToGame()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        PlayerResource.Instance.GameIsPaused = false;
    }


}
