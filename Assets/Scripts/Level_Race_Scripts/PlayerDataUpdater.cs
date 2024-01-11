using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerDataUpdater : MonoBehaviour
{
    public static PlayerDataUpdater Instance;

    public event EventHandler OnPlayerWin;
    public event EventHandler OnPlayerLost;

    private const string PLAYER_INITIAL_LEVEL = "InitialPlayerLevel";
    private const string KEEP_ON_UPDATING = "KeepOnUpdating";

    [SerializeField] private Timer timer;

    [SerializeField] private int maxLevels = 30;

    [Space(10)] [SerializeField] private List<string> randomPlayerNames;

    private bool keepOnUpdating;
    private int NoOfBoards = 5;

    private Player _player;
    private List<Bots> _botsList;
    private int playerSavedLevel;

    public static int GetInitialPlayerLevel() => PlayerPrefs.GetInt(PLAYER_INITIAL_LEVEL, -2);

    //Test variable Delete when Done with testing
    public static int PlayerLevel = 5;

    //Set & get keep on Updating playerpref
    private bool GetKeepOnUpdatingPref()
    {
        int intValue = PlayerPrefs.GetInt(KEEP_ON_UPDATING, 0); // Default value is 0 (false)
        return intValue == 1;
    }

    public static void SetKeepOnUpdatingPref(bool value)
    {
        int intValue = value ? 1 : 0;
        PlayerPrefs.SetInt(KEEP_ON_UPDATING, intValue);
        PlayerPrefs.Save();
    }

    public static List<LeaderboardEntry> LeaderBoardEntryData;

    private void SetInitialPlayerLevel()
    {
        PlayerPrefs.SetInt(PLAYER_INITIAL_LEVEL, 5 /*UIManagerScript.Instance.GetSpecialLevelNumber()*/);
        PlayerPrefs.Save();
    }

    private void Awake()
    {
        Instance = this;

        _player = new Player();
        _botsList = new List<Bots>();

        InitializeBotList();
        timer.OnTimerStarted += StopWatchTimerOnTimerStarted;
    }

    private void Start()
    {
        playerSavedLevel = GetInitialPlayerLevel();
        if (playerSavedLevel == -2)
        {
            //First time Initializing

            //Just for Testing Changed the value to 0
            playerSavedLevel = 0 /*UIManagerScript.Instance.GetSpecialLevelNumber()*/;
            SetInitialPlayerLevel();
        }
        else
        {
            LoadProgress();
        }

        timer.OnTimerEnded += StopWatchTimerOnTimerEnded;

        //Debug.Log(SaveManager.Instance.state.levelProgress + "Level Progress");
    }

    private void StopWatchTimerOnTimerStarted(object sender, EventArgs e)
    {
        if (playerSavedLevel != -2)
        {
            _player = new Player();
            _botsList = new List<Bots>();
            InitializeBotList();
        }

        keepOnUpdating = true;
        SetKeepOnUpdatingPref(keepOnUpdating);
        Debug.Log(keepOnUpdating + " Keep on updating value");
    }

    private void StopWatchTimerOnTimerEnded(object sender, EventArgs e)
    {
        SaveProgress();
        CheckForWinAndLooseCase(LeaderBoardEntryData);
        
        _player = null;
        _botsList = null;
        keepOnUpdating = false;
        SetKeepOnUpdatingPref(keepOnUpdating);

    }

    private void Update()
    {
        //If timer is not going on then do not update the values
        if (!GetKeepOnUpdatingPref()) return;

        foreach (var bot in _botsList.Where(bot => bot.CanBotProgress()))
        {
            bot.LevelProgress++;
            bot.SetRandomTimeInterval();
            Debug.Log(bot.LevelProgress + " " + bot.BotName);
        }

        UpdateLeaderBoardEntries();
        SaveProgress();
    }

    private void InitializeBotList()
    {
        for (var i = 0; i < NoOfBoards - 1; i++)
        {
            if (randomPlayerNames == null)
            {
                _botsList.Add(new Bots("", 0));
            }
            else
            {
                string randomName = randomPlayerNames[Random.Range(0, randomPlayerNames.Count)];
                _botsList.Add(new Bots(randomName, 0));
            }
        }
    }

    private void UpdateLeaderBoardEntries()
    {
        //Just for testing changed the level progress Value to 5
        List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>
        {
            new(_player.PlayerName, PlayerLevel /*UIManagerScript.Instance.GetSpecialLevelNumber() - playerSavedLevel*/)
        };

        foreach (var bot in _botsList)
        {
            leaderboardEntries.Add(new LeaderboardEntry(bot.BotName, bot.LevelProgress));
        }

        leaderboardEntries.Sort((a, b) => b.LevelProgress.CompareTo(a.LevelProgress));

        LeaderBoardEntryData = leaderboardEntries;
        //Update the UI

        CheckForWinAndLooseCase(leaderboardEntries);
    }

    private void CheckForWinAndLooseCase(List<LeaderboardEntry> leaderboardEntries)
    {
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            if (leaderboardEntries[i].LevelProgress == 30 && leaderboardEntries[i].Name == _player.PlayerName && i < 3)
            {
                //Win Case
                SaveProgress();
                keepOnUpdating = false;
                SetKeepOnUpdatingPref(keepOnUpdating);

                Debug.Log("WinLooseCheckCalled");
                switch (i)
                {
                    case 0:
                        Debug.Log("1st pos");
                        UIManagerScript.Instance.GiftOpenFun(5);
                        //On 1st Position
                        break;
                    case 1:
                        Debug.Log("2nd pos");
                        UIManagerScript.Instance.GiftOpenFun(3);
                        //On Second Position
                        break;
                    case 2:
                        Debug.Log("3rd pos");
                        UIManagerScript.Instance.GiftOpenFun(4);
                        //On 3rd Position
                        break;
                }

                OnPlayerWin?.Invoke(this, EventArgs.Empty);
            }
            else if (DidPlayerLoose(leaderboardEntries))
            {
                //Loose Case
                Debug.Log("Loose");

                SaveProgress();
                keepOnUpdating = false;
                SetKeepOnUpdatingPref(keepOnUpdating);

                OnPlayerLost?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private bool DidPlayerLoose(List<LeaderboardEntry> leaderboardEntries)
    {
        Debug.Log("This Condition is called");
        for (int i = 0; i < leaderboardEntries.Count - 2; i++)
        {
            if (leaderboardEntries[i].LevelProgress == 30 && leaderboardEntries[i].Name != _player.PlayerName)
            {
                continue;
            }
            
            return false;
        }

        return true;
    }

    public void SaveProgress()
    {
        for (int i = 0; i < _botsList.Count; i++)
        {
            PlayerPrefs.SetInt("Bot" + i + "Progress", _botsList[i].LevelProgress);
            PlayerPrefs.SetString("Bot" + i + "Name", _botsList[i].BotName);
        }

        Debug.Log("Save Called");
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        for (int i = 0; i < _botsList.Count; i++)
        {
            var levelProgressTemp = PlayerPrefs.GetInt("Bot" + i + "Progress", 0) +
                                    Convert.ToInt32(timer.TimePassedSinceLastClosed() / Random.Range(2f, 8f));
            Debug.Log(Convert.ToInt32(timer.TimePassedSinceLastClosed() / Random.Range(2f, 8f)) + " Added Value");

            _botsList[i].LevelProgress = Mathf.Clamp(levelProgressTemp, 0, maxLevels);
            _botsList[i].BotName = PlayerPrefs.GetString("Bot" + i + "Name", "");
        }
    }

    public void DeleteProgress()
    {
        PlayerPrefs.DeleteKey(PLAYER_INITIAL_LEVEL);

        for (int i = 0; i < _botsList.Count; i++)
        {
            PlayerPrefs.DeleteKey("Bot" + i + "Progress");
            PlayerPrefs.DeleteKey("Bot" + i + "Name");
        }
    }
}

public class Player
{
    public string PlayerName => "YOU";
    public int LevelProgress { get; set; }
}

public class Bots
{
    public string BotName { get; set; }
    public int LevelProgress { get; set; }

    public float ProgressInterval;
    public float NextProgressTime = Time.time + Random.Range(5f, 10f);

    public bool CanBotProgress()
    {
        return LevelProgress < 30 && Time.time >= NextProgressTime;
    }

    public Bots()
    {
        SetRandomTimeInterval();
    }

    public void SetRandomTimeInterval()
    {
        ProgressInterval = Random.Range(2f, 8f);
        NextProgressTime = Time.time + ProgressInterval;
    }

    public Bots(string name, int levelProgress)
    {
        BotName = name;
        LevelProgress = levelProgress;
    }
}

public class LeaderboardEntry
{
    public string Name { get; }
    public int LevelProgress { get; }

    public LeaderboardEntry(string name, int levelProgress)
    {
        Name = name;
        LevelProgress = levelProgress;
    }
}

[Serializable]
public struct PlayersData
{
    public TextMeshProUGUI playerNameHolder;
    public TextMeshProUGUI levelsCompletedHolder;

    public Slider playerProgressSlider;
}