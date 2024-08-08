using ByteBrewSDK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ByteBrewManager : MonoBehaviour
{
    public static ByteBrewManager instance;

    void Awake()
    {
        instance = this;
        // Initialize ByteBrew
        ByteBrew.InitializeByteBrew();
    }

    public void LevelStart(string levelname)
    {
        ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Started, "Level_Start", "Level" + levelname);
        ByteBrew.NewCustomEvent("Level_Start", "Level" + levelname);
    }

    public void LevelFail(string levelname)
    {
        ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Failed, "Level_Fail", "Level" + levelname);
        ByteBrew.NewCustomEvent("Level_Fail", "Level" + levelname);
    }

    public void LevelCompleted(string levelname)
    {
        ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Completed, "Level_Complete", "Level" + levelname);
        ByteBrew.NewCustomEvent("Level_Complete", "Level" + levelname);
    }

    /*public void ProgressEvent(string levelNo, string hintsCount, string gameMode, string eventName)
    {
        var additionalData = new Dictionary<string, string>()
        {
            { "LevelNumber", levelNo },
            { "UsageofHints", hintsCount },
            { "GameMode", gameMode }
        };

        ByteBrew.NewCustomEvent(eventName, ParseEventValues(additionalData));
    }
    Function Calling------------
    ByteBrewManager.instance.ProgressEvent(SaveData.GetSpecialLevelNumber().ToString(),hintCounter.ToString(), "NormalMode", "Hints");*/

    //////----Using RV Data
    public static void AdEventTracking(bool isRv, string levelNum, string adPlacement)
    {
        var rvPlacement = new Dictionary<string, string>()
        {
            {"level_num",levelNum},
            {"ad_placement", adPlacement},
        };
        ByteBrew.NewCustomEvent(isRv ? "rewarded_video":"interstitial", ParseEventValues(rvPlacement));
        ByteBrew.TrackAdEvent(isRv ? ByteBrewAdTypes.Reward : ByteBrewAdTypes.Interstitial, adPlacement);
    }
    //////----Using Coins Data
    public static void PowerUpsTracking(string levelNum, string powerUpName)
    {
        var rvPlacement = new Dictionary<string, string>()
        {
            {"level_num",levelNum},
            {"power_up_name", powerUpName},
        };
        ByteBrew.NewCustomEvent("power_up", ParseEventValues(rvPlacement));
    }
    public void EmojiEvent(string levelNo, string hintsCount, string gameMode, string emojiModeIndex, string eventName)
    {
        var additionalData = new Dictionary<string, string>()
        {
            { "LevelNumber", levelNo },
            { "UsageofHints", hintsCount },
            { "EmojiPanelNo", emojiModeIndex },
            { "GameMode", gameMode }
        };

        ByteBrew.NewCustomEvent(eventName, ParseEventValues(additionalData));
    }

    private static string ParseEventValues(Dictionary<string, string> values)
    {
        return values.Aggregate("", (current, keyPair) => current + $"{keyPair.Key}={keyPair.Value};");
    }
    
    public static void IAPEvents(UnityEngine.Purchasing.Product product, string levelNo)
    {
        var data = new Dictionary<string, string>()
        {
            {"level_number", levelNo},
            {"product_id", product.definition.id},
            {"product_name", product.metadata.localizedTitle},
            {"product_price", product.metadata.localizedPriceString},
            {"price_currency", product.metadata.isoCurrencyCode}
        };
        ByteBrew.NewCustomEvent("iap", ParseEventValues(data));
    }
}