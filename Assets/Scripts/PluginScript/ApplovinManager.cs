using System;
using DDZ;
using LionStudios.Suite.Analytics.Events;
using UnityEngine;

public class ApplovinManager : MonoBehaviour
{
    public static ApplovinManager instance; 
    public static void ActivateMaxDebugger() => MaxSdk.ShowMediationDebugger();
    public float timeLimitToAds = 300f;
    
    public bool BannerAdsAvailable() => _bannerAdsAvailable;
    public bool RewardAdsAvailable() => MaxSdk.IsRewardedAdReady(rewardedAdUnitId);
    public bool InterstitialAdsAvailable() => MaxSdk.IsInterstitialReady(interstitialAdUnitId);
    public bool TimeToShowAd() => timeLimitToAds <= 0;

    
    private int _retryAttemptInter, _retryAttemptRewarded;
    private bool  _bannerAdsAvailable;
    private float _initializedTimeChecker = 0f;
    private static bool _isInitialized;

    private static string bannerAdUnitId => "3dc3b45782b09dc3";
    private static string rewardedAdUnitId => "efbe817559d0a8d4";
    private static string interstitialAdUnitId=>"da4b0cc348b6f4f3";
    
    private void Awake()
    {
        instance = this;
    }
    
    private void AdsInit() => Init();

    private void Init()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // AppLovin SDK is initialized, start loading ads
            if (bannerAdUnitId == "" || rewardedAdUnitId == "" || interstitialAdUnitId == "") 
                return;
            
            _isInitialized = true;
            print("contains Id's :: " + _isInitialized);

            //MaxSdk.ShowMediationDebugger(); // Turn Off Mediation Debugger
            InitializeBannerAds();

            InitializeRewardedAds();

