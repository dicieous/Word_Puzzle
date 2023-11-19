using System.Collections;
using System.Collections.Generic;
using DDZ;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;



// [System.Serializable]
// public class HintWords { public List<int> wordNoToShow; }


public class UIManagerScript : MonoBehaviour
{
	public static UIManagerScript Instance;
	public CoinManager cm;
	public GameObject specialLevelObject;
	public GameObject endScreen,prefab,failPanel;
	public TextMeshProUGUI levelNo,movesText;
	public Material originalColor;
	public RectTransform coinEndReference;
	public RectTransform coinStartReference;
	
	public Button nextButton;
    public Button retryButton;
    [Header("Special level Button details")]
    public Button fiftyFiftyButton;
    public Button shuffleButton;
    [Header("DoubleCoins Button Details")] 
    public Button doubleCoinsButton;
    public Button loseItButton;
    [Header("FailPanelDetails")] 
    public Button getMovesButton;
    
    [Header("hint Button Details")] 
    public GameObject hintObj;
	public Button hintButton;
	public Button specialHintButton;
	[Header("Restart Button Details")]
	public Button restartButton;
	[Header("AutoWord Details")]
    public Button autoWordButton;
    public bool autoWordDisableWordBool;
    [Header("EmojiReveal")] 
    public Button emojiRevealButton;
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
    [Header("CoinDouble BarMeter Details")]
    public GameObject barMeterObj;
    public Slider slideBar;
    public TextMeshProUGUI slideCoinsText;
    //public Button looseButton;
    [Header("Calender Details")]
    public GameObject calendarPanel;
    public Button calenderButton;
    private void Awake()
	{
		if (!Instance) Instance = this;
	}

	private void Start()
	{
		StartButtonActivateFun();
		// autoWordButton.interactable = true; 
		if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1 
		    || SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2)
		{
			specialLevelObject.SetActive(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2);
			//print("One");
			hintButton.gameObject.SetActive(false);
			autoWordButton.gameObject.SetActive(false);
			restartButton.image.enabled = false;
			levelNo.gameObject.SetActive(false);
			movesText.gameObject.SetActive(false);
			emojiRevealButton.gameObject.SetActive(false);
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
				AutoButtonActiveFun();
				EmojiRevelButtonActiveFun();
			}

