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

    [Header("Gift Details")] 
    public GameObject giftPanel; 
    public Button claimButton;
    public GameObject coinsText;
    //public Button looseButton;
    public GameObject gifAnimationObj;
    public bool giftLevel;

    private void Awake()
	{
		if (!Instance) Instance = this;
	}

	private void Start()
	{
		
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
				
				if (CoinManager.instance.GetCoinsCount() >= 100)
				{
					autoWordButton.interactable = true;
				}
				else
				{
					autoWordButton.interactable = false;
				}
			}

			if (s == '5')
			{
				giftLevel = true;
			}
			
			//StartCoroutine(PlayCoinCollectionFx());

			if ((PlayerPrefs.GetInt("Level", 1) == 1))
			{
				if(GameManager.Instance)
					GameManager.Instance.ShowTheText();
				/*hintButton.GetComponent<Image>().enabled = false;
				hintButton.interactable = false;*/
				hintButton.gameObject.SetActive(false);
				autoWordButton.gameObject.SetActive(false);
				MonitizationScript.instance.giftObject.SetActive(false);
				if(tutorialtext)
					tutorialtext.SetActive(true);
				//HelpHand();
				foreach (var t in gameobject2)
				{
					t.enabled = false;
				}
			}
			
			if(GAScript.instance) GAScript.instance.LevelStart(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);
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
	
	[Header("Collection Fx")]
	public RectTransform diamondIcon;
	public GameObject diamondFxParent;
	public RectTransform centerParticleRect;
	public List<RectTransform> diamondParticlesRect;
	public IEnumerator PlayCoinCollectionFx()
	{
		//yield return new WaitForSeconds(1.5f);
		diamondFxParent.SetActive(true);
		for (int i = 0; i < diamondParticlesRect.Count; i++)
		{
			diamondParticlesRect[i].gameObject.SetActive(true);
			diamondParticlesRect[i].DOLocalMove(centerParticleRect.localPosition, 0.25f).From();
		}
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
		//SoundsController.instance.PlaySound(SoundsController.instance.moneyGot);
		yield return new WaitForSeconds(0.5f);
		//SoundsController.instance.PlaySound(SoundsController.instance.coinCollection);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("FinalCoins");
		for (int i = 0; i < diamondParticlesRect.Count; i++)
		{
			var i1 = i;
			var dofundup = diamondParticlesRect[i].DOMove(diamondIcon.position, 0.8f);
			dofundup.OnComplete(() =>
			{
				diamondParticlesRect[i1].gameObject.SetActive(false);
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
                   //6 print((1f / 9f));
                    var num = CoinManager.instance.GetLoaderPercent() + ((1f / 9f));
                    //CoinManager.instance.SetLoaderPercentage(CoinManager.instance.GetLoaderPercent() + ((1f / 9f)));
                    CoinManager.instance.progressionBarImage.DOFillAmount(num, 1f);/*
                        .OnComplete(
                            () =>
                            {
                                nextButton.interactable = true;
                            });*/
                    CoinManager.instance.progressionBarText.text = ((int)(num * 100) + "%");
                });
                endScreen.SetActive(true);
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    StartCoroutine(PlayCoinCollectionFx());
                });
                DOVirtual.DelayedCall(2f, () =>
                {
                    CoinManager.instance.CoinsIncrease(25);
                    //nextButton.interactable = true;
                    if (giftLevel)
                    {
	                    GiftOpenFun();
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
                    StartCoroutine(PlayCoinCollectionFx());
                });
                DOVirtual.DelayedCall(2f, () =>
                {
                    CoinManager.instance.CoinsIncrease(25);
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

    public void AutoButtonActive()
    {
        autoWordDisableWordBool = false;
        if (CoinManager.instance.GetCoinsCount() >= 100 && !GameManager.Instance.levelCompleted)
        {
            autoWordButton.interactable = true;
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
            AutoButtonDisActive();
            GameManager.Instance.autoWordClick = true;
            if(GameManager.Instance)
				GameManager.Instance.AutoCompleteFunc();
            
            CoinManager.instance.AutoWordReduce();
            Debug.Log("AutoComplete");
        }
        
    }
    
	private void Update()
	{
		var s = GetSpecialLevelNumber().ToString()[^1];
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
			/*if (Input.GetMouseButtonUp(0))
			{
				//AutoButtonDisActive();
				AutoButtonActive();
			}

			if (Input.GetMouseButtonDown(0))
			{
				// AutoButtonActive();
				AutoButtonDisActive();
			}*/
			if (!GameManager.Instance.autoWordClick)
			{
				if (GameManager.Instance.wordTouch && autoWordButton.interactable)
					autoWordButton.interactable = false;
				else if(!GameManager.Instance.wordTouch && !autoWordButton.interactable)
					AutoButtonActive();
			}
		}

		
	}

	public void OnHintButtonClick()
	{
		//Debug.Log("Hint Button");
		if (GameManager.Instance)
		{
			GameManager.Instance.ShowTheText();
			if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
			if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
		}
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
    
    public void GiftOpenFun()
    {
	    DOVirtual.DelayedCall(1f, () =>
	    {
		    giftPanel.SetActive(true);
		    giftPanel.transform.GetChild(0).DOLocalRotate(new Vector3(0, 0, 360f), 1f).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear);
		    DOVirtual.DelayedCall(1.5f, () =>
		    {
			    coinsText.GetComponent<RectTransform>().DOJumpAnchorPos(new Vector2(0, -87f), 400f, 1, 1f)
				    .SetEase(Ease.Linear);
			    coinsText.GetComponent<RectTransform>().DOScale(Vector3.one, 0.05f);
			    DOVirtual.DelayedCall(1.15f, () =>
			    {
				    claimButton.GetComponent<RectTransform>().DOScale(Vector3.one, 0.05f).OnComplete(() =>
				    {
					    claimButton.interactable = true;
				    });
				    //StartCoroutine(PlayCoinCollectionFx());
			    });
		    });
	    });
    }

    public void GiftClaimFun()
    {
	    claimButton.interactable = false;
	    DOVirtual.DelayedCall(0.5f, () =>
	    {
		    StartCoroutine(PlayCoinCollectionFx());
	    });
	    DOVirtual.DelayedCall(2f, () =>
	    {
		    CoinManager.instance.CoinsIncrease(50);
		    DOVirtual.DelayedCall(1f, () =>
		    {
			    MapLevelCall();
		    });
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
}

