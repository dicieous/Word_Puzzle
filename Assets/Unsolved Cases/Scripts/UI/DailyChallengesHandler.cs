using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using DDZ;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DailyChallengesHandler : MonoBehaviour
{
    public static DailyChallengesHandler instance;
    
    public TMP_Text monthNameTxt, dateMonthTxt;
    public Button leftBtn, rightBtn, playBtn;
    public Image playRvIcon, playLoadingIcon, levelScene ;
    public Sprite playRvSprite, playNormalSprite;
    public int date, month, year;
    public string day, monthName;
    
    public Color defaultColor, selectedColor, currentColor, greyedOutColor;
    public Button[] days;
    public Sprite[] levelsSprites;
    [SerializeField]private int previousMonth, previousYrs;
    [SerializeField]private int currentMonth, currentYrs;
    [SerializeField]private int nextMonth, nextYrs;
    private readonly string[] _levelsData = new[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
    
    private int _selectedDate;
    private DateTime _currentDateTime, _gameStartDateTime;

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

    private int getGapMonths => GetMonths(gap);
    private TimeSpan gap => _currentDateTime.Subtract(_gameStartDateTime);
    
    private void Awake()
    {
        instance = this;
        GetData();
        SetData();
        CheckIfDailyChallengesCompleted();
        CalendarButtonStatus();
    }
    private void GetData()
    {
        _gameStartDateTime = GameEssentials.GameStartTime;
        _currentDateTime = DateTime.Now;
        date = _currentDateTime.Day;
        month = _currentDateTime.Month;
        year = _currentDateTime.Year;

        day = _currentDateTime.DayOfWeek.ToString();
        monthName = _currentDateTime.ToString("MMMM");
        
        _selectedDate = date;
        
        currentMonth = month;
        nextMonth = month + 1;
        previousMonth = month - 1;

        currentYrs = year;
        nextYrs = year + 1;
        previousYrs = year - 1;
    }

    private void SetData()
    {
        monthNameTxt.text = monthName + "";
        var daysInMonth = DateTime.DaysInMonth(currentYrs, month);
        for (var i = 0; i < days.Length; i++)
        {
            var num = i + 1;
            var numTxt = num.ToString();
            days[i].gameObject.SetActive(i<daysInMonth);
            if (num == date)
            {
                days[i].image.color = currentColor;
            } else if (num < date)
            {
                days[i].image.color = defaultColor; 
            }
            else
            {
                days[i].image.color = greyedOutColor;
            }
            days[i].name = numTxt;
            days[i].transform.GetChild(0).GetComponent<TMP_Text>().text = numTxt;
        }

        dateMonthTxt.text = "Play "+ date + " " + monthName[..3];
    }

    private void CheckIfDailyChallengesCompleted()
    {
        for (var index = 0; index < days.Length; index++)
        {
            var num = index + 1;
            var t = days[index];
            var isCompleted = PlayerPrefs.GetInt("DailyChallenges_" + num + currentMonth + year, 0);
            t.transform.GetChild(1).gameObject.SetActive(isCompleted != 0);
        }
    }

    private static int GetMonths(TimeSpan timespan)
    {
        return (int)(timespan.Days/30.436875);
    }

    private void Update()
    {
        if (!GameEssentials.instance || _selectedDate == date)
        {
            //playBtn.interactable = true;
            playRvIcon.gameObject.SetActive(false);
            playLoadingIcon.gameObject.SetActive(false);
            return;
        }
        playLoadingIcon.gameObject.SetActive(!GameEssentials.IsRvAvailable() && playBtn.interactable);
        playRvIcon.gameObject.SetActive(GameEssentials.IsRvAvailable() && playBtn.interactable);
       // playBtn.interactable = GameEssentials.IsRvAvailable();
    }

    public void LeftArrowBtnPress()
    {
        leftBtn.interactable = false;
        monthNameTxt.text = _monthIndexName[previousMonth];
        var daysInMonth = DateTime.DaysInMonth(currentYrs, previousMonth);
        for (var i = 0; i < days.Length; i++)
        {
            var num = i + 1;
            var numTxt = num.ToString();
            days[i].gameObject.SetActive(i<daysInMonth);
            if (month == previousMonth)
            {
                if (num == date)
                {
                    days[i].image.color = currentColor;
                } else if (num < date)
                {
                    days[i].image.color = defaultColor; 
                }
                else
                {
                    days[i].image.color = greyedOutColor;
                }
            }
            else
            {
                days[i].image.color = num < date ? defaultColor : greyedOutColor;
            }
            days[i].name = numTxt;
            days[i].transform.GetChild(0).GetComponent<TMP_Text>().text = numTxt;
        }

        CalculateMonths(false);
        CheckIfDailyChallengesCompleted();
        CalendarButtonStatus();
    }

    public void RightArrowBtnPress()
    {
        rightBtn.interactable = false;
        monthNameTxt.text = _monthIndexName[nextMonth] + "";
        var daysInMonth = DateTime.DaysInMonth(currentYrs, nextMonth);
        for (var i = 0; i < days.Length; i++)
        {
            var num = i + 1;
            var numTxt = num.ToString();
            days[i].gameObject.SetActive(i<daysInMonth);
            if (month == nextMonth)
            {
                if (num == date)
                {
                    days[i].image.color = currentColor;
                } else if (num < date)
                {
                    days[i].image.color = defaultColor; 
                }
                else
                {
                    days[i].image.color = greyedOutColor;
                }
            }
            else
            {
                days[i].image.color = num < date ? defaultColor : greyedOutColor;
            }
            days[i].name = numTxt;
            days[i].transform.GetChild(0).GetComponent<TMP_Text>().text = numTxt;
        }

        CalculateMonths(true);
        CheckIfDailyChallengesCompleted();
        CalendarButtonStatus();
    }

    private void CalculateMonths(bool canAdd)
    {
        if (canAdd)
        {
            previousMonth++;
            currentMonth++;
            nextMonth++;
            if (currentMonth > 12)
            {
                previousYrs++;
                currentYrs++;
                nextYrs++;
            }
            previousMonth = previousMonth > 12 ? 1 : previousMonth;
            currentMonth = currentMonth > 12 ? 1 : currentMonth;
            nextMonth = nextMonth > 12 ? 1 : nextMonth;
        }
        else
        {
            previousMonth--;
            currentMonth--;
            nextMonth--;
            if (currentMonth <= 12)
            {
                previousYrs--;
                currentYrs--;
                nextYrs--;
            }
            previousMonth = previousMonth > 12 ? 1 : previousMonth;
            currentMonth = currentMonth > 12 ? 1 : currentMonth;
            nextMonth = nextMonth > 12 ? 1 : nextMonth;
        }
    }

    private void CalendarButtonStatus()
    {
        rightBtn.interactable = !(currentYrs >= _gameStartDateTime.Year && currentMonth >= _gameStartDateTime.Month);
        leftBtn.interactable = previousMonth == _gameStartDateTime.Month && previousYrs == _gameStartDateTime.Year;
    }
    
    public void OnDatePress()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        var btnNum = int.Parse(btn.name);
        for (var i = 0; i < days.Length; i++)
        {
            var num = i + 1;
            if (days[i] == btn)
            {
                days[i].image.color = selectedColor;
               
            }
            else
            {
                if (num == date)
                {
                    days[i].image.color = currentColor;
                } else if (num < date)
                {
                    days[i].image.color = defaultColor;
                }
                else
                {
                    days[i].image.color = greyedOutColor;
                }
            }
        }
        _selectedDate = btnNum;
        var levelIndex = _selectedDate - 1;
        levelScene.sprite = levelsSprites[levelIndex];
        playBtn.image.sprite = _selectedDate == date ? playNormalSprite : playRvSprite;
        playBtn.interactable = _selectedDate <= date;
        dateMonthTxt.text = "Play "+ _selectedDate + " " + monthName[..3];
    }

    public void OnPlayBtnPress()
    {
        if(_selectedDate > date) return;
        var isPlayed = PlayerPrefs.GetInt("DailyChallenges_" + _selectedDate + month + year, 0);
        if (_selectedDate == date || isPlayed == 1)
        {
            DailyChallenge_Callback();
            return;
        }
        GameEssentials.RvType = RewardType.Calendar;
        GameEssentials.ShowRewardedAds("Calendar");
        if(LionStudiosManager.instance)
            LionStudiosManager.AdsEvents(true, AdsEventState.Start,UIManagerScript.Instance.GetSpecialLevelNumber(),"Applovin","Calendar",CoinManager.instance.GetCoinsCount());
    }

    public void DailyChallenge_Callback()
    {
        if(_selectedDate == 0) return;
        
        SaveDailyChallenge(_selectedDate);
        CheckIfDailyChallengesCompleted();
        // LoadLevel
        
        //SceneManager.LoadScene(_selectedDate);
        
        
    }

    private void SaveDailyChallenge(int savedDate) => PlayerPrefs.SetInt("DailyChallenges_" + savedDate + month + year, 1);
}
