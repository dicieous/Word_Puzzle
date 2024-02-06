using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RaceWinUI : MonoBehaviour
{
    public event EventHandler OnWinUIContinueButtonClicked;

    [SerializeField] private CompletedLevelData completedLevelData;
    
    [FormerlySerializedAs("raceCompletedContinueButton")] [SerializeField] private Button raceWinContinueButton;
   
    private void Awake()
    {
       
        
        raceWinContinueButton.onClick.AddListener( () =>
        {
            OnWinUIContinueButtonClicked?.Invoke(this, EventArgs.Empty);
            completedLevelData.DeleteProgress();
            PlayerDataUpdater.PlayerLevel = 5;
            Hide();
        });
        
        Debug.Log(gameObject.activeSelf + " Is completed menu Active");
        Hide();
    }

    public void Show()
    {
       
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
