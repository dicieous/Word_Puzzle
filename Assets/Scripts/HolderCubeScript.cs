using TMPro;
using UnityEngine;


public class HolderCubeScript : MonoBehaviour
{
	
	public int rowNo;
	public int colNo;

	[Space(10)]

	public bool isFilled = false;


	private void Update()
	{
		
	}

	private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Cube"))
		{
			isFilled = true;
			//Debug.Log(rowNo + " is Row " + colNo + " is Column");
			
			var value = other.gameObject.GetComponentInChildren<TextMeshPro>().text;
			GameManager.Instance.AddWords(rowNo-1,colNo-1,value);
			
			//Debug.Log(other.gameObject.GetComponentInChildren<TextMeshPro>().text + " is Text");
			//Debug.Log("FILLED");
		}
    }

	private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Cube"))
		{
			isFilled = false;
			GameManager.Instance.RemoveWords(rowNo-1,colNo-1);
			//Debug.Log("NotFilled");
		}
            
    }
}
