using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CompletedLevelData : MonoBehaviour
{
    private const string PLAYER_INITIAL_LEVEL = "InitialPlayerLevel";
    private const string KEEP_ON_UPDATING = "KeepOnUpdating";
    
    [SerializeField] private List<PlayersData> playersData;

    private Player _player;
    private List<Bots> _botsList;
    
    [SerializeField] private Sprite playerSprite;
    
    [SerializeField] private Timer timer;

    private int maxLevels = 30;

    private void Start()
    {
        _player = new Player();
        _botsList = new List<Bots>();
        
        InitializeBotList();
        LoadProgress();
        UpdateLeaderBoardEntries();
       
    }

    private void InitializeBotList()
    {
        for (var i = 0; i < playersData.Count - 1; i++)
        {
            _botsList.Add(new Bots("", 0));
        }
    }


    void LoadProgress()
    {
        for (int i = 0; i < _botsList.Count; i++)
        {
            var levelProgressTemp = PlayerPrefs.GetInt("Bot" + i + "Progress", 0);

            _botsList[i].LevelProgress = Mathf.Clamp(levelProgressTemp, 0, maxLevels);
            _botsList[i].BotName = PlayerPrefs.GetString("Bot" + i + "Name", _botsList[i].BotName);
        }
    }

    private void UpdateLeaderBoardEntries()
    {
        //Just for testing changed the level progress Value to 5
        List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>
            { new(_player.PlayerName, 5 /*UIManagerScript.Instance.GetSpecialLevelNumber() - PlayerDataUpdater.GetInitialPlayerLevel()*/) };

        foreach (var bot in _botsList)
        {
            leaderboardEntries.Add(new LeaderboardEntry(bot.BotName, bot.LevelProgress));
        }

        leaderboardEntries.Sort((a, b) => b.LevelProgress.CompareTo(a.LevelProgress));

        //Update the UI
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            playersData[i].playerNameHolder.text = leaderboardEntries[i].Name;
            if (leaderboardEntries[i].Name == _player.PlayerName)
            {
                playersData[i].playerProgressSlider.handleRect.gameObject.GetComponent<Image>().sprite = playerSprite;
            }
            
            playersData[i].levelsCompletedHolder.text = $"{leaderboardEntries[i].LevelProgress.ToString()}/30";
            playersData[i].playerProgressSlider.value = leaderboardEntries[i].LevelProgress / 30f;
        }
    }
    
    public void DeleteProgress()
    {
        PlayerPrefs.DeleteKey(PLAYER_INITIAL_LEVEL);
        PlayerPrefs.DeleteKey(KEEP_ON_UPDATING);

        for (int i = 0; i < _botsList.Count; i++)
        {
            PlayerPrefs.DeleteKey("Bot" + i + "Progress");
            PlayerPrefs.DeleteKey("Bot" + i + "Name");
        }
    }
}