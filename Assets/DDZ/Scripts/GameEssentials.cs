using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DDZ
{
    public enum OurPlatforms
    {
        None,
        Android,
        Ios,
        Editor,
        Other
    }

    public enum RewardType
    {
        None,
        Hint,
        LevelCompleteReward,
        BubbleRv,
        Magnet,
        GiftBox,
        NoMoreMoves,
        ImageReveal,
        FiftyFifty,
        Shuffle,
        Calendar,
        SpinWheel,
        DailyReward,
        CalenderStart
    }

    public class GameEssentials : Singleton<GameEssentials>
    {
        public static event Action<string> GameStartAction;
        public static event Action<bool, string> GameEndAction;

        public static RewardType RvType;

        public string remoteConfigVal = "0";

        private static OurPlatforms _platforms;

        public static DateTime GameStartTime;
        
        public float bubbleTime=120f;
        protected override void Awake()
        {
            base.Awake();
            OnInIt();
            Application.targetFrameRate = 120;
            OnGameStarted();
            _platforms = Application.platform switch
            {
                RuntimePlatform.Android => OurPlatforms.Android,
                RuntimePlatform.IPhonePlayer => OurPlatforms.Ios,
                RuntimePlatform.WindowsEditor or RuntimePlatform.WindowsEditor => OurPlatforms.Editor,
                _=> OurPlatforms.None
            };
            RecordDateTime();
        }
        
        private void RecordDateTime()
        {
            var gameStartTime = PlayerPrefs.GetString("GameStartTime","");
            GameStartTime = gameStartTime == "" ? DateTime.Now : DateTime.Parse(gameStartTime);
        }

        private void OnGameStarted()
        {
            // testing
        }


        private static void OnInIt()
        {
            Vibration.Init();
        }

        private void OnEnable()
        {
          //  GameAnalytics.OnRemoteConfigsUpdatedEvent += MyOnRemoteConfigsUpdateFunction;
        }

        private void OnDisable()
        {
           // GameAnalytics.OnRemoteConfigsUpdatedEvent += MyOnRemoteConfigsUpdateFunction;
        }

        private void MyOnRemoteConfigsUpdateFunction()
        {
            //remoteConfigVal = GameAnalytics.IsRemoteConfigsReady() ? GameAnalytics.GetRemoteConfigsValueAsString("LevelMode") : "0" ;
        }

        private void Start()
        {
            Invoke(nameof(GetRemoteConfigValue),2.5f);
        }

        private void Update()
        {
            if (bubbleTime > 0) bubbleTime -= Time.deltaTime;
        }

        private void GetRemoteConfigValue()
        {
            // 2.5 secs delay to get remoteconfig value
            remoteConfigVal = "0";
           // remoteConfigVal = GameAnalytics.GetRemoteConfigsValueAsString("LevelMode", "0");
        }

        //public static bool IsRewardedAdsAvailable() => ApplovinManager.instance.RewardAdsAvailable();
        //public static bool IsRewardedAdsAvailable() => ISManager.IsRewardedVideoAvailable;

        public static void VibrationOrHaptic(long val)
        {
            if (SavedData.GetHapticState().Equals("Off"))
                return;
            
            switch (_platforms)
            {
                case OurPlatforms.None:
                    Print("Not Selected any Platform");
                    break;
                case OurPlatforms.Android:
                    Vibration.VibrateAndroid(val);
                    break;
                case OurPlatforms.Ios:
#if UNITY_IOS
              // TapticPlugin.TapticManager.Impact(TapticPlugin.ImpactFeedback.Light);
                 Vibration.VibrateIOS(ImpactFeedbackStyle.Light);
#endif
                    break;
                case OurPlatforms.Editor:
                    Print("Vibrating");
                    break;
                case OurPlatforms.Other:
                    Print("Not Supported Platform");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_platforms), _platforms, null);
            }
        } 
        
        public static bool IsRvAvailable() => false;
        public static bool IsBannerAvailable() => false;
        public static bool IsInterstitialAvailable() => false;


        public static void Print(object val) => print(val);

        /// <summary>
        /// PlayerPrefs and Get / Set Scene Build Index & Names
        /// </summary>
        /// <returns></returns>

        public static string GetSoundState() => SavedData.GetSoundState();
        public static void SetSoundState(string val) => SavedData.SetSoundState(val);
        
        public static string GetHapticState() => SavedData.GetHapticState();
        public static void SetHapticState(string val) => SavedData.SetHepaticState(val);
      

        
        public static void Retry(Action callback)
        {
            callback?.Invoke();
          
        }

        public static void Restart(Action callback)
        {
            callback?.Invoke();
         
        }

        public static void StartScene(bool isTesting, int levelNum, Action callback)
        {
           
        }
        
        public static void ShowInterstitialsAds(string placement)
        {
            if(!IsInterstitialAvailable() || !ConnectedToInternet() ) return;
            Print("ShowingAds");
            //ApplovinManager.instance.ShowInterstitialAds_120Sec(placement);
        }

        public static void ShowRewardedAds(string placement)
        {
            if(!IsRvAvailable() || !ConnectedToInternet()) return;
             print("showing Rewarded Ads");
             
             //ApplovinManager.instance.ShowRewardedAds(placement);
        }

        public static void ShowBannerAds()
        {
            if(!ConnectedToInternet()) return;
           // ApplovinManager.instance.ShowBannerAds();
        }
        
        // public static void HideBannerAds() =>  ApplovinManager.instance.HideBannerAds();

        private static bool ConnectedToInternet() => Application.internetReachability != NetworkReachability.NotReachable;


        // ReSharper disable Unity.PerformanceAnalysis
        public static void RewardedCallBack()
        {

            // Place Reward CallBack's Here 
          switch (RvType)
          {
              case RewardType.None:
                  break;
              case RewardType.Hint:
                  var s = SavedData.GetSpecialLevelNumber().ToString()[^1];
                  if(s=='0') EmojiManager.Instance.SpecialHint_CallBack();
                  else UIManagerScript.Instance.Hint_CallBack();
                  break;
              case RewardType.LevelCompleteReward: 
                  UIManagerScript.Instance.DoubleCoins_CallBack();
                  break;
              case RewardType.BubbleRv:
                  MonitizationScript.instance.Bubble2X_CallBack();
                  break;
              case RewardType.Magnet:
                  //UIManagerScript.Instance.AutoWordComplete_Callback();
                  print("RV Calling");
                  break;
              case RewardType.GiftBox:
                  MonitizationScript.instance.GiftBox_CallBack();
                  break;
              case RewardType.NoMoreMoves:
                  UIManagerScript.Instance.GetMoreMoves_CallBack();
                  break;
              case RewardType.ImageReveal:
                  UIManagerScript.Instance.Emoji_CallBack();
                  break;
              case RewardType.FiftyFifty:
                  EmojiManager.Instance.FiftyFifty_CallBack();
                  break;
              case RewardType.Shuffle:
                  EmojiManager.Instance.Shuffle_CallBack();
                  break;
              case RewardType.Calendar:
                  DailyChallengesHandler.instance.DailyChallenge_Callback();
                  break;
              case RewardType.SpinWheel:
                  SpinWheelManager.instance.StopTheSpin_Callback();
                  break;
              case RewardType.DailyReward:
                  DailyRewardManager.instance.DailyCallBack();
                  break;
              case RewardType.CalenderStart:
                  UIManagerScript.Instance.CalenderUnlock_CallBack();
                  break;
              default:
                  print("default");
                  break;
          }
          RvType = RewardType.None;
        }

        /// <summary>
        /// Level Events
        /// </summary>
        /// <param name="levelName"></param>
        public static void OnGameStart(string levelName) => GameStartAction?.Invoke(levelName);

        public static void OnGameEnd(bool isWin, string levelName) => GameEndAction?.Invoke(isWin, levelName);

        // Level Event's Ends Here//

    }

}

