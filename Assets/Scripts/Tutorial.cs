using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public GameObject[] Tips; //список уровней

    // Start is called before the first frame update
    void Awake()
    {
        if (Settings.Instance.showtutorial == false && PlayerResource.Instance.isLoaded == false)
        {
            Debug.Log("Показываем туториал");
            gameObject.SetActive(true);
            //Time.timeScale = 0f; //ставим паузу
            PlayerResource.Instance.GameIsPaused = true;
        }
        else if (Settings.Instance.showtutorial == true)
        {
            Debug.Log("Выключаем туториал");
            gameObject.SetActive(false);
        }
        else if (Settings.Instance.showtutorial == false && PlayerResource.Instance.isLoaded == true)
        {
            Tips[0].SetActive(false);
            StartCoroutine(ShowTip());
        }
    }

    public void CloseTip()
    {
        Debug.Log("Close Tip");
        Tips[0].SetActive(false);
        gameObject.SetActive(false);
        Time.timeScale = 1f; //ставим паузу
        PlayerResource.Instance.GameIsPaused = false;
    }

    public void NextTip(int index)
    {
        switch (index)
        {
            case 2: //про ui
                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);
                break;
            case 3: //про соединение цифр
                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);
                break;
            case 4: //тут про длительность последовательности
                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);
                break;
            case 5: //тут про смену уровня
                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);
                break;
            case 6: //тут про время если режим на время или закрываем 5 подсказку
                if (PlayerResource.Instance.gameMode == "timetrial")
                {
                    Tips[index - 1].SetActive(false);
                    Tips[index].SetActive(true);
                    //тут про время
                }
                else
                {
                    Tips[index].SetActive(false);
                    gameObject.SetActive(false);
                    Settings.Instance.showtutorial = true;
                    SaveSystem.Instance.SettingsSave();
                    Time.timeScale = 1f; //ставим паузу
                    PlayerResource.Instance.GameIsPaused = false;
                }

                break;
            case 7: //тут закрываем про время подсказку
                Tips[index - 1].SetActive(false);
                gameObject.SetActive(false);
                Settings.Instance.showtutorial = true;
                SaveSystem.Instance.SettingsSave();
                Time.timeScale = 1f; //ставим паузу
                PlayerResource.Instance.GameIsPaused = false;
                break;

        }

    }

    public IEnumerator ShowTip() //про боссов
    {
        while (PlayerResource.Instance.TurnIsOn == true)
        {
            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(true);
        Tips[1].SetActive(true);
        //Time.timeScale = 0f; //ставим паузу
        PlayerResource.Instance.GameIsPaused = true;
        Debug.Log("Включаем подсказки в начале и из меню паузы");
    }
}
