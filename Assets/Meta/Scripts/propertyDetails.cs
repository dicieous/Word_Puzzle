using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public enum IslandData
{
    None,
    BoatIsland,
    
    NightCamp,
    Japan,
    London
}
public enum DataGeting
{
    None,
    Birds,
    
    FireLight,
    Van,
    
    Bus,
    Bridge
}

public class propertyDetails : MonoBehaviour
{
    public float punchValue;
    public Material dissolveMaterial;
    public List<GameObject> attachObjects;
    
    public IslandData islandDataGet;
    public DataGeting getDataEnum;
    public FunctionManager fM;
    
    public float rangeStartNumber;
    public float rangeFinishNumber;
    public float percentValueIncrease;
    public int coinsRequired;
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
        fM = FunctionManager.instance;
        
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
            if (attachObjects.Count != 0)
            {
                for (int i = 0; i < attachObjects.Count; i++)
                {
                    var obj= attachObjects[i].transform.GetComponent<MeshRenderer>().materials[0];
                    obj.DOFloat(Metadata.GetMaterialFill(objName), $"_DisAmount", 0.01f);
                }
            }
            if (getDataEnum != DataGeting.None)
            {
                DataFun(islandDataGet.ToString(),getDataEnum.ToString());
            }
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
    // ReSharper disable Unity.PerformanceAnalysis
    public void FillingAmount()
    {
        var tempval = Metadata.GetMaterialFill(objName) - percentValueIncrease;
        // print(Metadata.GetMaterialFill(objName));
        if (tempval <= (rangeFinishNumber-percentValueIncrease) && !_done)
        {
            _done = true;
            var checkObj = Metadata.instance.propertyClassList[Metadata.GetParentNumber()].propertiesList;
            if (Metadata.GetChildNumber() == checkObj.Count - 1)
            {
                print(Metadata.GetChildNumber());
                Metadata.SetChildNumber(0);
                Metadata.SetParentNumber(Metadata.GetParentNumber() + 1);
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    FunctionManager.instance.ColoredIteamsData();
                    MetaManager.instance.RightButtonPress();
                });
                DOVirtual.DelayedCall(2f, () =>
                {
                    Metadata.instance.unlockEffect.Play();
                });
                DOVirtual.DelayedCall(3.5f, () =>
                {
                    MetaManager.instance.notBuild = false;
                });
            }
            else
            {
                Metadata.SetChildNumber(Metadata.GetChildNumber() + 1);
                var parent = transform.parent;
                var childCount = parent.transform.childCount; 
                parent.transform.GetChild(childCount-1).GetComponent<ParticleSystem>().Play();
                if (getDataEnum != DataGeting.None)
                {
                    DataFun(islandDataGet.ToString(),getDataEnum.ToString());
                }

                DOVirtual.DelayedCall(1f, () =>
                {
                    MetaManager.instance.notBuild = false;
                });
            }
            MetaManager.instance.notBuild = true;
            MetaManager.instance.RevelingObject();
        }
        else
        {
            if (MetaManager.instance._jumping) return;
            transform
                .DOPunchPosition(new Vector3(0f, punchValue, 0f), 0.15f, 5, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
                {
                    MetaManager.instance._jumping = false;
                });
            MetaManager.instance._jumping = true;
            dissolveMaterial.DOFloat(tempval, $"_DisAmount", 0.15f).OnComplete(() =>
            {
                Metadata.SetMaterialFill(objName, tempval);
            });
            if (attachObjects.Count != 0)
            {
                for (int i = 0; i < attachObjects.Count; i++)
                {
                    var obj= attachObjects[i].transform.GetComponent<MeshRenderer>().materials[0];
                    var objTransform = attachObjects[i].transform;
                    obj.DOFloat(tempval, $"_DisAmount", 0.15f);
                    objTransform
                        .DOPunchPosition(new Vector3(0f, punchValue, 0f), 0.15f, 5, 0.5f).SetEase(Ease.Flash);
                }
            }
        }

        if (Metadata.GetMaterialStartCheck(objNameRefCheck) == 0)
        {
            Metadata.SetMaterialStartCheck(objNameRefCheck, 1);
            print(Metadata.GetMaterialStartCheck(objNameRefCheck));
        }
        
    }


    public void DataFun(String callingClass,string callingFun)
    {
        switch (callingClass)
        {
            case "BoatIsland":
                fM.boatIslandData.DecidingFun(callingFun);
                break;
            case "NightCamp":
                fM.nightCampData.DecidingFun(callingFun);
                break;
            case "London":
                fM.londonData.DecidingFun(callingFun);
                break;
            default:
                break;
        }
    }
}