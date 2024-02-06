using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelRaceManager : MonoBehaviour
{
    private const string WORD_RACE_UI_STATE = "WordRaceUIState";

    [SerializeField] private Button levelRaceButton;

    [SerializeField] private Timer timer;

    [SerializeField] private RaceStartUI raceStartUI;
    [SerializeField] private RaceStatusUI raceStatusUI;
    [SerializeField] private RaceLostUI raceLooseUI;
    [SerializeField] private RaceWinUI raceWinUI;

    private bool canShowWinOrLoosePanel = false;
    


    private enum RaceMenus
    {
        StartMenu,
        StatusMenu,
        RaceWinMenu,
        RaceLooseMenu
    }

    private RaceMenus _raceUIState;

    private void Awake()
    {
        _raceUIState = LoadEnumValue();
        Debug.Log(_raceUIState + "Awake Enum");
        levelRaceButton.onClick.AddListener(() =>
        {
            ChangeWordRaceUIStates();
        });
    }

    private void Start()
    {
        //timer.OnTimerEnded += TimerOnTimerEnded;
        raceStartUI.OnStartButtonClicked += RaceStartUIOnStartButtonClicked;
        raceWinUI.OnWinUIContinueButtonClicked += RaceWinUIOnWinContinueButtonClicked;
        raceLooseUI.OnLostUIContinueButtonClicked += RaceLooseUIOnContinueButtonClicked;
        PlayerDataUpdater.Instance.OnPlayerLost += InstanceOnPlayerLost;
        PlayerDataUpdater.Instance.OnPlayerWin += InstanceOnPlayerWin;
    }

    /*private void TimerOnTimerEnded(object sender, EventArgs e)
    {
        if (Timer.remainingTime < 0)
        {
            _raceUIState = RaceMenus.RaceLooseMenu; 
            SaveEnumValue(_raceUIState);
            ChangeWordRaceUIStates();
        }
    }*/


    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        if (SceneManager.GetActiveScene().name == "Map" && canShowWinOrLoosePanel)
        {
            ChangeWordRaceUIStates();
        }
    }

    private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        
    }

    private void InstanceOnPlayerWin(object sender, EventArgs e)
    {
        _raceUIState = RaceMenus.RaceWinMenu; 
        SaveEnumValue(_raceUIState);
        canShowWinOrLoosePanel = true;
        //ChangeWordRaceUIStates();
    }
    
    private void InstanceOnPlayerLost(object sender, EventArgs e)
    {
        _raceUIState = RaceMenus.RaceLooseMenu; 
        SaveEnumValue(_raceUIState);
        canShowWinOrLoosePanel = true;
        // ChangeWordRaceUIStates();
    }

    private void RaceLooseUIOnContinueButtonClicked(object sender, EventArgs e)
    {
        _raceUIState = RaceMenus.StartMenu;
        PlayerPrefs.DeleteKey(WORD_RACE_UI_STATE);
    }
    
    private void RaceWinUIOnWinContinueButtonClicked(object sender, EventArgs e)
    {
        _raceUIState = RaceMenus.StartMenu;
        PlayerPrefs.DeleteKey(WORD_RACE_UI_STATE);
    }

    private void RaceStartUIOnStartButtonClicked(object sender, EventArgs e)
    {
        _raceUIState = RaceMenus.StatusMenu;
        SaveEnumValue(_raceUIState);
    }

    private void ChangeWordRaceUIStates()
    {
        switch (_raceUIState)
        {
            case RaceMenus.StartMenu:

                raceStartUI.Show();
                break;
            case RaceMenus.StatusMenu:
                
                raceStartUI.Hide();
                raceStatusUI.Show();
                break;
            case RaceMenus.RaceWinMenu:

                raceStatusUI.Hide();
                raceWinUI.Show();
                break;
            case RaceMenus.RaceLooseMenu:

                raceStatusUI.Hide();
                raceLooseUI.Show();
                break;
        }
        
        SaveEnumValue(_raceUIState);
        Debug.Log("RaceUI_State " + _raceUIState );
        canShowWinOrLoosePanel = false;
    }

    private void SaveEnumValue(RaceMenus raceMenus)
    {
        int value = (int)raceMenus;
        PlayerPrefs.SetInt(WORD_RACE_UI_STATE, value);
        PlayerPrefs.Save();
    }

    private RaceMenus LoadEnumValue()
    {
        RaceMenus defaultValue = RaceMenus.StartMenu;
        if (PlayerPrefs.HasKey(WORD_RACE_UI_STATE))
        {
            var raceState = (RaceMenus)PlayerPrefs.GetInt(WORD_RACE_UI_STATE,(int)RaceMenus.StartMenu);
            return raceState;
        }

        return defaultValue;
    }
}