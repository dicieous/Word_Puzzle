using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceStatusUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;

    [SerializeField] private Button levelForwardButton;

    private void OnEnable()
    {
        
    }

    private void Awake()
    {
        levelForwardButton.onClick.AddListener( () =>
        {
            PlayerDataUpdater.PlayerLevel++;
        });
        
        continueButton.onClick.AddListener(() =>
        {
            Timer.SaveData();
            Hide();
        });
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
