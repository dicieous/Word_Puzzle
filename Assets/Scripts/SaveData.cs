using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public static int GetSpecialLevelNumber() => PlayerPrefs.GetInt("SpecialLevelNumber", 1);
    public static void SetSpecialLevelNumber(int levelNum) => PlayerPrefs.SetInt("SpecialLevelNumber", levelNum);
    //---Coins count
    public static int GetCoinsCount() => PlayerPrefs.GetInt("Coins Count", 0);

    public static void SetCoinCount(int countCoin) => PlayerPrefs.SetInt("Coins Count", countCoin);
    
    //---Hint Count Details
    public static int GetHintCount() => PlayerPrefs.GetInt("Hint Count", 0);
    public static void SetHintCount(int countHint) => PlayerPrefs.SetInt("Hint Count", countHint);
    
    //-----Magnet Count details
    public static int GetMagnetCount() => PlayerPrefs.GetInt("MagnetCount", 0);
    public static void SetMagnetCount(int num) => PlayerPrefs.SetInt("MagnetCount", num);
    //---Moves Count
    public static int GetMovesCount() => PlayerPrefs.GetInt("MovesCount", 0);
    public static void SetMovesCount(int num) => PlayerPrefs.SetInt("MovesCount", num);
    
    //---Shuffle count
    public static int GetShuffleCount() => PlayerPrefs.GetInt("ShuffleCount", 0);
    public static void SetShuffleCount(int countShuffle) => PlayerPrefs.SetInt("ShuffleCount", countShuffle);
    
    //--50/50 count
    public static int Get5050Count() => PlayerPrefs.GetInt("Count5050", 0);
    public static void Set5050Count(int count5050) => PlayerPrefs.SetInt("Count5050", count5050);
    

    //----Calender Lock and unlock
    public static string GetCalenderUnlockCheck() => PlayerPrefs.GetString("CalenderUnlockCheck", "Lock");
    public static void SetCalenderUnlockCheck(string lockCheck) => PlayerPrefs.SetString("CalenderUnlockCheck", lockCheck);
    
    //--Spin wheel
    public static int GetSpinCount() => PlayerPrefs.GetInt("Spins Count", 1);
    public static void SetSpinCount(int countSpin) => PlayerPrefs.SetInt("Spins Count", countSpin);
    
    //---Boss level Loader Details-----------------
    //---Loader percentage details
    public static float GetLoaderPercent() => PlayerPrefs.GetFloat("LoaderPercentage", 0);
    public static void SetLoaderPercentage(float percent) => PlayerPrefs.SetFloat("LoaderPercentage", percent);
    
    //--Image details
    public static int GetLoaderImageCount() => PlayerPrefs.GetInt("LoaderImageNumber", 0);
    public static void SetLoaderImageCount(int num) => PlayerPrefs.SetInt("LoaderImageNumber", num);
    
    
    //-----IAP Saving
    public static int GetIAPBoughtCount() => PlayerPrefs.GetInt("IAPBoughtCount", 0);
    public static void SetIAPBoughtCount(int num) => PlayerPrefs.SetInt("IAPBoughtCount", num);
    
    
    public static void SavedData()
    {
        PlayerPrefs.Save();
    }
}
