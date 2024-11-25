using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LetterGroupSet : SingletonInstance<LetterGroupSet>
{
    public bool lettersSpawning;
    public static Action OnLetterGroupSet;
    public List<CubesGroupScript> letterSets;

    [SerializeField] public List<CubesGroupScript> currentlyActiveSets;
    
    private List<GameManager.CompleteWordCubes> _completeWordCubesList = new();
    private List<GameManager.CompleteWordPosition> _completeWordPositionsList = new();

    public List<GameObject> currentAutoWordSets;
    public List<Vector3> currentAutoWordPositions;
    public int magnetIndexNum;
    
    public List<GameObject> lettersList;
    public List<HighlightTextScript> hintValue;

    private void OnEnable()
    {
        lettersSpawning = true;
        OnLetterGroupSet += RemoveElement;
    }

    private void OnDisable()
    {
        OnLetterGroupSet -= RemoveElement;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RemoveElement();
        }
    }

    private void Start()
    {
        letterSets ??= new List<CubesGroupScript>();
        currentlyActiveSets ??= new List<CubesGroupScript>();
        hintValue ??= new List<HighlightTextScript>();

        letterSets.AddRange(GetComponentsInChildren<CubesGroupScript>(true));
        /*foreach (var letterSet in letterSets)
        {
            if(!letterSet.canPlaceNow)
                letterSet.gameObject.SetActive(false);
        }*/

        _completeWordCubesList = GameManager.Instance.completeWordCubesList;
        for (int i = 0; i < _completeWordCubesList.Count; i++)
        {
            for (int j = 0; j < _completeWordCubesList[i].completeWordCubeGroup.Count; j++)
            {
                var obj = _completeWordCubesList[i].completeWordCubeGroup[j];
                currentAutoWordSets.Add(obj);   
            }
        }
        _completeWordPositionsList= GameManager.Instance.completeWordPositionsList;
        for (int i = 0; i < _completeWordPositionsList.Count; i++)
        {
            for (int j = 0; j < _completeWordPositionsList[i].completeWordCubePositionGroup.Count; j++)
            {
                var vector3Pos = _completeWordPositionsList[i].completeWordCubePositionGroup[j];
                currentAutoWordPositions.Add(vector3Pos);
            }
        }
        NewLetterSet();
    }

    private void RemoveElement()
    {
        var letterSetDone = currentlyActiveSets.FirstOrDefault(letterSet => letterSet != null && letterSet.doneWithWord);
        if (letterSetDone == null) return;
        letterSets.Remove(letterSetDone);
        currentlyActiveSets.Remove(letterSetDone);

        if (!CurrentlyActiveSet()) return;
        lettersSpawning = true;
        NewLetterSet();
    }

    public int MagnetData()
    {
        if (lettersSpawning) return -1;
        var activeObject = currentAutoWordSets.Find(x => x.activeInHierarchy && !x.GetComponent<CubesGroupScript>().doneWithWord);
        return activeObject == null ? -1 : currentAutoWordSets.FindIndex(x => x.name == activeObject.name);
    }
    public GameObject HintActivateDecider()
    {
        if (hintValue.Count <= 0 || lettersList.Count <= 0) return null;
        var hintValueObj = hintValue.Find(x => !x.done &&
                                               lettersList.Find(a => a.name == x.gameObject.name && a.activeInHierarchy))?.gameObject;
        if (hintValueObj != null)
        {
            var script = hintValueObj.GetComponent<HighlightTextScript>();
            hintValue.Remove(script);
        }
        var hintObject = lettersList.Find(a => hintValueObj != null && a.name == hintValueObj.name)?.gameObject;
        if (hintObject != null) lettersList.Remove(hintObject);
        
        return hintValueObj;
    }
    
    private void NewLetterSet()
    {
        UIManagerScript.Instance.HintButtonDeActiveFun();
        UIManagerScript.Instance.AutoButtonDisActive();
        for (int i = 0; i < letterSets.Count; i++)
        {
            if (!currentlyActiveSets.Contains(letterSets[i]))
                currentlyActiveSets.Add(letterSets[i]);
        }

        foreach (var currentlyActiveSet in currentlyActiveSets)
        {
            if(!currentlyActiveSet.canPlaceNow)
            {
                currentlyActiveSet.gameObject.SetActive(true);
                currentlyActiveSet.transform.DOScale(0, .5f).SetEase(Ease.OutBack).From().OnComplete(() =>
                {
                    currentlyActiveSet.StartPositionAssignFun(currentlyActiveSet.transform.position);
                });
            }
        }

        DOVirtual.DelayedCall(1f, ()=>
        {
            lettersSpawning = false;
            if (GameManager.Instance)
            {
                GameManager.Instance.scriptOff = false;
                UIManagerScript.Instance.AbilityTutorial();
            }
            UIManagerScript.Instance.HintButtonActiveFun();
            GameManager.Instance.autoWordClick = false;
            UIManagerScript.Instance.AutoButtonActiveFun();
        });
    }

    private bool CurrentlyActiveSet() => currentlyActiveSets.Count == 0;
}