using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Coffee.UIExtensions;
using DDZ;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
   public GameObject bubble2XParent;
   public GameObject hintBubble;
  // public GameObject magnetBubble;
   public GameObject instanceObj;
   public Sprite hintSprite;
   public Sprite magnetSprite;
   public GameObject hintMovePos;
   public GameObject magnetMovePos;
   public RectTransform[] bubbleWayPoints;
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
      giftImage.GetComponent<Button>().interactable = GameEssentials.IsRvAvailable();
      BubbleCheck();
   }

   public void MagnetOrHintDecider()
   {
      var num = Random.value;
      if (num >= 0.5f)
      {
         _powerName = "Magnet";
         //_powerObj = magnetBubble;
         hintBubble.GetComponent<Animator>().SetTrigger("Magnet");
      }
      else
      {
         _powerName = "Hint";
         //_powerObj = hintBubble;
         ///--hint
      }
   }
   private void BubbleCheck()
   {
      if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1) return;
      
     // bubbleTimeLimit -= Time.deltaTime;
      if (bubbleTimeLimit <= 0 && UIManagerScript.Instance.GetSpecialLevelNumber() > 5)
      {
         GameEssentials.instance.bubbleTime = hintBubble.GetComponent<Image>().enabled ? 30f : 120f;
         hintBubble.GetComponent<Image>().enabled = true;
         MagnetOrHintDecider();
         BubbleMovement();
         
         rvIconBubble.gameObject.SetActive(GameEssentials.IsRvAvailable());
         loadingBubble.gameObject.SetActive(!GameEssentials.IsRvAvailable());
      }
   }
   public Image rvIconBubble,loadingBubble;
   public float bubbleTimeLimit => GameEssentials.instance.bubbleTime;
   public int bubbleWayPointIndex ;
   private void BubbleMovement()
   {
      if (!hintBubble.GetComponent<Image>().enabled) return;
      
      hintBubble.GetComponent<Image>().rectTransform.DOAnchorPos(bubbleWayPoints[bubbleWayPointIndex].anchoredPosition, 3f)
         .SetEase(Ease.Flash).OnComplete(() =>
         {
            bubbleWayPointIndex++;
            bubbleWayPointIndex = bubbleWayPointIndex >= bubbleWayPoints.Length ? 0 : bubbleWayPointIndex;
            BubbleMovement();
         });
   }

   private void OnBubbleBtnPress()
   {
      GameEssentials.instance.bubbleTime =UIManagerScript.Instance.GetSpecialLevelNumber() < 31 ? 60 : 80;
      GameEssentials.RvType = RewardType.BubbleRv;
      GameEssentials.ShowRewardedAds("Bubble2X");
   }
   public void BubbleFunHit()
   {
      if (!GameEssentials.IsRvAvailable()) return;
      if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
      if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
      OnBubbleBtnPress();
      //Bubble2X_CallBack();
   }

   //private static GameObject _powerObj;
   private string _powerName;
   public void Bubble2X_CallBack()
   {
      hintBubble.GetComponent<Image>().enabled = false;
      rvIconBubble.gameObject.SetActive(false);
      loadingBubble.gameObject.SetActive(false);
      hintBubble.transform.GetChild(0).GetComponent<UIParticle>().Play();
      var obj = Instantiate(instanceObj, Vector3.zero, instanceObj.transform.rotation);
            print("BubbleCallback ::" + _powerName);
      switch (_powerName)
      {
         
         case "Magnet":
            print("BubbleCallback Magnet");
           
            obj.GetComponent<Image>().sprite = magnetSprite;
            obj.transform.parent = hintBubble.transform.parent;
            obj.transform.GetComponent<RectTransform>().anchoredPosition =
               hintBubble.GetComponent<RectTransform>().anchoredPosition;
            BubblePopFun(obj, magnetMovePos, 200);
            // BubblePopFun(powerObj.transform.GetChild(1))
            break;
         case "Hint":
            print("BubbleCallback Hint");
            obj.GetComponent<Image>().sprite = hintSprite;
            obj.transform.parent = hintBubble.transform.parent;
            obj.transform.GetComponent<RectTransform>().anchoredPosition =
               hintBubble.GetComponent<RectTransform>().anchoredPosition;
            BubblePopFun(obj, hintMovePos, 100);
            break;
         default:
            break;
      }
      
   }
   
    public void BubblePopFun(GameObject popObj2,GameObject popPosition2,int coinsCount)
    {
       // if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("RewardOpen");
       CoinManager.instance.CoinsIncrease(coinsCount);
       popObj2.GetComponent<RectTransform>().DOScale(Vector3.one, 0.45f).SetEase(Ease.OutBounce);
       popObj2.GetComponent<RectTransform>()
          .DOJumpAnchorPos(popPosition2.GetComponent<RectTransform>().anchoredPosition, 400f, 1, 0.5f)
          .SetEase(Ease.Linear).OnComplete(() =>
          {
             popObj2.transform.GetChild(0).GetComponent<UIParticle>().Play();
             DOVirtual.DelayedCall(0.25f, () =>
             {
                popObj2.GetComponent<RectTransform>().localScale = Vector3.zero;
                hintBubble.GetComponent<RectTransform>().anchoredPosition=bubbleWayPoints[0].anchoredPosition;
                bubbleWayPointIndex = 0;
             },false);
          });
    }
   public void GiftButtonFun()
   {
      if (!GameEssentials.IsRvAvailable()) return;
      if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
      if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
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
      GameObject obj=Instantiate(instanceObj, instancePos.transform.position,instanceObj.transform.rotation);
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
            },false);
         });
   }
   // ReSharper disable Unity.PerformanceAnalysis
   IEnumerator UpdateMoneyOnWin(GameObject instanceObj,GameObject instancePos,GameObject movePosition,int coinIncreaseNumber)
   {
      DOVirtual.DelayedCall(2f, () =>
      {
         CoinManager.instance.CoinsIncrease(coinIncreaseNumber);
      },false);
      for (int i = 0; i < 10; i++)
      {
         GameObject obj = Instantiate(instanceObj, instancePos.transform.position, instancePos.transform.rotation);
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
