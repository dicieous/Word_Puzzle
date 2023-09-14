using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;





public class CameraMotionScript : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private List<Vector3> camPos;

    [SerializeField] private List<float> orthoSize;

    [SerializeField] private float moveDuration;
    [SerializeField] private List<StickingAreaToShow> stickingAreaToShow;
    
    private static int _cameraMoved;

    [Serializable] 
    public struct StickingAreaToShow
    {
        public List<GameObject> stickingAreaCubesGroup;
    }
    

    
    
    public void MoveCamera()
    {
        mainCamera.transform.DOMove(camPos[_cameraMoved], moveDuration).SetEase(Ease.InOutCirc);
        mainCamera.DOOrthoSize(orthoSize[_cameraMoved], moveDuration).SetEase(Ease.InOutCirc);
        _cameraMoved++;
    }
    
    
}


