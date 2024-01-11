using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public enum IslandData
{
    None,
    BoatIsland,
    NightCamp,
    Park,
    Japan,
    London
}
public enum DataGeting
{
    None,
    Birds,
    
    FireLight,
    Van,
    
    JainWheel,
    Carousel,
    Temple,
    JainWheelLondon,
    BigBen,
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
    [FormerlySerializedAs("coinsRequired")] public int bricksRequired;
    private int _coinsCount;
    public GameObject brickRequiredCountObject;
    public string objName,objNameRefCheck,objNameDone,objBricksName;
    
    //private static readonly int DisAmount = Shader.PropertyToID("_DisAmount");

    private void Awake()
    {
        objName = gameObject.name;
        objNameRefCheck = name + "Checker" ;
        objNameDone = name + "Done" ;
        objBricksName = name + "Bricks";
    }

    private void Start()
    {
        fM = FunctionManager.instance;
        
        dissolveMaterial = GetComponent<MeshRenderer>().materials[0];
        var val = rangeStartNumber + (-(rangeFinishNumber));
        percentValueIncrease = (float)((val) / bricksRequired);

        if (Metadata.GetMaterialStartCheck(objNameRefCheck) == 0)
        {
            Metadata.SetBricksRequired(objBricksName, bricksRequired);
            if (brickRequiredCountObject)
                brickRequiredCountObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Metadata.GetBricksRequired(objBricksName).ToString();
            
            dissolveMaterial.DOFloat(rangeStartNumber, $"_DisAmount", 0.01f);
            Metadata.SetMaterialFillAmount(objName, rangeStartNumber);
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
            dissolveMaterial.DOFloat(Metadata.GetMaterialFillAmount(objName), $"_DisAmount", 0.01f);
            
            if (brickRequiredCountObject)
                brickRequiredCountObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Metadata.GetBricksRequired(objBricksName).ToString();
            
            if (attachObjects.Count != 0)
            {
                for (int i = 0; i < attachObjects.Count; i++)
                {
                    var obj= attachObjects[i].transform.GetComponent<MeshRenderer>().materials[0];
                    obj.DOFloat(Metadata.GetMaterialFillAmount(objName), $"_DisAmount", 0.01f);
                }
            }

            if (Metadata.GetObjectDoneOrNot(objNameDone) == 0) return;
            brickRequiredCountObject.SetActive(false);
            if (getDataEnum != DataGeting.None)
            {
                DataFun(islandDataGet.ToString(),getDataEnum.ToString());
            }
        }
        _coinsCount = bricksRequired - Metadata.GetBricksRequired(objBricksName);
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
        //var tempval = Metadata.GetMaterialFillAmount(objName) - percentValueIncrease;
        _coinsCount++;
        var tempval = Remap(_coinsCount,bricksRequired,0,rangeFinishNumber,rangeStartNumber);
        
        Metadata.SetBricksRequired(objBricksName, Metadata.GetBricksRequired(objBricksName) - 1);
        //coinsCount++;
        
        if (brickRequiredCountObject != null)
            brickRequiredCountObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Metadata.GetBricksRequired(objBricksName).ToString();
        
        // print(Metadata.GetMaterialFillAmount(objName));
        if (tempval < (rangeFinishNumber + percentValueIncrease) && !_done)
        {
            _done = true;
            DOVirtual.DelayedCall(0.25f,() =>
            {
                var checkObj = Metadata.instance.propertyClassList[Metadata.GetParentNumber()].propertiesList;
                if (Metadata.GetChildNumber() == checkObj.Count - 1)
                {
                    var parent = transform.parent;
                    var childCount = parent.transform.childCount;
                    parent.transform.GetChild(childCount - 1).GetComponent<ParticleSystem>().Play();
                    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("MetaObjDone");
                    if (getDataEnum != DataGeting.None && (Metadata.GetObjectDoneOrNot(objNameDone) != 1))
                    {
                        Metadata.SetObjectDoneOrNot(objNameDone, 1);
                        DataFun(islandDataGet.ToString(), getDataEnum.ToString());
                    }

                    if (Metadata.GetParentNumber() < Metadata.instance.propertyClassList.Count - 1)
                    {
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
                            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("MetaNewLand");
                        });
                        DOVirtual.DelayedCall(3.5f, () =>
                        {
                            MetaManager.instance.notBuild = false;
                        });
                    }

                }
                else
                {
                    Metadata.SetChildNumber(Metadata.GetChildNumber() + 1);
                    var parent = transform.parent;
                    var childCount = parent.transform.childCount;
                    parent.transform.GetChild(childCount - 1).GetComponent<ParticleSystem>().Play();
                    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("MetaObjDone");
                    if (getDataEnum != DataGeting.None && (Metadata.GetObjectDoneOrNot(objNameDone) != 1))
                    {
                        Metadata.SetObjectDoneOrNot(objNameDone, 1);
                        DataFun(islandDataGet.ToString(), getDataEnum.ToString());
                    }

                    DOVirtual.DelayedCall(1f, () =>
                    {
                        MetaManager.instance.notBuild = false;
                    });

                }
                brickRequiredCountObject.SetActive(false);
                MetaManager.instance.notBuild = true;
                MetaManager.instance.RevelingObject();
            },false);
        }
        if (MetaManager.instance._jumping) return;
        transform
            .DOPunchPosition(new Vector3(0f, punchValue, 0f), 0.15f, 5, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
            {
                MetaManager.instance._jumping = false;
            });
        MetaManager.instance._jumping = true;
        /*dissolveMaterial.DOFloat(tempval, $"_DisAmount", 0.15f).OnComplete(() =>
        {
            Metadata.SetMaterialFillAmount(objName, tempval);
        });*/
        dissolveMaterial.DOFloat(tempval, $"_DisAmount", 0.15f).OnComplete(() =>
        {
            Metadata.SetMaterialFillAmount(objName, tempval);
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
        if (Metadata.GetMaterialStartCheck(objNameRefCheck) == 0)
        {
            Metadata.SetMaterialStartCheck(objNameRefCheck, 1);
        }
        
    }
    public static float Remap(float value, float from1, float to1, float from2, float to2, bool isClamped = false)
    {
        if (isClamped)
        {
            value = Mathf.Clamp(value, from1, to1);
        }
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
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
            case "Park":
                fM.parkData.DecidingFun(callingFun);
                break;
            case "Japan":
                fM.japanData.DecidingFun(callingFun);
                break;
            case "London":
                fM.londonData.DecidingFun(callingFun);
                break;
            default:
                break;
        }
    }
}