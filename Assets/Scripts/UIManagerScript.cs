using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerScript : MonoBehaviour
{
	public static UIManagerScript Instance;

	public GameObject endScreen,prefab;
	public TextMeshProUGUI levelNo;
	public Material newMat;

	private void Awake()
	{
		if (!Instance) Instance = this;
	}

	private void Start()
	{
		levelNo.text = "LEVEL " + PlayerPrefs.GetInt("Level", 1);
		// GameObject plane = Instantiate(prefab);
		// plane.GetComponent<MeshRenderer>().material = newMat;
	}


	public void OnHintButtonClick()
	{
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");

		Debug.Log("Hint Button");
		GameManager.Instance.ShowTheText();
	}

	public void NextSceneLoader()
	{
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");

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
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
		GameManager.Instance.ResetScreen();
	}
}