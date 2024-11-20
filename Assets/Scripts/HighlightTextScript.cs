using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighlightTextScript : MonoBehaviour
{
	[HideInInspector]
	public bool isVisible = false;
	public bool done;

	private void Start()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, -0.4f);
	}

	private void OnTriggerEnter(Collider other)
	{
		//if (!other.gameObject.CompareTag("Player_Cube")) return;
//		Debug.Log("SAME");
		if (other.gameObject.CompareTag("Player_Cube") && !transform.parent.GetComponent<HintParent>().doneObject)
		{
			if (other.gameObject.GetComponentInChildren<TextMeshPro>().text == gameObject.GetComponentInChildren<TextMeshPro>().text)
			{
				//gameObject.SetActive(false);
				transform.parent.GetComponent<HintParent>().takenString.Add(other.gameObject.GetComponentInChildren<TextMeshPro>().text);
				//Destroy(gameObject);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player_Cube") && !transform.parent.GetComponent<HintParent>().doneObject)
		{
			if (other.gameObject.GetComponentInChildren<TextMeshPro>().text == gameObject.GetComponentInChildren<TextMeshPro>().text)
			{
				if (transform.parent.GetComponent<HintParent>().takenString.Contains(other.gameObject.GetComponentInChildren<TextMeshPro>().text))
				{
					transform.parent.GetComponent<HintParent>().takenString.Remove(other.gameObject.GetComponentInChildren<TextMeshPro>().text);
				}
			}
		}
	}
}
