using System;
using System.Collections;
using System.Collections.Generic;
using DDZ;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class UIManagerScript : MonoBehaviour
{
	public static UIManagerScript Instance;
	public CoinManager cm;
	public GameObject specialLevelObject;
	//public string question;
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
    
    [Header("Coin Deduce Animation Details")] 
    //public Sprite ReducecoinImage;
    public GameObject instanceImageRef;
    public GameObject instancePos;
    public GameObject magnetPosDeduct;
    public GameObject hintPosDeduct;

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

	public Image tutorialHand;
    // public List<Collider> gameobject1, gameobject2;
	// public List<Vector2> targetPos;
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
    public int _dailyRewardNumber;
    public static String dailyRewardDetails;
    public bool giftLevel;
    public GameObject giftJumpPLace;
    public GameObject giftJumpPLace2;
    public GameObject giftPanel; 
    public GameObject coinsObj;
    public GameObject giftMagnetObj;
    public GameObject giftHintObj;
    public GameObject giftMagnetInstancePos;
    public GameObject giftMagnetMovePosition;
    public GameObject giftHintMovePosition;
    public TextMeshProUGUI giftMagnetCountTemp;
    public TextMeshProUGUI giftHintCountTemp;
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
    public Image calIndicator;
    public Image calenderRvImage;
    public static int _hintCount = 1, _magnetCount = 1, _imageRevealCount = 1, _levelCompleteRewardCount = 1, _noMoreMovesCount = 1;
    [Header("SpinWheel")] 
    public Button spinwheel;
    [Header("DailyReward")] 
    public Button dailyRewardBtr;
    
    [Header("Background Dotted")]
    [SerializeField] private Canvas bgDottedCanvas;

    [Header("Background Plane")] [SerializeField]
    private ScrollingTex backgroundPlane;

    private void Awake()
	{
		if (!Instance) Instance = this;
		if(SceneManager.GetActiveScene().name == "Map") return;
		var originalBg = GameObject.Find("Plane");
		if(!GameObject.Find("BgCanvas"))
		{
			var bgCanvas = Instantiate(bgDottedCanvas);
			bgCanvas.worldCamera = Camera.main;
		}
		var directionalLight = FindObjectOfType<Light>();
		directionalLight.color = Color.white;
		if(!originalBg.GetComponent<ScrollingTex>())
		{
			Destroy(originalBg);
			Instantiate(backgroundPlane, new Vector3(2.9000001f, 1.29999995f, 0.790000021f),
				new Quaternion(-0.670953453f, 0.223207206f, -0.223207206f, 0.670953453f));
		}
		//CalendarIndicatorCheck();
	}

    private void CalendarIndicatorCheck()
    {
	    var canShowCalIndicator =
		    PlayerPrefs.GetInt("DailyChallenges_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year, 0) == 0 && GetSpecialLevelNumber() <= 30; 
	    calIndicator.gameObject.SetActive(canShowCalIndicator);
    }

	private void Start()
	{
		StartButtonActivateFun();
		// autoWordButton.interactable = true; 
		if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
		{
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
			//if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2)
			cm = CoinManager.instance;
			var s = GetSpecialLevelNumber().ToString()[^1];
			specialLevelObject.SetActive(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2);
			if (s == '0' && SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2)
			{
				levelNo.text = "LEVEL " + GetSpecialLevelNumber() +"\n(Boss Level)";
				hintButton.gameObject.SetActive(false);
				autoWordButton.gameObject.SetActive(false);
				restartButton.image.enabled = false;
				movesText.gameObject.SetActive(false);
				emojiRevealButton.gameObject.SetActive(false);
				endScreen.transform.GetChild(4).gameObject.SetActive(false);
				endScreen.transform.GetChild(5).gameObject.SetActive(false);
				MonitizationScript.instance.giftObject.SetActive(false);
			}
			else
			{
				if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 33)
				{
					levelNo.text = "DAILY \n CHALLENGE";
					endScreen.transform.GetChild(4).gameObject.SetActive(false);
					endScreen.transform.GetChild(5).gameObject.SetActive(false);
				}
				else levelNo.text = "LEVEL " + GetSpecialLevelNumber();
				
				if (GameManager.Instance)
				{
					if (GameManager.Instance.question!=null)
					{
						levelNo.transform.GetChild(0).gameObject.SetActive(false);
						levelNo.transform.GetChild(1).gameObject.SetActive(true);
						levelNo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameManager.Instance.question;
					}
				}
				HintButtonActiveFun();
				AutoButtonActiveFun();
				EmojiRevelButtonActiveFun();
			}
			if (s == '5' || SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 33 &&
			    SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 2)
			{
				if (((SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 33 &&
				      SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 2)))
				{
					movesText.gameObject.SetActive(true);
					levelNo.transform.GetChild(2).gameObject.SetActive(true);
					DOVirtual.DelayedCall(1.5f, () =>
					{
						levelNo.transform.GetChild(2).gameObject.SetActive(false);
					},false);
				}
				giftLevel = true;
			}
			else 
			{
				giftLevel = false;
				// levelNo.gameObject.SetActive(false);
			}

			if ((SceneManager.GetActiveScene().buildIndex <= SceneManager.sceneCountInBuildSettings - 33 ||
			     SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2))
			{
				if(GAScript.instance) GAScript.instance.LevelStart(GetSpecialLevelNumber().ToString(),levelAttempts);
			}
			
		}

		switch (dailyRewardDetails)
		{
			case "C*200 H*1":
				_dailyRewardNumber = 1;
				//print("dailyReward:::::::::::"+_dailyRewardNumber);
				break;
			case "C*100 H*2":
				_dailyRewardNumber = 2;
				//print("dailyReward:::::::::::"+_dailyRewardNumber);
				break;
			case "C*50 M*1":
				_dailyRewardNumber = 3;
				//print("dailyReward:::::::::::"+_dailyRewardNumber);
				break;
			case "C*100 M*1":
				_dailyRewardNumber = 4;
				//print("dailyReward:::::::::::"+_dailyRewardNumber);
				break;
			case "C*150 H*2":
				_dailyRewardNumber = 5;
				//print("dailyReward:::::::::::"+_dailyRewardNumber);
				break;
			default:
				break;
		}
	}

	public void StartButtonActivateFun()
	{
		if ((PlayerPrefs.GetInt("Level", 1) == 1))
		{
			//hintButton.gameObject.SetActive(false);
			//MonitizationScript.instance.giftObject.SetActive(false);
			/*if(GameManager.Instance)
				GameManager.Instance.ShowTheText();*/
			if(tutorialtext)
				tutorialtext.SetActive(true);
			/*foreach (var t in gameobject2)
			{
				t.enabled = false;
			}*/
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

		if ((GetSpecialLevelNumber() == 11))
		{
			movesText.gameObject.transform.GetChild(0).gameObject.SetActive(true);
			DOVirtual.DelayedCall(5f, () =>
			{
				movesText.gameObject.transform.GetChild(0).gameObject.SetActive(false);
			},false);
		}
		
		/*if ((GetSpecialLevelNumber() <= 11))
		{
			MonitizationScript.instance.bubble2X.SetActive(false);
		}*/

		if ((GetSpecialLevelNumber() <= 13) || SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1)
		{
			MonitizationScript.instance.giftObject.SetActive(false);
		}

		if (GetSpecialLevelNumber() == 121)
		{
			levelNo.transform.GetChild(2).gameObject.SetActive(true);
		}
		if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1)
		{
			calenderButton.gameObject.SetActive(GetSpecialLevelNumber() >= 16);
			if (GetSpecialLevelNumber() == 30)
			{
				calenderButton.GetComponent<DOTweenAnimation>().DOPlay();
			}
		}

		if (GetSpecialLevelNumber() == 30 && GetCalenderUnlockCheck() != "Unlock")
		{
			SetCalenderUnlockCheck("Unlock");
		}
		if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
		{
			spinwheel.gameObject.SetActive(true);
		}
		/*if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
		{
			dailyRewardBtr.gameObject.SetActive(true);
		}*/
		/*if (GetSpecialLevelNumber() <= 130)
		{
			emojiRevealButton.gameObject.SetActive(false);
		}*/
	}
	public void HelpHand()
	{
		if (tutorialHand) tutorialHand.gameObject.SetActive(false);
	}
	[Header("Claim Coins ParticleEffect")]
	public RectTransform claimCoinMovePos;
	public GameObject claimDiamondFxParent;
	public RectTransform claimCenterParticleRect;
	public List<RectTransform> claimDiamondParticlesRect;
	[Header("Coins ParticleEffect")]
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
		/*if (tutorialHand2)
        {
            tutorialHand2.enabled = false;
            if(tutorialtext) tutorialtext.GetComponent<TextMeshProUGUI>().enabled = false;
        }*/
		//if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
		DOVirtual.DelayedCall(2.5f,()=>
		{
            var s = GetSpecialLevelNumber().ToString()[^1];
            MonitizationScript.instance.giftObject.SetActive(false);
            targetCongratulationImage.GetComponent<Image>().sprite =
	            congratulationsImages[Random.Range(0, congratulationsImages.Count-1)];
           
            if (/*s != 0 &&*/ (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 33))
            {
	            DOVirtual.DelayedCall(0.05f, () => { LevelProgressionBarFun(); },false);
            }
            // endScreen.SetActive(true);
            CoinsDoubleClaimFun(10);
            return;
            ///////////----without double coins load bar ---------/////////
            if (GetSpecialLevelNumber() <= 3)
            {
	            CoinsDoubleClaimFun(10);
	            //print(":::::::::::::::::::::::::::::::::::::");
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
		
		var rvIcon = doubleCoinsButton.image.rectTransform.GetChild(2).gameObject;
		var loadingIcon = doubleCoinsButton.image.rectTransform.GetChild(3).gameObject;
		rvIcon.SetActive(GameEssentials.IsRvAvailable());
		loadingIcon.SetActive(!GameEssentials.IsRvAvailable());
		doubleCoinsButton.interactable = GameEssentials.IsRvAvailable();
		_barTween = slideBar.DOValue(1, 0.75f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).OnUpdate(() =>
		{
			var val=slideBar.value;
			if (val <= 0.17)
			{
				//print("X2");
				if (_coinNum == (x * 2)) return;
				_coinNum = (x * 2);
				slideCoinsText.text = _coinNum.ToString();
			}
			else if (val > 0.17 && val <= 0.39)
			{
				//print("X4");
				if (_coinNum == (x * 4)) return;
				_coinNum = (x * 4);
				slideCoinsText.text = _coinNum.ToString();
			}
			else if (val > 0.39 && val <= 0.615)
			{
				//print("X6");
				if (_coinNum == (x * 6)) return;
				_coinNum = (x * 6);
				slideCoinsText.text = _coinNum.ToString();
			}
			else if (val > 0.615 && val <= 0.84)
			{
				//print("X8");
				if (_coinNum == (x * 8)) return;
				_coinNum = (x * 8);
				slideCoinsText.text = _coinNum.ToString();
			}
			else if (val > 0.84 && val <= 1.00)
			{
				//print("X10");
				if (_coinNum == (x * 10)) return;
				_coinNum = (x * 10);
				slideCoinsText.text = _coinNum.ToString();
			}
		});
		if (!SoundHapticManager.Instance) return;
		SoundHapticManager.Instance.Play("BarMeter");
		//SoundHapticManager.Instance.Vibrate(30);
	}

	public static int coinIncreaseNum;
	private bool _claimCoinsClick;
	public void DoubleCoinsButtonFun()
	{
		doubleCoinsButton.interactable = false;
		int num = 0;
		_barTween.Pause();
		var val=slideBar.value;
		if (val <= 0.17)
		{
			//print("X2");
			num = 20;
		}
		else if (val > 0.17 && val <= 0.39)
		{
			//print("X4");
			num = 40;
		}
		else if (val > 0.39 && val <= 0.615)
		{
			//print("X6");
			num = 60;
		}
		else if (val > 0.615 && val <= 0.84)
		{
			//print("X8");
			num = 80;
		}
		else if (val > 0.84 && val <= 1.00)
		{
			//print("X10");
			num = 100;
		}

		coinIncreaseNum = num;
		//print("coins number:::::::::::::::::::::: "+coinIncreaseNum);
		///----------Ad calling-----------

		_claimCoinsClick = true;
		if (!GameEssentials.IsRvAvailable()) return;
		GameEssentials.RvType = RewardType.LevelCompleteReward;
		GameEssentials.ShowRewardedAds("LevelCompleteReward");
		if(LionStudiosManager.instance)
			LionStudiosManager.AdsEvents(true, AdsEventState.Start,GetSpecialLevelNumber(),"Applovin","LevelCompleteReward",CoinManager.instance.GetCoinsCount());
		//loseItButton.interactable = false;
		if (SoundHapticManager.Instance)
		{
			SoundHapticManager.Instance.Pause("BarMeter");
			SoundHapticManager.Instance.Play("Pop");
			SoundHapticManager.Instance.Vibrate(30);
		}
	}

	public void DoubleCoins_CallBack()
	{
		CoinsDoubleClaimFun(coinIncreaseNum);
	}
	public void CoinsDoubleClaimFun(int x)
	{
		
		_barTween.Pause();
		if (doubleCoinsButton.interactable) doubleCoinsButton.interactable = false;
		if (SoundHapticManager.Instance)
		{
			SoundHapticManager.Instance.Pause("BarMeter");
			SoundHapticManager.Instance.Play("Pop");
			SoundHapticManager.Instance.Vibrate(30);
		}
		DOVirtual.DelayedCall(0.5f, () =>
		{
			if (_claimCoinsClick)
			{
				StartCoroutine(PlayCoinCollectionFx(claimCoinMovePos, claimDiamondFxParent, claimCenterParticleRect,
					claimDiamondParticlesRect));
			}
			else
			{
				StartCoroutine(PlayCoinCollectionFx(coinMovePos, diamondFxParent, centerParticleRect,
					diamondParticlesRect));
				
			}
		},false);
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
				if((SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 33))
				{
					switch (_dailyRewardNumber)
					{
						case 1:
							GiftOpenFun(2);
							break;
						case 2:
							GiftOpenFun(3);
							break;
						case 3:
							GiftOpenFun(4);
							break;
						case 4:
							GiftOpenFun(5);
							break;
						case 5:
							GiftOpenFun(6);
							break;
						default:
							break;
					}
					giftMagnetCountTemp.text = ((int)CoinManager.instance.GetCoinsCount() / 100).ToString();
					giftHintCountTemp.text = ((int)CoinManager.instance.GetCoinsCount() / 50).ToString();
				}
				else
				{
					if (GetSpecialLevelNumber() == 5)
					{
						GiftOpenFun(1);
						giftMagnetCountTemp.text = ((int)CoinManager.instance.GetCoinsCount() / 100).ToString();
						giftHintCountTemp.text = ((int)CoinManager.instance.GetCoinsCount() / 50).ToString();
					}
					else
					{
						var num = Random.Range(0, 2);
						GiftOpenFun(num);
						giftMagnetCountTemp.text = ((int)CoinManager.instance.GetCoinsCount() / 100).ToString();
						giftHintCountTemp.text = ((int)CoinManager.instance.GetCoinsCount() / 50).ToString();
					}
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
		if (loseItButton.interactable) loseItButton.interactable = false;
	}
	public void FailPanelActive()
    {
        failPanel.SetActive(true);
	    var rvIcon = getMovesButton.image.rectTransform.GetChild(0).gameObject;
	    var loadingIcon = getMovesButton.image.rectTransform.GetChild(1).gameObject;
	    rvIcon.SetActive(GameEssentials.IsRvAvailable());
	    loadingIcon.SetActive(!GameEssentials.IsRvAvailable());
	    getMovesButton.interactable = GameEssentials.IsRvAvailable();
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
			        _autoButtonActive = true;
			        
			       // var magnetText = autoWordButton.image.rectTransform.GetChild(0).gameObject;
			       // var coinsTxt = autoWordButton.image.rectTransform.GetChild(1).gameObject;
			        var rvIcon = autoWordButton.image.rectTransform.GetChild(2).gameObject;
			        var loadingIcon = autoWordButton.image.rectTransform.GetChild(3).gameObject;
			
			        //magnetText.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			        //coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
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
        _autoButtonActive = false;
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
        //var magnetText = autoWordButton.image.rectTransform.GetChild(0).gameObject;
       // var coinsTxt = autoWordButton.image.rectTransform.GetChild(1).gameObject;
        var rvIcon = autoWordButton.image.rectTransform.GetChild(2).gameObject;
        var loadingIcon = autoWordButton.image.rectTransform.GetChild(3).gameObject;
			
        //magnetText.SetActive(CoinManager.instance.GetCoinsCount()>=100);
        //coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
        rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<100 && GameEssentials.IsRvAvailable());
        loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<100 && !GameEssentials.IsRvAvailable());
    }
    public void AutoWordCompleteButton()
    {
	    
	    if (CoinManager.instance.GetCoinsCount() >= 100)
	    {
		    StartCoroutine(CoinsReduceAnim(instanceImageRef, instancePos, magnetPosDeduct));
		    AutoWordComplete_Callback();
		    CoinManager.instance.AutoWordReduce();
	    }
	    else
	    {
		    if (!GameEssentials.IsRvAvailable())  return;
		    GameEssentials.RvType = RewardType.Magnet;
		    GameEssentials.ShowRewardedAds("Magnet");
		    if(LionStudiosManager.instance)
			    LionStudiosManager.AdsEvents(true, AdsEventState.Start,GetSpecialLevelNumber(),"Applovin","Magnet",CoinManager.instance.GetCoinsCount());
	    }
	    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
	    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
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
		    DOVirtual.DelayedCall(2.5f,() =>
		    {
			    // print("One attack::::::::::::::::::::::::::::::::::::::");
			    GameManager.Instance.autoWordClick = false;
			    AutoButtonActiveFun();
		    },false);
		    /*print("autoword::::"+autoWordButton.interactable);
		    print("WordClick::::::::"+GameManager.Instance.autoWordClick);*/
		    
		    if(!LionStudiosManager.instance) return;
		    LionStudiosManager.Magnet(GetSpecialLevelNumber().ToString(),_magnetCount);
		    _magnetCount++;
	    }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator CoinsReduceAnim(GameObject instanceObj,GameObject instancePosObj,GameObject movePosition)
    {
	    for (int i = 0; i < 3; i++)
	    {
		    GameObject obj = Instantiate(instanceObj, instancePosObj.transform.position, instancePosObj.transform.rotation);
		    //obj.transform.DOScale(Vector3.zero, 0.3f).From();
		    //moneyUI.transform.parent = moneyDisplayContent.transform;
		    obj.transform.SetParent(instancePos.transform.parent);
		    obj.GetComponent<RectTransform>().anchoredPosition = instancePosObj.GetComponent<RectTransform>().anchoredPosition;
		    yield return new WaitForSeconds(2f/20f);
		    obj.transform.DOScale(obj.transform.localScale * 0.15f, 3f);
         
		    obj.GetComponent<RectTransform>().DOAnchorPos(movePosition.GetComponent<RectTransform>().anchoredPosition, 0.5f).SetEase(Ease.Linear).OnComplete(
			    () =>
			    {
				    obj.gameObject.SetActive(false);
				    //Destroy(obj);
			    });
	    }
    }
    public void OnHintButtonClick()
	{
		
		if (CoinManager.instance.GetCoinsCount() >= 50)
		{
			StartCoroutine(CoinsReduceAnim(instanceImageRef, instancePos, hintPosDeduct));
			Hint_CallBack();
			CoinManager.instance.HintReduce(50);
			
		}
		else
		{
			if (!GameEssentials.IsRvAvailable()) return;
			GameEssentials.RvType = RewardType.Hint;
			GameEssentials.ShowRewardedAds("Hint");
			if(LionStudiosManager.instance)
				LionStudiosManager.AdsEvents(true, AdsEventState.Start,GetSpecialLevelNumber(),"Applovin","Hint",CoinManager.instance.GetCoinsCount());

		}
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}

	// ReSharper disable Unity.PerformanceAnalysis
	public void Hint_CallBack()
	{
		HintButtonDeActiveFun();
        DOVirtual.DelayedCall(0.1f, () =>
        {
            HintButtonActiveFun();
        },false);
		GameManager.Instance.ShowTheText();
		
		if (!SoundHapticManager.Instance) return;
		SoundHapticManager.Instance.Vibrate(30);
		SoundHapticManager.Instance.Play("ButtonClickMG");
		
		if(!LionStudiosManager.instance) return;
		LionStudiosManager.Hint(GetSpecialLevelNumber().ToString(),_hintCount.ToString());
		_hintCount++;
	}

	private bool _hintActive;
	private bool _autoButtonActive;
	// ReSharper disable Unity.PerformanceAnalysis
	public void HintButtonActiveFun()
	{
		DOVirtual.DelayedCall(0.2f, () =>
		{
			if (GameManager.Instance.hintCubesHolder.Count == 0) return;

			if (GameManager.Instance.hintCubesHolder[0].activeInHierarchy)
			{
				if (!GameManager.Instance.cameraMoving && !GameManager.Instance.levelCompleted &&
				    !GameManager.Instance.wordTouch)
				{
					if (!hintButton.interactable)
					{
						hintButton.interactable = true;
						_hintActive = true;
						//var hintsTxt = hintButton.image.rectTransform.GetChild(0).gameObject;
						// coinsTxt = hintButton.image.rectTransform.GetChild(1).gameObject;
						var rvIcon = hintButton.image.rectTransform.GetChild(2).gameObject;
						var loadingIcon = hintButton.image.rectTransform.GetChild(3).gameObject;

						//hintsTxt.SetActive(CoinManager.instance.GetCoinsCount() >= 50);
						//coinsTxt.SetActive(CoinManager.instance.GetCoinsCount() >= 50);

						rvIcon.SetActive(CoinManager.instance.GetCoinsCount() < 50 && GameEssentials.IsRvAvailable());
						loadingIcon.SetActive(CoinManager.instance.GetCoinsCount() < 50 &&
						                      !GameEssentials.IsRvAvailable());
					}

				}
			}
		},false);

	}
	public void HintButtonDeActiveFun()
	{
		if (hintButton.interactable && !GameManager.Instance.hintSpawnObject)
		{
			_hintActive = false;
			hintButton.interactable = !hintButton.interactable;
			//var hintsTxt = hintButton.image.rectTransform.GetChild(0).gameObject;
			//var coinsTxt = hintButton.image.rectTransform.GetChild(1).gameObject;
			var rvIcon = hintButton.image.rectTransform.GetChild(2).gameObject;
			var loadingIcon = hintButton.image.rectTransform.GetChild(3).gameObject;
			
			//hintsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			//coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			
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
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
		if (CoinManager.instance.GetCoinsCount() >= 50) Emoji_CallBack();
		else
		{
			GameEssentials.RvType = RewardType.ImageReveal;
			GameEssentials.ShowRewardedAds("ImageReveal");
			if(LionStudiosManager.instance)
				LionStudiosManager.AdsEvents(true, AdsEventState.Start,GetSpecialLevelNumber(),"Applovin","ImageReveal",CoinManager.instance.GetCoinsCount());
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
				if (GameManager.Instance.stickingCubes[i].GetComponent<StickingAreaCheckingScript>().emojiRevel)
				{
					GameManager.Instance.stickingCubes[i].emojiRevel.SetActive(true);
                    GameManager.Instance.stickingCubes[i].GetComponent<StickingAreaCheckingScript>().emojiRevel = null;
					EmojiRevelButtonDeActivate();
					break;
				}
			}
		}
		
		if(!LionStudiosManager.instance) return;
		LionStudiosManager.ImageReveal(GetSpecialLevelNumber().ToString(),_imageRevealCount);
		_imageRevealCount++;
	}
	// ReSharper disable Unity.PerformanceAnalysis
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
			//var coinsTxt = emojiRevealButton.image.rectTransform.GetChild(1).gameObject;
			var rvIcon = emojiRevealButton.image.rectTransform.GetChild(2).gameObject;
			var loadingIcon = emojiRevealButton.image.rectTransform.GetChild(3).gameObject;
			
			emojiText.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			//coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			
			rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && GameEssentials.IsRvAvailable());
			loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && !GameEssentials.IsRvAvailable());
		}
	}

	public void EmojiRevelButtonDeActivate()
	{
		emojiRevealButton.interactable = true;
		var emojiText = emojiRevealButton.image.rectTransform.GetChild(0).gameObject;
		//var coinsTxt = emojiRevealButton.image.rectTransform.GetChild(1).gameObject;
		var rvIcon = emojiRevealButton.image.rectTransform.GetChild(2).gameObject;
		var loadingIcon = emojiRevealButton.image.rectTransform.GetChild(3).gameObject;
			
		emojiText.SetActive(CoinManager.instance.GetCoinsCount()>=50);
		//coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=50);
			
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
        //NextMoveFun();
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
        //if(GAScript.instance) GAScript.instance.LevelCompleted(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);

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
        if(GAScript.instance) GAScript.instance.LevelFail(GetSpecialLevelNumber().ToString(),levelAttempts);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }

    public void GetMoreMovesFun()
    {
	   if(!GameEssentials.instance) return;

	   GameEssentials.RvType = RewardType.NoMoreMoves;
	   GameEssentials.ShowRewardedAds("NoMoreMoves");
	   if(LionStudiosManager.instance)
		   LionStudiosManager.AdsEvents(true, AdsEventState.Start,GetSpecialLevelNumber(),"Applovin","NoMoreMoves",CoinManager.instance.GetCoinsCount());
	   if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
	   if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
    }

    public void GetMoreMoves_CallBack()
    {
	    failPanel.SetActive(false);
	    GameManager.Instance.movesCount = 5;
	    movesText.text = "Moves: " + GameManager.Instance.movesCount;
	    if(GameManager.Instance.scriptOff)
		    GameManager.Instance.scriptOff = false;
	    if(GameManager.Instance.levelFail)
		    GameManager.Instance.levelFail = false;
	    hintButton.interactable = true;
	    autoWordButton.interactable = true;
	    if (!LionStudiosManager.instance) return;
	    LionStudiosManager.NoMoreMoves(GetSpecialLevelNumber().ToString(),"Coins","100",_noMoreMovesCount.ToString());
	    _noMoreMovesCount++;
    }

    public void GiftOpenFun(int num)
    {
	    DOVirtual.DelayedCall(1f, () =>
	    {
		    // if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("GiftOpen");
		    giftPanel.SetActive(true);
		    giftPanel.transform.GetChild(0).DOLocalRotate(new Vector3(0, 0, 360f), 1f).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear);
		    var countList = new List<int>() {20,30,40,50};
		    countList.Sort((a, b) => 1 - 2 * Random.Range(0, countList.Count));
		    var coinCount = countList[0];
		    switch (num)
		    {
			    case 0:
				    GiftPopFun(coinsObj, giftHintObj,giftJumpPLace,giftJumpPLace2,coinCount, 50,1);
				    _magnetOrHint = "Hint";
				    if (LionStudiosManager.instance)
				    {
					    LionStudiosManager.LevelCompleteReward(GetSpecialLevelNumber().ToString(),"Coins","50",_levelCompleteRewardCount.ToString());
					    _levelCompleteRewardCount++;
				    }
				    //_revelItem = "Hint";
				    break;
			    case 1:
				    GiftPopFun(coinsObj, giftMagnetObj,giftJumpPLace,giftJumpPLace2, coinCount,100,1);
				    _magnetOrHint = "Magnet";
				    if (LionStudiosManager.instance)
				    {
					    LionStudiosManager.LevelCompleteReward(GetSpecialLevelNumber().ToString(),"Coins","100",_levelCompleteRewardCount.ToString());
					    _levelCompleteRewardCount++;
				    }
				    //_revelItem = "Magnet";
				    break;
			    case 2:
				    GiftPopFun(coinsObj, giftHintObj,giftJumpPLace,giftJumpPLace2,200, 50,1);
				    _magnetOrHint = "Hint";
				    break;
			    case 3:
				    GiftPopFun(coinsObj, giftHintObj,giftJumpPLace,giftJumpPLace2, 100,100,2);
				    _magnetOrHint = "Hint";
				    break;
			    case 4:
				    GiftPopFun(coinsObj, giftMagnetObj,giftJumpPLace,giftJumpPLace2, 100,100,1);
				    _magnetOrHint = "Magnet";
				    break;
			    case 5:
				    GiftPopFun(coinsObj, giftMagnetObj,giftJumpPLace,giftJumpPLace2, 100,100,1);
				    _magnetOrHint = "Magnet";
				    break;
			    case 6:
				    GiftPopFun(coinsObj, giftHintObj,giftJumpPLace,giftJumpPLace2, 100,100,2);
				    _magnetOrHint = "Hint";
				    break;
			    default:
				    break;
		    }
		    
	    },false);
    }
    
    public void GiftPopFun(GameObject popObj,GameObject popObj2,GameObject popPosition,GameObject popPosition2,int randomCoinCount,int magnetOrHintCoins,int magnetOrHintCount)
    {
	    _coinIncreaseNUm = randomCoinCount + magnetOrHintCoins;
	    //print(_coinIncreaseNUm+"       "+ randomCoinCount +"          "+coinsIncreaseNum);
	    DOVirtual.DelayedCall(1.3f, () =>
	    {
		    // if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("RewardOpen");
		    popObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + randomCoinCount;
		    popObj2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + magnetOrHintCount;
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
	    },false);
	    DOVirtual.DelayedCall(1.9f, () =>
	    {
		    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("RewardOpen");
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
	    },false);
    }

    private string _magnetOrHint;
    private int _coinIncreaseNUm;
    //private string _revelItem;
    public void GiftClaimFun()
    {
	    claimButton.interactable = false;
	   
	    DOVirtual.DelayedCall(0.25f, () =>
	    {
		    
		    StartCoroutine(PlayCoinCollectionFx(giftCoinMovePOs,giftDiamondFxParent,giftCenterParticleRect,giftDiamondParticlesRect));
	    },false);
	    DOVirtual.DelayedCall(1.7f, () =>
	    {
		    var instanceObj = MonitizationScript.instance.instanceImageRef;
		    switch (_magnetOrHint)
		    {
			    case "Magnet":
				    instanceObj.GetComponent<Image>().sprite = MonitizationScript.instance.magnetImage;
				    var magnetNum = CoinManager.instance.GetCoinsCount() / 100;
				    MagnetSpawn(instanceObj, giftJumpPLace2, giftMagnetMovePosition, "Magnet",magnetNum);
				    break;
			    case "Hint":
				    instanceObj.GetComponent<Image>().sprite = MonitizationScript.instance.hintImage;
				    var hintNum = CoinManager.instance.GetCoinsCount() / 50;
				    MagnetSpawn(instanceObj, giftJumpPLace2, giftHintMovePosition, "Hint",hintNum);
				    break;
			    default:
				    break;
		    }
	    },false);
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
    public void MagnetSpawn(GameObject instanceObj,GameObject instancePos,GameObject movePosition,string spawnObj,int countNum)
    {
	    GameObject obj=Instantiate(instanceObj, instancePos.transform.position, instanceObj.transform.rotation);
	    //magnet.transform.DOScale(Vector3.zero, 0.15f).From();
	    obj.transform.SetParent(instancePos.transform.parent);
	    obj.GetComponent<RectTransform>().anchoredPosition = instancePos.GetComponent<RectTransform>().anchoredPosition;
	    obj.GetComponent<RectTransform>().DOScale(obj.transform.localScale * 2.5f, 0.65f).OnComplete(() =>
	    {
		    obj.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.75f).SetEase(Ease.Linear);
	    });
	    obj.GetComponent<RectTransform>()
		    .DOJumpAnchorPos(movePosition.GetComponent<RectTransform>().anchoredPosition, 300f, 1, 1f)
		    .SetEase(Ease.Linear).OnComplete(() =>
		    {
			    //magnet.GetComponent<RectTransform>().DOPunchAnchorPos(Vector2.one * 2.5f, 0.15f, 2);
			    if (spawnObj == "Magnet")  giftMagnetCountTemp.text = (countNum + 1).ToString();
			    else if(spawnObj == "Hint") giftHintCountTemp.text = (countNum + 1).ToString();
			    DOVirtual.DelayedCall(0.2f, () =>
			    {
				    obj.gameObject.SetActive(false);
				    Destroy(obj);
			    },false);
		    });
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
		    if (PlayerPrefs.GetInt("Level") > (SceneManager.sceneCountInBuildSettings) - 34)
		    {
			    var i = Random.Range(109, SceneManager.sceneCountInBuildSettings - 34);
			    PlayerPrefs.SetInt("ThisLevel", i);
			    SceneManager.LoadScene(i);
		    }
		    else
		    {
			    SceneManager.LoadScene(PlayerPrefs.GetInt("Level", 1));
		    }
		    //CoinManager.instance.SetLoaderPercentage(CoinManager.instance.GetLoaderPercent() + ((1f / 9f)));
	    }
	    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
	    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
    }
    public void MapLevelCall()
    {
	    if ((SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 33 &&
	         SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 2))
	    {
		    DailyChallengesHandler.SaveDailyChallengeAtLc();
		    SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
	    }
	    else
	    {
		    
		    if (EmojiManager.Instance)
		    {
			    EmojiManager.Instance.SetListNumber(EmojiManager.Instance.GetListNumbers() + 1);
			    EmojiManager.Instance.SetPanelsDone(EmojiManager.Instance.GetPanelsDone() + 1);
		    }
		    //if(SceneManager.GetActiveScene().buildIndex)
		    var s = GetSpecialLevelNumber().ToString()[^1];
		    if (s != '0' && SceneManager.GetActiveScene().buildIndex <= SceneManager.sceneCountInBuildSettings - 33)
		    {
			    PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
		    }
		    if(GAScript.instance) GAScript.instance.LevelCompleted(GetSpecialLevelNumber().ToString(),levelAttempts);
		    CoinManager.instance.SetLoaderPercentage(CoinManager.instance.GetLoaderPercent() + ((1f / 9f)));
		    
		    SetSpecialLevelNumber(GetSpecialLevelNumber() + 1);
		    SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
	    }
	    ///if(GAScript.instance) GAScript.instance.LevelStart(GetSpecialLevelNumber().ToString(),levelAttempts);
    }
	public int GetSpecialLevelNumber() => PlayerPrefs.GetInt("SpecialLevelNumber", 1);
	public void SetSpecialLevelNumber(int levelNum) => PlayerPrefs.SetInt("SpecialLevelNumber", levelNum);
	

	public int GetLevelNumberDetails() => PlayerPrefs.GetInt("LevelNumberDetails", 1);
	public void SetLevelNumberDetails(int levelNum) => PlayerPrefs.SetInt("LevelNumberDetails", levelNum);

	public string GetCalenderUnlockCheck() => PlayerPrefs.GetString("CalenderUnlockCheck", "Lock");
	public void SetCalenderUnlockCheck(string lockCheck) => PlayerPrefs.SetString("CalenderUnlockCheck", lockCheck);
	public void SkipLevelFun()
	{
		WinPanelActive();
	}
	public void CalenderBack()
	{
		calendarPanel.gameObject.SetActive(false);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}

	
	public void CalenderUnLockButton()
	{
		GameEssentials.RvType = RewardType.CalenderStart;
		GameEssentials.ShowRewardedAds("CalenderStart");
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
		//CalenderUnlock_CallBack();
	}
	// ReSharper disable Unity.PerformanceAnalysis
	public void CalenderUnlock_CallBack()
	{
		SetCalenderUnlockCheck("Unlock");
		if (calenderPanelPop.transform.localScale.x != 0)
		{
			calenderPanelPop.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutBounce);
			calendarPanel.gameObject.SetActive(true);
		}
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}

	public GameObject calenderPanelPop;
	public void CalendarButtonPress()
	{
		if (GetCalenderUnlockCheck() == "Unlock")
		{
			if (!calendarPanel.gameObject.activeInHierarchy)
			{
				calendarPanel.gameObject.SetActive(true);
			}
			if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
			if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		}
		else
		{
			if (GetSpecialLevelNumber() < 30)
			{
				var calVal = calenderPanelPop.transform;
				var calRv=calenderPanelPop.transform.GetChild(0).GetChild(0).GetChild(0).transform;
				var calLoad=calenderPanelPop.transform.GetChild(0).GetChild(0).GetChild(1).transform;
				if (calVal.localScale.x == 0)
				{
					calVal.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
					DOVirtual.DelayedCall(10f, () => {calVal.DOScale(Vector3.zero, 0.15f).SetEase(Ease.Linear);},
						false);
					if (GameEssentials.IsRvAvailable())
					{
						calRv.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
						DOVirtual.DelayedCall(10f, () => { calRv.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutBounce); },
							false);
					}
					else
					{
						calLoad.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
						DOVirtual.DelayedCall(10f, () => { calLoad.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutBounce); },
							false);
					}
					
				}
				if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
				if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
			}
		}
		/*if (GetSpecialLevelNumber() < 30)
		{
			var calVal = calenderButton.transform.GetChild(1).transform;
			if (calVal.localScale.x == 0)
			{
				calVal.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
				DOVirtual.DelayedCall(2f, () =>
				{
					calVal.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutBounce);
				},false);
			}
			return;
		}
		calendarPanel.gameObject.SetActive(!calendarPanel.gameObject.activeInHierarchy);*/

	}

	public void CalenderClose()
	{
		var calVal = calenderPanelPop.transform;
		if (calVal.localScale.x != 0)
		{
			calVal.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutBounce);
		}
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
		
	}
}