using System;
using DDZ;
using UnityEngine;

public class ApplovinManager : MonoBehaviour
{
    public static ApplovinManager instance;
    public float timeLimitToAds = 300f;
    
    public bool BannerAdsAvailable() => _bannerAdsAvailable;
    public bool RewardAdsAvailable() => ISManager.instance.isRewardedAdsLoaded;
    public bool InterstitialAdsAvailable() => ISManager.instance.isInterstitialAdsLoaded;
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
    
    public void AdsInit() => Init();

    private void Init()
    {
        _isInitialized = true;
    }

    private void Update()
    {
        if (timeLimitToAds > 0) timeLimitToAds -= Time.deltaTime;
        CheckApplovinInitialized();
    }

    public void ShowInterstitialAds_120Sec(string placement)
    { 
        ShowInterstitialAds(placement);
        ISManager.instance.ShowInterstitialAds();
    }

    private void CheckApplovinInitialized()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable || _isInitialized ) return;
        
        _initializedTimeChecker -= Time.deltaTime;

        if (_initializedTimeChecker > 0) return;
        
        _initializedTimeChecker = 10f;
        AdsInit();
    }

  
    public static void ShowBannerAds()
    {
        if(!_isInitialized) return;
        ISManager.instance.ShowBannerAds();
        //MaxSdk.ShowBanner(bannerAdUnitId);
    }

    public static void HideBannerAds()
    {
        if(!_isInitialized) return;
        ISManager.instance.HideBannerAds();
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

        if (!InterstitialAdsAvailable())
        {
            print("Ads are not ready");
            return;
        }

        if (!TimeToShowAd())
        {
            print("Still Time Limit:: "+ timeLimitToAds);
            return;
        }

        Time.timeScale = 0;
        ISManager.instance.ShowInterstitialAds();
        /*MaxSdk.ShowInterstitial(interstitialAdUnitId, placement);
        LionStudiosManager.AdsEvents(false,AdsEventState.Start,SaveData.GetSpecialLevelNumber(),"Applovin","LevelComplete",SaveData.GetCoinsCount());*/
        ResetAdsTimeLimit(180);
    }
    public void ShowRewardedAds(string placement)
    {
        if (rewardedAdUnitId == "")
        {
            print("Reward ID is empty");
            return;
        }

        if (!ISManager.instance.isRewardedAdsLoaded)
        {
            print("Reward Ads is not ready:: " + ISManager.instance.isRewardedAdsLoaded);
            return;
        }
        
        if (!_isInitialized)
        {
            print("Not initialized ::" + _isInitialized);
            return;
        }
        ISManager.instance.ShowRewardedVideo();
        //ResetAdsTimeLimit(90);
        print("Reward Ads Called");
        //MaxSdk.ShowRewardedAd(rewardedAdUnitId, placement);
    }

    private void Reward_Callback()
    {
        GameEssentials.RewardedCallBack();
    }
}