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

    public TMP_Text monthNameTxt, dateMonthTxt, questionTxt,playOnText;
    public Button leftBtn, rightBtn, playBtn;
    public Image playRvIcon, playLoadingIcon, levelScene;
    public Sprite playRvSprite, playNormalSprite;
    public int date, month, year;
    public string day, monthName;

    public Color defaultColor, selectedColor, currentColor, greyedOutColor;
    public Button[] days;
    public Sprite[] levelsSprites;
    [SerializeField] private int previousMonth, previousYrs;
    [SerializeField] private int currentMonth, currentYrs;
    [SerializeField] private int nextMonth, nextYrs;

    private readonly string[] _levelsData = new[]
    {
        "CAN BE ADDICTIVE",
        "PEOPLE MAY CRY HERE",
        "DATING ACTIVITIES",
        "MONEY MATTERS",
        "TOUCH IT WITH YOUR LIPS",
        "IT'S STICKY",
        "INSTAGRAM FUNCTIONS",
        "START & END WITH SAME LETTER",
        "WORD CONTAINING 'HA'",
        "REASONS FOR MARRIAGE",
        "BAD FEELING",
        "INVISIBLE THINGS",
        "PLACES WITH A LOT OF DOORS",
        "PEOPLE MAY CRY HERE",
        "FLIES WITHOUT WINGS",
        "COLOUR OF LIPSTICK",
        "IN THE BEDROOM",
        "I GLOVE MY JOB",
        "Love Symbol",
        "Things that require two people",
        "People can book it",
        "Throw it away after breaking up",
        "Changes all the Time",
        "Keeping Fit",
        "Disney princess Names",
        "You only use it once",
        "You need a card to enter",
        "Used in the shower",
        "Crops that start with 'P'",
        "Things naked people do",
        "WITH EYES CLOSED"
    };

    //public readonly Dictionary<int,dtring>
    private int _selectedDate, _baseNumber = 189;
    private DateTime _currentDateTime, _gameStartDateTime;

    private readonly Dictionary<int, string> _monthIndexName = new()
    {
        { 1, "January" },
        { 2, "February" },
        { 3, "March" },
        { 4, "April" },
        { 5, "May" },
        { 6, "June" },
        { 7, "July" },
        { 8, "August" },
        { 9, "September" },
        { 10, "October" },
        { 11, "November" },
        { 12, "December" },
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
            days[i].gameObject.SetActive(i < daysInMonth);
            if (num == date)
            {
                days[i].image.color = currentColor;
            }
            else if (num < date)
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

        dateMonthTxt.text = "Play " + date + " " + monthName[..3];
        questionTxt.text = _levelsData[date - 1];
        RewardCoins(date);
        playBtn.gameObject.SetActive(GetDailyChallenge() != 1);
        _savedDateString = "DailyChallenges_";
    }

    private void CheckIfDailyChallengesCompleted()
    {
        for (var index = 0; index < days.Length; index++)
        {
            var num = index + 1;
            var t = days[index];
            var isCompleted = PlayerPrefs.GetInt("DailyChallenges_" + num + currentMonth + currentYrs, 0);
            t.transform.GetChild(1).gameObject.SetActive(isCompleted != 0);
        }
    }

    private static int GetMonths(TimeSpan timespan)
    {
        return (int)(timespan.Days / 30.436875);
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
            days[i].gameObject.SetActive(i < daysInMonth);
            if (month == previousMonth)
            {
                if (num == date)
                {
                    days[i].image.color = currentColor;
                }
                else if (num < date)
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
            days[i].gameObject.SetActive(i < daysInMonth);
            if (month == nextMonth)
            {
                if (num == date)
                {
                    days[i].image.color = currentColor;
                }
                else if (num < date)
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
                }
                else if (num < date)
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
        //levelScene.sprite = levelsSprites[levelIndex];
        RewardCoins(btnNum);
        questionTxt.text = _levelsData[levelIndex];
        playBtn.image.sprite = _selectedDate == date ? playNormalSprite : playRvSprite;
        if (GetDailyChallenge() == 1)
        {
            playBtn.gameObject.SetActive(false);
        }
        else
        {
            playBtn.gameObject.SetActive(_selectedDate <= date);
        }

        playOnText.text="You can Play On "+_selectedDate + " " + monthName[..3];
        playOnText.gameObject.SetActive(_selectedDate>date);
        dateMonthTxt.text = "Play " + _selectedDate + " " + monthName[..3];
    }

    public void OnPlayBtnPress()
    {
        
        if (_selectedDate > date) return;
        SoundHapticManager.Instance.Play("Pop");
        SoundHapticManager.Instance.Vibrate(30);
        var isPlayed = PlayerPrefs.GetInt("DailyChallenges_" + _selectedDate + month + year, 0);
        if (isPlayed == 1) return;
        if (_selectedDate == date)
        {
            DailyChallenge_Callback();
            return;
        }
        GameEssentials.RvType = RewardType.Calendar;
        GameEssentials.ShowRewardedAds("Calendar");
        /*if (LionStudiosManager.instance)
            LionStudiosManager.AdsEvents(true, AdsEventState.Start, UIManagerScript.Instance.GetSpecialLevelNumber(),
                "Applovin", "Calendar", CoinManager.instance.GetCoinsCount());*/
    }

    public void DailyChallenge_Callback()
    {
        if (_selectedDate == 0) return;

        //SaveDailyChallenge(_selectedDate);
        _savedDateString =_savedDateString +""+ _selectedDate +""+ currentMonth +""+ currentYrs;
        CheckIfDailyChallengesCompleted();
        // LoadLevel
        var sceneIndex = _baseNumber + _selectedDate;
        print( "BaseNUmber::::"+_baseNumber);
        print( "BaseNUmber::::selected"+(_baseNumber + _selectedDate));
        print( "::::selected"+(_selectedDate));
        RewardCoins(_selectedDate);
        SceneManager.LoadScene(sceneIndex);
    }
    [SerializeField] private Sprite coinRefImage;
    [SerializeField] private Sprite hintRefImage;
    [SerializeField] private Sprite magnetRefImage;
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;
    [SerializeField] private TextMeshProUGUI count1Text;
    [SerializeField] private TextMeshProUGUI count2Text;
    [SerializeField] private int count1;
    [SerializeField] private int count2;
    public void RewardCoins(int datePressed)
    {
        switch (datePressed)
        {
            case 1:
                RewardsAssignFun(1);
                break;
            case 2:
                RewardsAssignFun(2);
                break;
            case 3:
                RewardsAssignFun(3);
                break;
            case 4:
                RewardsAssignFun(4);
                break;
            case 5:
                RewardsAssignFun(5);
                break;
            case 6:
                RewardsAssignFun(3);
                break;
            case 7:
                RewardsAssignFun(4);
                break;
            case 8:
                RewardsAssignFun(1);
                break;
            case 9:
                RewardsAssignFun(5);
                break;
            case 10:
                RewardsAssignFun(2);
                break;
            case 12:
                RewardsAssignFun(4);
                break;
            case 13:
                RewardsAssignFun(1);
                break;
            case 14:
                RewardsAssignFun(4);
                break;
            case 15:
                RewardsAssignFun(2);
                break;
            case 16:
                RewardsAssignFun(3);
                break;
            case 17:
                RewardsAssignFun(1);
                break;
            case 18:
                RewardsAssignFun(5);
                break;
            case 19:
                RewardsAssignFun(2);
                break;
            case 20:
                RewardsAssignFun(1);
                break;
            case 21:
                RewardsAssignFun(4);
                break;
            case 22:
                RewardsAssignFun(3);
                break;
            case 23:
                RewardsAssignFun(1);
                break;
            case 24:
                RewardsAssignFun(5);
                break;
            case 25:
                RewardsAssignFun(2);
                break;
            case 26:
                RewardsAssignFun(3);
                break;
            case 27:
                RewardsAssignFun(4);
                break;
            case 28:
                RewardsAssignFun(1);
                break;
            case 29:
                RewardsAssignFun(3);
                break;
            case 30:
                RewardsAssignFun(2);
                break;
            case 31:
                RewardsAssignFun(1);
                break;
            default:
                break;
                
        }
    }

    public void RewardsAssignFun(int num)
    {
        switch (num)
        {
            case 1:
                image1.sprite = coinRefImage;
                image2.sprite = hintRefImage;
                count1 = 200;
                count2 = 1;
                ////----Coins Count = 250-----
                count1Text.text = count1.ToString();
                count2Text.text = count2.ToString();
                UIManagerScript.dailyRewardDetails = "C*200 H*1";
                break;
            case 2:
                image1.sprite = coinRefImage;
                image2.sprite = hintRefImage;
                count1 = 100;
                count2 = 2;
                ////----Coins Count = 200-----
                count1Text.text = count1.ToString();
                count2Text.text = count2.ToString();
                UIManagerScript.dailyRewardDetails = "C*100 H*2";
                break;
            case 3:
                image1.sprite = coinRefImage;
                image2.sprite = magnetRefImage;
                count1 = 50;
                count2 = 1;
                ////----Coins Count = 150-----
                count1Text.text = count1.ToString();
                count2Text.text = count2.ToString();
                UIManagerScript.dailyRewardDetails = "C*50 M*1";
                break;
            case 4:
                image1.sprite = coinRefImage;
                image2.sprite = magnetRefImage;
                count1 = 100;
                count2 = 1;
                ////----Coins Count = 200-----
                count1Text.text = count1.ToString();
                count2Text.text = count2.ToString();
                UIManagerScript.dailyRewardDetails = "C*100 M*1";
               
                break;
            case 5:
                image1.sprite = coinRefImage;
                image2.sprite = hintRefImage;
                count1 = 150;
                count2 = 2;
                ////----Coins Count = 250-----
                count1Text.text = count1.ToString();
                count2Text.text = count2.ToString();
                UIManagerScript.dailyRewardDetails = "C*150 H*2";
                break;
            default:
                break;
        }
    }
    private void SaveDailyChallenge(int savedDate) => PlayerPrefs.SetInt("DailyChallenges_" + savedDate + month + year, 1);

    private static string _savedDateString = "DailyChallenges_";

    public static void SaveDailyChallengeAtLc()
    {
        print(_savedDateString);
        PlayerPrefs.SetInt(_savedDateString, 1);
    }

    private int GetDailyChallenge()
    {
       print("DailyChallenges_" + _selectedDate + currentMonth + currentYrs);
        return  PlayerPrefs.GetInt("DailyChallenges_" + _selectedDate + currentMonth + currentYrs, 0);
    } 
}