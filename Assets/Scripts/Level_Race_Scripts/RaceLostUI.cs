using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceLostUI : MonoBehaviour
{
    public event EventHandler OnLostUIContinueButtonClicked;

    [SerializeField] private CompletedLevelData completedLevelData;
    
    [SerializeField] private Button raceCompletedContinueButton;
   
    private void Awake()
    {
       
        
        raceCompletedContinueButton.onClick.AddListener( () =>
        {
            OnLostUIContinueButtonClicked?.Invoke(this, EventArgs.Empty);
            completedLevelData.DeleteProgress();
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
