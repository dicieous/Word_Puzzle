using System;
using Events.Level.EventArgs;
using Events.InGame.EventArgs;
using System.Collections.Generic;
using Events.Mission.EventArgs;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Analytics.Events;
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

    public static void LevelStart(int levelNo, int attemptNum, int? score = null)
    {
        print("Level_Start:: " + levelNo);
        LionAnalytics.LevelStart(levelNo, attemptNum, score, null,null, null, null);
    } 
    
    public static void LevelComplete(int levelNo, int attemptNum, int? score = null)
    {
        print("Level_Completed:: " + levelNo);
        LionAnalytics.LevelComplete(levelNo, attemptNum, score,null, null,null,null, null);
    }
    
    public static void LevelFail(int levelNo, int attemptNum, int? score = null)
    {
        print("Level_Fail:: " + levelNo);
        LionAnalytics.LevelFail(levelNo, attemptNum, score, null, null,null,null);
    } 
    
    public static void LevelRestart(int levelNo, int attemptNum, int? score = null)
    {
        LionAnalytics.LevelRestart(levelNo, attemptNum, score, null, null,null,null);
    }

    public static void MissionStart(int levelNo, int attemptNum, int score)
    {
        var missionEventArgs = new MissionEventArgs
        {
            IsTutorial = false,
            MissionAttempt = attemptNum,
            MissionName = "CalendarLevels",
            MissionID = levelNo.ToString(),
            UserScore = score,
            MissionType = "Calendar Level"
        };
        LionAnalytics.MissionStarted(missionEventArgs);
    } 
    
    public static void MissionComplete(int levelNo, int attemptNum, int score)
    {
        var missionEventArgs = new MissionCompletedEventArgs()
        {
            IsTutorial = false,
            MissionAttempt = attemptNum,
            MissionName = "CalendarLevels",
            MissionID = levelNo.ToString(),
            UserScore = score,
            MissionType = "Calendar Level"
        };
        LionAnalytics.MissionCompleted(missionEventArgs);
    }
    
    public static void MissionFail(int levelNo, int attemptNum, int score)
    {
        var missionEventArgs = new MissionEventArgs
        {
            IsTutorial = false,
            MissionAttempt = attemptNum,
            MissionName = "CalendarLevels",
            MissionID = levelNo.ToString(),
            UserScore = score,
            MissionType = "Calendar Level"
        };
        var missionFailed = new MissionFailedEventArgs(missionEventArgs);
        LionAnalytics.MissionFailed(missionFailed);
    }
    public static void Hint(string levelNo, string hintsCount)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "hints_count", hintsCount }
        };
        
        LionAnalytics.ItemActioned("hint", "5", "HintButton", "HintType", additionalData);
        
        var data = new PowerUpUsedEventArgs
        {
            MissionAttempt = int.Parse(hintsCount),
            PowerUpName = "hint",
            MissionID = levelNo
        };
        LionAnalytics.PowerUpUsed(data,additionalData);
    }

    public static void Magnet(string levelNo, int magnetCount)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "magnets_count", magnetCount }
        };

        LionAnalytics.ItemActioned("magnet","6","MagnetButton","MagnetType", additionalData);
        var data = new PowerUpUsedEventArgs
        {
            MissionAttempt = magnetCount,
            PowerUpName = "magnet",
            MissionID = levelNo
        };

        LionAnalytics.PowerUpUsed(data,additionalData);
    }
    
    public static void Shuffle(string levelNo, int shuffleCount)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "shuffle_count", shuffleCount }
        };

        LionAnalytics.ItemActioned("shuffle","7","ShuffleButton","ShuffleType", additionalData);
        var data = new PowerUpUsedEventArgs
        {
            MissionAttempt = shuffleCount,
            PowerUpName = "shuffle",
            MissionID = levelNo
        };
        LionAnalytics.PowerUpUsed(data,additionalData);
    }
    
    public static void FiftyFifty(string levelNo, int fiftyfiftyCount)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "fiftyFifty_count", fiftyfiftyCount }
        };

        LionAnalytics.ItemActioned("fiftyfifty","8","FiftyFiftyButton","FiftyFiftyType", additionalData);
        var data = new PowerUpUsedEventArgs
        {
            MissionAttempt = fiftyfiftyCount,
            PowerUpName = "fiftyfifty",
            MissionID = levelNo
        };

        LionAnalytics.PowerUpUsed(data,additionalData);
    }
    
    public static void ImageReveal(string levelNo, int numberTimes)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "imageReveal_count", numberTimes }
        };

        LionAnalytics.ItemActioned("imageReveal","9","ImageRevealButton","ImageRevealType", additionalData);
        var data = new PowerUpUsedEventArgs
        {
            MissionAttempt = numberTimes,
            PowerUpName = "imageReveal",
            MissionID = levelNo
        };
        LionAnalytics.PowerUpUsed(data,additionalData);
    }
    
    public static void LevelCompleteReward(string levelNo, string rewardType, string rewardValue, string numberTimes)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "levelCompleteReward_count", numberTimes },
            { "reward_type", rewardType},
            { "reward_values",rewardValue}
        };
        LionAnalytics.ItemActioned("levelcompletedreward", "10", "LevelCompleteRewardButton", "LevelCompleteRewardType", additionalData);
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
        LionAnalytics.ItemActioned("giftbox", "11", "GiftBox", "GiftBoxType", additionalData);
    }
    
    public static void NoMoreMoves(string levelNo, string rewardType, string rewardValue, string numberTimes)
    {
        var additionalData = new Dictionary<string, object>
        {
            { "level_number", levelNo },
            { "noMoreMoves_count", numberTimes },
            { "reward_type", rewardType},
            { "reward_values",rewardValue}
        };
        LionAnalytics.ItemActioned("nomoremoves", "12", "NoMoreMoves", "NoMoreMovesType", additionalData);
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
    
    public static void IAPEvent(UnityEngine.Purchasing.Product product, int noAds, int moves, int coins)
    {
        var spentCurrency = new RealCurrency(product.metadata.isoCurrencyCode, (float)product.metadata.localizedPrice);
        var spent = new Product();
        spent.AddRealCurrency(currency: spentCurrency);
        var productsReceived = new Product
        {
            items = new List<Item>()
            {
                new("noads","rewards",noAds),
                new("moves","rewards",moves),
                new("coins","rewards",coins)
            }
        };
        var storeTransaction = new Transaction(
            name: product.metadata.localizedTitle,
            type: "Purchase",
            productsReceived: productsReceived,
            productsSpent: spent,
            transactionID: product.transactionID,
            productID: product.definition.id);

        LionAnalytics.InAppPurchase(storeTransaction);
    }
}
