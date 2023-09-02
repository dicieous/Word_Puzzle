using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinManager : MonoBehaviour
{
	public static CoinManager instance;
	
    public GameManager gm;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI coinCountText;
    
    public List<Color> colorData;
    //public List<Color> colorsAdded;
	private void Awake()
	{
		instance = this;
	}

	void Start()
    {
        gm = GameManager.Instance;
        if (gm.rowsInGrid != 0)
        {
            colorData.Sort((a, b) => 1 - 2 * Random.Range(0, colorData.Count));
            for (int i = 0; i < gm.rowsInGrid; i++)
            {
                gm.rowColor.Add(colorData[i]);
            }
        }
        coinCountText.text = GetCoinsCount().ToString();
        if (GetCoinsCount() >= 20)
        {
            var countNum = (int)GetCoinsCount() / 20;
            SetHintCount(countNum);
            hintText.text = GetHintCount().ToString();
        }
        else
        {
            SetHintCount(0);
            hintText.text = GetHintCount().ToString();
        }
    }
	

    public void HintReduce()
    {
        if (GetHintCount() > 0)
        {
            SetHintCount(GetHintCount()-1);
            SetCoinCount(GetCoinsCount()-20);
            coinCountText.text = GetCoinsCount().ToString();
            hintText.text = GetHintCount().ToString();
        }
    }

    public void CoinsIncrease(int x)
    {
        SetCoinCount(GetCoinsCount()+x);
        SetHintCount((int)(GetCoinsCount()/20));
        
        coinCountText.text = GetCoinsCount().ToString();
        hintText.text = GetHintCount().ToString();
    }
    
    public  int GetHintCount() => PlayerPrefs.GetInt("Hint Count", 0);
    public  void SetHintCount(int countHint) => PlayerPrefs.SetInt("Hint Count", countHint);
    
    public  int GetCoinsCount() => PlayerPrefs.GetInt("Coins Count", 0);
    public  void SetCoinCount(int countCoin) => PlayerPrefs.SetInt("Coins Count", countCoin);
    
}
