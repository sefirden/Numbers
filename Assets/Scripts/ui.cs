using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Experimental.Rendering.Universal;


public class ui : MonoBehaviour
{

    //ниже все переменные интерфейса, какбы названия говорят сами за себя
    public GameObject timerimg;

    public Text damageText;
    public GameObject damagePic;

    public Text scoreText;
    public Text HighscoreText;
    public Text time;

    public Text adsConfirmText;
    public Text NoTimeScore;
    public Text NoTimeHiScore;
    public Text EndGameScore;
    public Text EndGameHiScore;

    public GameObject EndGameLayer;
    public GameObject NoMatchLayer;
    public GameObject AdsConfirmLayer;
    public GameObject AdsAlreadyShowLayer;
    public GameObject ButtonHolder;

    public Button Pause;
    public Button Tutorial;

    public Button DamageX2Button;
    public Button ScoreX2Button;
    public Button PlusTimeButton;
    public Button HintButton;
    public Button AdHintButton;    
    public Button AdRefillButton;
    public Button AdRefillButtonLayer;
    public Button RefillButton;
    public Button RefillButtonLayer;
    

    public GameObject lifeTurnLayer;
    public GameObject LifeBarBackground;
    public GameObject LifeBar;
    public Text lifeText;

    public GameObject turnLeft;
    public Text turnLeftText;

    public Pause PauseLayer;

    public Light2D[] Lights;
    public SpriteRenderer[] Lights_sprite;
    public Button[] ButtonHolderButtons;
    public bool[] ButtonInteractable;
    public Board board; //объект поля
    
    string buttonName;



    void Awake()
    {
        board = FindObjectOfType<Board>();
        //ебаные костыли из-за изменения камеры
        Vector3 temp = lifeTurnLayer.transform.position;

        lifeTurnLayer.transform.position = new Vector3(0, 10.25f, transform.position.z); //двигаем лайфбар и его бекграунд на стартовую позицию, сделано из-за разницы в размерах экранов
        lifeTurnLayer.transform.position = new Vector3(temp.x, LifeBarBackground.transform.position.y, transform.position.z); //двигаем лайфбар и его бекграунд на стартовую позицию, сделано из-за разницы в размерах экранов

        if (PlayerResource.Instance.gameMode == "timetrial")
        {
                PlusTimeButton.gameObject.SetActive(true);
        }
    }


#if UNITY_ANDROID // || UNITY_EDITOR
    void OnApplicationFocus(bool focusStatus) //при сворачивании игры ставит ее на паузу, даже если этого нделал игрок и как раз сейвит игру, если так работает, то можно убрать сейв при выходе из игры
    {
        if (focusStatus == false)
        {
            //PauseLayer = FindObjectOfType<Pause>();
            PauseLayer.PauseClick();
        }        
    }
#endif
    
    public void AdsConfirmShow(string buttonNameTemp)
    {
        if (PlayerResource.Instance.adsAlreadyLoading == true)
        {
            AdsAlreadyLoadingShow();
        }
        else
        {
            buttonName = buttonNameTemp;
            AdsConfirmLayer.SetActive(true);
            switch (buttonName) //в зависимости от размера поля меняем сложность (для рандома цифр) и размер обьектов поля
            {
                case "damagex2":
                    adsConfirmText.GetComponent<Text>().text = SaveSystem.GetText("ads_confirm_damage");
                    break;

                case "hint":
                    adsConfirmText.GetComponent<Text>().text = SaveSystem.GetText("ads_confirm_hint");
                    break;

                case "refill":
                    adsConfirmText.GetComponent<Text>().text = SaveSystem.GetText("ads_confirm_refill");
                    break;

                case "plustime":
                    adsConfirmText.GetComponent<Text>().text = SaveSystem.GetText("ads_confirm_plustime");
                    break;

                case "scorex2":
                    adsConfirmText.GetComponent<Text>().text = SaveSystem.GetText("ads_confirm_score");
                    break;

                default:
                    Debug.LogError("not found button");
                    break;
            }
            PlayerResource.Instance.GameIsPaused = true;
        }
        
    }

    private void AdsAlreadyLoadingShow()
    {
        AdsAlreadyShowLayer.SetActive(true);
        PlayerResource.Instance.GameIsPaused = true;
    }

    public void AdsAlreadyLoadingClose()
    {
        AdsAlreadyShowLayer.SetActive(false);
        PlayerResource.Instance.GameIsPaused = false;
    }

    public void AdsConfirmHide()
    {
        PlayerResource.Instance.GameIsPaused = false;
        AdsConfirmLayer.SetActive(false);
    }

