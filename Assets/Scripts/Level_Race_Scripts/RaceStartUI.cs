using System;
using UnityEngine;
using UnityEngine.UI;

public class RaceStartUI : MonoBehaviour
{
    public event EventHandler OnStartButtonClicked;

    [SerializeField] private Timer timer;
    
    [SerializeField] private Button startRaceButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private RaceStatusUI raceStatusUI;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
        
        startRaceButton.onClick.AddListener(() =>
        {
            timer.StartTheTimer();
            
            raceStatusUI.Show();
            
            Debug.Log(raceStatusUI.isActiveAndEnabled + " Is Active?");
            OnStartButtonClicked?.Invoke(this,EventArgs.Empty);
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
