using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metadata : MonoBehaviour
{
    public static Metadata instance;

    public List<PropertyClass> propertyClassList;
    public GameObject mainParentObj;
    private float _objMoveDistance = 40;
    public float posNumber;

    public ParticleSystem unlockEffect;
    
    private void Awake()
    {
        instance = this;
    }
    
    ////---Required number to Construct object
    public static int GetTotalBricksRequired() => PlayerPrefs.GetInt("TotalBricksRequired", 0);
    public static void SetTotalBricksRequired(int reqNum) => PlayerPrefs.SetInt("TotalBricksRequired", reqNum);
    ////---Bar meter percentagevalue
    public static float GetBarPercentage() => PlayerPrefs.GetFloat("barPercentage",1);
    public static void SetBarPercentage(float percent) => PlayerPrefs.SetFloat("barPercentage", percent);

    ////---Parent and child number
    public static int GetParentNumber() => PlayerPrefs.GetInt("ParentCountNumber", 0);
    public static void SetParentNumber(int parentNum) => PlayerPrefs.SetInt("ParentCountNumber", parentNum);

    public static int GetChildNumber() => PlayerPrefs.GetInt("childCountNumber", 0);
    public static void SetChildNumber(int childNum) => PlayerPrefs.SetInt("childCountNumber", childNum);
    
    ////---Material details
    public static int GetMaterialStartCheck(string objNamingCheck) => PlayerPrefs.GetInt(objNamingCheck, 0);
    public static void SetMaterialStartCheck(string objNamingCheck,int startOrNot) => PlayerPrefs.SetInt(objNamingCheck, startOrNot);
    
    
    public static float GetMaterialFill(string objNaming)=> PlayerPrefs.GetFloat(objNaming, 0);
    public static void SetMaterialFill(string objNaming,float val) => PlayerPrefs.SetFloat(objNaming, val);
    
    [Serializable]
    public class PropertyClass
    {
        public List<GameObject> propertiesList;
    }
}

