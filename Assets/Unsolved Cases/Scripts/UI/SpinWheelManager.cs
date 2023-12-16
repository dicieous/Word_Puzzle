
using System.Collections;

using Coffee.UIExtensions;
using DDZ;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SpinWheelManager : SingletonInstance<SpinWheelManager>
{
    public GameObject spinWheelPanel;
    public Button spinWheelBtn;
  
    public Image dummyObj , raySpr;
    public Sprite hintSpr, spinWheelSpr, magnetSpr, coinSpr;

    public RectTransform hintPos, magnetPos, coinPos, spinPos;

    // Spin Wheel
    public TMP_Text remainingTxt;
    public Button stopBtn, closeBtn;
    public RectTransform spinBoard, pin, glow;
    public AudioSource tickingSound;
    public Image smiley;
    public GameObject coinEffect, hintEffect, magnetEffect;
    
    [SerializeField] private float speed, pinSpeed, audioSpeed;
    [SerializeField] private float tickFrequency = 0.5f;
    [SerializeField]private int totalSpinWheel;
    private bool _spinNow, _stopPressed, _stopTickSound, startSpin;
    
    private Image StopRvIcon => stopBtn.transform.GetChild(0).GetComponent<Image>();
    private Image StopLoading => stopBtn.transform.GetChild(1).GetComponent<Image>();
    
    private void Update()
    {
        CheckSpinWheelRvButtons();
        if (startSpin)
        {
            if (!_spinNow) return;
            spinBoard.Rotate(new Vector3(0, 0, 10f) * (speed * Time.deltaTime));
            glow.Rotate(new Vector3(0, 0, -5f) * (speed * Time.deltaTime));
            TickSoundHandler();
        }
    }

    public void SpinTheWheel()
    {
        if (totalSpinWheel <= 0)
        {
            //ad call and event
            GameEssentials.RvType = RewardType.SpinWheel;
            GameEssentials.ShowRewardedAds("SpinWheel");
            return;
        }

        StopTheSpin_Callback();
    }
    
    private void TickSoundHandler()
    {
        if (!_stopTickSound || !spinWheelPanel.activeInHierarchy) return;
        audioSpeed -= Time.deltaTime;
        if (!(audioSpeed <= 0)) return;
        
        if (!_stopPressed)
        {
            tickFrequency -= 0.1f;
            tickFrequency = tickFrequency <= 0.15f ? 0.15f : tickFrequency;
        }
        else
        {
            tickFrequency += 0.1f;
            tickFrequency = tickFrequency >= 0.5f ? 0.5f : tickFrequency;
            if (tickFrequency >= 0.5f) _stopTickSound = false;
        }
        audioSpeed = tickFrequency;
        tickingSound.Play();
    }

    private void PinRotate()
    {
        pinSpeed = 0.15f;
        pin.localEulerAngles = new Vector3(0, 0, 15f);
        pin.DORotate(new Vector3(0, 0, -15f), pinSpeed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    // public void StopTheSpin()
    // {
    //     if (totalSpinWheel <= 0)
    //     {
    //         //ad call and event
    //         GameEssentials.RvType = RewardType.SpinWheel;
    //         GameEssentials.ShowRewardedAds("SpinWheel");
    //         return;
    //     }
    //     StopTheSpin_Callback();
    //     // GameEssentials.instance.shm.PlayAudio("Find");
    // }

    public void StopTheSpin_Callback()
    {
        startSpin = true;
        stopBtn.interactable = false;
        closeBtn.interactable = false;
        PinRotate();
        DOVirtual.DelayedCall(3.5f, StopSpin);
    }

    private void StopSpin()
    {
        totalSpinWheel--;
        _stopPressed = true;
        stopBtn.interactable = false;
        closeBtn.interactable = false;
        totalSpinWheel = totalSpinWheel <= 0 ? 0 : totalSpinWheel;
        remainingTxt.text = "Remaining Spins : " + totalSpinWheel;
        stopBtn.image.rectTransform.GetChild(0).gameObject.SetActive(totalSpinWheel <= 0);
        //PinRotate();
        pin.DOKill();
        pinSpeed = 0.25f;
        audioSpeed = 0.5f;
        pin.DORotate(new Vector3(0, 0, -15f), pinSpeed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        DOTween.To(() => speed, x => speed = x, 0, 3).SetEase(Ease.OutSine).OnComplete(() =>
        {
            pin.DOKill();
            pin.localEulerAngles = Vector3.zero;
            StartCoroutine(GiveTheReward());
            startSpin = false;
            DOVirtual.DelayedCall(1, (() =>
            {
                stopBtn.interactable = true;
                closeBtn.interactable = true;
            }));
        });
    }

    private IEnumerator GiveTheReward()
    {
        RewardBasedOnRotation(spinBoard.localEulerAngles.z);
        // Give Reward
        yield return new WaitForSeconds(3f);
         _stopPressed = false;
         _stopTickSound = true;
        DOTween.To(() => speed, x => speed = x, 35, 0.5f).OnComplete(() => { stopBtn.interactable = true; });
       // PinRotate();
    }

    private void RewardBasedOnRotation(float val)
    { 
        if (val > 0 && val < -180f)
        {
            val = 360 + val;
        }

        switch (val)
        {
            case >= 337 and <= 359:
            case >= 0 and <= 22f:
                print("250 Coins");
                dummyObj.sprite = coinSpr;
                dummyObj.gameObject.SetActive(true);
                dummyObj.gameObject.transform.DOScale(1, 1);
                raySpr.gameObject.transform.DOScale(1, 1);
                dummyObj.GetComponent<AudioSource>().Play();
                DOVirtual.DelayedCall(2, (() =>
                {
                    dummyObj.rectTransform.DOAnchorPos(coinPos.anchoredPosition, 1.25f).OnComplete(() =>
                    {
                        closeBtn.interactable = true;
                        dummyObj.rectTransform.anchoredPosition = Vector2.zero;
                        dummyObj.gameObject.SetActive(false);
                        raySpr.gameObject.transform.DOScale(0, 1);
                        GameEssentials.VibrationOrHaptic( 13);
                        CoinManager.instance.CoinsIncrease(250);
                        coinEffect.GetComponent<ParticleSystem>().Play();
                        DOVirtual.DelayedCall(0.5f, (() =>
                        {
                            coinEffect.GetComponent<ParticleSystem>().Stop();
                        }));
                    });
                }));
                break;
            case >= 23f and <= 66f:
                print("Extra Spin Wheel");
                dummyObj.sprite = spinWheelSpr;
                dummyObj.gameObject.SetActive(true);
                dummyObj.GetComponent<AudioSource>().Play();
                dummyObj.rectTransform.DOAnchorPos(spinPos.anchoredPosition, 1.5f).OnComplete(() =>
                {
                    closeBtn.interactable = true;
                    dummyObj.rectTransform.anchoredPosition = Vector2.zero;
                    dummyObj.gameObject.SetActive(false);
                    totalSpinWheel += 1;
                    remainingTxt.text = "Remaining Spins : " + totalSpinWheel;
                    stopBtn.image.rectTransform.GetChild(0).gameObject.SetActive(totalSpinWheel <= 0);
                    // dataManager.SetTotalSpinWheel(1);
                    remainingTxt.rectTransform.GetChild(0).GetComponent<UIParticle>().Play();
                    remainingTxt.GetComponent<AudioSource>().Play();
                    GameEssentials.VibrationOrHaptic( 13);
                    
                });
                break;
            case >= 67f and <= 111f:
                print("2 Hints");
                dummyObj.sprite = hintSpr;
                dummyObj.gameObject.SetActive(true);
                dummyObj.gameObject.transform.DOScale(1, 1);
                raySpr.gameObject.transform.DOScale(1, 1);
                dummyObj.GetComponent<AudioSource>().Play();
                DOVirtual.DelayedCall(2, (() =>
                {
                    dummyObj.rectTransform.DOAnchorPos(hintPos.anchoredPosition, 1.25f).OnComplete(() =>
                    {
                        closeBtn.interactable = true;
                        dummyObj.rectTransform.anchoredPosition = Vector2.zero;
                        dummyObj.gameObject.SetActive(false);
                        raySpr.gameObject.transform.DOScale(0, 1);
                        GameEssentials.VibrationOrHaptic( 13);
                        CoinManager.instance.CoinsIncrease(100);
                        hintEffect.GetComponent<ParticleSystem>().Play();
                        DOVirtual.DelayedCall(0.5f, (() =>
                        {
                            hintEffect.GetComponent<ParticleSystem>().Stop();
                        }));
                    });
                }));
                break;
            case >= 112f and <= 157f:
                print("100 coins");
                dummyObj.sprite = coinSpr;
                dummyObj.gameObject.SetActive(true);
                dummyObj.gameObject.transform.DOScale(1, 1);
                raySpr.gameObject.transform.DOScale(1, 1);
                dummyObj.GetComponent<AudioSource>().Play();
                DOVirtual.DelayedCall(2, (() =>
                {
                    dummyObj.rectTransform.DOAnchorPos(coinPos.anchoredPosition, 1.25f).OnComplete(() =>
                    {
                        closeBtn.interactable = true;
                        dummyObj.rectTransform.anchoredPosition = Vector2.zero;
                        dummyObj.gameObject.SetActive(false);
                        raySpr.gameObject.transform.DOScale(0, 1);
                        GameEssentials.VibrationOrHaptic( 13);
                        CoinManager.instance.CoinsIncrease(100);
                        coinEffect.GetComponent<ParticleSystem>().Play();
                        DOVirtual.DelayedCall(0.5f, (() =>
                        {
                            coinEffect.GetComponent<ParticleSystem>().Stop();
                        }));
                    });
                }));
                break;
            case >= 158f and <= 201f:
                print("2 magnet");
                dummyObj.sprite = magnetSpr;
                dummyObj.gameObject.SetActive(true);
                dummyObj.gameObject.transform.DOScale(1, 1);
                raySpr.gameObject.transform.DOScale(1, 1);
                dummyObj.GetComponent<AudioSource>().Play();
                DOVirtual.DelayedCall(2, (() =>
                {
                    dummyObj.rectTransform.DOAnchorPos(magnetPos.anchoredPosition, 1.25f).OnComplete(() =>
                    {
                        closeBtn.interactable = true;
                        dummyObj.rectTransform.anchoredPosition = Vector2.zero;
                        dummyObj.gameObject.SetActive(false);
                        raySpr.gameObject.transform.DOScale(0, 1);
                        GameEssentials.VibrationOrHaptic( 13);
                        CoinManager.instance.CoinsIncrease(200);
                        magnetEffect.GetComponent<ParticleSystem>().Play();
                        DOVirtual.DelayedCall(0.5f, (() =>
                        {
                            magnetEffect.GetComponent<ParticleSystem>().Stop();;
                        }));
                    });
                }));
                break;
            case >= 202f and <= 246f:
                print("200 coins");
                dummyObj.sprite = coinSpr;
                dummyObj.gameObject.SetActive(true);
                dummyObj.gameObject.transform.DOScale(1, 1);
                raySpr.gameObject.transform.DOScale(1, 1);
                dummyObj.GetComponent<AudioSource>().Play();
                DOVirtual.DelayedCall(2, (() =>
                {
                    dummyObj.rectTransform.DOAnchorPos(coinPos.anchoredPosition, 1.25f).OnComplete(() =>
                    {
                        closeBtn.interactable = true;
                        dummyObj.rectTransform.anchoredPosition = Vector2.zero;
                        dummyObj.gameObject.SetActive(false);
                        raySpr.gameObject.transform.DOScale(0, 1);
                        GameEssentials.VibrationOrHaptic( 13);
                        CoinManager.instance.CoinsIncrease(200);
                        coinEffect.GetComponent<ParticleSystem>().Play();
                        DOVirtual.DelayedCall(0.5f, (() =>
                        {
                            coinEffect.GetComponent<ParticleSystem>().Stop();
                        }));
                    });
                }));
                break;
            case >=247 and <= 292f:
                print("1 hint");
                dummyObj.sprite = hintSpr;
                dummyObj.gameObject.SetActive(true);
                dummyObj.gameObject.transform.DOScale(1, 1);
                raySpr.gameObject.transform.DOScale(1, 1);
                dummyObj.GetComponent<AudioSource>().Play();
                DOVirtual.DelayedCall(2, (() =>
                {
                    dummyObj.rectTransform.DOAnchorPos(hintPos.anchoredPosition, 1.25f).OnComplete(() =>
                    {
                        closeBtn.interactable = true;
                        dummyObj.rectTransform.anchoredPosition = Vector2.zero;
                        dummyObj.gameObject.SetActive(false);
                        raySpr.gameObject.transform.DOScale(0, 1);
                        GameEssentials.VibrationOrHaptic( 13);
                        CoinManager.instance.CoinsIncrease(50);
                        hintEffect.GetComponent<ParticleSystem>().Play();
                        DOVirtual.DelayedCall(0.5f, (() =>
                        {
                            hintEffect.GetComponent<ParticleSystem>().Stop();
                        }));
                    });
                }));
                break;
            case >= 293f and <= 336f:
                print("Smiley");
                smiley.gameObject.SetActive(true);
                smiley.GetComponent<AudioSource>().Play();
                var da = smiley.GetComponent<DOTweenAnimation>();
                da.onComplete.AddListener(() => { da.DOPlayBackwards(); });
                da.DOPlayForward();
                break;
            default:
                print("default");
                break;
        }
    }

    public void CloseTheWheel()
    {
        stopBtn.interactable = false;
        closeBtn.interactable = false;
        spinWheelPanel.SetActive(false);
        spinWheelBtn.gameObject.SetActive(true);
    }

    public void OpenSpinWheel()
    {
        spinWheelPanel.SetActive(true);
        InitSpinWheel();
        stopBtn.interactable = true;
        closeBtn.interactable = true;
        spinWheelBtn.gameObject.SetActive(false);
    }

    private void InitSpinWheel()
    {
        //PinRotate();
        audioSpeed = tickFrequency;
        _spinNow = true;
        _stopTickSound = true;
        // _totalSpinWheel = GameEssentials.GetTotalSpinWheel();
        remainingTxt.text = "Remaining Spins : " + totalSpinWheel;
        stopBtn.image.rectTransform.GetChild(0).gameObject.SetActive(totalSpinWheel <= 0);
        DOTween.To(() => speed, x => speed = x, 35, 1f).SetEase(Ease.InOutQuad);
        DOTween.To(() => pinSpeed, x => pinSpeed = x, 0.1f, 1f).SetEase(Ease.InOutQuad);
    }
    
    private void CheckSpinWheelRvButtons()
    {
        if (!spinWheelPanel.activeInHierarchy) return;
        
        if (totalSpinWheel <= 0)
        {
            StopRvIcon.gameObject.SetActive(GameEssentials.IsRvAvailable());
            StopLoading.gameObject.SetActive(!GameEssentials.IsRvAvailable());
        }
        else
        {
            StopRvIcon.gameObject.SetActive(false);
            StopLoading.gameObject.SetActive(false);
        }
    }
}