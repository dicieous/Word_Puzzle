using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighlightTextScript : MonoBehaviour
{
	[HideInInspector]
	public bool isVisible = false;
	
	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("Player_Cube")) return;
		Debug.Log("SAME");
		if (other.gameObject.GetComponentInChildren<TextMeshPro>().text == gameObject.GetComponentInChildren<TextMeshPro>().text)
		{
			//gameObject.SetActive(false);
			Destroy(gameObject);
		}
	}
}
