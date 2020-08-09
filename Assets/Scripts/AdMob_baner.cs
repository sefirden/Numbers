using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
using UnityEngine.Events;


public class AdMob_baner : MonoBehaviour
{
    private BannerView adBannerDown;
    private RewardedAd adReward;
    private Board board;

    public UnityEvent OnAdLoadedEvent;
    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;
    public UnityEvent OnAdLeavingApplicationEvent;

    private string idApp, idBanner, idReward;

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


    void Start()
    {

        idApp = "ca-app-pub-3940256099942544~3347511713";
        idBanner = "ca-app-pub-3940256099942544/6300978111";
        idReward = "ca-app-pub-3940256099942544/5224354917";

        MobileAds.Initialize(idApp);
           
        RequestBannerAd();

        RequestAndLoadRewardedAd();
    }



    #region Banner Methods --------------------------------------------------

    public void RequestBannerAd()
    {
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        
        adBannerDown = new BannerView(idBanner, adaptiveSize, AdPosition.Bottom);
       
        adBannerDown.OnAdLoaded += HandleAdLoaded;
        adBannerDown.OnAdFailedToLoad += HandleAdFailedToLoad;
        adBannerDown.OnAdOpening += HandleAdOpened;
        adBannerDown.OnAdClosed += HandleAdClosed;
        adBannerDown.OnAdLeavingApplication += HandleAdLeftApplication;

        AdRequest request = AdRequestBuild();
        adBannerDown.LoadAd(request);
        Hide();

    }

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



    public void Hide()
    {
        adBannerDown.Hide();
    }
    public void Show()
    {   
        adBannerDown.Show();
    }

    public void DestroyBannerAd()
    {
        if (adBannerDown != null)
            adBannerDown.Destroy();
    }

    #endregion


    #region REWARDED ADS

    public void RequestAndLoadRewardedAd()
    {
        // create new rewarded ad instance
        adReward = new RewardedAd(idReward);

        // Add Event Handlers
        adReward.OnAdLoaded += (sender, args) => OnAdLoadedEvent.Invoke();
        adReward.OnAdFailedToLoad += (sender, args) => OnAdFailedToLoadEvent.Invoke();
        adReward.OnAdOpening += (sender, args) => OnAdOpeningEvent.Invoke();
        adReward.OnAdFailedToShow += (sender, args) => OnAdFailedToShowEvent.Invoke();
        adReward.OnAdClosed += (sender, args) => OnAdClosedEvent.Invoke();
        adReward.OnUserEarnedReward += (sender, args) => OnUserEarnedRewardEvent.Invoke();

        // Create empty ad request
        adReward.LoadAd(AdRequestBuild());
    }

    public void ShowRewardedAd()
    {
        if (adReward != null)
        {
            adReward.Show();

            // Create empty ad request
            adReward.LoadAd(AdRequestBuild());
        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }

    public void OnGetMoreHintClicked()
    {
        ShowRewardedAd();
    }
    #endregion

    //------------------------------------------------------------------------
    AdRequest AdRequestBuild()
    {
        return new AdRequest.Builder().Build();
    }

    void OnDestroy()
    {
        DestroyBannerAd();
    }
}



   /* private BannerView bannerView;

    // Use this for initialization
    void Start()
    {
        RequestBanner();
    }


    public void Hide()
    {
        bannerView.Hide();
    }
    public void Show()
    {
        bannerView.Show();
    }

    public void OnGUI()
    {
        GUI.skin.label.fontSize = 60;
        Rect textOutputRect = new Rect(
          0.15f * Screen.width,
          0.25f * Screen.height,
          0.7f * Screen.width,
          0.3f * Screen.height);
        GUI.Label(textOutputRect, "Adaptive Banner Example");
    }

    private void RequestBanner()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3212738706492790/6113697308";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3212738706492790/5381898163";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (adBanner != null)
        {
            adBanner.Destroy();
        }

        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        adBanner = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        // Register for ad events.
        adBanner.OnAdLoaded += HandleAdLoaded;
        adBanner.OnAdFailedToLoad += HandleAdFailedToLoad;
        adBanner.OnAdOpening += HandleAdOpened;
        adBanner.OnAdClosed += HandleAdClosed;
        adBanner.OnAdLeavingApplication += HandleAdLeftApplication;

        AdRequest adRequest = new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            .Build();

        // Load a banner ad.
        adBanner.LoadAd(adRequest);
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        MonoBehaviour.print(String.Format("Ad Height: {0}, width: {1}",
            adBanner.GetHeightInPixels(),
            adBanner.GetWidthInPixels()));
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

    #endregion
}*/
