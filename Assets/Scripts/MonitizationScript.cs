using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
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
   [FormerlySerializedAs("giftMagnet")] [Header("Magnet details")]
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
   private void Awake()
   {
      instance = this;
   }

   private void Start()
   {
      // StartCoroutine(UpdateMoneyOnWin(instanceImageRef,giftCoinInstancePos,giftCoinMovePosition));
   }

   public void GiftButtonFun()
   {
      var num = UnityEngine.Random.Range(0, 3);
      giftImage.GetComponent<Button>().interactable = false;
      switch (num)
      {
         case 0 :
            instanceImageRef.GetComponent<Image>().sprite = coinImage;
            StartCoroutine(UpdateMoneyOnWin(instanceImageRef,giftCoinInstancePos,giftCoinMovePosition,50));
            break;
         case  1:
            instanceImageRef.GetComponent<Image>().sprite = magnetImage;
            MagnetSpawn(instanceImageRef,giftMagnetInstancePosition,giftMagnetMovePosition,100);
            break;
         case 2:
            instanceImageRef.GetComponent<Image>().sprite = hintImage;
            MagnetSpawn(instanceImageRef,giftHintInstancePosition,giftHintMovePosition,50);
            break;
         default:
            break;
      }
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
