using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveState
{
    //public ChallengeMode challengeMode;
    public int challengeLevel;
    public bool isChallengeCompleted;
    public bool isBackFromChallenge;
    public int remainingSpin ;

    public int totalHelicopters, totalHints, totalFreeze;
    // IAP
    public string welcomePackTime = "";
    public bool noAds;
    //public bool unlimitedPowerUps;
    public bool challengeUnlocked;
    public int iapBoughtCount;
    
    //RaceMode
    public bool firstTimeRaceMode;
    public bool isRaceStarted;
    public int globalMoves;
    public int levelNum;
    public bool isRaceOpened;
    
}


//[Serializable]
//public class ProductDetails
//{
//    public bool isLocked = false;
//    public int pIndex = -1;
//}

//[Serializable]
//public class CustomerDetails
//{
//    public int custIndex = -1;
//    public int custProductIndex = -1;
//    public double coinAmt = 0;
//    public int energy = 0;
//}