using System;
using System.Collections;
using System.Collections.Generic;
using ByteBrewSDK;
using GameAnalyticsSDK;
using UnityEngine;

public class GAScript : MonoBehaviour
{
    public static GAScript instance;

    private void Awake()
    {
        instance = this;
    }

    public static void Init()
    {
        GameAnalytics.Initialize();
    }


    public void LevelStart(string levelName, int levelAttempts)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelName);
        LionStudiosManager.LevelStart(int.Parse(levelName), levelAttempts);
        ByteBrewManager.instance.LevelStart(levelName);
    }

    public void LevelFail(string levelName, int levelAttempts)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelName);
        LionStudiosManager.LevelFail(int.Parse(levelName), levelAttempts);
        ByteBrewManager.instance.LevelFail(levelName);
    }

    public void LevelCompleted(string levelName, int levelAttempts)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelName);
        LionStudiosManager.LevelComplete(int.Parse(levelName), levelAttempts);
        ByteBrewManager.instance.LevelCompleted(levelName);
    }

    public void LevelRestart(string levelName, int levelAttempts)
    {
        LionStudiosManager.LevelRestart(int.Parse(levelName), levelAttempts);
    }
}
