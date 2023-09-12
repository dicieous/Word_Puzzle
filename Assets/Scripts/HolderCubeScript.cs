using DG.Tweening;
using TMPro;
using UnityEngine;


public class HolderCubeScript : MonoBehaviour
{
	
	public int rowNo;
	public int colNo;

    [HideInInspector] public string inputext  = "*";
 
	[Space(10)]

	public bool isFilled = false;


	/*private void Update()
	{
		Debug.Log("GrabWord value "+ GameManager.Instance.grabwords);
	}*/

	/*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Cube"))
		{
			
			var value = other.gameObject;
			GameManager.Instance.AddWords(rowNo-1,colNo-1,value);
			isFilled = true;
			//Debug.Log(rowNo + " is Row " + colNo + " is Column");
			//Debug.Log(other.gameObject.GetComponentInChildren<TextMeshPro>().text + " is Text");
			//Debug.Log("FILLED");
		}
    }*/

	private bool once;
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Player_Cube") && !once)
		{
			staying = true;
			//print("OnceCheck ::::::::::::::::");
			DOVirtual.DelayedCall(0.25f, () =>
			{
				if (other.gameObject.CompareTag("Player_Cube") && staying)
				{
					//print("One DomeBAhv");
					var value = other.gameObject;
                    inputext = other.gameObject.GetComponentInChildren<TextMeshPro>().text;
					GameManager.Instance.AddWords(rowNo - 1, colNo - 1, value);
					isFilled = true;
					staying = false;
				}
			});
			once = true;
			//print("OnceCheck");
		}
		
	}

	private bool staying;
	private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Cube") && once)
		{
			isFilled = false;
            inputext = "*";
			once = false;
			if (staying) staying = false;
			GameManager.Instance.RemoveWords(rowNo-1,colNo-1);
			//Debug.Log("NotFilled");
		}
            
    }
}
