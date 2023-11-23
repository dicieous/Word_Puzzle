using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HintParent : MonoBehaviour
{
	public List<String> crctString = new List<string>();

	public List<String> takenString;

	public bool doneObject;
	private void Start()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			crctString.Add(transform.GetChild(i).GetChild(0).GetComponent<TextMeshPro>().text);
		}
	}

	private bool once;
	private void Update()
	{
		if (crctString.Count == takenString.Count && !once)
		{
			checkfun();
			once = true;
//			print("Once");
		}

		if (count == crctString.Count && !doneObject)
		{
			for (int i = 0; i < crctString.Count; i++)
			{
				transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
			}
			
			doneObject = true;
            
			if (GameManager.Instance.hintSpawnObject == this.gameObject)
			{
				GameManager.Instance.hintSpawnObject = null;
				UIManagerScript.Instance.HintButtonActiveFun();	
			}
            
            if (GameManager.Instance.hintCubesHolder.Count > 0 && GameManager.Instance.hintCubesHolder.Contains(gameObject))
            {
                for (int i = 0; i < GameManager.Instance.hintCubesHolder.Count; i++)
                {
                    if (GameManager.Instance.hintCubesHolder[i].gameObject == gameObject)
                    {
                        GameManager.Instance.hintCubesHolder.RemoveAt(i);
                    }
                }
            }
		}
	}
	
	private int count;
	public void checkfun()
	{
		for (int i = 0; i < crctString.Count; i++)
		{
			if (crctString.Contains(takenString[i]))
			{
				count = i+1;
				//print(count);
			}
			else
			{
				once = false;
			}
		}
	}
}
