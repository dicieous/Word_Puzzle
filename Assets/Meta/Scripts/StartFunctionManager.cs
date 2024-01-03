using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFunctionManager : MonoBehaviour
{
    public static StartFunctionManager instance;
    
       private void Awake()
       {
          instance = this;
       }
    
       public BoatIslandStart boatIslandData;
       public NightCampStart nightCampData;
       
    }
    [System.Serializable]
    public class BoatIslandStart
    {
       [Header("BirdsData")] public List<GameObject> birds;
       public void DecidingFun(string nameData)
       {
          switch (nameData)
          {
             case "Birds":
                BirdsFun();
                break;
             default:
                break;
          }
       }
       public void BirdsFun()
       {
          for (int i = 0; i < birds.Count; i++)
          {
             birds[i].SetActive(true);
          }
       }
    }
    
    [System.Serializable]
    public class NightCampStart
    {
       [Header("TentFire")] 
       public GameObject kitten;
       public GameObject fireObj;
       [Header("Van details")] 
       public GameObject vanLightObjects;
    
       public void DecidingFun(string nameData)
       {
          switch (nameData)
          {
             case "FireLight":
                FireLight();
                break;
             case "Van":
                Van();
                break;
             default:
                break;
          }
       }
    
       public void FireLight()
       {
          kitten.SetActive(true);
          fireObj.SetActive(true);
       }
    
       public void Van()
       {
          vanLightObjects.SetActive(true);
       }
    }