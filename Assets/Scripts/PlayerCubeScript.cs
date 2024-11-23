using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerCubeScript : MonoBehaviour
{
    [HideInInspector] public bool isPlaced;

    public int checknumber;
    [HideInInspector] public bool anim;

    [HideInInspector] public GameObject stickingCubeObjRef;
    [HideInInspector] public Vector3 startPos;
    
    private void Start()
    {
        var posStart = transform.localPosition;
        startPos = posStart;
        transform.GetChild(2).GetComponent<TextMeshPro>().text = transform.GetChild(0).GetComponent<TextMeshPro>().text;
        name = checknumber.ToString();
        // yield return new WaitForSeconds(0.01f);
        LetterGroupSet.instance.lettersList.Add(gameObject);
    }

    private void Update()
    {
        /*var rayOrigin = transform.position;
        var rayDirection = transform.TransformDirection(Vector3.forward);
        RaycastHit hitInfo;

        var parentObjPosition = parentTransform.position;
        var rayCheck = Physics.Raycast(rayOrigin, rayDirection, out hitInfo, Mathf.Infinity, mask);

        //Debug.DrawRay(rayOrigin, rayDirection * Mathf.Infinity, Color.red);

        if (rayCheck)
        {
            Debug.Log("Hitting");
            isFilled = hitInfo.collider.GetComponent<HolderCubeScript>().isFilled;
            _cubesGroupScript.isFilledC = isFilled;
            Debug.Log("isFilled Value "+ isFilled);
            if (!isFilled && Input.GetMouseButtonUp(0))
            {
                var position = hitInfo.transform.position;
                transform.position = position;
                parentObjPosition.z = position.z;

            }
        }*/
        
    }
}