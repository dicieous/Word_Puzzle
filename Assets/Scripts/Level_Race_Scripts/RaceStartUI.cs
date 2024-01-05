using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceStartUI : MonoBehaviour
{
    [SerializeField] private Button startRaceButton;

    [SerializeField] private RaceStatusUI raceStatusUI;

    private void Awake()
    {
        startRaceButton.onClick.AddListener(() =>
        {
            
            Hide();
            raceStatusUI.Show();
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
