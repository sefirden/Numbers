using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
using UnityEngine.Events;


public class AdMob_baner : MonoBehaviour
{
    private BannerView adBannerDown; //банер внизу экрана

    private RewardBasedVideoAd adRewardHint; //видео за подсказки
    private RewardBasedVideoAd adRewardRefill; //видео за перемешивание поля

    private RewardBasedVideoAd adRewardTurnX2; //видео за перемешивание поля
    private RewardBasedVideoAd adRewardPlusTime; //видео за перемешивание поля

    private Board board; //поле

    //переменные для отслеживания была реклама просмотрена или закрыта, работает только так, по гайду адмоб не работает
    public bool rewardHint;
    public bool rewardRefill;
    public bool rewardTurnX2;
    public bool rewardPlusTime;

    public bool closeHint;
    public bool closeRefill;
    public bool closeTurnX2;
    public bool closePlusTime;


    private string idApp, idBanner, idReward; //стринговые ид приложения, банера и видео

    public static AdMob_baner Instance { get; private set; } //определяем

    private void Awake() //запускается до всех стартов
    {
        if (Instance == null) //если объекта ещё нет
        {
            Instance = this; //говорим что вот кагбе он
            DontDestroyOnLoad(gameObject); //и говорим что его нельзя ломать между уровнями, иначе он нахер не нужен
        }
        else //но, если вдруг на уровне такой уже есть
        {
            Destroy(gameObject); //то ломаем его к херам
        }

    }

    private void Update() //такая херня, что в апдейте надо проверять была ли реклама закрыта или просмотрена до конца, и помле этого запускать нужный метод в скрипте боард
    {
        if (rewardHint) //если реклама для подсказок была просмотрена
        {
            board = FindObjectOfType<Board>(); //ищем поле
            board.AdHintRecieve(); //запускаем метод в скрипте боард, который присваивает уже награду
            rewardHint = false; //тут говорим что реклама не просмотрена, это надо чтобы можно было еще раз смотреть рекламу и получить награду
        }

        if(closeHint) //если реклама была закрыта
        {
            board = FindObjectOfType<Board>(); //ищем поле
            board.AdHintClose(); //запускаем метод в скрипте боард, который обрабатывает закрытие рекламы (возвращает кнопки и итд.)
            closeHint = false; //говорим что реклама не была закрыта, иначе повторно закрытая реклама не сработает
        }

        if (rewardRefill) //см выше, но для перемешивания поля
        {
            board = FindObjectOfType<Board>();
            board.AdRefillRecieve();
            rewardRefill = false;
        }

        if (closeRefill)
        {
            board = FindObjectOfType<Board>();
            board.AdRefillClose();
            closeRefill = false;
        }

        if (rewardTurnX2) //см выше, но для перемешивания поля
        {
            board = FindObjectOfType<Board>();
            board.AdTurnX2Recieve();
            rewardTurnX2 = false;
        }

        if (closeTurnX2)
        {
            board = FindObjectOfType<Board>();
            board.AdTurnX2Close();
            closeTurnX2 = false;
        }

        if (rewardPlusTime) //см выше, но для перемешивания поля
        {
            board = FindObjectOfType<Board>();
            board.AdPlusTimeRecieve();
            rewardPlusTime = false;
        }

        if (closePlusTime)
        {
            board = FindObjectOfType<Board>();
            board.AdPlusTimeClose();
            closePlusTime = false;
        }

    }

    void Start()
    {
        //это тестовые ид, взять оригинальные перед софт лаунчем приложения и изменить
        idApp = "ca-app-pub-6603668453005485~7265657750";
        idBanner = "ca-app-pub-6603668453005485/9269198086";
        idReward = "ca-app-pub-6603668453005485/2512218040";

        //ниже по гайду от адмоб, не понимаю зачем все это и как
        MobileAds.Initialize(idApp);

        RequestBannerAd(); //сразу при старте приложения запрашиваем банерную рекламу

        adRewardHint = RewardBasedVideoAd.Instance;
        adRewardRefill = RewardBasedVideoAd.Instance;
        adRewardTurnX2 = RewardBasedVideoAd.Instance;
        adRewardPlusTime = RewardBasedVideoAd.Instance;
    }



    #region Banner Methods --------------------------------------------------

