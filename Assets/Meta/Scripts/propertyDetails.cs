using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class propertyDetails : MonoBehaviour
{
    public Material dissolveMaterial;
    public List<GameObject> attachObjects;
    
    public float rangeStartNumber;
    public float rangeFinishNumber;
    public float percentValueIncrease;
    public int coinsRequired;
    [SerializeField]
    //private int _coinsCount;
    public string objName,objNameRefCheck;
    
    //private static readonly int DisAmount = Shader.PropertyToID("_DisAmount");

    private void Awake()
    {
        objName = gameObject.name;
        objNameRefCheck = gameObject.name + "Checker" ;
    }

    private void Start()
    {
        dissolveMaterial = GetComponent<MeshRenderer>().materials[0]; 
        var val = rangeStartNumber + (-(rangeFinishNumber));
        percentValueIncrease = (float)((val) / coinsRequired);
        
        if (Metadata.GetMaterialStartCheck(objNameRefCheck) == 0)
        {
            dissolveMaterial.DOFloat(rangeStartNumber, $"_DisAmount", 0.01f);
            Metadata.SetMaterialFill(objName, rangeStartNumber);
            if (attachObjects.Count != 0)
            {
                for (int i = 0; i < attachObjects.Count; i++)
                {
                    var obj= attachObjects[i].transform.GetComponent<MeshRenderer>().materials[0];
                    obj.DOFloat(rangeStartNumber, $"_DisAmount", 0.15f);
                }
            }
        }
        else if (Metadata.GetMaterialStartCheck(objNameRefCheck) == 1)
        {
            dissolveMaterial.DOFloat(Metadata.GetMaterialFill(objName), $"_DisAmount", 0.01f);
        }
    }

    private void Update()
    {
        /*if (Input.GetKey(KeyCode.A))
        {
            MetaManager.instance.elapsedTime += Time.deltaTime;
            if (MetaManager.instance.elapsedTime >= MetaManager.instance.instantiationInterval)
            {
                FillingAmount();
                MetaManager.instance.elapsedTime = 0f;
            }
        }*/
    }

    private bool _done;
    public void FillingAmount()
    {
        var tempval = Metadata.GetMaterialFill(objName) - percentValueIncrease;
        print(Metadata.GetMaterialFill(objName));
        if (tempval <= (rangeFinishNumber-percentValueIncrease) && !_done)
        {
            _done = true;
            var checkObj = Metadata.instance.propertyClassList[Metadata.GetParentNumber()].propertiesList;
            if (Metadata.GetChildNumber() == checkObj.Count - 1)
            {
                print(Metadata.GetChildNumber());
                Metadata.SetChildNumber(0);
                Metadata.SetParentNumber(Metadata.GetParentNumber() + 1);
                MetaManager.instance.RightButtonPress();
            }
            else
            {
                Metadata.SetChildNumber(Metadata.GetChildNumber() + 1);
                print(objName);
            }
            MetaManager.instance.RevelingObject();
        }
        else
        {
            dissolveMaterial.DOFloat(tempval, $"_DisAmount", 0.15f).OnComplete(() =>
            {
                Metadata.SetMaterialFill(objName, tempval);
            });
            if (attachObjects.Count != 0)
            {
                for (int i = 0; i < attachObjects.Count; i++)
                {
                    var obj= attachObjects[i].transform.GetComponent<MeshRenderer>().materials[0];
                    obj.DOFloat(tempval, $"_DisAmount", 0.15f);
                }
            }
        }

        if (Metadata.GetMaterialStartCheck(objNameRefCheck) == 0)
        {
            Metadata.SetMaterialStartCheck(objNameRefCheck, 1);
            print(Metadata.GetMaterialStartCheck(objNameRefCheck));
        }
        
    }
}