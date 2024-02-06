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
    public static int GetTotalBricks() => PlayerPrefs.GetInt("TotalBricks", 0);
    public static void SetTotalBricks(int reqNum) => PlayerPrefs.SetInt("TotalBricks", reqNum);
    
    //Bricks Required For Object
    public static int GetBricksRequired(string bricksObjName) => PlayerPrefs.GetInt(bricksObjName, 0);
    public static void SetBricksRequired(string bricksObjName,int brickCount) => PlayerPrefs.SetInt(bricksObjName, brickCount);
    
    
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
    
    
    public static float GetMaterialFillAmount(string objNaming)=> PlayerPrefs.GetFloat(objNaming, 0);
    public static void SetMaterialFillAmount(string objNaming,float val) => PlayerPrefs.SetFloat(objNaming, val);

    public static int GetObjectDoneOrNot(string objDoneNaming) => PlayerPrefs.GetInt(objDoneNaming, 0);
    public static void SetObjectDoneOrNot(string objDoneNaming,int num) => PlayerPrefs.SetInt(objDoneNaming, num);
    
    
    [Serializable]
    public class PropertyClass
    {
        public List<GameObject> propertiesList;
    }
}

