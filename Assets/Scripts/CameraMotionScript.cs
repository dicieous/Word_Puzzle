using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CameraMotionScript : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private List<Vector3> camPos;

    [SerializeField] private List<float> orthoSize;

    [SerializeField] private float moveDuration;
    
    [SerializeField] private List<StickingAreaToShow> stickingAreaCubes;

    [SerializeField] private List<GameObject> letterGroups;

    private int cameraMoved;
    
    private bool levelCompleted = true;

    public Color deactivatedColor;
    public Color activatedColor;

    [Serializable]
    private struct StickingAreaToShow
    {
        public List<GameObject> stickingAreaCubesGroup;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().isLoaded)
        {
            DeactivateCubes();
            DOVirtual.DelayedCall(.5f, () =>
            {
                MoveCamera(() =>
                {
                    StartCoroutine(ActivateStickingCubes(() =>
                    {
                        ActivateLetterCubes();
                        cameraMoved++;
                    }));
                });
            });
        }

        GameManager.Instance.OnPartComplete += InstanceOnOnPartComplete;
    }

    private void InstanceOnOnPartComplete(object sender, EventArgs e)
    {
        DOVirtual.DelayedCall(.5f, () =>
        {
            MoveCamera(() =>
            {
                StartCoroutine(ActivateStickingCubes(() =>
                {
                    ActivateLetterCubes();
                    cameraMoved++;
                }));
            });
        });
    }

    private void Update()
    {
        if (GameManager.Instance.levelCompleted && levelCompleted)
        {
            GameManager.Instance.OnPartComplete -= InstanceOnOnPartComplete;
            levelCompleted = false;
            MoveCamera(null);
        }
    }


    private void MoveCamera(Action callAction)
    {
        //Debug.Log($"CameraMoved {cameraMoved} times");
        mainCamera.transform.DOMove(camPos[cameraMoved], moveDuration).SetEase(Ease.Linear);
        if (!levelCompleted)
        {
            mainCamera.DOOrthoSize(orthoSize[cameraMoved], moveDuration).SetEase(Ease.Linear);
        }
        else
        {
            mainCamera.DOOrthoSize(orthoSize[cameraMoved], moveDuration).SetEase(Ease.Linear).OnComplete(callAction.Invoke);
        }
        
        //Debug.Log("Camera Moved");
       
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator ActivateStickingCubes(Action callBack)
    {
        //Debug.Log("Sticking Cubes Activated");
        if (stickingAreaCubes.Count >= cameraMoved)
        {
            var stickingCubeGroup = stickingAreaCubes[cameraMoved].stickingAreaCubesGroup;
            for (var index = 0; index < stickingCubeGroup.Count; index++)
            {
                var t = stickingCubeGroup[index];
                var index1 = index;
                for (int j = 0; j < t.transform.childCount; j++)
                {
                    var j1 = j;
                    //print("Color Changed");
                    t.transform.GetChild(j1).GetComponent<Collider>().enabled = true;
                    t.transform.GetChild(j1).transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color =
                        activatedColor;
                    yield return new WaitForSeconds(0.1f);
                }
           
            }

            callBack.Invoke();
        }
    }

    void ActivateLetterCubes()
    {
        //Debug.Log("Letter Cubes Activated");
        if(letterGroups.Count < cameraMoved)return;
        var t = letterGroups[cameraMoved];
        t.SetActive(true);
        var cubes = t.transform;
        for (int j = 0; j < cubes.childCount; j++)
        {
            var j1 = j;
            var innerCubes = cubes.GetChild(j);
            for (int i = 0; i < innerCubes.transform.childCount; i++)
            {
                cubes.GetChild(j).transform.GetChild(i).transform.DOScale(new Vector3(1f, 1f, 1f), 0.7f).SetEase(Ease.OutElastic)
                    .OnComplete(() =>
                    {
                        var numColliders = cubes.GetChild(j1).transform.GetComponents<Collider>().Length;
                        for (int i = 0; i < numColliders; i++)  
                        {
                            cubes.GetChild(j1).transform.GetComponents<Collider>()[i].enabled = true;
                        }
                    });
            }
        } 
    }

    private void DeactivateCubes()
    {
        for (int i = 0; i < stickingAreaCubes.Count; i++)
        {
            var stickingCubeGroup = stickingAreaCubes[i].stickingAreaCubesGroup;
            foreach (var t in stickingCubeGroup)
            {
                var childCount = t.transform.childCount;
                for (int j = 0; j < childCount; j++)
                {
                    t.transform.GetChild(j).GetComponent<Collider>().enabled = false;
                }
                
                
                
                for (int j = 0; j < t.transform.childCount; j++)
                {
                    t.transform.GetChild(j).transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color = deactivatedColor;
                }
            }
        }

        for (var index = 0; index < letterGroups.Count; index++)
        {
            var t = letterGroups[index];
            var cubes = t.transform;

            for (int j = 0; j < cubes.childCount; j++)
            {
                var num =  cubes.GetChild(j).transform.GetComponents<Collider>().Length;
                for (int k = 0; k < num; k++)
                {
                    cubes.GetChild(j).transform.GetComponents<Collider>()[k].enabled = false;
                }
                
                var innerCubes = cubes.GetChild(j);
                for (int i = 0; i < innerCubes.transform.childCount; i++)
                {
                    cubes.GetChild(j).transform.GetChild(i).transform.localScale = new Vector3(0, 0, 0);
                    cubes.GetChild(j).transform.GetChild(i).transform.localScale = new Vector3(0, 0, 0);
                }
                
            }
        }
    }
}