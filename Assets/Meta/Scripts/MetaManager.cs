using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MetaManager : MonoBehaviour
{
   public static MetaManager instance;
   
   private int bricksCount;
   private int bricksRequired;
   [Header("Build Button Details")]
   public Image buildImage;
   [Header("JumpPosition")] 
   public GameObject revelingObject;
   public RectTransform uiImageRectTransform;
   public GameObject objectToInstantiate;
   
   public float instantiationInterval = 1f;
   private bool _isButtonHeld = false;
   [SerializeField]public float elapsedTime = 0f;

   private void Awake()
   {
      instance = this;
   }

   private void Start()
   {
      bricksRequired = Metadata.GetTotalBricksRequired();
      bricksCount = bricksRequired;
      print("Bricks Count:::::::::::::"+bricksCount);
      buildImage.fillAmount = Metadata.GetBarPercentage();
      RevelingObject();
      
      var num = Metadata.GetParentNumber();
      var objPos = Metadata.instance.mainParentObj.transform.localPosition;
      objPos = new Vector3((objPos.x + (num * -40)), 0f, 0f);
      Metadata.instance.mainParentObj.transform.localPosition = objPos;
      Metadata.instance.posNumber = objPos.x;

   }

   void Update()
   {
      // Check for user input or other conditions to trigger instantiation
      if (Input.GetKey(KeyCode.A))
      {
         if (bricksCount <= 0) return;
         elapsedTime += Time.deltaTime;
         /*if (!_isButtonHeld) return;
            elapsedTime += Time.deltaTime;*/
         if (elapsedTime >= instantiationInterval)
         {
            InstantiateObjectAtUIPosition();
            revelingObject.GetComponent<propertyDetails>().FillingAmount();
            elapsedTime = 0f;
         }
      }

      if (Input.GetKeyDown(KeyCode.R))
      {
         RightButtonPress();
      }
      if (Input.GetKeyDown(KeyCode.L))
      {
         LeftButtonPress();
      }
   }
   void InstantiateObjectAtUIPosition()
   {
      // Ensure the required components are set
      if (uiImageRectTransform == null || objectToInstantiate == null)
      {
         Debug.LogError("UI Image RectTransform or Object to Instantiate is not set!");
         return;
      }

      // Get the position of the UI element in screen space
      Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, uiImageRectTransform.position);

      // Convert the screen position to a world position
      Vector3 worldPos;
      if (RectTransformUtility.ScreenPointToWorldPointInRectangle(uiImageRectTransform, screenPos, Camera.main, out worldPos))
      {
         // Instantiate the object at the world position
         var obj=Instantiate(objectToInstantiate, worldPos, Quaternion.identity);
         InstanceObjectJumpFun(obj);
      }
   }

   public void InstanceObjectJumpFun(GameObject obj)
   {
      var value = Remap(bricksCount,bricksRequired, 0, 1, 0);
      buildImage.DOFillAmount(value, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
      {
         Metadata.SetBarPercentage(buildImage.fillAmount);
      });
      bricksCount--;
      Metadata.SetTotalBricksRequired(bricksCount);
      /*if (bricksCount == 1)
      {
         Metadata.SetBarPercentage(buildImage.fillAmount);
      }*/
      obj.transform.parent = revelingObject.transform;
      obj.transform.DOLocalJump(revelingObject.transform.position, 20, 1, 1f);
      var metaParentNum = Metadata.GetParentNumber();
      var metaChildNum = Metadata.GetChildNumber();
      if (_jumping) return;
      Metadata.instance.propertyClassList[metaParentNum].propertiesList[metaChildNum].transform
         .DOPunchPosition(new Vector3(0f, 0.1f, 0f), 0.15f, 5, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
         {
            _jumping = false;
         });
      _jumping = true;
   }

   private bool _jumping;
   public static float Remap(float value, float from1, float to1, float from2, float to2, bool isClamped = false)
   {
      if (isClamped)
      {
         value = Mathf.Clamp(value, from1, to1);
      }
      return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
   }

   public void RevelingObject()
   {
      if(!Metadata.instance.propertyClassList[Metadata.GetParentNumber()].propertiesList[Metadata.GetChildNumber()].activeInHierarchy)
            Metadata.instance.propertyClassList[Metadata.GetParentNumber()].propertiesList[Metadata.GetChildNumber()].SetActive(true);
      print("Meta data parent num :::::::::::::::::::::::"+Metadata.GetParentNumber());
      if (Metadata.GetParentNumber() > 0)
      {
         for (int i = 0; i <= Metadata.GetParentNumber(); i++)
         {
            if (i < Metadata.GetParentNumber())
            {
               for (int j = 0; j <   Metadata.instance.propertyClassList[i].propertiesList.Count; j++)
               {
                  Metadata.instance.propertyClassList[i].propertiesList[j].SetActive(true);
               }
            }
            else
            {  
               if (Metadata.GetChildNumber() == 0)
               {
                  Metadata.instance.propertyClassList[i].propertiesList[0].SetActive(true);
               }
               else
               {
                  for (int j = 0; j < Metadata.GetChildNumber(); j++)
                  {
                     Metadata.instance.propertyClassList[i].propertiesList[j].SetActive(true);
                  }
               }
            }
         }
      }
      else
      {
         if (Metadata.GetChildNumber() == 0)
         {
            Metadata.instance.propertyClassList[0].propertiesList[0].SetActive(true);
         }
         else
         {
            for (int j = 0; j <  Metadata.GetChildNumber(); j++)
            {
               Metadata.instance.propertyClassList[0].propertiesList[j].SetActive(true);
            }
         }
      }
      revelingObject = Metadata.instance.propertyClassList[Metadata.GetParentNumber()]
         .propertiesList[Metadata.GetChildNumber()].transform.GetChild(0).gameObject;
   }

   public void RightButtonPress()
   {
      var objPos = Metadata.instance.mainParentObj.transform.localPosition;
      var num= Metadata.instance.posNumber -40;
      if (objPos.x <= (5 * -40)) return;
      if (num <= (5 * -40)) return;
      Metadata.instance.posNumber -= 40;
      //var num = Metadata.GetParentNumber();
      objPos = new Vector3((Metadata.instance.posNumber), 0f, 0f);
      //Metadata.instance.mainParentObj.transform.localPosition = objPos;
      Metadata.instance.mainParentObj.transform.DOLocalMove(objPos, 0.15f);
   }
   public void LeftButtonPress()
   {
      var objPos = Metadata.instance.mainParentObj.transform.localPosition;
      var incNUm= Metadata.instance.posNumber + 40;
      if (objPos.x >= 0) return;
      if (incNUm >= 40) return;
      Metadata.instance.posNumber += 40;
      //var num = Metadata.GetParentNumber();
      objPos = new Vector3((Metadata.instance.posNumber), 0f, 0f);
      //Metadata.instance.mainParentObj.transform.localPosition = objPos;
      Metadata.instance.mainParentObj.transform.DOLocalMove(objPos, 0.15f);
   }
}
