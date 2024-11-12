using UnityEditor;
using UnityEngine;

public class SavedData
{
    private const string HighScore = "HighScore";
    private const string TotalCurrency = "TotalCurrency";

    private const string LevelName = "LevelName";
    private const string LevelNumber = "LevelNumber";

    private const string AudioState = "SoundState";
    private const string HapticState = "HapticState";

    public static int HintTutorial
    {
        get => PlayerPrefs.GetInt("Hint", 1);
        set => PlayerPrefs.SetInt("Hint", value);
    }

    public static int MagnetTutorial
    {
        get => PlayerPrefs.GetInt("Magnet", 1);
        set => PlayerPrefs.SetInt("Magnet", value);
    }


    // Get Value // Get//
    
    public static int GetHighScore()=>PlayerPrefs.GetInt(HighScore,0);
    public static int GetTotalCurrency() => PlayerPrefs.GetInt(TotalCurrency,0);

    public static int GetLevelNumber()=>PlayerPrefs.GetInt(LevelNumber, 1);
    public static int GetLevelByBuildIndex()=>PlayerPrefs.GetInt(LevelName, 1);
    public static string GetLevelByName()=>PlayerPrefs.GetString(LevelName, "1");

    public static string GetSoundState()=>PlayerPrefs.GetString(AudioState, "On");
    public static string GetHapticState()=>PlayerPrefs.GetString(HapticState, "On");


    // Set Value // Set//
    
    public static void SetHighScore(int val) => PlayerPrefs.SetInt(HighScore, val);
    public static void SetTotalCurrency(int val) =>PlayerPrefs.SetInt(TotalCurrency,val);

    public static void SetLevelByName(string val) =>  PlayerPrefs.SetString(LevelName, val);
    public static void SetLevelByBuildIndex(int val) =>  PlayerPrefs.SetInt(LevelName, val);
    public static void SetLevelNumber(int val) =>  PlayerPrefs.SetInt(LevelNumber, val);

    public static void SetSoundState(string state) =>  PlayerPrefs.SetString(AudioState, state);
    public static void SetHepaticState(string state) =>  PlayerPrefs.SetString(HapticState, state);


    public static void SaveData() => PlayerPrefs.Save();
    
}