			if (s == '5')
			{
				giftLevel = true;
			}
			if(GAScript.instance) GAScript.instance.LevelStart(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);
		}

	}

	public void StartButtonActivateFun()
	{
		if ((PlayerPrefs.GetInt("Level", 1) == 1))
		{
			hintButton.gameObject.SetActive(false);
			//MonitizationScript.instance.giftObject.SetActive(false);
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

		if (GetSpecialLevelNumber() <= 3)
		{
			barMeterObj.SetActive(false);
		}
		if ((GetSpecialLevelNumber() <= 10))
		{
			movesText.gameObject.SetActive(false);
		}
		
		/*if ((GetSpecialLevelNumber() <= 11))
		{
			MonitizationScript.instance.bubble2X.SetActive(false);
		}*/

		if ((GetSpecialLevelNumber() <= 13))
		{
			MonitizationScript.instance.giftObject.SetActive(false);
		}

		if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1)
		{
			calenderButton.gameObject.SetActive(GetSpecialLevelNumber() >= 30);
		}

		if (GetSpecialLevelNumber() <= 130)
		{
			emojiRevealButton.gameObject.SetActive(false);
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
            MonitizationScript.instance.giftObject.SetActive(false);
            targetCongratulationImage.GetComponent<Image>().sprite =
	            congratulationsImages[Random.Range(0, congratulationsImages.Count)];

            if (s != '0')
            {
	            DOVirtual.DelayedCall(0.05f, () => { LevelProgressionBarFun(); },false);
            }
            endScreen.SetActive(true);
	            
            ///////////----without double coins load bar ---------/////////
            if (GetSpecialLevelNumber() <= 3)
            {
	            CoinsDoubleClaimFun(10);
	            print(":::::::::::::::::::::::::::::::::::::");
            }
            ///////////-------- with double coinsFun -------------/////////
            else
            {
	            CoinsDoubleBarFun(10);
            }
            
            /*if (s != '0' )
            {
	            MonitizationScript.instance.giftObject.SetActive(false);
	            targetCongratulationImage.GetComponent<Image>().sprite =
		            congratulationsImages[Random.Range(0, congratulationsImages.Count)];
	            DOVirtual.DelayedCall(0.05f, () => { LevelProgressionBarFun(); },false);
	            endScreen.SetActive(true);
	            
	            ///////////----without double coins load bar ---------/////////
	            if (GetSpecialLevelNumber() <= 3)
	            {
		            CoinsDoubleClaimFun(10);
	            }
	            ///////////-------- with double coinsFun -------------/////////
	            else
	            {
		            CoinsDoubleBarFun(10);
	            }
	            /*DOVirtual.DelayedCall(2f, () =>
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
	        }#1#
            }
            else
            {
	            print("Win Calling");
                EmojiManager.Instance.winPanel.SetActive(true);
                DOVirtual.DelayedCall(0.5f, () =>
                {
	                
                    StartCoroutine(PlayCoinCollectionFx(coinMovePos,diamondFxParent,centerParticleRect,diamondParticlesRect));
                    
                },false);
                DOVirtual.DelayedCall(2f, () =>
                {
                    CoinManager.instance.CoinsIncrease(10);
                    DOVirtual.DelayedCall(1f, ()=>
                    {
	                    if(GameEssentials.instance)GameEssentials.ShowInterstitialsAds("LevelComplete");
	                    MapLevelCall();
                    },false);
                    //EmojiManager.Instance.nextButton.interactable = true;
                },false);
                /*DOVirtual.DelayedCall(0.5f, () =>
                {
                    CoinManager.instance.CoinsIncrease(25);
                });#1#
            }*/
		},false);
	}

	private Tween _barTween;
	public void CoinsDoubleBarFun(int x)
	{
		int _coinNum = 0;
		_barTween = slideBar.DOValue(1, 0.75f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).OnUpdate(() =>
		{
			var val=slideBar.value;
			if (val <= 0.17)
			{
				print("X2");
				if (_coinNum == (x * 2)) return;
				_coinNum = (x * 2);
				slideCoinsText.text = _coinNum.ToString();
			}
			else if (val > 0.17 && val <= 0.39)
			{
				print("X4");
				if (_coinNum == (x * 4)) return;
				_coinNum = (x * 4);
				slideCoinsText.text = _coinNum.ToString();
			}
			else if (val > 0.39 && val <= 0.615)
			{
				print("X6");
				if (_coinNum == (x * 6)) return;
				_coinNum = (x * 6);
				slideCoinsText.text = _coinNum.ToString();
			}
			else if (val > 0.615 && val <= 0.84)
			{
				print("X8");
				if (_coinNum == (x * 8)) return;
				_coinNum = (x * 8);
				slideCoinsText.text = _coinNum.ToString();
			}
			else if (val > 0.84 && val <= 1.00)
			{
				print("X10");
				if (_coinNum == (x * 10)) return;
				_coinNum = (x * 10);
				slideCoinsText.text = _coinNum.ToString();
			}
		});
	}

	public static int coinIncreaseNum;
	public void DoubleCoinsButtonFun()
	{
		doubleCoinsButton.interactable = false;
		loseItButton.interactable = false;
		int num = 0;
		_barTween.Pause();
		var val=slideBar.value;
		if (val <= 0.17)
		{
			print("X2");
			num = 20;
		}
		else if (val > 0.17 && val <= 0.39)
		{
			print("X4");
			num = 40;
		}
		else if (val > 0.39 && val <= 0.615)
		{
			print("X6");
			num = 60;
		}
		else if (val > 0.615 && val <= 0.84)
		{
			print("X8");
			num = 80;
		}
		else if (val > 0.84 && val <= 1.00)
		{
			print("X10");
			num = 100;
		}

		coinIncreaseNum = num;
		//print("coins number:::::::::::::::::::::: "+coinIncreaseNum);
		///----------Ad calling-----------
		
		GameEssentials.RvType = RewardType.LevelCompleteReward;
		GameEssentials.ShowRewardedAds("LevelCompleteReward");
	}

	public void DoubleCoins_CallBack()
	{
		CoinsDoubleClaimFun(coinIncreaseNum);
	}
	public void CoinsDoubleClaimFun(int x)
	{
		if (loseItButton.interactable) loseItButton.interactable = false;
		_barTween.Pause();
		if (doubleCoinsButton.interactable) doubleCoinsButton.interactable = false;
			
		DOVirtual.DelayedCall(0.5f, () =>
		{
			StartCoroutine(PlayCoinCollectionFx(coinMovePos, diamondFxParent, centerParticleRect,
				diamondParticlesRect));
		});
		/*DOVirtual.DelayedCall(0.5f, () =>
		{
	                
			StartCoroutine(PlayCoinCollectionFx(coinMovePos,diamondFxParent,centerParticleRect,diamondParticlesRect));
	                
		});*/
		DOVirtual.DelayedCall(2f, () =>
		{
			CoinManager.instance.CoinsIncrease(x);
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
					if(GameEssentials.instance)GameEssentials.ShowInterstitialsAds("LevelComplete");
					MapLevelCall();
				},false);
			}
		},false);
	}
	public void FailPanelActive()
    {
        failPanel.SetActive(true);
    }

    //private bool _autoButtonActivate;
    public void AutoButtonActiveFun()
    {
	    if(!GameManager.Instance) return;
        autoWordDisableWordBool = false;
        if (!GameManager.Instance.levelCompleted && !GameManager.Instance.autoWordClick)
        {
	        if (GameManager.Instance.completeWordCubesList.Count > 0)
	        {
		        if (!GameManager.Instance.cameraMoving && !GameManager.Instance.wordTouch)
		        {
			        //_autoButtonActivate = true;
			        autoWordButton.interactable = true; 
			        
			        var magnetText = autoWordButton.image.rectTransform.GetChild(0).gameObject;
			        var coinsTxt = autoWordButton.image.rectTransform.GetChild(1).gameObject;
			        var rvIcon = autoWordButton.image.rectTransform.GetChild(2).gameObject;
			        var loadingIcon = autoWordButton.image.rectTransform.GetChild(3).gameObject;
			
			        magnetText.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			        coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
			        rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<100 && GameEssentials.IsRvAvailable());
			        loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<100 && !GameEssentials.IsRvAvailable());
		        }
	        }
        }
    }

    public void AutoButtonDisActive()
    {
	    if(!GameManager.Instance) return;
        autoWordDisableWordBool = true;
        autoWordButton.interactable = false;
        print("autowordclick");
        GameManager.Instance.autoWordClick = true;
        //_autoButtonActivate = false;
        HintButtonDeActiveFun();
        if (GameManager.Instance)
        {
	        GameManager.Instance.AutoCompleteFunc();
	        GameManager.Instance.scriptOff = true;
        }
        var magnetText = autoWordButton.image.rectTransform.GetChild(0).gameObject;
        var coinsTxt = autoWordButton.image.rectTransform.GetChild(1).gameObject;
        var rvIcon = autoWordButton.image.rectTransform.GetChild(2).gameObject;
        var loadingIcon = autoWordButton.image.rectTransform.GetChild(3).gameObject;
			
        magnetText.SetActive(CoinManager.instance.GetCoinsCount()>=100);
        coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
        rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<100 && GameEssentials.IsRvAvailable());
        loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<100 && !GameEssentials.IsRvAvailable());
    }
    public void AutoWordCompleteButton()
    {
	    if (CoinManager.instance.GetCoinsCount() >= 100)
	    {
		    AutoWordComplete_Callback();
		    CoinManager.instance.AutoWordReduce();
	    }
	    else
	    {
		    GameEssentials.RvType = RewardType.Magnet;
		    GameEssentials.ShowRewardedAds("Magnet");
	    }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void AutoWordComplete_Callback()
    {
	    if (autoWordButton.interactable && !autoWordDisableWordBool)
	    {
		    //AutoButtonDisActive();
		    if (!GameManager.Instance.autoWordClick /*&& _autoButtonActivate*/)
		    {
			    AutoButtonDisActive();
		    }
		    DOVirtual.DelayedCall(1.75f, () =>
		    {
			    HintButtonActiveFun();
			    GameManager.Instance.scriptOff = false;
		    },false);
		    DOVirtual.DelayedCall(4.5f,() =>
		    {
			    // print("One attack::::::::::::::::::::::::::::::::::::::");
			    GameManager.Instance.autoWordClick = false;
			    AutoButtonActiveFun();
		    },false);
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
		if (CoinManager.instance.GetCoinsCount() >= 50) Hint_CallBack();
		else
		{
			GameEssentials.RvType = RewardType.Hint;
			GameEssentials.ShowRewardedAds("Hint");
		}
	}

	// ReSharper disable Unity.PerformanceAnalysis
	public void Hint_CallBack()
	{
		if (!GameManager.Instance) return;
		HintButtonDeActiveFun();
		GameManager.Instance.ShowTheText();
		
		if (!SoundHapticManager.Instance) return;
		SoundHapticManager.Instance.Vibrate(30);
		SoundHapticManager.Instance.Play("ButtonClickMG");
	}

	// ReSharper disable Unity.PerformanceAnalysis
	public void HintButtonActiveFun()
	{
		if(!GameManager.Instance) return;
		if (!GameManager.Instance.cameraMoving && !GameManager.Instance.levelCompleted && !GameManager.Instance.wordTouch)
		{
			if (!GameManager.Instance.hintSpawnObject && !hintButton.interactable)
			{
				hintButton.interactable = true;
			
				var hintsTxt = hintButton.image.rectTransform.GetChild(0).gameObject;
				var coinsTxt = hintButton.image.rectTransform.GetChild(1).gameObject;
				var rvIcon = hintButton.image.rectTransform.GetChild(2).gameObject;
				var loadingIcon = hintButton.image.rectTransform.GetChild(3).gameObject;
			
				hintsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
				coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			
				rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && GameEssentials.IsRvAvailable());
				loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && !GameEssentials.IsRvAvailable());
			}
			
		}
	}
	public void HintButtonDeActiveFun()
	{
		if (!hintButton.interactable && !GameManager.Instance.hintSpawnObject)
		{
			hintButton.interactable = !hintButton.interactable;
		
			var hintsTxt = hintButton.image.rectTransform.GetChild(0).gameObject;
			var coinsTxt = hintButton.image.rectTransform.GetChild(1).gameObject;
			var rvIcon = hintButton.image.rectTransform.GetChild(2).gameObject;
			var loadingIcon = hintButton.image.rectTransform.GetChild(3).gameObject;
			
			hintsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			
			rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && GameEssentials.IsRvAvailable());
			loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && !GameEssentials.IsRvAvailable());
		}
		
	}

	public void OnEmojiRevelButton()
	{
		/*var countNum=GameManager.Instance.stickingCubes.Count;
		for (int i = 0; i < countNum; i++)
		{
			if (GameManager.Instance.stickingCubes[i].gameObject.activeInHierarchy &&
			    !GameManager.Instance.stickingCubes[i].correctWordMade)
			{
				if (GameManager.Instance.stickingCubes[i].emojiRevel)
				{
					if (CoinManager.instance.GetCoinsCount() >= 50) Emoji_CallBack();
					else
					{
						GameEssentials.RvType = RewardType.ImageReveal;
						GameEssentials.ShowRewardedAds("ImageReveal");
					}
					break;
				}
			}
		}*/
		if (CoinManager.instance.GetCoinsCount() >= 50) Emoji_CallBack();
		else
		{
			GameEssentials.RvType = RewardType.ImageReveal;
			GameEssentials.ShowRewardedAds("ImageReveal");
		}
	}

	public void Emoji_CallBack()
	{
		var countNum=GameManager.Instance.stickingCubes.Count;
		for (int i = 0; i < countNum; i++)
		{
			if (GameManager.Instance.stickingCubes[i].gameObject.activeInHierarchy &&
			    !GameManager.Instance.stickingCubes[i].correctWordMade)
			{
				if (GameManager.Instance.stickingCubes[i].emojiRevel && !GameManager.Instance.stickingCubes[i].emojiRevel)
				{
					EmojiRevelButtonDeActivate();
					GameManager.Instance.stickingCubes[i].emojiRevel.SetActive(true);
					break;
				}
			}
		}
	}
	public void EmojiRevelButtonActiveFun()
	{
		if(!GameManager.Instance) return;
		if (GameManager.Instance.cameraMoving)
		{
			var countNum=GameManager.Instance.stickingCubes.Count;
			for (int i = 0; i < countNum; i++)
			{
				if (GameManager.Instance.stickingCubes[i].gameObject.activeInHierarchy &&
				    !GameManager.Instance.stickingCubes[i].correctWordMade)
				{
					if (GameManager.Instance.stickingCubes[i].emojiRevel && !GameManager.Instance.stickingCubes[i].emojiRevel)
					{
						emojiRevealButton.interactable = true;
						break;
					}
				}
			}
			var emojiText = emojiRevealButton.image.rectTransform.GetChild(0).gameObject;
			var coinsTxt = emojiRevealButton.image.rectTransform.GetChild(1).gameObject;
			var rvIcon = emojiRevealButton.image.rectTransform.GetChild(2).gameObject;
			var loadingIcon = emojiRevealButton.image.rectTransform.GetChild(3).gameObject;
			
			emojiText.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			
			rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && GameEssentials.IsRvAvailable());
			loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && !GameEssentials.IsRvAvailable());
		}
	}

	public void EmojiRevelButtonDeActivate()
	{
		emojiRevealButton.interactable = true;
		var emojiText = emojiRevealButton.image.rectTransform.GetChild(0).gameObject;
		var coinsTxt = emojiRevealButton.image.rectTransform.GetChild(1).gameObject;
		var rvIcon = emojiRevealButton.image.rectTransform.GetChild(2).gameObject;
		var loadingIcon = emojiRevealButton.image.rectTransform.GetChild(3).gameObject;
			
		emojiText.SetActive(CoinManager.instance.GetCoinsCount()>=50);
		coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			
		rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && GameEssentials.IsRvAvailable());
		loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && !GameEssentials.IsRvAvailable());
		DOVirtual.DelayedCall(0.2f, () =>
		{
			EmojiRevelButtonActiveFun();
		},false);
	}
	public void LevelProgressionBarFun()
	{
		var num = CoinManager.instance.GetLoaderPercent() + ((1f / 9f));
		//CoinManager.instance.SetLoaderPercentage(CoinManager.instance.GetLoaderPercent() + ((1f / 9f)));
		CoinManager.instance.progressionBarImage.DOFillAmount(num, 1f);
		CoinManager.instance.progressionBarText.text = ((int)(num * 100) + "%");
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
	   if(!GameEssentials.instance) return;

	   GameEssentials.RvType = RewardType.NoMoreMoves;
	   GameEssentials.ShowRewardedAds("NoMoreMoves");
    }

    public void GetMoreMoves_CallBack()
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
			    if(GameEssentials.instance)GameEssentials.ShowInterstitialsAds("LevelComplete");
			    MapLevelCall();
		    },false);
	    },false);
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
	    SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }
	public int GetSpecialLevelNumber() => PlayerPrefs.GetInt("SpecialLevelNumber", 1);
	public void SetSpecialLevelNumber(int levelNum) => PlayerPrefs.SetInt("SpecialLevelNumber", levelNum);

	public int GetStart100Coins() => PlayerPrefs.GetInt("GetFree100Coins", 0);
	public void SetStart100Coins(int val) => PlayerPrefs.SetInt("GetFree100Coins", val);

	public void CalendarButtonPress() =>
		calendarPanel.gameObject.SetActive(!calendarPanel.gameObject.activeInHierarchy);
}

