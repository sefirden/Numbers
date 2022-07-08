using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Tutorial : MonoBehaviour
{

    public GameObject[] Tips; //список уровней
    private zeroPlayer zero; //скрипт ноля
    private bossPlayer boss; //скрипт босса
    public Board board; //объект поля
    private ui ui; //скрипт всего УИ
    public GameObject fingerBoss;
    public GameObject fingerLineLife;
    public GameObject tip3NextTime;
    public GameObject tip3Next;
    public GameObject tip6Close;
    public GameObject tip6Next;
    public SpriteRenderer[] NumbersAndLines; //список уровней

    // Start is called before the first frame update
    void Awake()
    {
        zero = FindObjectOfType<zeroPlayer>(); ////присваиваем скрипт к переменной
        boss = FindObjectOfType<bossPlayer>(); ////присваиваем скрипт к переменной
        board = FindObjectOfType<Board>();
        ui = FindObjectOfType<ui>(); //присваиваем скрипт к переменным

        if (Settings.Instance.showtutorial == false && PlayerResource.Instance.isLoaded == false)
        {
            gameObject.SetActive(true);
            Tips[0].SetActive(true);
            PlayerResource.Instance.GameIsPaused = true;
        }
        else if (Settings.Instance.showtutorial == true)
        {
            gameObject.SetActive(false);
            Tips[0].SetActive(false);
        }
        else if (Settings.Instance.showtutorial == false && PlayerResource.Instance.isLoaded == true && board.level < 9)
        {
            Tips[0].SetActive(false);
            StartCoroutine(ShowTip());
        }
        else if(Settings.Instance.showtutorial == false && PlayerResource.Instance.isLoaded == true && board.level > 8)
        {
            gameObject.SetActive(false);
        }


    }

    public void CloseTip()
    {
        Debug.Log("Close Tip");
        Tips[0].SetActive(false);
        gameObject.SetActive(false);
        Time.timeScale = 1f; //убираем паузу
        PlayerResource.Instance.GameIsPaused = false;
    }

    public void NextTip(int index)
    {
        switch (index)
        {
            case 2: //2 про дамаг
                ui.SaveButtonInteractableStatus(false);
                boss.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100;
                boss.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

                ui.DamageX2Button.GetComponent<ButtonController>().Finger.SetActive(true);

                ui.ButtonHolder.gameObject.GetComponent<Transform>().SetSiblingIndex(11); //7
                ui.DamageX2Button.interactable = true;
                ui.HintButton.interactable = false;
                ui.RefillButton.interactable = false;


                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);
                break;
            case 3: //про hint         
                ui.HintButton.GetComponent<ButtonController>().Finger.SetActive(true);
                ui.AdHintButton.GetComponent<ButtonController>().Finger.SetActive(true);
                ui.AdHintButton.interactable = true;
                ui.HintButton.interactable = true;

                ui.DamageX2Button.GetComponent<ButtonController>().Finger.SetActive(false);
                ui.DamageX2Button.interactable = false;

                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);

                if (PlayerResource.Instance.gameMode == "timetrial")
                {
                    tip3NextTime.SetActive(true);
                }
                else
                {
                    tip3Next.SetActive(true);
                }

                break;
            case 4://4 про таймплюс в режиме на время
                ui.PlusTimeButton.GetComponent<ButtonController>().Finger.SetActive(true);
                ui.PlusTimeButton.interactable = true;

                ui.HintButton.GetComponent<ButtonController>().Finger.SetActive(false);
                ui.AdHintButton.GetComponent<ButtonController>().Finger.SetActive(false);
                ui.HintButton.interactable = false;
                ui.AdHintButton.interactable = false;


                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);
                break;
            case 5: //про refill
                if (PlayerResource.Instance.gameMode == "timetrial")
                {
                    ui.PlusTimeButton.GetComponent<ButtonController>().Finger.SetActive(false);
                    ui.PlusTimeButton.interactable = false;
                    Tips[index - 1].SetActive(false);
                    Tips[index].SetActive(true);
                }
                else
                {
                    ui.HintButton.GetComponent<ButtonController>().Finger.SetActive(false);
                    ui.AdHintButton.GetComponent<ButtonController>().Finger.SetActive(false);
                    ui.AdHintButton.interactable = false;
                    ui.HintButton.interactable = false;
                    Tips[index - 2].SetActive(false);
                    Tips[index].SetActive(true);
                }

                ui.RefillButton.GetComponent<ButtonController>().Finger.SetActive(true);
                ui.AdRefillButton.GetComponent<ButtonController>().Finger.SetActive(true);
                ui.RefillButton.interactable = true;
                ui.AdRefillButton.interactable = true;
                break;
            case 6: //про соединение цифр
                ui.RefillButton.GetComponent<ButtonController>().Finger.SetActive(false);
                ui.AdRefillButton.GetComponent<ButtonController>().Finger.SetActive(false);
                ui.ButtonHolder.gameObject.GetComponent<Transform>().SetSiblingIndex(6); //11
                ui.SaveButtonInteractableStatus(true);


                NumbersAndLines = new SpriteRenderer[board.transform.childCount];
                NumbersAndLines = board.gameObject.GetComponentsInChildren<SpriteRenderer>();

                foreach (SpriteRenderer k in NumbersAndLines)
                {
                    k.sortingOrder = 201;
                    k.sortingLayerName = "super_hi";
                }

                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);
                break;
            case 7: //тут про длительность последовательности                
                board.Hint(0, 0);
                foreach (LineRenderer k in board.lines)
                {
                    k.sortingOrder = 250;
                    k.sortingLayerName = "super_hi";
                }              

                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);
                break;
            case 8: //тут урон и про смену уровня

                foreach (SpriteRenderer k in NumbersAndLines)
                {
                    k.sortingOrder = 20;
                    k.sortingLayerName = "Default";
                }
                Array.Clear(NumbersAndLines, 0, NumbersAndLines.Length);

                foreach (LineRenderer k in board.lines)
                {
                    k.sortingOrder = 25;
                    k.sortingLayerName = "Default";
                }
                board.Draw(false);

                if (PlayerResource.Instance.gameMode == "timetrial")
                {
                    tip6Next.SetActive(true);
                    tip6Close.SetActive(false);
                }
                else
                {
                    tip6Close.SetActive(true);
                    tip6Next.SetActive(false);
                }

                ui.lifeTurnLayer.gameObject.GetComponent<Transform>().SetSiblingIndex(10);
                fingerLineLife.transform.position = new Vector3(3f, 12f, transform.position.z);


                Tips[index - 1].SetActive(false);
                Tips[index].SetActive(true);
                break;
            case 9: //тут про время если режим на время или закрываем 5 подсказку

                ui.lifeTurnLayer.gameObject.GetComponent<Transform>().SetSiblingIndex(3);

                if (PlayerResource.Instance.gameMode == "timetrial")
                {
                    ui.timerimg.gameObject.GetComponent<Transform>().SetSiblingIndex(10);


                    Tips[index - 1].SetActive(false);
                    Tips[index].SetActive(true);
                    //тут про время
                }
                else
                {
                    Tips[index - 1].SetActive(false);
                    Tips[index].SetActive(false);
                    gameObject.SetActive(false);
                    ui.Tutorial.interactable = true;
                    Settings.Instance.showtutorial = true;
                    SaveSystem.Instance.SettingsSave();
                    Time.timeScale = 1f; //убираем паузу
                    PlayerResource.Instance.GameIsPaused = false;                 
                }

                break;
            case 10: //тут закрываем про время подсказку
                ui.timerimg.gameObject.GetComponent<Transform>().SetSiblingIndex(3);
                Tips[index - 1].SetActive(false);
                gameObject.SetActive(false);
                ui.Tutorial.interactable = true;
                Settings.Instance.showtutorial = true;
                SaveSystem.Instance.SettingsSave();
                Time.timeScale = 1f; //ставим паузу
                PlayerResource.Instance.GameIsPaused = false;
                
                break;

        }

    }

    public IEnumerator ShowTip() //про боссов
    {

            while (PlayerResource.Instance.TurnIsOn == true || Tips[0].activeSelf == true)
            {
                yield return new WaitForFixedUpdate();
            }


            board.Draw(false);
            ui.Tutorial.interactable = false;
            gameObject.GetComponent<Image>().color = new Color(0.4823529f, 0.5529412f, 0.5882353f, 0.5882353f);
            boss.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 201;
            boss.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "super_hi";
            fingerBoss.transform.position = new Vector3(3f, 12f, transform.position.z);
            
            gameObject.SetActive(true);            
            Tips[1].SetActive(true);
            //Time.timeScale = 0f; //ставим паузу
            PlayerResource.Instance.GameIsPaused = true;           

    }
}
