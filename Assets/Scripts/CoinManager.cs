using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    public UIParticle confettiFx;
    public UIParticle confettiFx1;
    public GameManager gm;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI specialLevelHintText;
    public TextMeshProUGUI shuffleCountText;
    public TextMeshProUGUI fiftyFiftyCountText;
    public TextMeshProUGUI coinCountText;
    public TextMeshProUGUI autoWordCountText;
    public TextMeshProUGUI emojiRevelCountText;
    
    public List<Color> colorData;
    [HideInInspector] public Color singleColor;
    public Color redColor;

    public Color greenColor;
    //public List<Color> colorsAdded;
    public int hintCounter;

    [Header("Progression bar")] 
    public Image progressionBarImage;
    public TextMeshProUGUI progressionBarText;
    public Image progressionImage;
    public List<Sprite> progressionImageList;

    public RectTransform winEmoji;
    public List<Sprite> winEmojiSprites;
    [Header("GiftFillingBar")]
    public Image giftProgressionBarImage;
    public TextMeshProUGUI giftProgressionBarText;
    public Image giftProgressionImage;
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        coinCountText.text = SaveData.GetCoinsCount().ToString();
        
        var s = SaveData.GetSpecialLevelNumber().ToString()[^1];
        if (s != '0')
        {
            if (GameManager.Instance)
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

                if (GameManager.Instance.levelTypeChanged)
                {
                    //singleColor = colorData[Random.Range(0, colorData.Count)];
                    singleColor = greenColor;
                } 
            }
            if (SaveData.GetCoinsCount() >= 50)
            {
                var countNum = (int)SaveData.GetCoinsCount() / 50;
                SaveData.SetHintCount(countNum);
                hintText.text =SaveData. GetHintCount().ToString();
                specialLevelHintText.text =SaveData. GetHintCount().ToString();
                if (SaveData.GetCoinsCount() >= 100)
                {
                    autoWordCountText.text = ((int)(SaveData.GetCoinsCount() / 100f)).ToString();
                }
                else
                {
                    autoWordCountText.text = ((int)0).ToString();
                }
            }
            else
            {
                SaveData.SetHintCount(0);
                hintText.text = SaveData.GetHintCount().ToString();
                specialLevelHintText.text = SaveData.GetHintCount().ToString();
            }
            
            progressionBarImage.fillAmount = SaveData.GetLoaderPercent();
            //print(GetLoaderPercent());
            progressionBarText.text = (int)(SaveData.GetLoaderPercent() * 100) + "%";
            
            if (s == '1')
            {
                print(s);
                SaveData.SetLoaderPercentage(0f);
                if (SaveData.GetLoaderImageCount() >= progressionImageList.Count)
                {
                    SaveData.SetLoaderImageCount(0);
                }
                SaveData.SetLoaderImageCount(SaveData.GetLoaderImageCount() + 1);
            }
            progressionImage.sprite = progressionImageList[SaveData.GetLoaderImageCount()];
            
        }
        winEmoji.GetComponent<Image>().sprite = winEmojiSprites[Random.Range(0, winEmojiSprites.Count - 1)];
    }

    public void ShuffleReduce(int x)
    {
        CountTextDetails(x);
    }

    public void FiftyFiftyReduce(int x)
    {
        CountTextDetails(x);
    }
    public void HintReduce(int x)
    {
        hintCounter++;
        CountTextDetails(x);
    }

    public void EmojiReduce()
    {
        CountTextDetails(50);
    }
    public void CountTextDetails(int cutNum)
    {
        var totalCoins = SaveData.GetCoinsCount() - cutNum;
        totalCoins = totalCoins <= 0 ? 0 : totalCoins;
        SaveData.SetCoinCount(totalCoins);
        SaveData.SetHintCount((int)totalCoins / 50);
        coinCountText.text = totalCoins.ToString();
        hintText.text = SaveData.GetHintCount().ToString();
        specialLevelHintText.text = SaveData.GetHintCount().ToString();
        shuffleCountText.text = ((int)(totalCoins / 25)).ToString();
        fiftyFiftyCountText.text=((int)(totalCoins / 25)).ToString();
        autoWordCountText.text = ((int)(totalCoins / 100)).ToString();
        if (totalCoins < 100)
        {
            UIManagerScript.Instance.AutoButtonActiveFun();
            autoWordCountText.text = "0";
        }
    }
    public void AutoWordReduce()
    {
        CountTextDetails(100);
        /*if (ByteBrewManager.instance)
           {
               ByteBrewManager.instance.ProgressEvent(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),
                   hintCounter.ToString(), "NormalMode", "Hints");
           }*/
        //SetHintCount(GetHintCount() - 1);
        /*if (GetCoinsCount() >= 100)
        {
            UIManagerScript.Instance.autoWordButton.interactable = true;
        }*/
    }
    public void CoinsIncrease(int x)
    {
        //var s1 = UIManagerScript.Instance.GetSpecialLevelNumber().ToString()[^1];
        SaveData.SetCoinCount(SaveData.GetCoinsCount() + x);
        if (SaveData.GetCoinsCount() >= 50)
        {
            SaveData.SetHintCount((int)(SaveData.GetCoinsCount() / 50));
            /*if (s1 != '0')
            {
                UIManagerScript.Instance.hintButton.interactable = true;
            }*/
            
        }

        var s = SaveData.GetSpecialLevelNumber().ToString()[^1];
        if (s != '0')
        {
            if (SaveData.GetCoinsCount() >= 100)
            {
                UIManagerScript.Instance.AutoButtonActiveFun();
                autoWordCountText.text = ((int)(SaveData.GetCoinsCount() / 100)).ToString();
            }
            else
            {
                autoWordCountText.text = ((int)0).ToString();
            }
        }
        
        coinCountText.text = SaveData.GetCoinsCount().ToString();
        hintText.text = SaveData.GetHintCount().ToString();
        specialLevelHintText.text = SaveData.GetHintCount().ToString();
        shuffleCountText.text = ((int)(SaveData.GetCoinsCount() / 25)).ToString();
        fiftyFiftyCountText.text=((int)(SaveData.GetCoinsCount() / 25)).ToString();;

    }
    
    // public int GetSpinCount() => PlayerPrefs.GetInt("Spins Count", 1);
    // public void SetSpinCount(int countSpin) => PlayerPrefs.SetInt("Spins Count", countSpin);
    
}