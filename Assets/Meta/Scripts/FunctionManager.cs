using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class FunctionManager : MonoBehaviour
{
   public static FunctionManager instance;

   private void Awake()
   {
      instance = this;
   }
   public List<StartingFunCalls> startFunLists;
   
   public BoatIsland boatIslandData;
   public NightCamp nightCampData;
   public London londonData;

   private void Start()
   {
      ColoredIteamsData();
   }

   public void ColoredIteamsData()
   {
      for (int i = 0; i <= Metadata.GetParentNumber(); i++)
      {

         for (int j = 0; j < startFunLists[i].objDetails.Count; j++)
         {
            //startFunLists[i].objDetails[j].SetActive(true);
            //if (startFunLists[i].objDetails[j].GetComponent<MeshRenderer>().materials.Length < 1) return;
            var meter = startFunLists[i].objDetails[j].GetComponent<MeshRenderer>().materials;
            if (meter.Length > 0)
            {
               List<Material> materialList = new List<Material>(meter);
               materialList.RemoveAt(materialList.Count - 1);

               startFunLists[i].objDetails[j].GetComponent<MeshRenderer>().materials = materialList.ToArray();
            }
            //startFunLists[i].objDetails[j].GetComponent<MeshRenderer>().materials[1] = null;
         }
      }
   }
}
[System.Serializable]
public class StartingFunCalls
{
   public List<GameObject> objDetails;
}
[System.Serializable]
public class BoatIsland
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
public class NightCamp
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

[System.Serializable]
public class London
{
   [Header("Bus")]
   public GameObject bus;
   [Header("Bridge")] 
   public List<GameObject> cars;
   public void DecidingFun(string nameData)
   {
      switch (nameData)
      {
         case "Bus":
            BusFun();
            break;
         case "Bridge":
            BridgeFun();
            break;
         default:
            break;
      }
   }

   public void BusFun()
   {
      bus.GetComponent<DOTweenAnimation>().DOPlay();
   }

   public void BridgeFun()
   {
      for (int i = 0; i < cars.Count; i++)
      {
         cars[i].SetActive(true);
      }
   }
}
