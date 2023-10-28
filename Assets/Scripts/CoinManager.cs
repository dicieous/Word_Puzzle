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
    public TextMeshProUGUI coinCountText;

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
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
        var s = UIManagerScript.Instance.GetSpecialLevelNumber().ToString()[^1];
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
            
            coinCountText.text = GetCoinsCount().ToString();
            if (GetCoinsCount() >= 50)
            {
                var countNum = (int)GetCoinsCount() / 50;
                SetHintCount(countNum);
                hintText.text = GetHintCount().ToString();
            }
            else
            {
                SetHintCount(0);
                hintText.text = GetHintCount().ToString();
            }
            
            progressionBarImage.fillAmount = GetLoaderPercent();
            //print(GetLoaderPercent());
            progressionBarText.text = (int)(GetLoaderPercent() * 100) + "%";
            
            if (s == '1')
            {
                print(s);
                SetLoaderPercentage(0f);
                if (GetLoaderImageCount() >= progressionImageList.Count)
                {
                    SetLoaderImageCount(0);
                }
                SetLoaderImageCount(GetLoaderImageCount() + 1);
            }
            progressionImage.sprite = progressionImageList[GetLoaderImageCount()];
            winEmoji.GetComponent<Image>().sprite = winEmojiSprites[Random.Range(0, winEmojiSprites.Count - 1)];
        }
    }

    private int GetLoaderImageCount() => PlayerPrefs.GetInt("LoaderImageNumber", 0);
    private void SetLoaderImageCount(int num) => PlayerPrefs.SetInt("LoaderImageNumber", num);
    public void HintReduce()
    {
        if (GetHintCount() > 0)
        {
            hintCounter++;
            if (ByteBrewManager.instance)
            {
                ByteBrewManager.instance.ProgressEvent(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),
                    hintCounter.ToString(), "NormalMode", "Hints");
            }
            //SetHintCount(GetHintCount() - 1);
            SetCoinCount(GetCoinsCount() - 50);
            SetHintCount((int)GetCoinsCount() / 50);
            coinCountText.text = GetCoinsCount().ToString();
            hintText.text = GetHintCount().ToString();
        }
    }

    public void CoinsIncrease(int x)
    {
        SetCoinCount(GetCoinsCount() + x);
        if (GetCoinsCount() >= 50)
        {
            SetHintCount((int)(GetCoinsCount() / 50));
        }
        coinCountText.text = GetCoinsCount().ToString();
        hintText.text = GetHintCount().ToString();
    }
    
    public float GetLoaderPercent() => PlayerPrefs.GetFloat("LoaderPercentage", 0);
    public void SetLoaderPercentage(float percent) => PlayerPrefs.SetFloat("LoaderPercentage", percent);
    
    
    public int GetHintCount() => PlayerPrefs.GetInt("Hint Count", 0);
    public void SetHintCount(int countHint) => PlayerPrefs.SetInt("Hint Count", countHint);

    public int GetShuffleCount() => PlayerPrefs.GetInt("ShuffleCount", 0);
    public void SetShuffleCount(int countShuffle) => PlayerPrefs.SetInt("ShuffleCount", countShuffle);
    

    public int Get5050Count() => PlayerPrefs.GetInt("Count5050", 0);
    public void Set5050Count(int count5050) => PlayerPrefs.SetInt("Count5050", count5050);
    
    public int GetCoinsCount() => PlayerPrefs.GetInt("Coins Count", 0);

    public void SetCoinCount(int countCoin) => PlayerPrefs.SetInt("Coins Count", countCoin);
}