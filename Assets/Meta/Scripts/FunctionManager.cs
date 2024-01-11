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
   public Park parkData;
   public Japan japanData;
   public London londonData;

   private void Start()
   {
      /*print("Function Manager::"+Metadata.GetParentNumber());
      print("Function Count::"+(Metadata.instance.propertyClassList.Count-1));
      if (Metadata.GetParentNumber() > Metadata.instance.propertyClassList.Count - 1)
      {
         var num = Metadata.instance.propertyClassList.Count - 1;
         Metadata.SetParentNumber(num);
         Metadata.SetChildNumber(Metadata.instance.propertyClassList[num].propertiesList.Count - 1);
      }*/
      ColoredIteamsData();
   }

   public void ColoredIteamsData()
   {
      var num = Metadata.GetParentNumber();
      if (num > Metadata.instance.propertyClassList.Count - 1)
      {
         num = Metadata.instance.propertyClassList.Count - 1;
      }
      for (int i = 0; i <= num; i++)
      {
         for (int j = 0; j < startFunLists[i].objDetails.Count; j++)
         {
            //startFunLists[i].objDetails[j].SetActive(true);
            //if (startFunLists[i].objDetails[j].GetComponent<MeshRenderer>().materials.Length < 1) return;
            var meter = startFunLists[i].objDetails[j].GetComponent<MeshRenderer>().materials;
            if (meter.Length > 1)
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
public class Park
{
   [Header("jainWheel")] 
   public GameObject jainWheel;
   public GameObject animatedJainWheel;
   [Header("Carousel")] 
   public GameObject carouselObj;
   public void DecidingFun(string nameData)
   {
      switch (nameData)
      {
         case "JainWheel":
            JainWheelFun();
            break;
         case "Carousel":
            CarouselFun();
            break;
         default:
            break;
      }
   }

   public void JainWheelFun()
   {
      jainWheel.SetActive(false);
      animatedJainWheel.SetActive(true);
   }

   public void CarouselFun()
   {
      carouselObj.GetComponent<DOTweenAnimation>().DOPlay();
   }
}

[System.Serializable]
public class Japan
{
   [Header("Temple")] 
   public GameObject originalTemple;
   public GameObject dupTemple;
   public List<GameObject> Laterns;
   public void DecidingFun(string nameData)
   {
      switch (nameData)
      {
         case "Temple":
            TempleFun();
            break;
         default:
            break;
      }
   }

   public void TempleFun()
   {
      originalTemple.SetActive(true);
      dupTemple.SetActive(false);
      /*for (int i = 0; i < Laterns.Count; i++)
      {
         Laterns[i].GetComponent<DOTweenAnimation>().DOPlay();
      }*/
   }
}
[System.Serializable]
public class London
{
   [Header("JainWheel")] 
   public GameObject jainWheel;
   public GameObject dupJainWheel;
   [Header("BigBen")] 
   public GameObject bigBen;
   public GameObject dupBigBen;
   [Header("Bus")]
   public GameObject bus;
   [Header("Bridge")] 
   public List<GameObject> cars;
   public void DecidingFun(string nameData)
   {
      switch (nameData)
      {
         case "JainWheelLondon":
            JainWheelLondon();
            break;
         case "BigBen":
            BigBen();
            break;
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

   public void JainWheelLondon()
   {
      dupJainWheel.SetActive(false);
      jainWheel.SetActive(true);
      jainWheel.transform.GetChild(0).GetComponent<DOTweenAnimation>().DOPlay();
   }

   public void BigBen()
   {
      bigBen.SetActive(true);
      dupBigBen.SetActive(false);
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
