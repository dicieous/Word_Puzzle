using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



// [System.Serializable]
// public class HintWords { public List<int> wordNoToShow; }


public class UIManagerScript : MonoBehaviour
{
	public static UIManagerScript Instance;
	public CoinManager cm;
	
	public GameObject endScreen,prefab;
	public TextMeshProUGUI levelNo;
	public Material originalColor;
	public RectTransform coinEndReference;
	public RectTransform coinStartReference;
	public Button nextButton;
	public Button hintButton;
	public Button restartButton;
	
	[Header("Levels changing")]
	public GameObject starparticleEffect;

	public Image tutorialHand, tutorialHand2, tutorialHand3;
	public List<Collider> gameobject1, gameobject2, gameobject3;
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

	private void Awake()
	{
		if (!Instance) Instance = this;
	}

	private void Start()
	{
		
		var s = GetSpecialLevelNumber().ToString()[^1];
		if (s == '0')
		{
			levelNo.text = "LEVEL " + GetSpecialLevelNumber() +"\n(Boss Level)";
		}
		else
		{
			levelNo.text = "LEVEL " + GetSpecialLevelNumber();
		}
        if(GAScript.instance) GAScript.instance.LevelStart(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);
		
		cm = CoinManager.instance;
		if (endScreen)
		{
			endScreen.SetActive(false);
		}

		//StartCoroutine(PlayCoinCollectionFx());

		if ((PlayerPrefs.GetInt("Level", 1) == 1))
		{
			GameManager.Instance.ShowTheText();
            if(tutorialtext)
                tutorialtext.SetActive(true);
			//HelpHand();
			foreach (var t in gameobject2)
            {
                t.enabled = false;
            }
			foreach (var t in gameobject3)
            {
                t.enabled = false;
            }
		}
		
	}

	public void HelpHand()
	{
		if (tutorialHand || tutorialHand2 || tutorialHand3)
		{
			if (countnumhelp == 0)
			{
				tutorialHand.gameObject.SetActive(true);
                //tutorialHand.enabled = true; 
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
				for (int i = 0; i < gameobject2.Count; i++)
				{
					gameobject2[i].enabled = true;
				}
				for (int i = 0; i < gameobject1.Count; i++)
				{
					gameobject1[i].enabled = false;
				}
//				print(countnumhelp);
			
			}
			else if(countnumhelp ==2)
			{
				tutorialHand3.gameObject.SetActive(true);
                tutorialHand2.enabled = false;
				tutorialHand3.rectTransform.DOAnchorPos(targetPos[2], 2f).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Restart);
				for (int i = 0; i < gameobject3.Count; i++)
				{
					gameobject3[i].enabled = true;
				}
				for (int i = 0; i < gameobject2.Count; i++)
				{
					gameobject2[i].enabled = false;
				}
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
		yield return new WaitForSeconds(1.5f);
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
			diamondParticlesRect[i].DOMove(diamondIcon.position, 0.8f).OnComplete(() =>
			{
				diamondParticlesRect[i1].gameObject.SetActive(false);
			});
			yield return new WaitForSeconds(0.04f);
		}

		//diamondNumOnWinPanel.text = GameController.instance.GetTotalCoin().ToString();
	}
	public void WinPanelActive()
	{
		if (tutorialHand3)
        {
            tutorialHand3.enabled = false;
            tutorialtext.GetComponent<TextMeshProUGUI>().enabled = false;
        }
		StartCoroutine(PlayCoinCollectionFx());
		//if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
		DOVirtual.DelayedCall(2f, () =>
		{
			CoinManager.instance.CoinsIncrease(30);
		});
		
		DOVirtual.DelayedCall(3.25f,()=>
		{
			targetCongratulationImage.GetComponent<Image>().sprite =
				congratulationsImages[Random.Range(0, congratulationsImages.Count)];
			endScreen.SetActive(true);
		});
	}
	
	private void Update()
	{
		var s = GetSpecialLevelNumber().ToString()[^1];
		if (s != '0')
		{
			if (cm.GetHintCount() == 0 && hintButton.interactable)
			{
				hintButton.interactable = false;
			}
			else if (cm.GetHintCount() != 0 && !hintButton.interactable)
			{
				hintButton.interactable = true;
			}
		}
	}

	public void OnHintButtonClick()
	{
		
		//Debug.Log("Hint Button");
		GameManager.Instance.ShowTheText();
		
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}

	public void NextSceneLoader()
	{
        if (!GameManager.Instance.levelTypeChanged)
        {
            GameManager.Instance.DestroyBlocks();
        }
        else
        {
            NextMoveFun();
        }
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
		}
        //if(GAScript.instance) GAScript.instance.LevelCompleted(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);

	}
	public void ResetScreenOnClick()
	{
		//GameManager.Instance.ResetScreen();
        if(GAScript.instance) GAScript.instance.LevelRestart(PlayerPrefs.GetInt("Level", 1).ToString(),levelAttempts);
        levelAttempts++;
		DOTween.KillAll();
		var loadedScene = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(loadedScene);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}
	
	public int GetSpecialLevelNumber() => PlayerPrefs.GetInt("SpecialLevelNumber", 1);
	public void SetSpecialLevelNumber(int levelNum) => PlayerPrefs.SetInt("SpecialLevelNumber", levelNum);
}

