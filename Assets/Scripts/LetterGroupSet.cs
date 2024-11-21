using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LetterGroupSet : SingletonInstance<LetterGroupSet>
{
    public static Action OnLetterGroupSet;
    public List<CubesGroupScript> letterSets;

    [SerializeField] private List<CubesGroupScript> currentlyActiveSets;

    public List<GameManager.CompleteWordCubes> completeWordCubesList = new();

    [SerializeField] public List<GameManager.CompleteWordPosition> completeWordPositionsList = new();

    public List<GameObject> currentAutoWordSets;
    public List<Vector3> currentAutoWordPositions;

    private void OnEnable()
    {
        OnLetterGroupSet += RemoveElement;
    }

    private void OnDisable()
    {
        OnLetterGroupSet -= RemoveElement;
    }

    private void Start()
    {
        letterSets ??= new List<CubesGroupScript>();
        currentlyActiveSets ??= new List<CubesGroupScript>();

        letterSets.AddRange(GetComponentsInChildren<CubesGroupScript>());
        foreach (var letterSet in letterSets)
        {
            letterSet.gameObject.SetActive(false);
        }

        completeWordCubesList = GameManager.Instance.completeWordCubesList;
        for (int i = 0; i < completeWordCubesList.Count; i++)
        {
            for (int j = 0; j < completeWordCubesList[i].completeWordCubeGroup.Count; j++)
            {
                var obj = completeWordCubesList[i].completeWordCubeGroup[j];
                currentAutoWordSets.Add(obj);   
            }
        }
        completeWordPositionsList= GameManager.Instance.completeWordPositionsList;
        for (int i = 0; i < completeWordPositionsList.Count; i++)
        {
            for (int j = 0; j < completeWordPositionsList[i].completeWordCubePositionGroup.Count; j++)
            {
                var vector3Pos = completeWordPositionsList[i].completeWordCubePositionGroup[j];
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

        if (CurrentlyActiveSet())
        {
            NewLetterSet();
        }
    }

    public int MagnetData() => currentAutoWordSets.FindIndex(x=>x.activeInHierarchy);
    

    private void NewLetterSet()
    {
        for (int i = 0; i < Mathf.Min(3, letterSets.Count); i++)
        {
            if (!currentlyActiveSets.Contains(letterSets[i]))
                currentlyActiveSets.Add(letterSets[i]);
        }

        foreach (var currentlyActiveSet in currentlyActiveSets)
        {
            currentlyActiveSet.gameObject.SetActive(true);
            print("Calling");
            currentlyActiveSet.transform.DOScale(0, .5f).SetEase(Ease.OutBack).From().OnComplete(() =>
            {
                currentlyActiveSet.StartPositionAssignFun(currentlyActiveSet.transform.position);
            });
        }
    }

    private bool CurrentlyActiveSet() => currentlyActiveSets.Count == 0;
}