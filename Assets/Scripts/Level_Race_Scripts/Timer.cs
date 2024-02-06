using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TimeSpan Elapsed;

    public event EventHandler OnTimerStarted;
    public event EventHandler OnTimerEnded;

    public const string START_TIME = "StartTime";
    public const string LAST_CLOSED_REMAINING_TIME = "LastRemainingTime";
    public string GetStartTimeData() => PlayerPrefs.GetString(START_TIME, "");

    public float GetRemainingTimeData() => PlayerPrefs.GetFloat(LAST_CLOSED_REMAINING_TIME, 0);


    private float totalTime = 3600.0f;
    public static double remainingTime;

    [SerializeField] private string savedDateTime;

    private static DateTime startTime;

    private bool dateAndTimeCheck = false;

    private void Start()
    {
        savedDateTime = PlayerPrefs.GetString(START_TIME, "");
        if (savedDateTime != "")
        {
            savedDateTime = DateTime.Parse(savedDateTime).ToString("o");
            startTime = DateTime.Parse(savedDateTime);
            
            CalculateRemainingTime();
            UpdateUI();
        }
        
        PlayerDataUpdater.Instance.OnPlayerWin += InstanceOnPlayerWin;
        PlayerDataUpdater.Instance.OnPlayerLost += InstanceOnPlayerLost;
    }

    private void InstanceOnPlayerLost(object sender, EventArgs e)
    {
        remainingTime = 0;
    }

    private void InstanceOnPlayerWin(object sender, EventArgs e)
    {
        remainingTime = 0;
    }

    private void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateUI();
        }
        else if (dateAndTimeCheck)
        {
            //Reset The Timer
            ResetTimer();
            Debug.Log("Timer Ended");
            OnTimerEnded?.Invoke(this, EventArgs.Empty);
        }
    }

    public string UpdateUI()
    {
        var minutes = Mathf.FloorToInt((float)remainingTime / 60f);
        var seconds = Mathf.FloorToInt((float)remainingTime % 60f);

        //Debug.Log($"{minutes}min {seconds}s");
        return $"{minutes}min {seconds}s";
    }

    private void CalculateRemainingTime()
    {
        Elapsed = DateTime.Now - startTime;
        //Debug.Log(Elapsed.TotalSeconds + "ELASPED TIME");
        remainingTime = Mathf.Clamp((float)(totalTime - Elapsed.TotalSeconds), 0f, totalTime);
        dateAndTimeCheck = true;
    }


    public void StartTheTimer()
    {
        //To Load Old Value
        savedDateTime = PlayerPrefs.GetString(START_TIME, "");
        if (savedDateTime == "")
        {
            //To start the Timer and Update it's initial value for update function
            savedDateTime = DateTime.Now.ToString("o");
            Debug.Log("Timer is started");
            OnTimerStarted?.Invoke(this, EventArgs.Empty);
        }

        startTime = DateTime.Parse(savedDateTime);

        CalculateRemainingTime();
        UpdateUI();
    }

    private void ResetTimer()
    {
        PlayerPrefs.SetString(START_TIME, "");
        UpdateUI();
        dateAndTimeCheck = false;
    }

    public static void SaveData()
    {
        PlayerPrefs.SetString(START_TIME, startTime.ToString("o"));
        PlayerPrefs.SetFloat(LAST_CLOSED_REMAINING_TIME, (float)remainingTime);
        PlayerPrefs.Save();
    }

    public float TimePassedSinceLastClosed()
    {
        return Mathf.Clamp(GetRemainingTimeData() - (float)remainingTime, 0 , totalTime);
    }

    public bool DidRaceEndWhenDeviceOff() => Math.Abs(TimePassedSinceLastClosed() + (totalTime - GetRemainingTimeData()) - totalTime) < 0;

    private void OnApplicationQuit()
    {
        SaveData();
    }
}