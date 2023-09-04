using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class GAScript : MonoBehaviour
{
    public static GAScript instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        GameAnalytics.Initialize();
    }


    public void LevelStart(string levelName, int levelAttempts)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelName);
        LionStudiosManager.LevelStart(int.Parse(levelName), levelAttempts);
    }

    public void LevelFail(string levelName, int levelAttempts)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelName);
        LionStudiosManager.LevelFail(int.Parse(levelName), levelAttempts);
    }

    public void LevelCompleted(string levelName, int levelAttempts)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelName);
        LionStudiosManager.LevelComplete(int.Parse(levelName), levelAttempts);
    }

    public void LevelRestart(string levelName, int levelAttempts)
    {
        LionStudiosManager.LevelRestart(int.Parse(levelName), levelAttempts);
    }
}
