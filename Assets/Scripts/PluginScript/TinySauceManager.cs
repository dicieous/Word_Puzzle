using UnityEngine;

public class TinySauceManager : MonoBehaviour
{
    public static TinySauceManager instance;
    public bool adConsent, analyticsConsent;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        TinySauce.SubscribeOnInitFinishedEvent(OnConsentGiven);
    }

    private void OnConsentGiven(bool adConsent, bool analyticsConsent)
    {
        this.adConsent = adConsent;
        this.analyticsConsent = analyticsConsent;
        print(adConsent + " : " + analyticsConsent); 
    }

    public void LevelStart(string levelName)
    {
        TinySauce.OnGameStarted(levelName);
    }
    
    public void LevelEnd(bool won, float score, string levelName)
    {
        TinySauce.OnGameFinished(won, score, levelName);
    }

    public void PowerUpUsed(string powerUpName, string levelName)
    {
        TinySauce.OnPowerUpUsed(powerUpName, levelName); 
    }
    
}
