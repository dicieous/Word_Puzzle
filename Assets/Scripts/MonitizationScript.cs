using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Coffee.UIExtensions;
using DDZ;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;
using Random = System.Random;

public class MonitizationScript : MonoBehaviour
{
   public static MonitizationScript instance;

   public GameObject instanceImageRef;
   [Header("Coin Animation Details")] 
   public Sprite coinImage;
   public GameObject giftObject;
   public GameObject giftCoinInstancePos;
   public GameObject giftCoinMovePosition;
   [Header("Gift Details")]
   public Image giftImage;
   public UIParticle giftParticle;
   [Header("Magnet details")]
   public Sprite magnetImage;
   public GameObject giftMagnetInstancePosition;
   public GameObject giftMagnetMovePosition;
   [FormerlySerializedAs("giftHint")] [Header("Hint details")]
   public Sprite hintImage;
   public GameObject giftHintInstancePosition;
   public GameObject giftHintMovePosition;
   [Header("*2X Button Details")]
   public GameObject bubble2X;
   public Button bubble2XButton;

   private GameObject rvGiftIcon => giftObject.transform.GetChild(2).gameObject;
   private GameObject loadingGiftIcon => giftObject.transform.GetChild(3).gameObject;

   private static int _giftBoxCount = 0;
   private void Awake()
   {
      instance = this;
   }

   private void Start()
   {
      // StartCoroutine(UpdateMoneyOnWin(instanceImageRef,giftCoinInstancePos,giftCoinMovePosition));
   }

   private void Update()
   {
      if(!GameEssentials.instance) return;
      rvGiftIcon.SetActive(GameEssentials.IsRvAvailable());
      loadingGiftIcon.SetActive(!GameEssentials.IsRvAvailable());
   }

   public void GiftButtonFun()
   {
      if(!GameEssentials.instance) return;
      GameEssentials.RvType = RewardType.GiftBox;
      GameEssentials.ShowRewardedAds("GiftBox");
      if(LionStudiosManager.instance)
         LionStudiosManager.AdsEvents(true, AdsEventState.Start,UIManagerScript.Instance.GetSpecialLevelNumber(),"Applovin","GiftBox",CoinManager.instance.GetCoinsCount());
   }

   // ReSharper disable Unity.PerformanceAnalysis
   public void GiftBox_CallBack()
   {
      var num = UnityEngine.Random.Range(0, 3);
      giftImage.GetComponent<Button>().interactable = false;
      giftParticle.Play();
      switch (num)
      {
         case 0 :
            instanceImageRef.GetComponent<Image>().sprite = coinImage;
            StartCoroutine(UpdateMoneyOnWin(instanceImageRef,giftCoinInstancePos,giftCoinMovePosition,50));
            if (LionStudiosManager.instance)
            {
               LionStudiosManager.GiftBox(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),"Coins","100",_giftBoxCount.ToString());
               _giftBoxCount++;
            }
            break;
         case  1:
            instanceImageRef.GetComponent<Image>().sprite = magnetImage;
            MagnetSpawn(instanceImageRef,giftMagnetInstancePosition,giftMagnetMovePosition,100);
            if (LionStudiosManager.instance)
            {
               LionStudiosManager.GiftBox(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),"Coins","100",_giftBoxCount.ToString());
               _giftBoxCount++;
            }
            break;
         case 2:
            instanceImageRef.GetComponent<Image>().sprite = hintImage;
            MagnetSpawn(instanceImageRef,giftHintInstancePosition,giftHintMovePosition,50);
            if (LionStudiosManager.instance)
            {
               LionStudiosManager.GiftBox(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),"Coins","100",_giftBoxCount.ToString());
               _giftBoxCount++;
            }
            break;
         default:
            break;
      }

      DOVirtual.DelayedCall(2, () =>
      {
         giftImage.GetComponent<Button>().interactable = true;
      },false);
      
   }

   public void MagnetSpawn(GameObject instanceObj,GameObject instancePos,GameObject movePosition,int coinIncreaseNumber)
   {
      GameObject obj=Instantiate(instanceObj, instancePos.transform.position, Quaternion.identity);
      //magnet.transform.DOScale(Vector3.zero, 0.15f).From();
      obj.transform.SetParent(instancePos.transform.parent);
      obj.GetComponent<RectTransform>().anchoredPosition = instancePos.GetComponent<RectTransform>().anchoredPosition;
      obj.GetComponent<RectTransform>().DOScale(obj.transform.localScale * 2.5f, 0.65f).OnComplete(() =>
      {
         obj.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.75f).SetEase(Ease.Linear);
      });
      obj.GetComponent<RectTransform>()
         .DOJumpAnchorPos(movePosition.GetComponent<RectTransform>().anchoredPosition, 300f, 1, 1f)
         .SetEase(Ease.Linear).OnComplete(() =>
         {
            //magnet.GetComponent<RectTransform>().DOPunchAnchorPos(Vector2.one * 2.5f, 0.15f, 2);
            CoinManager.instance.CoinsIncrease(coinIncreaseNumber);
            DOVirtual.DelayedCall(0.2f, () =>
            {
               obj.gameObject.SetActive(false);
               Destroy(obj);
            });
         });
   }
   // ReSharper disable Unity.PerformanceAnalysis
   IEnumerator UpdateMoneyOnWin(GameObject instanceObj,GameObject instancePos,GameObject movePosition,int coinIncreaseNumber)
   {
      DOVirtual.DelayedCall(2f, () =>
      {
         CoinManager.instance.CoinsIncrease(coinIncreaseNumber);
      });
      for (int i = 0; i < 10; i++)
      {
         GameObject obj = Instantiate(instanceObj, instancePos.transform.position, Quaternion.identity);
         obj.transform.DOScale(Vector3.zero, 0.3f).From();
         //moneyUI.transform.parent = moneyDisplayContent.transform;
         obj.transform.SetParent(instancePos.transform.parent);
         obj.GetComponent<RectTransform>().anchoredPosition = instancePos.GetComponent<RectTransform>().anchoredPosition;
         yield return new WaitForSeconds(2f/20f);
         obj.transform.DOScale(obj.transform.localScale * 3f, .06f);
         
         obj.GetComponent<RectTransform>().DOAnchorPos(movePosition.GetComponent<RectTransform>().anchoredPosition, 0.75f).SetEase(Ease.OutBack).OnComplete(
            () =>
            {
               obj.gameObject.SetActive(false);
               Destroy(obj);
            });
      }
   }
}
