using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardDataViewer : MonoBehaviour
{
    [SerializeField] private List<PlayersData> playersData;

    [SerializeField] private Sprite playerSprite;
    [SerializeField] private Sprite botSprite;

    private Player _player;

    private List<LeaderboardEntry> leaderboardEntries;

    private void Start()
    {
        _player = new Player();
       
    }

    private void Update()
    {
        UpdateData();
    }

    private void UpdateData()
    {
        leaderboardEntries = PlayerDataUpdater.LeaderBoardEntryData;
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            playersData[i].playerNameHolder.text = leaderboardEntries[i].Name;
            
            playersData[i].playerProgressSlider.handleRect.gameObject.GetComponent<Image>().sprite =
                leaderboardEntries[i].Name == _player.PlayerName ? playerSprite : botSprite;

            //Debug.Log("LeaderBoardViewerCalled");
            playersData[i].levelsCompletedHolder.text = $"{leaderboardEntries[i].LevelProgress.ToString()}/30";
            playersData[i].playerProgressSlider.value = leaderboardEntries[i].LevelProgress / 30f;
        }
    }
}