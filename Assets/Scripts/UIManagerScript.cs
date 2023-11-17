using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;



// [System.Serializable]
// public class HintWords { public List<int> wordNoToShow; }


public class UIManagerScript : MonoBehaviour
{
	public static UIManagerScript Instance;
	public CoinManager cm;
	
	public GameObject endScreen,prefab,failPanel;
	public TextMeshProUGUI levelNo,movesText;
	public Material originalColor;
	public RectTransform coinEndReference;
	public RectTransform coinStartReference;
	
	public Button nextButton;
    public Button retryButton;
    [Header("FailpanelDetails")] 
    public Button getMovesButton;
    
    [Header("hint Button Details")] 
    public GameObject hintObj;
	public Button hintButton;
	[Header("Restart Button Details")]
	public Button restartButton;
	[Header("AutoWord Details")]
    public Button autoWordButton;
    public bool autoWordDisableWordBool;
    
	[Header("Levels changing")]
	public GameObject starparticleEffect;

	public Image tutorialHand, tutorialHand2;
    public List<Collider> gameobject1, gameobject2;
	public List<Vector2> targetPos;
	public GameObject tutorialtext;

    // [Space(10)] [Header("Hint Stuff")]
    // public List<GameObject> hintsWordList; 
    // public List<HintWords> hintWordsToShow;
    
    [Space(10)]
	public GameObject targetCongratulationImage;
	public List<Sprite> congratulationsImages;
	private int countnumhelp;
    
    // Lion Level Attempts Counter;
    private static int levelAttempts;

    //public GameObject gifAnimationObj;
    [Header("Gift Details")] 
    public bool giftLevel;
    public GameObject giftJumpPLace;
    public GameObject giftJumpPLace2;
    public GameObject giftPanel; 
    public GameObject coinsObj;
    public GameObject giftMagnetObj;
    public GameObject giftHintObj;
    public Button claimButton;
    public RectTransform giftCoinMovePOs;
    public GameObject giftDiamondFxParent;
    public RectTransform giftCenterParticleRect;
    public List<RectTransform> giftDiamondParticlesRect;
    //public Button looseButton;

    private void Awake()
	{
		if (!Instance) Instance = this;
	}

	private void Start()
	{
		autoWordButton.interactable = true; 
		if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
		{
			//print("One");
			hintButton.gameObject.SetActive(false);
			autoWordButton.gameObject.SetActive(false);
			restartButton.image.enabled = false;
			levelNo.gameObject.SetActive(false);
			movesText.gameObject.SetActive(false);
		}
		else
		{
			cm = CoinManager.instance;
			var s = GetSpecialLevelNumber().ToString()[^1];
			if (s == '0')
			{
				levelNo.text = "LEVEL " + GetSpecialLevelNumber() +"\n(Boss Level)";
			}
			else
			{
				levelNo.text = "LEVEL " + GetSpecialLevelNumber();
				
				HintButtonActiveFun();
				AutoButtonActive();
			}

			if (s == '5')
			{
				giftLevel = true;
			}
			
			StartButtonActivateFun();
			
			if(GAScript.instance) GAScript.instance.LevelStart(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);
		}

	}

