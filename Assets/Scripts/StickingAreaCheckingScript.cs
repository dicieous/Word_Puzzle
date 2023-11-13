using System.Linq;
using UnityEngine;

public class StickingAreaCheckingScript : MonoBehaviour
{
    [HideInInspector] public bool correctWordMade = false;
    
    public string answerString;
    
    private HolderCubeScript[] holderCubeScripts;

    private void Start()
    {
        holderCubeScripts = GetComponentsInChildren<HolderCubeScript>();
    }

    public bool IsAllPlacesFullCheck()
    {
        foreach (var vScript in holderCubeScripts)
        {
            if(!vScript.isFilled) return false;
        }

        return true;
    }

    public bool CheckForAnswer()
    {
        if (IsAllPlacesFullCheck())
        {
            var madeWord = holderCubeScripts.Aggregate("", (current, hScript) => current + hScript.inputext);

            //Debug.Log(madeWord);
            if (madeWord == answerString)
            {
                correctWordMade = true;
                return true;
            }
        }

        return false;
    }
    
    
}