    public void AdsStart()
    {        
        switch (buttonName) //в зависимости от размера поля меняем сложность (для рандома цифр) и размер обьектов поля
        {
            case "damagex2":
                board.AdTurnX2();
                break;

            case "hint":
                board.AdHint();
                break;

            case "refill":
                board.AdRefill();
                break;

            case "plustime":
                board.AdPlusTime();                
                break;

            case "scorex2":
                board.AdTurnX2();
                break;

            default:
                Debug.LogError("not found button");
                break;
        }
        PlayerResource.Instance.GameIsPaused = false;
        AdsConfirmLayer.SetActive(false);
        PlayerResource.Instance.adsAlreadyLoading = true;

    }

   /* public void SaveButtonInteractableStatus(bool status)
    {
        if (status == false) //сохраняем текущее состояние кнопок и выключаем интерактивность
{
            for(int i = 0; i < ButtonHolderButtons.Length; i++)
            {
                ButtonInteractable[i] = ButtonHolderButtons[i].interactable;
                ButtonHolderButtons[i].interactable = false;               
            }
            Tutorial.interactable = false;
        }
        else if (status == true) //восстанавливаем интерактивность согласно сохраненному
        {
            for (int i = 0; i < ButtonHolderButtons.Length; i++)
            {
                ButtonHolderButtons[i].interactable = ButtonInteractable[i];
            }
            if(board.level < PlayerResource.Instance.scoreToNextLevel.Length && PlayerResource.Instance.bossMove == false)
            Tutorial.interactable = true;
        }
    }*/




    public void BossHealth(int damage, int level) //этим скриптом обновляем значение хп боса, которое осталось
    {
        if (level != PlayerResource.Instance.scoreToNextLevel.Length) //если у нас не последний уровень
        {
            int scoreToNextLevel = 0;

            for (int j = 0; j < level; j++)
            {
                scoreToNextLevel += PlayerResource.Instance.scoreToNextLevel[j]; //количество очков, которые нужны для перехода на след уровень
            }

            if (PlayerResource.Instance.gameMode == "normal" && level == 0) //если нормальный режим и мы на нулевом уровне
            {
                LifeBar.GetComponent<Image>().fillAmount = 1f - (float)damage / (float)PlayerResource.Instance.scoreToNextLevel[level]; //опустошаем шкалу хп босса, 1 - урон / нужно дамага (типа нанесли 500, а хп всего 1000, то 1-(500/1000)
            }
            else if (PlayerResource.Instance.gameMode == "timetrial" && level == 0) //тоже что и выше, не помню зачем делил на режимы, пусть будет
            {
                LifeBar.GetComponent<Image>().fillAmount = 1f - (float)damage / (float)PlayerResource.Instance.scoreToNextLevel[level];
            }
            else if (PlayerResource.Instance.gameMode == "normal") //тоже что и выше, но для всех уровней кроме нулевого
            {
                LifeBar.GetComponent<Image>().fillAmount = 1f - (float)(damage - scoreToNextLevel) / (float)PlayerResource.Instance.scoreToNextLevel[level]; //опустошаем шкалу хп, от урона отнимаем весь лишний урон с пред уровней и делим на нужный дамаг
            }
            else if (PlayerResource.Instance.gameMode == "timetrial")
            {
                LifeBar.GetComponent<Image>().fillAmount = 1f - (float)(damage - scoreToNextLevel) / (float)PlayerResource.Instance.scoreToNextLevel[level]; //тоже что и выше, но для другого режима, хз зачем я так делал, если что удалить
            }

            lifeText.text = Convert.ToString(PlayerResource.Instance.scoreToNextLevel[level] - (damage - scoreToNextLevel)); //присваиваем текстовому полю оставшееся хп босса по формуле = нужно урона - (урон - уже нанесенный урон на пред уровнях)

        }
        else //если у нас последний уровень
        {
              LifeBarBackground.SetActive(false); //выключаем лайфбар
        }
    }

  /*  public void CheatLvlDamage()
    {
        board.level = 8;
        board.damage = SaveSystem.Encrypt(Convert.ToString(279900));
        board.ToPlayerResources("level");
        board.ToPlayerResources("damage");
    }*/
  
    public IEnumerator LightsOnOff(bool On) //включение света на уровне
    {
        if(On)
        {
            yield return new WaitForSeconds(0.5f);
            int i = 0;          
            while(i < Lights.Length)
            {
                Lights[i].GetComponent<Light2D>().pointLightInnerRadius = 0.5f;
                Lights[i].GetComponent<Light2D>().pointLightOuterRadius = 7.5f;
                Lights_sprite[i].GetComponent<SpriteRenderer>().color = new Color(0.9921569f, 0.9411765f, 0.8235294f, 1f);
                i++;
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
            }
        }
        else
        {            
            for (int i = 0; i < Lights.Length; i++)
            {
                Lights[i].GetComponent<Light2D>().pointLightInnerRadius = 0f;
                Lights[i].GetComponent<Light2D>().pointLightOuterRadius = 0f;
                Lights_sprite[i].GetComponent<SpriteRenderer>().color = new Color(0.9921569f, 0.9411765f, 0.8235294f, 0.4705882f);
            }
        }
    }
}
