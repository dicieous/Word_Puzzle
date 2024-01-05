using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerDataUpdater : MonoBehaviour
{
    private const string PLAYER_INITIAL_LEVEL = "InitialPlayerLevel";

    //[Header("Testing Stuff")] [Space(10)] private int levelnum = 5;
   // [SerializeField] private Button levelForwardingButton;

    [SerializeField] private List<PlayersData> playersData;

    [SerializeField] private StopWatchTimer _stopWatchTimer;

    [Space(10)] [SerializeField] private List<string> randomPlayerNames;

    private Player _player = new Player();
    private List<Bots> _botsList = new List<Bots>();

    private SaveState _saveState;

    private void SetInitialPlayerLevel()
    {
        PlayerPrefs.SetInt(PLAYER_INITIAL_LEVEL,UIManagerScript.Instance.GetSpecialLevelNumber());
        PlayerPrefs.Save();
    }

    private int GetInitialPlayerLevel() => PlayerPrefs.GetInt(PLAYER_INITIAL_LEVEL, -2);
    private int playerSavedLevel;

    [Serializable]
    public struct PlayersData
    {
        public TextMeshProUGUI playerNameHolder;
        public TextMeshProUGUI levelsCompletedHolder;

        public Image playerProgressImage;
    }

    private void Awake()
    {
        //_saveState = SaveManager.Instance.state;

        
        //levelForwardingButton.onClick.AddListener(() => { levelnum++; });
    }

    private void Start()
    {
        playerSavedLevel = GetInitialPlayerLevel();
        if (playerSavedLevel == -2)
        {
            playerSavedLevel = UIManagerScript.Instance.GetSpecialLevelNumber();
            SetInitialPlayerLevel();
        }
        
        InitializeBotList();
        LoadProgress();
        UpdateLeaderBoardEntries();

            //Debug.Log(SaveManager.Instance.state.levelProgress + "Level Progress");
    }

    private void Update()
    {
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
        for (var i = 0; i < playersData.Count - 1; i++)
        {
            string randomName = randomPlayerNames[Random.Range(0, randomPlayerNames.Count)];
            _botsList.Add(new Bots(randomName, 0));
        }
    }

    private void UpdateLeaderBoardEntries()
    {
        List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>
            { new(_player.PlayerName, UIManagerScript.Instance.GetSpecialLevelNumber() - playerSavedLevel) };

        foreach (var bot in _botsList)
        {
            leaderboardEntries.Add(new LeaderboardEntry(bot.BotName, bot.LevelProgress));
        }

        leaderboardEntries.Sort((a, b) => b.LevelProgress.CompareTo(a.LevelProgress));

        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            playersData[i].playerNameHolder.text = leaderboardEntries[i].Name;
            playersData[i].levelsCompletedHolder.text = $"{leaderboardEntries[i].LevelProgress.ToString()}/30";
            playersData[i].playerProgressImage.fillAmount = leaderboardEntries[i].LevelProgress / 30f;
        }
    }

    void SaveProgress()
    {
        for (int i = 0; i < _botsList.Count; i++)
        {
            PlayerPrefs.SetInt("Bot" + i + "Progress", _botsList[i].LevelProgress);
            PlayerPrefs.SetString("Bot" + i + "Name", _botsList[i].BotName);
        }

        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        for (int i = 0; i < _botsList.Count; i++)
        {
            _botsList[i].LevelProgress = PlayerPrefs.GetInt("Bot" + i + "Progress", 0);
            _botsList[i].BotName = PlayerPrefs.GetString("Bot" + i + "Name", _botsList[i].BotName);
            _botsList[i].LevelProgress = Convert.ToInt32(_stopWatchTimer.Elapsed.TotalSeconds / Random.Range(5f, 10f));
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
        return Time.time >= NextProgressTime;
    }

    public Bots()
    {
        SetRandomTimeInterval();
    }

    public void SetRandomTimeInterval()
    {
        ProgressInterval = Random.Range(5f, 10f);
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