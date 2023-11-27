using System.Linq;
using DG.Tweening;
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
            if (!vScript.isFilled)
            {
                if(_allPlaced)
                    _allPlaced = false;
                return false;
            }
            /*if(_allPlaced)
                _allPlaced = false;*/
        }

        return true;
    }

    private bool _allPlaced;
    public bool CheckForAnswer()
    {
        if (IsAllPlacesFullCheck() && !_allPlaced && !correctWordMade)
        {
            _allPlaced = true;
            var madeWord = holderCubeScripts.Aggregate("", (current, hScript) => current + hScript.inputext);

            Debug.Log(madeWord);
            if (madeWord == answerString)
            {
                if (emojiRevel)
                {
                    emojiRevel = null;
                } 
                correctWordMade = true;
                ColliderFun();   
                return true;
            }
        }

        return false;
    }

    public void ColliderFun()
    {
        DOVirtual.DelayedCall(0.5f, () =>
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var obj = transform.GetChild(i);
                var coll = obj.GetComponents<Collider>().ToList();
                if (coll.Count > 1)
                {
                    for (int j = 0; j < coll.Count; j++)
                    {
                        coll[j].enabled = false;
                    }
                }
                else obj.GetComponent<Collider>().enabled = false;
            }
        }, false);

    }
}
