using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StopWatchTimer : MonoBehaviour
{
    [SerializeField] private Timer timer;
    [SerializeField]  private TextMeshProUGUI timerText;
    private void Update()
    {
        timerText.text = timer.UpdateUI();
    }
}