using System;
using System.Collections;
using System.Collections.Generic;
using Events.Level.EventArgs;
using UnityEngine;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Analytics.Events;
using DDZ;
using Events.InGame.EventArgs;
using Events.Mission.EventArgs;
using LionStudios.Suite.Analytics.Events.EventArgs;


public enum AdsEventState
{
    None,
    Start,
    Shown,
    Clicked,
    RewardReceived,
    Ended
}


public class LionStudiosManager : SingletonInstance<LionStudiosManager>
{
    private void Start()
    {
        LionAnalytics.SetWhitelistPriorityLevel(EventPriorityLevel.P2);
        LionAnalytics.SetWhitelistPriorityLevel(EventPriorityLevel.P3);
        LionAnalytics.SetWhitelistPriorityLevel(EventPriorityLevel.P4);
        
        LionAnalytics.GameStart();
    }


    private void OnApplicationQuit()
    {
        LionAnalytics.GameEnded();
    }

    public static void LevelStart(int levelNo, int attemptNum, int? score = null, string missionType = null, string missionName = null)
    {
        print("Level_Start:: " + levelNo);
        LionAnalytics.LevelStart(levelNo, attemptNum, score, null,null, missionType, missionName);
    } 
    
    public static void LevelComplete(int levelNo, int attemptNum, int? score = null, string missionType = null, string missionName = null)
    {
        print("Level_Completed:: " + levelNo);
        LionAnalytics.LevelComplete(levelNo, attemptNum, score,null, null,null,missionType, missionName);
    }
    
    public static void LevelFail(int levelNo, int attemptNum, int? score = null, string missionType = null, string missionName = null)
    {
        print("Level_Fail:: " + levelNo);
        LionAnalytics.LevelFail(levelNo, attemptNum, score, missionType, missionName);
    } 
    
    public static void LevelRestart(int levelNo, int attemptNum, int? score = null, string missionType = null, string missionName = null)
    {
        LionAnalytics.LevelRestart(levelNo, attemptNum, score, missionType, missionName);
    }

    public static void LevelStep(int levelNo)
    {
        print("Level_Step:: " + levelNo);
        var levelData = new LevelStepEventArgs()
        {
            LevelNum = levelNo,
        };
        LionAnalytics.LevelStep(levelData);
    }