	public void StartButtonActivateFun()
	{
		if ((PlayerPrefs.GetInt("Level", 1) == 1))
		{
			hintButton.gameObject.SetActive(false);
			MonitizationScript.instance.giftObject.SetActive(false);
			if(GameManager.Instance)
				GameManager.Instance.ShowTheText();
			if(tutorialtext)
				tutorialtext.SetActive(true);
			foreach (var t in gameobject2)
			{
				t.enabled = false;
			}
		}

		if ((GetSpecialLevelNumber() <= 5))
		{
			autoWordButton.gameObject.SetActive(false);
		}
		
		if ((GetSpecialLevelNumber() <= 10))
		{
			movesText.gameObject.SetActive(false);
		}
		
		if ((GetSpecialLevelNumber() <= 11))
		{
			MonitizationScript.instance.bubble2X.SetActive(false);
		}

		if ((GetSpecialLevelNumber() <= 13))
		{
			MonitizationScript.instance.giftObject.SetActive(false);
		}
		
	}
	public void HelpHand()
	{
		if (tutorialHand || tutorialHand2)
		{
			if (countnumhelp == 0)
			{
				tutorialHand.gameObject.SetActive(true);
                tutorialHand.enabled = true; 
				tutorialHand.rectTransform.DOAnchorPos(targetPos[0], 2f).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Restart);
				countnumhelp = 1;
			}
            
			else if (countnumhelp == 1)
			{
				//print("tutorial2");
				tutorialHand2.gameObject.SetActive(true);
                tutorialHand.enabled = false;
				tutorialHand2.rectTransform.DOAnchorPos(targetPos[1], 2f).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Restart);
				countnumhelp = 2;
                foreach (var t in gameobject2)
                {
                    t.enabled = true;
                }
				/*for (int i = 0; i < gameobject2.Count; i++)
				{
					gameobject2[i].enabled = true;
				}
				for (int i = 0; i < gameobject1.Count; i++)
				{
					gameobject1[i].enabled = false;
				}*/
//				print(countnumhelp);
			
			}
		}
		
	}
	
	
	public RectTransform coinMovePos;
	public GameObject diamondFxParent;
	public RectTransform centerParticleRect;
	public List<RectTransform> diamondParticlesRect;
	// ReSharper disable Unity.PerformanceAnalysis
	public IEnumerator PlayCoinCollectionFx(RectTransform iconObj,GameObject diamondParent,RectTransform centerParticlePos,List<RectTransform> diamondParticlesRectList)
	{
		//yield return new WaitForSeconds(1.5f);
		diamondParent.SetActive(true);
		for (int i = 0; i < diamondParticlesRectList.Count; i++)
		{
			diamondParticlesRectList[i].gameObject.SetActive(true);
			diamondParticlesRectList[i].DOLocalMove(centerParticlePos.localPosition, 0.25f).From();
		}
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
		//SoundsController.instance.PlaySound(SoundsController.instance.moneyGot);
		yield return new WaitForSeconds(0.5f);
		//SoundsController.instance.PlaySound(SoundsController.instance.coinCollection);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("FinalCoins");
		for (int i = 0; i < diamondParticlesRectList.Count; i++)
		{
			var i1 = i;
			var dofundup = diamondParticlesRectList[i].DOMove(iconObj.position, 0.8f);
			dofundup.OnComplete(() =>
			{
				diamondParticlesRectList[i1].gameObject.SetActive(false);
				dofundup.Rewind();
				
			});
			yield return new WaitForSeconds(0.04f);
		}

		//diamondNumOnWinPanel.text = GameController.instance.GetTotalCoin().ToString();
	}
	public void WinPanelActive()
	{
		if (tutorialHand2)
        {
            tutorialHand2.enabled = false;
            tutorialtext.GetComponent<TextMeshProUGUI>().enabled = false;
        }
		//if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
		DOVirtual.DelayedCall(1.5f,()=>
		{
			
            var s = GetSpecialLevelNumber().ToString()[^1];
            if (s != '0')
            {
	            MonitizationScript.instance.giftObject.SetActive(false);
                targetCongratulationImage.GetComponent<Image>().sprite = 
                    congratulationsImages[Random.Range(0, congratulationsImages.Count)];
                DOVirtual.DelayedCall(0.05f, () =>
                {
	                LevelProgressionBarFun();
                });
                endScreen.SetActive(true);
                DOVirtual.DelayedCall(0.5f, () =>
                {
	                
	                StartCoroutine(PlayCoinCollectionFx(coinMovePos,diamondFxParent,centerParticleRect,diamondParticlesRect));
	                
                });
                DOVirtual.DelayedCall(2f, () =>
                {
                    CoinManager.instance.CoinsIncrease(10);
                    //nextButton.interactable = true;
                    if (giftLevel)
                    {
	                    if (GetSpecialLevelNumber() == 5)
	                    {
		                    GiftOpenFun(1);
	                    }
	                    else
	                    {
		                    var num = Random.Range(0, 2);
		                    GiftOpenFun(num);
	                    }
	                   
                    }
                    else
                    {
	                    DOVirtual.DelayedCall(1f, ()=>
	                    {
		                    MapLevelCall();
	                    });
                    }
                });
            }
            else
            {
	            print("Win Calling");
                EmojiManager.Instance.winPanel.SetActive(true);
                DOVirtual.DelayedCall(0.5f, () =>
                {
	                
                    StartCoroutine(PlayCoinCollectionFx(coinMovePos,diamondFxParent,centerParticleRect,diamondParticlesRect));
                    
                });
                DOVirtual.DelayedCall(2f, () =>
                {
                    CoinManager.instance.CoinsIncrease(10);
                    DOVirtual.DelayedCall(1f, ()=>
                    {
	                    MapLevelCall();
                    });
                    //EmojiManager.Instance.nextButton.interactable = true;
                });
                /*DOVirtual.DelayedCall(0.5f, () =>
                {
                    CoinManager.instance.CoinsIncrease(25);
                });*/
            }
		});
	}

    public void FailPanelActive()
    {
        failPanel.SetActive(true);
    }

    //private bool _autoButtonActivate;
    public void AutoButtonActive()
    {
        autoWordDisableWordBool = false;
        if (CoinManager.instance.GetCoinsCount() >= 100 && !GameManager.Instance.levelCompleted && !GameManager.Instance.autoWordClick)
        {
	        if (GameManager.Instance.completeWordCubesList.Count > 0)
	        {
		        if (!GameManager.Instance.cameraMoving && !GameManager.Instance.wordTouch)
		        {
			        //_autoButtonActivate = true;
			        autoWordButton.interactable = true; 
		        }
	        }
        }
    }

    public void AutoButtonDisActive()
    {
        autoWordDisableWordBool = true;
        autoWordButton.interactable = false;
    }
    public void AutoWordCompleteButton()
    {
        if (autoWordButton.interactable && !autoWordDisableWordBool)
        {
	        //AutoButtonDisActive();
	        if (!GameManager.Instance.autoWordClick /*&& _autoButtonActivate*/)
	        {
		        print("autowordclick");
		        GameManager.Instance.autoWordClick = true;
		        //_autoButtonActivate = false;
		        HintButtonDeActiveFun();
		        autoWordButton.interactable = false;
		        autoWordDisableWordBool = true;
		        CoinManager.instance.AutoWordReduce();
		        if (GameManager.Instance)
		        {
			        GameManager.Instance.AutoCompleteFunc();
			        GameManager.Instance.scriptOff = true;
		        }
		        
	        }

	        DOVirtual.DelayedCall(1.75f, () =>
	        {
		        HintButtonActiveFun();
		        GameManager.Instance.scriptOff = false;
	        });
	       DOVirtual.DelayedCall(6.5f,() =>
	       {
		      // print("One attack::::::::::::::::::::::::::::::::::::::");
		       GameManager.Instance.autoWordClick = false;
		       AutoButtonActive();
	       });
	       /*print("autoword::::"+autoWordButton.interactable);
	       print("WordClick::::::::"+GameManager.Instance.autoWordClick);*/
        }
        
    }
    
	private void Update()
	{
		/*var s = GetSpecialLevelNumber().ToString()[^1];
		if (s != '0' && (SceneManager.GetActiveScene().buildIndex != SceneManager.sceneCountInBuildSettings - 1) && !GameManager.Instance.levelCompleted)
		{
			if (cm.GetCoinsCount() >= 50 && !hintButton.interactable)
			{
				hintButton.interactable = true;
			}
			else if(cm.GetCoinsCount() < 50 && hintButton.interactable)
			{
				hintButton.interactable = false;
			}
			
		}
		*/

		
	}

	public void OnHintButtonClick()
	{
		//Debug.Log("Hint Button");
		if (GameManager.Instance)
		{
			HintButtonDeActiveFun();
			GameManager.Instance.ShowTheText();
			if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
			if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
		}
	}

	public void HintButtonActiveFun()
	{
		
		if (CoinManager.instance.GetCoinsCount() >= 50 && !GameManager.Instance.cameraMoving && !GameManager.Instance.levelCompleted && !GameManager.Instance.wordTouch)
		{
			hintButton.interactable = true;
		}
	}
	public void HintButtonDeActiveFun()
	{
		if (hintButton.interactable)
			hintButton.interactable = false;
	}

	public void LevelProgressionBarFun()
	{
		var num = CoinManager.instance.GetLoaderPercent() + ((1f / 9f));
		//CoinManager.instance.SetLoaderPercentage(CoinManager.instance.GetLoaderPercent() + ((1f / 9f)));
		CoinManager.instance.progressionBarImage.DOFillAmount(num, 1f);
		CoinManager.instance.progressionBarText.text = ((int)(num * 100) + "%");
	}

	public void GiftProgressionBar()
	{
		var num = CoinManager.instance.GetGiftLoaderPercent() + ((1f / 5f));
		CoinManager.instance.giftProgressionBarImage.DOFillAmount(num, 1f);
		CoinManager.instance.giftProgressionBarText.text = ((int)(num * 100) + "%");
	}
	public void NextSceneLoader()
	{
        ////////////////------------------------Block falling down here------------------------------
        /*if (!GameManager.Instance.levelTypeChanged)
        {
            GameManager.Instance.DestroyBlocks();
        }
        else
        {
            NextMoveFun();
        }*/
        NextMoveFun();
		nextButton.interactable = false;
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}
	
	public void NextMoveFun()
	{
		SetSpecialLevelNumber(GetSpecialLevelNumber() + 1);
		
		var s = GetSpecialLevelNumber().ToString()[^1];
		if (s == '0')
		{
			SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
			
			PlayerPrefs.SetInt("Special",1);
            CoinManager.instance.SetLoaderPercentage(0f);
			//PlayerPrefs.SetInt("ThisLevel", SceneManager.sceneCountInBuildSettings - 1);
		}
		else
		{
			levelAttempts = 0;
			PlayerPrefs.SetInt("Special",0);
			if (PlayerPrefs.GetInt("Level") >= (SceneManager.sceneCountInBuildSettings) - 2)
			{
				PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
				var i = Random.Range(2, SceneManager.sceneCountInBuildSettings-2);
				PlayerPrefs.SetInt("ThisLevel", i);
				SceneManager.LoadScene(i);
			}
			else
			{
				/*PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex + 1);
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);*/
				PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
				SceneManager.LoadScene(PlayerPrefs.GetInt("Level", 1));
				print("one"+PlayerPrefs.GetInt("Level", 1));
			}
            CoinManager.instance.SetLoaderPercentage(CoinManager.instance.GetLoaderPercent() + ((1f / 9f)));
            if (GetSpecialLevelNumber() >= 2)
            {
	            CoinManager.instance.SetGiftLoaderPercent(CoinManager.instance.GetGiftLoaderPercent() + ((1f / 5f)));
            }
            CoinManager.instance.SetGiftLoaderPercent(CoinManager.instance.GetGiftLoaderPercent() + ((1f / 5f)));
		}
        if(GAScript.instance) GAScript.instance.LevelCompleted(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);

	}
	public void ResetScreenOnClick()
	{
		//GameManager.Instance.ResetScreen();
        if(GAScript.instance) GAScript.instance.LevelRestart(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);
        levelAttempts++;
		//DOTween.KillAll();
        //GameManager.Instance.compSequence.Kill();
		var loadedScene = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(loadedScene);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}

    public void Retry()
    {
        retryButton.interactable = false;
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }

    public void GetMoreMovesFun()
    {
	    failPanel.SetActive(false);
	    GameManager.Instance.movesCount = 5;
	    movesText.text = GameManager.Instance.movesCount.ToString();
	    if(GameManager.Instance.scriptOff)
			GameManager.Instance.scriptOff = false;
	    if(GameManager.Instance.levelFail)
			GameManager.Instance.levelFail = false;
	    hintButton.interactable = true;
	    autoWordButton.interactable = true;
    }
    public void GiftOpenFun(int num)
    {
	    DOVirtual.DelayedCall(1f, () =>
	    {
		    giftPanel.SetActive(true);
		    giftPanel.transform.GetChild(0).DOLocalRotate(new Vector3(0, 0, 360f), 1f).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear);
		    var countList = new List<int>() {20,30,40,50};
		    countList.Sort((a, b) => 1 - 2 * Random.Range(0, countList.Count));
		    var coinCount = countList[0];
		    switch (num)
		    {
			    case 0:
				    GiftPopFun(coinsObj, giftHintObj,giftJumpPLace,giftJumpPLace2, 50,coinCount);
				    //_revelItem = "Hint";
				    break;
			    case 1:
				    GiftPopFun(coinsObj, giftMagnetObj,giftJumpPLace,giftJumpPLace2, 100,coinCount);
				    //_revelItem = "Magnet";
				    break;
			    default:
				    break;
		    }
		    
	    });
    }

    public void GiftPopFun(GameObject popObj,GameObject popObj2,GameObject popPosition,GameObject popPosition2,int coinsIncreaseNum,int randomCoinCount)
    {
	    _coinIncreaseNUm = randomCoinCount+coinsIncreaseNum;
	    print(_coinIncreaseNUm+"       "+ randomCoinCount +"          "+coinsIncreaseNum);
	    DOVirtual.DelayedCall(1.5f, () =>
	    {
		    popObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + randomCoinCount;
		    popObj.GetComponent<RectTransform>().DOScale(Vector3.one, 0.65f).SetEase(Ease.OutBounce);
		    popObj.GetComponent<RectTransform>().DOJumpAnchorPos(popPosition.GetComponent<RectTransform>().anchoredPosition, 400f, 1, 0.5f)
			    .SetEase(Ease.Linear);
		    /*coinsObj.GetComponent<RectTransform>().DOScale(Vector3.one, 0.05f);
		    DOVirtual.DelayedCall(1.15f, () =>
		    {
			    claimButton.GetComponent<RectTransform>().DOScale(Vector3.one, 0.05f).OnComplete(() =>
			    {
				    claimButton.interactable = true;
			    });
			    //StartCoroutine(PlayCoinCollectionFx());
		    });*/
		    //coinsObj.GetComponent<RectTransform>().DOJumpAnchorPos(giftJumpPLace.GetComponent<RectTransform>().anchoredPosition, 400f, 1, 1f).SetEase(Ease.Linear);
	    });
	    DOVirtual.DelayedCall(1.75f, () =>
	    {
		    popObj2.GetComponent<RectTransform>().DOScale(Vector3.one, 0.65f).SetEase(Ease.OutBounce);
		    popObj2.GetComponent<RectTransform>()
			    .DOJumpAnchorPos(popPosition2.GetComponent<RectTransform>().anchoredPosition, 400f, 1, 0.5f)
			    .SetEase(Ease.Linear).OnComplete(() =>
			    {
				    claimButton.GetComponent<RectTransform>().DOScale(Vector3.one, 0.05f).OnComplete(() =>
				    {
					    claimButton.interactable = true;
				    });
			    });
	    });
    }

    private int _coinIncreaseNUm;
    //private string _revelItem;
    public void GiftClaimFun()
    {
	    claimButton.interactable = false;
	    DOVirtual.DelayedCall(0.5f, () =>
	    {
		  
		    StartCoroutine(PlayCoinCollectionFx(giftCoinMovePOs,giftDiamondFxParent,giftCenterParticleRect,giftDiamondParticlesRect));
	    });
	    DOVirtual.DelayedCall(2f, () =>
	    {
		    CoinManager.instance.CoinsIncrease(_coinIncreaseNUm);
		    print("coinsincreasenum          "+_coinIncreaseNUm);
		    DOVirtual.DelayedCall(1f, () =>
		    {
			    MapLevelCall();
		    });
	    });
	    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
	    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
    }
    public void MapLevelNextButton()
    {
	    var s = GetSpecialLevelNumber().ToString()[^1];
	    if (s == '0')
	    {
		    SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 2);
		    PlayerPrefs.SetInt("Special",1);
		    CoinManager.instance.SetLoaderPercentage(0f);
	    }
	    else
	    {
		    levelAttempts = 0;
		    PlayerPrefs.SetInt("Special",0);
		    if (PlayerPrefs.GetInt("Level") >= (SceneManager.sceneCountInBuildSettings) - 3)
		    {
			    var i = Random.Range(2, SceneManager.sceneCountInBuildSettings-3);
			    PlayerPrefs.SetInt("ThisLevel", i);
			    SceneManager.LoadScene(i);
		    }
		    else
		    {
			    SceneManager.LoadScene(PlayerPrefs.GetInt("Level", 1));
		    }
		    //CoinManager.instance.SetLoaderPercentage(CoinManager.instance.GetLoaderPercent() + ((1f / 9f)));
	    }
	    if(GAScript.instance) GAScript.instance.LevelCompleted(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);
	    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
	    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
    }
    public void MapLevelCall()
    {
	    if (EmojiManager.Instance)
	    {
		    EmojiManager.Instance.SetListNumber(EmojiManager.Instance.GetListNumbers() + 1);
		    EmojiManager.Instance.SetPanelsDone(EmojiManager.Instance.GetPanelsDone() + 1);
	    }
	    
	    SetSpecialLevelNumber(GetSpecialLevelNumber() + 1);
	    PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
	    CoinManager.instance.SetLoaderPercentage(CoinManager.instance.GetLoaderPercent() + ((1f / 9f)));
	    if (GetSpecialLevelNumber() >= 2)
	    {
		    CoinManager.instance.SetGiftLoaderPercent(CoinManager.instance.GetGiftLoaderPercent() + ((1f / 5f)));
	    }
	    SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }
	public int GetSpecialLevelNumber() => PlayerPrefs.GetInt("SpecialLevelNumber", 1);
	public void SetSpecialLevelNumber(int levelNum) => PlayerPrefs.SetInt("SpecialLevelNumber", levelNum);
}

