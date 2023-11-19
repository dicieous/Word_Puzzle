using System.Linq;
using UnityEngine;

public class StickingAreaCheckingScript : MonoBehaviour
{
    [HideInInspector] public bool correctWordMade = false;
    [HideInInspector] public bool imageRevelDone;
    
    public string answerString;
    
    private HolderCubeScript[] holderCubeScripts;

    public GameObject emojiRevel;
    
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

            Debug.Log(madeWord);
            if (madeWord == answerString)
            {
                if (!emojiRevel) emojiRevel = null; 
                    correctWordMade = true;
                return true;
            }
        }

        return false;
    }
    
    
}
