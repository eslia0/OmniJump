using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
using System;

public class UnityAdsHelper : MonoBehaviour
{
    private static UnityAdsHelper instance;
    public static UnityAdsHelper Instance {
        get {
            if (instance == null)
            {
                instance = FindObjectOfType<UnityAdsHelper>();
            }

            return instance;
        }
    }

    private const string android_game_id = "3017767";
    private const string ios_game_id = "3017766";

    private const string rewarded_video_id = "rewardedVideo";

    [SerializeField] private bool showAds = true;

    private BannerView bannerView;
    private TimeSpan lastTime;
    public TimeSpan LastTime {
        get { return lastTime; }
        private set { lastTime = value; }
    }

    private void Awake()
    {
        instance = FindObjectOfType<UnityAdsHelper>();

        if (instance != this)
        {
            Destroy(gameObject);
        }

        instance.Initialize();
    }

    private void Initialize()
    {
        // lastTime = DateTime.Now.TimeOfDay - new TimeSpan(0, 15, 0);

#if UNITY_ANDROID
        Advertisement.Initialize(android_game_id);
#elif UNITY_IOS
        Advertisement.Initialize(ios_game_id);
#endif

        //#if UNITY_ANDROID
        //        string appId = "ca-app-pub-4092634290096513~6395512416";
        //#elif UNITY_IPHONE
        //            string appId = "ca-app-pub-3940256099942544~1458002511";
        //#else
        //        string appId = "unexpected_platform";
        //#endif

        //        // Initialize the Google Mobile Ads SDK.
        //        MobileAds.Initialize(appId);
    }

    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady(rewarded_video_id)) {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show(rewarded_video_id, options);
        }
        else {
            if (Application.isPlaying && Creater.Instance)
                Creater.Instance.NextStage(-1);
        }
    }

    public void HandleShowResult(ShowResult result) {
        if (Application.isPlaying && Creater.Instance) {
            switch (result) {
                case ShowResult.Finished: {
                        Debug.Log("The ad was successfully shown.");
                        if (Creater.Instance.adReward == "Revive") {
                            Creater.Instance.NextStage(0);
                        }
                        else if (Creater.Instance.adReward == "Coin") {
                            FindObjectOfType<EndlessUI>().SetResultPanel();
                        }

                        lastTime = DateTime.Now.TimeOfDay;

                        break;
                    }
                case ShowResult.Skipped: {
                        Debug.Log("The ad was skipped before reaching the end.");

                        FindObjectOfType<EndlessUI>().SetResultPanel();

                        // to do ...
                        // 광고가 스킵되었을 때 처리

                        break;
                    }
                case ShowResult.Failed: {
                        Debug.LogError("The ad failed to be shown.");

                        FindObjectOfType<EndlessUI>().SetResultPanel();

                        // to do ...
                        // 광고 시청에 실패했을 때 처리

                        break;
                    }
            }
        }
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        // 기존 Id, 아래는 test용 샘플 Id
        // string adUnitId = "ca-app-pub-4092634290096513/4027782645";
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        bannerView.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        bannerView.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        bannerView.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // AdSize adSize = new AdSize(250, 250);
        // BannerView bannerView = new BannerView(adUnitId, adSize, AdPosition.Bottom);

        // Create a 320x50 banner ad at coordinate (0,50) on screen.
        // BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, 0, 50);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }
    
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    //void OnDisable()
    //{
    //    bannerView.Destroy();
    //}
}