using TMPro;
using UnityEngine;


public class HolderCubeScript : MonoBehaviour
{
	
	public int rowNo;
	public int colNo;

    public int checkNumberRef;
    
    [HideInInspector] public string inputext  = "*";
 
	[Space(10)]

	public bool isFilled = false;

    private void Start()
    {
        checkNumberRef = int.Parse(transform.GetChild(1).GetComponent<TextMeshPro>().text) ;
    }

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
            if (other.gameObject.CompareTag("Player_Cube") && staying)
            {
                //print("One DomeBAhv");
                if (other.gameObject.GetComponent<PlayerCubeScript>() && checkNumberRef != 0 && GameManager.Instance.levelTypeChanged)
                {
                    var playerscript = other.gameObject.GetComponent<PlayerCubeScript>();
                    if (playerscript.checknumber == checkNumberRef)
                    {
                        if (playerscript.anim && playerscript.transform.parent.GetComponent<CubesGroupScript>())
                        {
                            playerscript.transform.parent.GetComponent<CubesGroupScript>().AnimSeq();
                        }
                        var value = other.gameObject;
                        inputext = value.GetComponentInChildren<TextMeshPro>().text;
                        //GameManager.Instance.AddWords(rowNo - 1, colNo - 1, value);
                        isFilled = true;
                        staying = false;
                    }
                    else
                    {
                        print("One");
                        if (playerscript.anim && playerscript.transform.parent.GetComponent<CubesGroupScript>())
                        {
                            playerscript.transform.parent.GetComponent<CubesGroupScript>().WrongAnimSeq();
                        }
                        isFilled = true;
                        staying = false;
                    }
                }
                else
                {
                    var value = other.gameObject;
                    inputext = value.GetComponentInChildren<TextMeshPro>().text;
                    GameManager.Instance.AddWords(rowNo - 1, colNo - 1, value);
                    isFilled = true;
                    staying = false;
                }

                //print("OnceCheck ::::::::::::::::");
                /*DOVirtual.DelayedCall(0.25f, () =>
                {

                    }*/
			}
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
