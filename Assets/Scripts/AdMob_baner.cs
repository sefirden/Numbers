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
    private RewardBasedVideoAd adRewardHint;
    private RewardBasedVideoAd adRewardRefill;

    private Board board;

    public bool rewardHint;
    public bool rewardRefill;

    public bool closeHint;
    public bool closeRefill;

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

    private void Update()
    {
        if (rewardHint)
        {
            board = FindObjectOfType<Board>();
            board.AdHintRecieve();
            rewardHint = false;
        }

        if(closeHint)
        {
            board = FindObjectOfType<Board>();
            board.AdHintClose();
            closeHint = false;
        }

        if (rewardRefill)
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

    }

    void Start()
    {

        idApp = "ca-app-pub-3940256099942544~3347511713";
        idBanner = "ca-app-pub-3940256099942544/6300978111";
        idReward = "ca-app-pub-3940256099942544/5224354917";

        MobileAds.Initialize(idApp);

        RequestBannerAd();

        // Get singleton reward based video ad reference.
        adRewardHint = RewardBasedVideoAd.Instance;
        adRewardRefill = RewardBasedVideoAd.Instance;
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


    #region REWARDED ADS Hint

    public void RequestRewardAdHint()
    {
        AdRequest request = AdRequestBuild();
        adRewardHint.LoadAd(request, idReward);

        adRewardHint.OnAdLoaded += this.HandleOnRewardedAdLoadedHint;
        adRewardHint.OnAdRewarded += this.HandleOnAdRewardedHint;
        adRewardHint.OnAdClosed += this.HandleOnRewardedAdClosedHint;
        adRewardHint.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoadHint;
        // Called when an ad is shown.
    }

    public void ShowRewardAdHint()
    {
        if (adRewardHint.IsLoaded())
            adRewardHint.Show();
    }
    //events
    public void HandleOnRewardedAdLoadedHint(object sender, EventArgs args)
    {//ad loaded
        ShowRewardAdHint();
    }

    public void HandleOnAdRewardedHint(object sender, EventArgs args)
    {//user finished watching ad
        rewardHint = true;
    }

    public void HandleRewardBasedVideoFailedToLoadHint(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
        closeHint = true;

        adRewardHint.OnAdLoaded -= this.HandleOnRewardedAdLoadedHint;
        adRewardHint.OnAdRewarded -= this.HandleOnAdRewardedHint;
        adRewardHint.OnAdClosed -= this.HandleOnRewardedAdClosedHint;
        adRewardHint.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadHint;
    }

    public void HandleOnRewardedAdClosedHint(object sender, EventArgs args)
    {//ad closed (even if not finished watching)
        closeHint = true;

        adRewardHint.OnAdLoaded -= this.HandleOnRewardedAdLoadedHint;
        adRewardHint.OnAdRewarded -= this.HandleOnAdRewardedHint;
        adRewardHint.OnAdClosed -= this.HandleOnRewardedAdClosedHint;
        adRewardHint.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadHint;
    }

    #endregion

    #region REWARDED ADS Refill

    public void RequestRewardAdRefill()
    {
        AdRequest request = AdRequestBuild();
        adRewardRefill.LoadAd(request, idReward);

        adRewardRefill.OnAdLoaded += this.HandleOnRewardedAdLoadedRefill;
        adRewardRefill.OnAdRewarded += this.HandleOnAdRewardedRefill;
        adRewardRefill.OnAdClosed += this.HandleOnRewardedAdClosedRefill;
        adRewardRefill.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoadRefill;
    }

    public void ShowRewardAdRefill()
    {
        if (adRewardRefill.IsLoaded())
            adRewardRefill.Show();
    }
    //events
    public void HandleOnRewardedAdLoadedRefill(object sender, EventArgs args)
    {//ad loaded
        ShowRewardAdRefill();
    }

    public void HandleOnAdRewardedRefill(object sender, EventArgs args)
    {//user finished watching ad
        rewardRefill = true;
    }

    public void HandleRewardBasedVideoFailedToLoadRefill(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
        closeRefill = true;

        adRewardRefill.OnAdLoaded -= this.HandleOnRewardedAdLoadedRefill;
        adRewardRefill.OnAdRewarded -= this.HandleOnAdRewardedRefill;
        adRewardRefill.OnAdClosed -= this.HandleOnRewardedAdClosedRefill;
        adRewardRefill.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadRefill;

    }

    public void HandleOnRewardedAdClosedRefill(object sender, EventArgs args)
    {//ad closed (even if not finished watching)
        closeRefill = true;

        adRewardRefill.OnAdLoaded -= this.HandleOnRewardedAdLoadedRefill;
        adRewardRefill.OnAdRewarded -= this.HandleOnAdRewardedRefill;
        adRewardRefill.OnAdClosed -= this.HandleOnRewardedAdClosedRefill;
        adRewardRefill.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadRefill;
    }

    #endregion

    //other functions
    //btn (more points) clicked
    public void OnGetMoreHintClicked()
    {
        RequestRewardAdHint();
    }

    public void OnGetMoreRefillClicked()
    {
        RequestRewardAdRefill();
    }

    //------------------------------------------------------------------------
    AdRequest AdRequestBuild()
    {
        return new AdRequest.Builder().Build();
    }

    void OnDestroy()
    {
        DestroyBannerAd();

        /*adRewardHint.OnAdLoaded -= this.HandleOnRewardedAdLoadedHint;
        adRewardHint.OnAdRewarded -= this.HandleOnAdRewardedHint;
        adRewardHint.OnAdClosed -= this.HandleOnRewardedAdClosedHint;
        adRewardHint.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadHint;

        adRewardRefill.OnAdLoaded -= this.HandleOnRewardedAdLoadedRefill;
        adRewardRefill.OnAdRewarded -= this.HandleOnAdRewardedRefill;
        adRewardRefill.OnAdClosed -= this.HandleOnRewardedAdClosedRefill;
        adRewardRefill.OnAdFailedToLoad -= this.HandleRewardBasedVideoFailedToLoadRefill;*/
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