            InitializeInterstitialAds();
        };

        MaxSdk.SetSdkKey("SkwvnkQqr_6nRgTxzKfWoRVNNeiS5rVRZuxLLeCfOA21seXZ_7ZZ2Um8ynwrGqfJpfl-Fid2fExGmb9lT1QDHp");
        MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
        MaxSdk.InitializeSdk(); 
    }

    private void Update()
    {
        if (timeLimitToAds > 0) timeLimitToAds -= Time.deltaTime;
        CheckApplovinInitialized();
    }

    public void ShowInterstitialAds_120Sec(string placement)
    {
        ShowInterstitialAds(placement);
        ResetAdsTimeLimit(120);
    }

    private void CheckApplovinInitialized()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable || _isInitialized ) return;
        
        _initializedTimeChecker -= Time.deltaTime;

        if (_initializedTimeChecker > 0) return;
        
        _initializedTimeChecker = 10f;
        AdsInit();
    }

    private void InitializeBannerAds()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent      += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent  += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent     += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent    += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent   += OnBannerAdCollapsedEvent;
        
        // Banners are automatically sized to 320�50 on phones and 728�90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.clear);
        MaxSdk.SetBannerPlacement(bannerAdUnitId,"InGame");
        
    }

    private void OnBannerAdCollapsedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        print("Banner Ad Collapsed");
    }

    private void OnBannerAdExpandedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        print("Banner Ad Expended");
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        print("Banner Ad Revenue Paid");
    }

    private void OnBannerAdClickedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        print("Banner Ad Clicked");
    }

    private void OnBannerAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
    {
        print("Banner Ad Failed To Load::" + arg2.Code +"::"+arg2.Message);
    }

    private void OnBannerAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        print("Banner Ad Loaded");
        _bannerAdsAvailable = true;
    }

    public void ShowBannerAds()
    {
        if(!_isInitialized) return;

        MaxSdk.ShowBanner(bannerAdUnitId);
    }

    public void HideBannerAds()
    {
        if(!_isInitialized) return;
        MaxSdk.HideBanner(bannerAdUnitId);
    }

    private void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += InterstitialOnAdRevenuePaidEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void InterstitialOnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        print("Interstitial Ads Revenue paid");
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        _retryAttemptInter = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        _retryAttemptInter++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, _retryAttemptInter));
        Invoke(nameof(LoadInterstitial), (float)retryDelay);
        Time.timeScale = 1;
        print("Failed Load::"+ errorInfo.Code+"::"+errorInfo.Message);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        print("Ad Displayed");
        LionStudiosManager.AdsEvents(false,AdsEventState.Shown,UIManagerScript.Instance.GetSpecialLevelNumber(),adInfo.NetworkName,"LevelComplete",0);
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        LoadInterstitial();
        Time.timeScale = 1;
        print("Failed To Display::"+ errorInfo.Code+"::"+errorInfo.Message);
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        print("Ad Closed");
        LionStudiosManager.AdsEvents(false,AdsEventState.Clicked,UIManagerScript.Instance.GetSpecialLevelNumber(),adInfo.NetworkName,"LevelComplete",CoinManager.instance.GetCoinsCount());
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
        Time.timeScale = 1;
        print("Interstitial Ad Hidden::"+ adInfo.Placement+"::"+adInfo.NetworkName);
    }

    private void ResetAdsTimeLimit(float time) => timeLimitToAds = time;

    private void ShowInterstitialAds(string placement)
    {
        if (interstitialAdUnitId == "")
        {
            print("ID is empty");
            return;
        }
        
        if (!_isInitialized)
        {
            print("Not initialized");
            return;
        }

        if (!MaxSdk.IsInterstitialReady(interstitialAdUnitId))
        {
            print("Ads are not ready");
            return;
        }

        if (!TimeToShowAd())
        {
            print("Still Time Limit");
            return;
        }

        Time.timeScale = 0;
        MaxSdk.ShowInterstitial(interstitialAdUnitId, placement);
        LionStudiosManager.AdsEvents(false,AdsEventState.Start,UIManagerScript.Instance.GetSpecialLevelNumber(),"Applovin","LevelComplete",CoinManager.instance.GetCoinsCount());
    }

    private void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        _retryAttemptRewarded = 0;
        print("Rewarded Ad is Ready");
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        Time.timeScale = 1;
        _retryAttemptRewarded++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(3, _retryAttemptRewarded));
        Invoke(nameof(LoadRewardedAd), (float)retryDelay);
        print("Reward Failed To Display::"+ errorInfo.Code+"::"+errorInfo.Message +"::"+errorInfo.MediatedNetworkErrorCode+"::"+errorInfo.MediatedNetworkErrorMessage);

    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        var placement = GameEssentials.RvType switch
        {
            RewardType.BubbleRv => "HintBubble2X",
            RewardType.Hint => "Hint",
            RewardType.SpecialHint => "SpecialHint",
            RewardType.Magnet => "Magnet",
            RewardType.LevelCompleteReward => "LevelCompleteReward",
            RewardType.GiftBox => "GiftBox",
            RewardType.Calendar => "Calendar",
            RewardType.Shuffle => "Shuffle",
            RewardType.FiftyFifty => "FiftyFifty",
            RewardType.ImageReveal => "ImageReveal",
            RewardType.NoMoreMoves => "NoMoreMoves",
            _=> "None"
        };
        
        if (placement is "None") return;
        LionStudiosManager.AdsEvents(true,AdsEventState.Shown,UIManagerScript.Instance.GetSpecialLevelNumber(),adInfo.NetworkName,placement,CoinManager.instance.GetCoinsCount());
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        Time.timeScale = 1;
        LoadRewardedAd();
        var placement = GameEssentials.RvType switch
        {
            RewardType.BubbleRv => "HintBubble2X",
            RewardType.Hint => "Hint",
            RewardType.SpecialHint => "SpecialHint",
            RewardType.Magnet => "Magnet",
            RewardType.LevelCompleteReward => "LevelCompleteReward",
            RewardType.GiftBox => "GiftBox",
            RewardType.Calendar => "Calendar",
            RewardType.Shuffle => "Shuffle",
            RewardType.FiftyFifty => "FiftyFifty",
            RewardType.ImageReveal => "ImageReveal",
            RewardType.NoMoreMoves => "NoMoreMoves",
            _=> "None"
        };
        var adeErrorType = errorInfo.Code switch
        {
            MaxSdkBase.ErrorCode.Unspecified => AdErrorType.Undefined,
            MaxSdkBase.ErrorCode.NetworkError => AdErrorType.Unknown,
            MaxSdkBase.ErrorCode.NoFill => AdErrorType.NoFill,
            MaxSdkBase.ErrorCode.NoNetwork => AdErrorType.Offline,
            MaxSdkBase.ErrorCode.AdDisplayFailed => AdErrorType.Unknown,
            _ => AdErrorType.Unknown
        };
        
        LionStudiosManager.RvShowAdFail(UIManagerScript.Instance.GetSpecialLevelNumber(),adeErrorType,adInfo.NetworkName,placement);
        print("Reward Failed To Display::"+ errorInfo.Code+"::"+errorInfo.Message +"::" + adInfo.NetworkName +"::"+errorInfo.MediatedNetworkErrorCode+"::"+errorInfo.MediatedNetworkErrorMessage);
        
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        var placement = GameEssentials.RvType switch
        {
            RewardType.BubbleRv => "HintBubble2X",
            RewardType.Hint => "Hint",
            RewardType.SpecialHint => "SpecialHint",
            RewardType.Magnet => "Magnet",
            RewardType.LevelCompleteReward => "LevelCompleteReward",
            RewardType.GiftBox => "GiftBox",
            RewardType.Calendar => "Calendar",
            RewardType.Shuffle => "Shuffle",
            RewardType.FiftyFifty => "FiftyFifty",
            RewardType.ImageReveal => "ImageReveal",
            RewardType.NoMoreMoves => "NoMoreMoves",
            _=> "None"
        };
        
        if (placement is "None") return;
        
        LionStudiosManager.AdsEvents(true,AdsEventState.Clicked,UIManagerScript.Instance.GetSpecialLevelNumber(),adInfo.NetworkName,placement,CoinManager.instance.GetCoinsCount());
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        Time.timeScale = 1;
        LoadRewardedAd();
        print("Reward Ad Hidden::"+ adInfo.Placement+"::"+adInfo.NetworkName);
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        Time.timeScale = 1;
        
        if (!LionStudiosManager.instance) return;
        
        var placement = GameEssentials.RvType switch
        {
            RewardType.BubbleRv => "HintBubble2X",
            RewardType.Hint => "Hint",
            RewardType.SpecialHint => "SpecialHint",
            RewardType.Magnet => "Magnet",
            RewardType.LevelCompleteReward => "LevelCompleteReward",
            RewardType.GiftBox => "GiftBox",
            RewardType.Calendar => "Calendar",
            RewardType.Shuffle => "Shuffle",
            RewardType.FiftyFifty => "FiftyFifty",
            RewardType.ImageReveal => "ImageReveal",
            RewardType.NoMoreMoves => "NoMoreMoves",
            _=> "None"
        };
        if (placement is not "None")
        {
            LionStudiosManager.AdsEvents(true,AdsEventState.RewardReceived,UIManagerScript.Instance.GetSpecialLevelNumber(),adInfo.NetworkName,placement,CoinManager.instance.GetCoinsCount());
           
        }

        Reward_Callback();
        LoadRewardedAd();
        ResetAdsTimeLimit(120);
        
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }

    public void ShowRewardedAds(string placement)
    {
        if (rewardedAdUnitId == "")
        {
            print("Reward ID is empty");
            return;
        }

        if (!MaxSdk.IsRewardedAdReady(rewardedAdUnitId))
        {
            print("Reward Ads is not ready:: " +MaxSdk.IsRewardedAdReady(rewardedAdUnitId));
            return;
        }
        
        if (!_isInitialized)
        {
            print("Not initialized ::" + _isInitialized);
            return;
        }
        
        //ResetAdsTimeLimit(90);
        print("Reward Ads Called");
        MaxSdk.ShowRewardedAd(rewardedAdUnitId, placement);
    }

    private void Reward_Callback()
    {
        GameEssentials.RewardedCallBack();
    }
}