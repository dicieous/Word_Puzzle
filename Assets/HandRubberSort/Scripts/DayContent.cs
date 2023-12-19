using System;
using System.Collections.Generic;
using DDZ;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum DayState
{
    None, 
    Lock,
    Missed,
    Claim,
    ClaimAd,
    Claimed
}

public class DayContent : MonoBehaviour
{
    public DayState dayState;

    public int dayInt, monthInt, yearInt;
    public string dayString, monthString;
    
    public Image dayImage, rvImage, loadingImage, lockImage, missedImage, claimedImage;
    public TMP_Text dayTxt;
    public RectTransform rewardState;
    public string getKey;

    private Image _circle, _square;
    private readonly Dictionary<int, string> _monthIndexName = new()
    {
        {1  , "January"},
        {2  , "February"},
        {3  , "March"},
        {4  , "April"},
        {5  , "May"},
        {6  , "June"},
        {7  , "July"},
        {8  , "August"},
        {9  , "September"},
        {10 , "October"},
        {11 , "November"},
        {12 , "December"},
    };
    private void Awake()
    {
        _circle = rewardState.GetChild(1).GetComponent<Image>();
        _square = rewardState.GetChild(0).GetComponent<Image>();
    }

    public void SetClassData(int day, int month, int year)
    {
        dayInt = day;
        monthInt = month;
        yearInt = year;
        var todayDateTime = new DateTime(year, month, day);
        dayString = todayDateTime.ToString("dddd");
        monthString = _monthIndexName[monthInt];
        getKey = "SavedDate_" + dayInt + "" + monthInt + "" + yearInt;
    }

    public void SetButtonData(int day, DayState state)
    {
        dayState = state;
        dayTxt.text = "Day "+ day;
        switch (dayState)
        {
            case DayState.None:
                break;
            case DayState.Lock:
                rewardState.GetChild(0).gameObject.SetActive(false);
                rewardState.GetChild(1).gameObject.SetActive(true);
                lockImage.gameObject.SetActive(true);
                missedImage.gameObject.SetActive(false);
                claimedImage.gameObject.SetActive(false);
                _circle.color = new Color(1f, 0.56f, 0.2f);
                break;
            case DayState.Missed:
                rewardState.GetChild(0).gameObject.SetActive(false);
                rewardState.GetChild(1).gameObject.SetActive(true);
                lockImage.gameObject.SetActive(false);
                missedImage.gameObject.SetActive(true);
                claimedImage.gameObject.SetActive(false);
                _circle.color = Color.red;
                break;
            case DayState.Claim:
                rewardState.GetChild(0).gameObject.SetActive(true);
                rewardState.GetChild(1).gameObject.SetActive(false);
                rvImage.gameObject.SetActive(false);
                loadingImage.gameObject.SetActive(false);
                _square.color = Color.blue;
                break;
            case DayState.ClaimAd:
                rewardState.GetChild(0).gameObject.SetActive(true);
                rewardState.GetChild(1).gameObject.SetActive(false);
                rvImage.gameObject.SetActive(true);
                loadingImage.gameObject.SetActive(true);
                _square.color = Color.green;
                break;
            case DayState.Claimed:
                rewardState.GetChild(0).gameObject.SetActive(false);
                rewardState.GetChild(1).gameObject.SetActive(true);
                lockImage.gameObject.SetActive(false);
                missedImage.gameObject.SetActive(false);
                claimedImage.gameObject.SetActive(true);
                _circle.color = Color.green;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnButtonPress()
    {
        if(dayState is not (DayState.Claim or DayState.ClaimAd)) return;
        DailyRewardManager.instance.selectedDayContent = this;
        switch (dayState)
        {
            case DayState.Claim:
            print("Change to claimed");
            CoinManager.instance.CoinsIncrease(150);
            SaveDay();
            break;
            case DayState.ClaimAd:
            print("Show ad and Change to claimed");
            GameEssentials.RvType = RewardType.DailyReward;
            GameEssentials.ShowRewardedAds("DailyReward");
            //SaveDay();
            break;
        }
    }

    public void SaveDay()
    {
        PlayerPrefs.SetInt(getKey, 1);
        dayState = DayState.Claimed;
        SetButtonData(dayInt, dayState );
    }


    private void Update()
    {
        if(!GameEssentials.instance) return;
        if(dayState != DayState.ClaimAd) return;
        print(dayState +" sd");
        rvImage.gameObject.SetActive(GameEssentials.IsRvAvailable());
        loadingImage.gameObject.SetActive(!GameEssentials.IsRvAvailable());
    }
}
