using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class MonitizationScript : MonoBehaviour
{
   public static MonitizationScript instance;

   [Header("Coin Animation Details")] 
   public GameObject giftObject;
   public GameObject giftCoin;
   public GameObject giftCoinInstancePos;
   public GameObject giftCoinMovePosition;
   [Header("Gift Details")]
   public Image giftImage;

   private void Awake()
   {
      instance = this;
   }

   private void Start()
   {
      // StartCoroutine(UpdateMoneyOnWin(giftCoin,giftCoinInstancePos,giftCoinMovePosition));
   }

   public void GiftButtonFun()
   {
      giftImage.GetComponent<Button>().interactable = false;
      StartCoroutine(UpdateMoneyOnWin(giftCoin,giftCoinInstancePos,giftCoinMovePosition));
      DOVirtual.DelayedCall(2f, () =>
      {
         CoinManager.instance.CoinsIncrease(50);
      });
   }
   IEnumerator UpdateMoneyOnWin(GameObject instanceObj,GameObject instancePos,GameObject coinMovePos)
   {
      for (int i = 0; i < 10; i++)
      {
         GameObject moneyUI = Instantiate(instanceObj, instancePos.transform.position, Quaternion.identity);
         moneyUI.transform.DOScale(Vector3.zero, 0.3f).From();
         //moneyUI.transform.parent = moneyDisplayContent.transform;
         moneyUI.transform.SetParent(instancePos.transform.parent);
         moneyUI.GetComponent<RectTransform>().anchoredPosition = instancePos.GetComponent<RectTransform>().anchoredPosition;
         yield return new WaitForSeconds(2f/20f);
         moneyUI.transform.DOScale(moneyUI.transform.localScale * 3f, .06f);
         
         moneyUI.GetComponent<RectTransform>().DOAnchorPos(coinMovePos.GetComponent<RectTransform>().anchoredPosition, 0.75f).SetEase(Ease.OutBack).OnComplete(
            () =>
            {
               moneyUI.gameObject.SetActive(false);
            });
      }
   }
}
