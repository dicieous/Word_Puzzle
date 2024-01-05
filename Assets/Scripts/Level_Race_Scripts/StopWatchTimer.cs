using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class StopWatchTimer : MonoBehaviour
{
    public TimeSpan Elapsed;
    
    public const string START_TIME = "StartTime";
    public string GetStartTimeData() => PlayerPrefs.GetString(START_TIME, "");
    
    private float totalTime = 3600.0f;
    [SerializeField]private double remainingTime;

    [SerializeField]private TextMeshProUGUI timerText;
    [SerializeField] private string savedDateTime ;
    
    private static DateTime startTime;

    private bool dateAndTimeCheck = true;
    
    private void Start()
    {
       CheckDateAndTime();
    }

    private void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateUI();
        }
        else if(dateAndTimeCheck)
        {
            PlayerPrefs.SetString(START_TIME, "");
            CheckDateAndTime();
            dateAndTimeCheck = false;
        }
        
    }

    private void UpdateUI()
    {
        var minutes = Mathf.FloorToInt((float)remainingTime / 60f);
        var seconds = Mathf.FloorToInt((float)remainingTime % 60f);

        timerText.text = $"{minutes}min {seconds}s";
        //Debug.Log(minutes +" " + seconds);
    }

    private void CalculateRemainingTime()
    {
        Elapsed = DateTime.Now - startTime;
        //Debug.Log(elapsed.TotalSeconds);
        remainingTime = totalTime - Elapsed.TotalSeconds;
        dateAndTimeCheck = true;
    }

    private void CheckDateAndTime()
    {
        savedDateTime = PlayerPrefs.GetString(START_TIME, "");
        savedDateTime = savedDateTime == "" ? DateTime.Now.ToString("o"):DateTime.Parse(savedDateTime).ToString("o");
        startTime = DateTime.Parse(savedDateTime);
        
        CalculateRemainingTime();
        UpdateUI();
    }

    public static void SaveData()
    {
        PlayerPrefs.SetString(START_TIME, startTime.ToString("o"));
        PlayerPrefs.Save();
    }
}