    public void RequestBannerAd() //запрашиваем банерную рекламу
    {
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth); //тут про то что она адаптивная и по всей ширине экрана
        
        adBannerDown = new BannerView(idBanner, adaptiveSize, AdPosition.Bottom); //а тут про то что она внизу экрана
       
        //переопределяем методы, без этого не работает
        adBannerDown.OnAdLoaded += HandleAdLoaded;
        adBannerDown.OnAdFailedToLoad += HandleAdFailedToLoad;
        adBannerDown.OnAdOpening += HandleAdOpened;
        adBannerDown.OnAdClosed += HandleAdClosed;
        adBannerDown.OnAdLeavingApplication += HandleAdLeftApplication;

        AdRequest request = AdRequestBuild(); //запрашиваем рекламу
        adBannerDown.LoadAd(request); //загружаем
        Hide(); //и прячем, нам нужно показывать банер только в определенных местах

    }

    //ниже ебала по гайду от адмоб, там вроде ничего такого, по хорошему там надо отрабатывать неудачные запросы и тд, но для банерной похер
    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        MonoBehaviour.print(String.Format("Ad Height: {0}, width: {1}",
            adBannerDown.GetHeightInPixels(),
            adBannerDown.GetWidthInPixels()));
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
                "HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }



    public void Hide() //прячет банер
    {
        adBannerDown.Hide();
    }
    public void Show() //показывает банер
    {   
        adBannerDown.Show();
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "Show", "Banner");
    }

    public void DestroyBannerAd() //уничтожает банер, после этого надо заново отправвлять реквест
    {
        if (adBannerDown != null)
            adBannerDown.Destroy();
    }

    #endregion

    #region REWARDED ADS Turnx2

    public void RequestRewardAdTurnX2() //запрашиваем видео рекламу
    {
        AdRequest request = AdRequestBuild();
        adRewardTurnX2.LoadAd(request, idReward);

        //переопределяем базовые методы на свои
        adRewardTurnX2.OnAdLoaded += this.HandleOnRewardedAdLoadedTurnX2;
        adRewardTurnX2.OnAdRewarded += this.HandleOnAdRewardedTurnX2;
        adRewardTurnX2.OnAdClosed += this.HandleOnRewardedAdClosedTurnX2;
        adRewardTurnX2.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoadTurnX2;
    }

    public IEnumerator ShowRewardAdTurnX2() //показываем видео
    {
        if (adRewardTurnX2.IsLoaded())
        {
            while (PlayerResource.Instance.TurnIsOn == true)
            {
                yield return new WaitForFixedUpdate();
            }

            adRewardTurnX2.Show(); //показывает сразу как загрузится
        }
    }

    public void HandleOnRewardedAdLoadedTurnX2(object sender, EventArgs args) //когда реклама загружена
    {
        StartCoroutine(ShowRewardAdTurnX2()); //показывает видео
    }

    public void HandleOnAdRewardedTurnX2(object sender, EventArgs args) //когда видео досмотрели до конца
    {
        rewardTurnX2 = true; //тут говорим что просмотрели, дальше см в апдейте
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "Show", "TurnX2");
    }

    public void HandleRewardBasedVideoFailedToLoadTurnX2(object sender, AdFailedToLoadEventArgs args) //если реклама не загрузилась, например нажали кнопку а интернета нет, это этот случай
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
        closeTurnX2 = true; //говорим что реклама была закрыта, см в апдейте, иначе кнопка загрузки не отлипнет с нажатого состояния когде нет инета
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "FailToLoad", "TurnX2");

        adRewardTurnX2.OnAdLoaded -= this.HandleOnRewardedAdLoadedTurnX2;
        adRewardTurnX2.OnAdRewarded -= this.HandleOnAdRewardedTurnX2;
        adRewardTurnX2.OnAdClosed -= this.HandleOnRewardedAdClosedTurnX2;
        adRewardTurnX2.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadTurnX2;
    }

    public void HandleOnRewardedAdClosedTurnX2(object sender, EventArgs args) //когда рекламу закрыл пользователь сам
    {
        closeTurnX2 = true; //говорим что реклама закрыта, см в апдейте
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "Close", "TurnX2");

        adRewardTurnX2.OnAdLoaded -= this.HandleOnRewardedAdLoadedTurnX2;
        adRewardTurnX2.OnAdRewarded -= this.HandleOnAdRewardedTurnX2;
        adRewardTurnX2.OnAdClosed -= this.HandleOnRewardedAdClosedTurnX2;
        adRewardTurnX2.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadTurnX2;
    }

    #endregion

    #region REWARDED ADS PlusTime

    public void RequestRewardAdPlusTime() //запрашиваем видео рекламу
    {
        AdRequest request = AdRequestBuild();
        adRewardPlusTime.LoadAd(request, idReward);

        //переопределяем базовые методы на свои
        adRewardPlusTime.OnAdLoaded += this.HandleOnRewardedAdLoadedPlusTime;
        adRewardPlusTime.OnAdRewarded += this.HandleOnAdRewardedPlusTime;
        adRewardPlusTime.OnAdClosed += this.HandleOnRewardedAdClosedPlusTime;
        adRewardPlusTime.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoadPlusTime;
    }

    public IEnumerator ShowRewardAdPlusTime() //показываем видео
    {
        if (adRewardPlusTime.IsLoaded())
        {
            while (PlayerResource.Instance.TurnIsOn == true)
            {
                yield return new WaitForFixedUpdate();
            }

            adRewardPlusTime.Show(); //показывает сразу как загрузится
        }
    }

    public void HandleOnRewardedAdLoadedPlusTime(object sender, EventArgs args) //когда реклама загружена
    {
        StartCoroutine(ShowRewardAdPlusTime()); //показывает видео
    }

    public void HandleOnAdRewardedPlusTime(object sender, EventArgs args) //когда видео досмотрели до конца
    {
        rewardPlusTime = true; //тут говорим что просмотрели, дальше см в апдейте
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "Show", "PlusTime");
    }

    public void HandleRewardBasedVideoFailedToLoadPlusTime(object sender, AdFailedToLoadEventArgs args) //если реклама не загрузилась, например нажали кнопку а интернета нет, это этот случай
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
        closePlusTime = true; //говорим что реклама была закрыта, см в апдейте, иначе кнопка загрузки не отлипнет с нажатого состояния когде нет инета
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "FailToLoad", "PlusTime");

        adRewardPlusTime.OnAdLoaded -= this.HandleOnRewardedAdLoadedPlusTime;
        adRewardPlusTime.OnAdRewarded -= this.HandleOnAdRewardedPlusTime;
        adRewardPlusTime.OnAdClosed -= this.HandleOnRewardedAdClosedPlusTime;
        adRewardPlusTime.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadPlusTime;
    }

    public void HandleOnRewardedAdClosedPlusTime(object sender, EventArgs args) //когда рекламу закрыл пользователь сам
    {
        closePlusTime = true; //говорим что реклама закрыта, см в апдейте
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "Close", "PlusTime");

        adRewardPlusTime.OnAdLoaded -= this.HandleOnRewardedAdLoadedPlusTime;
        adRewardPlusTime.OnAdRewarded -= this.HandleOnAdRewardedPlusTime;
        adRewardPlusTime.OnAdClosed -= this.HandleOnRewardedAdClosedPlusTime;
        adRewardPlusTime.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadPlusTime;
    }

    #endregion


    #region REWARDED ADS Hint

    public void RequestRewardAdHint() //запрашиваем видео рекламу
    {
        AdRequest request = AdRequestBuild();
        adRewardHint.LoadAd(request, idReward);

        //переопределяем базовые методы на свои
        adRewardHint.OnAdLoaded += this.HandleOnRewardedAdLoadedHint;
        adRewardHint.OnAdRewarded += this.HandleOnAdRewardedHint;
        adRewardHint.OnAdClosed += this.HandleOnRewardedAdClosedHint;
        adRewardHint.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoadHint;
    }

    public IEnumerator ShowRewardAdHint() //показываем видео
    {
        if (adRewardHint.IsLoaded())
        {
            while (PlayerResource.Instance.TurnIsOn == true)
            {
                yield return new WaitForFixedUpdate();
            }

            adRewardHint.Show(); //показывает сразу как загрузится
        }
    }
    
    public void HandleOnRewardedAdLoadedHint(object sender, EventArgs args) //когда реклама загружена
    {
        StartCoroutine(ShowRewardAdHint()); //показывает видео
    }

    public void HandleOnAdRewardedHint(object sender, EventArgs args) //когда видео досмотрели до конца
    {
        rewardHint = true; //тут говорим что просмотрели, дальше см в апдейте
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "Show", "Hint");
    }

    public void HandleRewardBasedVideoFailedToLoadHint(object sender, AdFailedToLoadEventArgs args) //если реклама не загрузилась, например нажали кнопку а интернета нет, это этот случай
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
        closeHint = true; //говорим что реклама была закрыта, см в апдейте, иначе кнопка загрузки не отлипнет с нажатого состояния когде нет инета
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "FailToLoad", "Hint");

        adRewardHint.OnAdLoaded -= this.HandleOnRewardedAdLoadedHint;
        adRewardHint.OnAdRewarded -= this.HandleOnAdRewardedHint;
        adRewardHint.OnAdClosed -= this.HandleOnRewardedAdClosedHint;
        adRewardHint.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadHint;
    }

    public void HandleOnRewardedAdClosedHint(object sender, EventArgs args) //когда рекламу закрыл пользователь сам
    {
        closeHint = true; //говорим что реклама закрыта, см в апдейте
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "Close", "Hint");

        adRewardHint.OnAdLoaded -= this.HandleOnRewardedAdLoadedHint;
        adRewardHint.OnAdRewarded -= this.HandleOnAdRewardedHint;
        adRewardHint.OnAdClosed -= this.HandleOnRewardedAdClosedHint;
        adRewardHint.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadHint;
    }

    #endregion

    #region REWARDED ADS Refill 

    //все тоже самое что и выше, но для перемешивания поля, а не для подсказок

    public void RequestRewardAdRefill()
    {
        AdRequest request = AdRequestBuild();
        adRewardRefill.LoadAd(request, idReward);

        adRewardRefill.OnAdLoaded += this.HandleOnRewardedAdLoadedRefill;
        adRewardRefill.OnAdRewarded += this.HandleOnAdRewardedRefill;
        adRewardRefill.OnAdClosed += this.HandleOnRewardedAdClosedRefill;
        adRewardRefill.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoadRefill;
    }

    public IEnumerator ShowRewardAdRefill()
    {
        if (adRewardRefill.IsLoaded())
        {
            while (PlayerResource.Instance.TurnIsOn == true)
            {
                yield return new WaitForFixedUpdate();
            }

            adRewardRefill.Show();
        }

    }

    public void HandleOnRewardedAdLoadedRefill(object sender, EventArgs args)
    {
        StartCoroutine(ShowRewardAdRefill());
    }

    public void HandleOnAdRewardedRefill(object sender, EventArgs args)
    {
        rewardRefill = true;
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "Show", "Refill");
    }

    public void HandleRewardBasedVideoFailedToLoadRefill(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
        closeRefill = true;
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "FailToLoad", "Refill");

        adRewardRefill.OnAdLoaded -= this.HandleOnRewardedAdLoadedRefill;
        adRewardRefill.OnAdRewarded -= this.HandleOnAdRewardedRefill;
        adRewardRefill.OnAdClosed -= this.HandleOnRewardedAdClosedRefill;
        adRewardRefill.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadRefill;

    }

    public void HandleOnRewardedAdClosedRefill(object sender, EventArgs args)
    {
        closeRefill = true;
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Ads", "Close", "Refill");

        adRewardRefill.OnAdLoaded -= this.HandleOnRewardedAdLoadedRefill;
        adRewardRefill.OnAdRewarded -= this.HandleOnAdRewardedRefill;
        adRewardRefill.OnAdClosed -= this.HandleOnRewardedAdClosedRefill;
        adRewardRefill.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadRefill;
    }

    #endregion


    public void OnGetMoreHintClicked() //метод, который мы вешаем на кнопку +подсказки
    {
        RequestRewardAdHint();
    }

    public void OnGetMoreRefillClicked() //метод, который мы вешаем на кнопку перемешивание за рекламу
    {
        RequestRewardAdRefill();
    }

    public void OnGetMoreTurnX2Clicked()
    {
        RequestRewardAdTurnX2();
    }

    public void OnGetMorePlusTimeClicked()
    {
        RequestRewardAdPlusTime();
    }
    

    //ствндартная фигня по гайду от адмоб
    AdRequest AdRequestBuild()
    {
        return new AdRequest.Builder().Build();
    }

    void OnDestroy()
    {
        DestroyBannerAd();
    }
}