    public static void Hint(string levelNo, string hintObjIndex, string hintObjName, string hintsCount)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "hints_count", hintsCount },
            { "hint_objectname",hintObjName},
            { "hint_objectindex",hintObjIndex}
        };
        
        LionAnalytics.ItemActioned("hint", "5", "HintButton", "HintType", additionalData);
        PowerUpUsedEventArgs data = new PowerUpUsedEventArgs();
        data.MissionAttempt = int.Parse(hintsCount);
        data.PowerUpName = "hint";
        data.MissionID = levelNo;

        LionAnalytics.PowerUpUsed(data,additionalData);
    }

    public static void Magnet(string levelNo, int numberTimes, string firstObjName, string firstObjId)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "magnet_count", numberTimes },
            { "hint_objectname",firstObjName},
            { "hint_objectindex",firstObjId}
        };

        LionAnalytics.ItemActioned("magnet","6","MagnetButton","MagnetType", additionalData);
        PowerUpUsedEventArgs data = new PowerUpUsedEventArgs();
        data.MissionAttempt = numberTimes;
        data.PowerUpName = "magnet";
        data.MissionID = levelNo;

        LionAnalytics.PowerUpUsed(data,additionalData);
    }

    public static void Navigator(string levelNo, int numberTimes,string hintObjIndex, string hintObjName)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "navigator_count", numberTimes },
            { "hint_objectname",hintObjName},
            { "hint_objectindex",hintObjIndex}
        };
        
        LionAnalytics.ItemActioned("navigator", "7", "NavigatorButton", "NavigatorType", additionalData);
        PowerUpUsedEventArgs data = new PowerUpUsedEventArgs();
        data.MissionAttempt = numberTimes;
        data.PowerUpName = "navigator";
        data.MissionID = levelNo;

        LionAnalytics.PowerUpUsed(data,additionalData);
        //LionAnalytics.ItemActioned("navigator", additionalData);
    }

    public static void SpinWheel(string levelNo, string rewardType, string rewardValue, string numberTimes)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "spinwheel_count", numberTimes },
            { "reward_type", rewardType},
            { "reward_values",rewardValue}
        };
        
        LionAnalytics.ItemActioned("spinwheel", "9", "SpinWheelButton", "SpinWheelType", additionalData);
    }
    
    public static void DailyRewards(string levelNo, string rewardType, string rewardValue, string numberTimes)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "dailyrewards_count", numberTimes },
            { "reward_type", rewardType},
            { "reward_values",rewardValue}
        };
       
        LionAnalytics.ItemActioned("dailyrewards", "8", "DailyRewardsButton", "DailyRewardsType", additionalData);
      
    }

    public static void BonusGift(string levelNo, string rewardType, string rewardValue, string numberTimes)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "bonusgift_count", numberTimes },
            { "reward_type", rewardType},
            { "reward_values",rewardValue}
        };
        
        LionAnalytics.ItemActioned("levelcompletedreward", "10", "BonusGiftButton", "BonusGiftType", additionalData);
    }

    public static void GiftBox(string levelNo, string rewardType, string rewardValue, string numberTimes)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "giftBox_count", numberTimes },
            { "reward_type", rewardType},
            { "reward_values",rewardValue}
        };
        
        LionAnalytics.ItemActioned("giftbox", "10", "GiftBox", "GiftBoxType", additionalData);
    }
    
    public static void RewardChestBox(string levelNo, string rewardType, string rewardValue, string numberTimes)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "star_count", numberTimes },
            { "reward_type", rewardType},
            { "reward_values",rewardValue}
        };
        
        LionAnalytics.ItemActioned("star", "10", "Star", "StarType", additionalData);
    }
    
    public static void Punishment(string levelNo, string rewardType, string rewardValue, string numberTimes)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "punishment_count", numberTimes },
            { "reward_type", rewardType},
            { "reward_values",rewardValue}
        };
        
        LionAnalytics.ItemActioned("giftbox", "10", "Punishment", "PunishmentType", additionalData);
    }

    public static void AdsEvents(bool isRewardAds,AdsEventState state, int levelNumber, string network, string placement, int coinsCount)
    {
        var product = new Product
        {
            virtualCurrencies = new List<VirtualCurrency>()
            {
                new VirtualCurrency("Level Reward: ", "Coins", coinsCount)
            }
        };

        var reward = new Reward(product);
        
        var rvAdEventArgs = new AdRewardArgs()
        {
            Level = levelNumber,
            Reward = reward,
            Placement = placement
        };
        
        var adEventArgs = new AdEventArgs()
        {
            Level = levelNumber,
            Network = network,
            Placement = placement
        };
        
        var additionalDataRv = new Dictionary<string, object>
        {
            { "LevelNum: ", levelNumber },
            { "totalCoins", coinsCount },
            { "Placement", placement },
            { "Network", network }
        };
        
        var additionalDataInter = new Dictionary<string, object>
        {
            { "LevelNum: ", levelNumber },
            { "Placement", placement },
            { "Network", network }
        };
        
        switch (state)
        {
            case AdsEventState.None:
                break;
            case AdsEventState.Start:
                if (isRewardAds)
                {
                    LionAnalytics.RewardVideoStart(adEventArgs,additionalDataRv);
                }
                else
                {
                    LionAnalytics.InterstitialStart(adEventArgs,additionalDataInter);
                }
               
                break;
            case AdsEventState.Shown:
                if (isRewardAds)
                {
                    LionAnalytics.RewardVideoShow(adEventArgs,additionalDataRv);
                }
                else
                {
                    LionAnalytics.InterstitialShow(adEventArgs,additionalDataInter);
                }
                
                break;
            case AdsEventState.Clicked:
                if (isRewardAds)
                {
                    LionAnalytics.RewardVideoClick(adEventArgs,additionalDataRv);
                }
                else
                {
                    LionAnalytics.InterstitialClick(adEventArgs,additionalDataInter);
                }
                
                break;
            case AdsEventState.RewardReceived:
                if (isRewardAds)
                {
                    LionAnalytics.RewardVideoCollect(rvAdEventArgs,additionalDataRv);
                    
                }
                break;
            case AdsEventState.Ended:
                if (!isRewardAds)
                {
                    LionAnalytics.InterstitialEnd(adEventArgs,additionalDataInter);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public static void RvShowAdFail(int levelNum, AdErrorType adErrorType, string network, string placement )
    {
        var adFailEventArgs = new AdFailEventArgs
        {
            Level = levelNum,
            Reason = adErrorType,
            Network = network,
            Placement = placement
        };
        LionAnalytics.RewardVideoShowFail(adFailEventArgs);
    }

    public static void MissionStart(int levelNo, int attemptNum, int score)
    {
        var missionEventArgs = new MissionEventArgs
        {
            IsTutorial = false,
            MissionAttempt = attemptNum,
            MissionName = "VIPLevels",
            MissionID = levelNo.ToString(),
            UserScore = score,
            MissionType = "VIP Level"
        };
        LionAnalytics.MissionStarted(missionEventArgs);
    } 
    
    public static void MissionComplete(int levelNo, int attemptNum, int score)
    {
        var missionEventArgs = new MissionCompletedEventArgs()
        {
            IsTutorial = false,
            MissionAttempt = attemptNum,
            MissionName = "VIPLevels",
            MissionID = levelNo.ToString(),
            UserScore = score,
            MissionType = "VIP Level"
        };
        LionAnalytics.MissionCompleted(missionEventArgs);
    }
    
    public static void MissionFail(int levelNo, int attemptNum, int score)
    {
        var missionEventArgs = new MissionEventArgs
        {
            IsTutorial = false,
            MissionAttempt = attemptNum,
            MissionName = "VIPLevels",
            MissionID = levelNo.ToString(),
            UserScore = score,
            MissionType = "VIP Level"
        };
        var missionFailed = new MissionFailedEventArgs(missionEventArgs);
        LionAnalytics.MissionFailed(missionFailed);
    }

}
