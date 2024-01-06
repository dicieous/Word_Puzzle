using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MetaManager : MonoBehaviour
{
   public static MetaManager instance;
   
   private int bricksCount;
   private int bricksRequired;
   [Header("BricksCount TextDetails")] 
   public TextMeshProUGUI totalBricksText;
   public TextMeshProUGUI remainingBricksText;
   
   [Header("Build Button Details")]
   public Image buildImage;

   public GameObject playButton;
   public TextMeshProUGUI playButtonText;
   public GameObject buildButton;
   public Button rightButton;
   public Button leftButton;
   
   [Header("JumpPosition")] 
   public GameObject revelingObject;
   public RectTransform uiImageRectTransform;
   public GameObject objectToInstantiate;
   
   public float instantiationInterval = 1f;
   private bool _isButtonHeld = false;
   [SerializeField]public float elapsedTime = 0f;
   public bool notBuild;

   [Header("StartFunctioning Details")] public List<GameObject> objectDetails;
   
   private void Awake()
   {
      instance = this;
   }

   private void Start()
   {
      if (SceneManager.GetActiveScene().buildIndex != SceneManager.sceneCountInBuildSettings - 1) return;
      bricksRequired = Metadata.GetTotalBricksRequired();
      bricksCount = bricksRequired;
      remainingBricksText.text = bricksRequired.ToString();
      totalBricksText.text = bricksCount.ToString();
      
      buildImage.fillAmount = Metadata.GetBarPercentage();
      RevelingObject();
      
      var num = Metadata.GetParentNumber();
      var objPos = Metadata.instance.mainParentObj.transform.localPosition;
      objPos = new Vector3((objPos.x + (num * -40)), 0f, 0f);
      Metadata.instance.mainParentObj.transform.localPosition = objPos;
      Metadata.instance.posNumber = objPos.x;
      if (Metadata.GetTotalBricksRequired() > 0)
      {
         buildButton.SetActive(true);
         playButton.SetActive(false);
      }
      var value = Remap(bricksCount,bricksRequired, 0, 1, 0);
      buildImage.DOFillAmount(value, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
      {
         Metadata.SetBarPercentage(buildImage.fillAmount);
      });
      playButtonText.text = "Play " + UIManagerScript.Instance.GetSpecialLevelNumber();
      
   }

   void Update()
   {
      // Check for user input or other conditions to trigger instantiation
      //if (Metadata.GetParentNumber() > Metadata.instance.propertyClassList.Count - 1) return;

      if (Input.GetMouseButton(0) && !notBuild)
      {
         if (bricksCount <= 0)
         {
            if (!playButton.activeInHierarchy)
            {
               buildButton.SetActive(false);
               playButton.SetActive(true);
               LeftRightButtonsFun();
            }
            return;
         }
         elapsedTime += Time.deltaTime;
         /*if (!_isButtonHeld) return;
            elapsedTime += Time.deltaTime;*/
         if (elapsedTime >= instantiationInterval)
         {
            InstantiateObjectAtUIPosition();
            revelingObject.GetComponent<propertyDetails>().FillingAmount();
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("MetaCube");
            elapsedTime = 0f;
         }
      }

      /*if (Input.GetKeyDown(KeyCode.R))
      {
         RightButtonPress();
      }
      if (Input.GetKeyDown(KeyCode.L))
      {
         LeftButtonPress();
      }*/
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
         var WP = worldPos;
         WP.y = (worldPos.y + 50);
         WP.z = (worldPos.y + 2);
         worldPos = WP;
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
      if (bricksCount > 0)
      {
         remainingBricksText.text = bricksCount.ToString();
      }
      /*if (bricksCount == 1)
      {
         Metadata.SetBarPercentage(buildImage.fillAmount);
      }*/
      obj.transform.parent = revelingObject.transform;
      var metaParentNum = Metadata.GetParentNumber();
      var metaChildNum = Metadata.GetChildNumber();
      var objpos = Metadata.instance.propertyClassList[metaParentNum].propertiesList[metaChildNum].transform
         .GetChild(0);
      var dis = (obj.transform.localPosition - objpos.transform.localPosition);
      if (dis.z >= -1500)
      {
         jumpPower = 10f;
      }
      else
      {
         jumpPower = 1000f;
      }

      obj.transform.GetChild(0).GetComponent<ParticleSystem>().DOPlay();
      obj.transform.GetChild(1).GetComponent<ParticleSystem>().DOPlay();
      obj.transform.DOLocalJump(objpos.transform.position, jumpPower, 1, 1f).OnComplete(() =>
      {
         obj.gameObject.SetActive(false);
      });
   }
[SerializeField]
   public bool _jumping;
   public float jumpPower;
   
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
      if (Metadata.GetParentNumber() > Metadata.instance.propertyClassList.Count - 1) return;
      if (!Metadata.instance.propertyClassList[Metadata.GetParentNumber()].propertiesList[Metadata.GetChildNumber()]
             .activeInHierarchy)
      {
         Metadata.instance.propertyClassList[Metadata.GetParentNumber()].propertiesList[Metadata.GetChildNumber()].SetActive(true);
         Metadata.instance.propertyClassList[Metadata.GetParentNumber()].propertiesList[Metadata.GetChildNumber()].transform
            .DOPunchPosition(new Vector3(0f, -2.5f, 0f), 0.4f, 10, 1f).SetEase(Ease.OutBounce);
      }
            
      //print("Meta data parent num :::::::::::::::::::::::"+Metadata.GetParentNumber());
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

   public void LeftRightButtonsFun()
   {
      if (Metadata.instance.posNumber == 0)
      {
         leftButton.transform.gameObject.SetActive(false);
         rightButton.transform.gameObject.SetActive(true);
      }
      else if (Metadata.instance.posNumber <= (-5 * 40))
      {
         leftButton.transform.gameObject.SetActive(true);
         rightButton.transform.gameObject.SetActive(false);
      }
      else
      {
         leftButton.transform.gameObject.SetActive(true);
         rightButton.transform.gameObject.SetActive(true);
      }
   }
   public void RightButtonPress()
   {
      if(!leftButton.transform.gameObject.activeInHierarchy) leftButton.transform.gameObject.SetActive(true);
      var objPos = Metadata.instance.mainParentObj.transform.localPosition;
      var num= Metadata.instance.posNumber - 40;
      if (objPos.x <= (5 * -40)) return;
      if (num <= (5 * -40)) return;
      Metadata.instance.posNumber -= 40;
      objPos = new Vector3((Metadata.instance.posNumber), 0f, 0f);
      Metadata.instance.mainParentObj.transform.DOLocalMove(objPos, 0.25f);
      if (Metadata.GetTotalBricksRequired() <= 0)
      {
         if (num <= (4 * -40)) rightButton.transform.gameObject.SetActive(false);
      }
      
      if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
      if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
      if(SoundHapticManager.Instance) SoundHapticManager.Instance.Play("MetaOut");
   }
   public void LeftButtonPress()
   {
      if(!rightButton.transform.gameObject.activeInHierarchy) rightButton.transform.gameObject.SetActive(true);
      var objPos = Metadata.instance.mainParentObj.transform.localPosition;
      var incNUm= Metadata.instance.posNumber + 40;
      if (objPos.x >= 0) return;
      if (incNUm >= 40) return;
      Metadata.instance.posNumber += 40;
      objPos = new Vector3((Metadata.instance.posNumber), 0f, 0f);
      Metadata.instance.mainParentObj.transform.DOLocalMove(objPos, 0.25f);
      if (Metadata.GetTotalBricksRequired() <= 0)
      {
         if (incNUm >= 0)  leftButton.transform.gameObject.SetActive(false);
      }
      if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
      if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
      if(SoundHapticManager.Instance) SoundHapticManager.Instance.Play("MetaIn");
   }

   public List<GameObject> obj;
}
