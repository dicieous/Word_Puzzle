using TMPro;
using UnityEngine;


public class HolderCubeScript : MonoBehaviour
{
    public int rowNo;
    public int colNo;

    public int checkNumberRef;

    [HideInInspector] public string inputext = "*";

    [Space(10)] public bool isFilled;

    public GameObject objRef;

    [SerializeField] private LayerMask cubeLayerMask;

    private void Start()
    {
        checkNumberRef = int.Parse(transform.GetChild(1).GetComponent<TextMeshPro>().text);
        var mask = 1 << LayerMask.NameToLayer("StickingCube");
        cubeLayerMask = mask;
    }

    private CubesGroupScript lastCubeGroup;

    private void Update()
    {
        // if (isFilled) return;
        var rayOrigin = transform.position;
        var rayDirection = transform.TransformDirection(Vector3.back);
        RaycastHit hit;
        Debug.DrawRay(rayOrigin, rayDirection * 20, Color.yellow);
        HighlighterShort(rayOrigin, rayDirection);

        /*if (Physics.Raycast(rayOrigin, rayDirection, out hit, 20, cubeLayerMask))
        {
            // Get the CubeGroup from the hit object
            var cubeGroup = hit.collider.GetComponentInParent<CubesGroupScript>();

            // Track the lastCubeGroup to handle removal later
            lastCubeGroup = cubeGroup;

            // Add the hit object to tempList if it's not already there
            if (!cubeGroup.tempList.Contains(hit.collider.gameObject))
                cubeGroup.tempList.Add(hit.collider.gameObject);

            // Change the color based on tempList count
            GetComponentInChildren<SpriteRenderer>().color = cubeGroup.letterCount == cubeGroup.tempList.Count ? Color.green : Color.red;
        }
        else
        {
            // If raycast misses, handle the previously tracked cubeGroup
            if (lastCubeGroup != null)
            {
                // Remove the previously hit object(s) from tempList
                for (int i = lastCubeGroup.tempList.Count - 1; i >= 0; i--)
                {
                    var obj = lastCubeGroup.tempList[i];

                    // Check if the object is no longer hit
                    if (!Physics.Raycast(rayOrigin, rayDirection, out hit, 20, cubeLayerMask) || hit.collider.gameObject != obj)
                    {
                        lastCubeGroup.tempList.RemoveAt(i);
                    }
                }

                // Reset color to white if tempList is empty
                if (lastCubeGroup.tempList.Count == 0)
                    GetComponentInChildren<SpriteRenderer>().color = Color.white;

                // Clear the lastCubeGroup reference if no objects are left
                if (lastCubeGroup.tempList.Count == 0)
                    lastCubeGroup = null;
            }
            else
            {
                // Ensure the color is white when no raycast and no cubeGroup
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }*/
    }

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

    private void HighlighterShort(Vector3 origin, Vector3 direction)
    {
        GetComponentInChildren<SpriteRenderer>().color = Physics.Raycast(origin, direction, 20, cubeLayerMask)
            ? Color.yellow
            : new Color(0.65f, 0.69f, 0.78f);
    }

    private void Highligher(Vector3 origin, Vector3 direction, RaycastHit hit)
    {
        /*var rayOrigin = transform.position;
        var rayDirection = transform.TransformDirection(Vector3.back);
        RaycastHit hit;*/
        if (Physics.Raycast(origin, direction, out hit, 20, cubeLayerMask))
        {
            // Get the CubeGroup from the hit object
            var cubeGroup = hit.collider.GetComponentInParent<CubesGroupScript>();

            // Track the lastCubeGroup to handle removal later
            lastCubeGroup = cubeGroup;

            // Add the hit object to tempList if it's not already there
            if (!cubeGroup.tempList.Contains(hit.collider.gameObject))
                cubeGroup.tempList.Add(hit.collider.gameObject);

            // Change the color based on tempList count
            GetComponentInChildren<SpriteRenderer>().color = cubeGroup.letterCount == cubeGroup.tempList.Count ? Color.green : Color.red;
        }
        else
        {
            // If raycast misses, handle the previously tracked cubeGroup
            if (lastCubeGroup != null)
            {
                // Remove the previously hit object(s) from tempList
                for (int i = lastCubeGroup.tempList.Count - 1; i >= 0; i--)
                {
                    var obj = lastCubeGroup.tempList[i];

                    // Check if the object is no longer hit
                    if (!Physics.Raycast(origin, direction, out hit, 20, cubeLayerMask) || hit.collider.gameObject != obj)
                    {
                        lastCubeGroup.tempList.RemoveAt(i);
                    }
                }

                // Reset color to white if tempList is empty
                if (lastCubeGroup.tempList.Count == 0)
                    GetComponentInChildren<SpriteRenderer>().color = Color.white;

                // Clear the lastCubeGroup reference if no objects are left
                if (lastCubeGroup.tempList.Count == 0)
                    lastCubeGroup = null;
            }
            else
            {
                // Ensure the color is white when no raycast and no cubeGroup
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }
    }

    [HideInInspector] public bool once;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Cube") && !once)
        {
            staying = true;
            //print("One DomeBAhv sffsdf");
            if (other.gameObject.CompareTag("Player_Cube") && staying)
            {
                objRef = other.gameObject;
                other.gameObject.GetComponent<PlayerCubeScript>().stickingCubeObjRef = this.gameObject;
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
                        //print("One");
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
                    var playerScript = other.gameObject.GetComponent<PlayerCubeScript>();
                    if (playerScript.anim && playerScript.transform.parent.GetComponent<CubesGroupScript>())
                    {
                        if (playerScript.checknumber == checkNumberRef)
                        {
                            playerScript.transform.parent.GetComponent<CubesGroupScript>().AnimSeq();
                        }
                        else
                        {
                            playerScript.transform.parent.GetComponent<CubesGroupScript>().WrongAnimSeq();
                        }
                    }

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

    public void FunCallingObj()
    {
        isFilled = false;
        inputext = "*";
        once = false;
        if (staying) staying = false;
        GameManager.Instance.RemoveWords(rowNo - 1, colNo - 1);
    }

    [HideInInspector] public bool staying;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Cube") && once)
        {
            isFilled = false;
            inputext = "*";
            once = false;
            if (staying) staying = false;
            GameManager.Instance.RemoveWords(rowNo - 1, colNo - 1);
            //Debug.Log("NotFilled");
        }
    }
}