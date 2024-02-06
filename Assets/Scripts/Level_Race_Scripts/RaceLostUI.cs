using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RaceLostUI : MonoBehaviour
{
    public event EventHandler OnLostUIContinueButtonClicked;

    [SerializeField] private CompletedLevelData completedLevelData;
    
    [FormerlySerializedAs("raceCompletedContinueButton")] [SerializeField] private Button raceLostContinueButton;
   
    private void Awake()
    {
       
        
        raceLostContinueButton.onClick.AddListener( () =>
        {
            OnLostUIContinueButtonClicked?.Invoke(this, EventArgs.Empty);
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
