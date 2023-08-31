using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
	public static UIManagerScript Instance;
	public CoinManager cm;
	
	public GameObject endScreen,prefab;
	public TextMeshProUGUI levelNo;
	public Material originalColor;
	public RectTransform coinEndReference;
	public RectTransform coinStartReference;

	public Button hintButton;

	[Header("Levels changing")]
	public GameObject starparticleEffect;

	public Image tutorialHand, tutorialHand2, tutorialHand3;
	public List<Vector2> targetPos;
	private int countnumhelp;
	private void Awake()
	{
		if (!Instance) Instance = this;
	}

	private void Start()
	{
		levelNo.text = "LEVEL " + PlayerPrefs.GetInt("Level", 1);
		// GameObject plane = Instantiate(prefab);
		// plane.GetComponent<MeshRenderer>().material = newMat;
		cm = CoinManager.instance;
		if (endScreen)
		{
			endScreen.SetActive(false);
		}
        print(PlayerPrefs.GetInt("Level", 1));
		
		if ((PlayerPrefs.GetInt("Level", 1) == 1))
		{
			GameManager.Instance.ShowTheText();
			HelpHand();
		}
	}

	public void HelpHand()
	{
		if (countnumhelp == 0)
		{
			tutorialHand.gameObject.SetActive(true); 
			tutorialHand.rectTransform.DOAnchorPos(targetPos[0], 2f).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Restart);
			countnumhelp = 1;
		}
		
		else if (countnumhelp == 1)
		{
			print("tutorial2");
			tutorialHand2.gameObject.SetActive(true); 
			tutorialHand.gameObject.SetActive(false);
			tutorialHand2.rectTransform.DOAnchorPos(targetPos[1], 2f).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Restart);
			countnumhelp = 2;
			print(countnumhelp);
			
		}
		else if(countnumhelp ==2)
		{
			tutorialHand3.gameObject.SetActive(true); 
			tutorialHand2.gameObject.SetActive(false);
			tutorialHand3.rectTransform.DOAnchorPos(targetPos[2], 2f).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Restart);
		}
	}
	public void WinPanelActive()
	{
		if (tutorialHand3)
		{
			tutorialHand3.gameObject.SetActive(false);
		}
		var coinscript = FindObjectOfType<ParticleControlScript>();
		coinscript.PlayControlledParticles(coinStartReference.anchoredPosition,coinEndReference);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
		DOVirtual.DelayedCall(2f, () =>
		{
			CoinManager.instance.CoinsIncrease(50);
		});
		
		DOVirtual.DelayedCall(3,()=>
		{
			endScreen.SetActive(true);
		});
	}
	
	private void Update()
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

	public void OnHintButtonClick()
	{
		
		//Debug.Log("Hint Button");
		GameManager.Instance.ShowTheText();
		
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}

	public void NextSceneLoader()
	{
		GameManager.Instance.DestroyBlocks();
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}

	public void NextMoveFun()
	{
		DOTween.KillAll();
		if (PlayerPrefs.GetInt("Level") >= (SceneManager.sceneCountInBuildSettings) - 1)
		{
			PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
			var i = Random.Range(2, SceneManager.sceneCountInBuildSettings);
			PlayerPrefs.SetInt("ThisLevel", i);
			SceneManager.LoadScene(i);
		}
		else
		{
			PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex + 1);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
		
	}
	public void ResetScreenOnClick()
	{
		//GameManager.Instance.ResetScreen();
		var loadedScene = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(loadedScene);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
	}
}