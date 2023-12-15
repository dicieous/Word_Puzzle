using System;
using System.Collections;
using System.Linq;
using Coffee.UIExtensions;
using DDZ;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EmojiManager : MonoBehaviour
{
    public static EmojiManager Instance;

    public EmojiesData emojiDataScript;
    public int emojiListCount;
    public GameObject parentObj;

    [HideInInspector] public int listNumber;
    //public int count;
    //public List<int> listDataNum;

    public GameObject panelObj;
    [HideInInspector] public GameObject previousPanelObj;
    public int panelNumber;
    public Sprite crossMark;

    [Header("Hint Details")] 
    //public Button hintButton;
    public GameObject hintButtonDummy;
    public TextMeshProUGUI hintNumberText;
    private int hintCounter;
    [Header("50/50 Details")] 
    public Button button5050;
    public GameObject button5050Dummy;
    [Header("Shuffle")] 
    public Button shuffle;
    public GameObject shuffleDummy;
    [Header("Bar Filling Details")] public Image bar;
    public Image circle1;
    public Image circle2;
    public Image circle3;
    public Image circle4;
    public Image circle5;

    [Header("next and retry")] public Button nextButton;
    public Button retryButton;

    //[Header("Win And Lose Panels")] public GameObject winPanel;
    public GameObject losePanel;

    public UIParticle popperBlast;
    
    public GameObject hand1;
    public GameObject hand2;
    public TextMeshProUGUI tutorialText;
    public bool _levelCompletemain;

    private bool shuffleBo;
    
    private static int _shuffleCount = 1, _fiftyFiftyCount = 1, _specialHintCount = 1;
    // public Button
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //PanelInstanceFun(GetListNumbers());

        if (GetListNumbers() == 0)
        {
            tutorialText.gameObject.SetActive(true);
        }

        if (GetListNumbers() > emojiDataScript.detailsLists.Count - 1)
        {
            listNumber = UnityEngine.Random.Range(0, emojiDataScript.detailsLists.Count - 1);
            PanelInstanceFun(listNumber);
            //print(i);
        }
        else
        {
            listNumber = GetListNumbers();
            PanelInstanceFun(listNumber);
        }

        if (GetPanelsDone() == 5)
        {
            SetPanelsDone(0);
        }

        var coinsCount = CoinManager.instance.GetCoinsCount();
        CoinManager.instance.coinCountText.text = coinsCount.ToString();
        CoinManager.instance.hintText.text = ((int)(coinsCount/50)).ToString();
        CoinManager.instance.specialLevelHintText.text = ((int)(coinsCount/50)).ToString();
        CoinManager.instance.shuffleCountText.text = ((int)(coinsCount/25)).ToString();
        CoinManager.instance.fiftyFiftyCountText.text = ((int)(coinsCount/25)).ToString();
        HintButtonActiveFun();
        BarFilling();
        //SetPanelsDone(GetPanelsDone() + 1);
        
    }

    private void Update()
    {
        /*if (CoinManager.instance.GetCoinsCount() >= 25 && !shuffle.interactable && !_levelCompletemain)
        {
            shuffle.interactable = true;
            button5050.interactable = true;
            shuffleBo = true;
            //hintNumberText.text = ((int) (CoinManager.instance.GetHintCount() / 10f)).ToString();
        }
        else if (CoinManager.instance.GetCoinsCount() < 25 && shuffle.interactable && !_levelCompletemain)
        {
            shuffle.interactable = false;
            button5050.interactable = false;
            shuffleBo = false;
            //hintNumberText.text = CoinManager.instance.GetHintCount().ToString();
        }*/
    }

    ///////----- panels and list updates-----------
    // ReSharper disable Unity.PerformanceAnalysis
    public void PanelAndListUpdate()
    {
        if (GetPanelsDone() != 4)
        {
            SetListNumber(GetListNumbers() + 1);
            SetPanelsDone(GetPanelsDone() + 1);
            BarFilling();
            if (GetListNumbers() > emojiDataScript.detailsLists.Count - 2)
            {
                listNumber = UnityEngine.Random.Range(0, emojiDataScript.detailsLists.Count - 1);
                PanelInstanceFun(listNumber);
            }
            else
            {
                listNumber = GetListNumbers();
                PanelInstanceFun(GetListNumbers());
            }
        }
        else
        {
            BarFilling();
            popperBlast.Play();
            DOVirtual.DelayedCall(1f, () =>
            {
                if(UIManagerScript.Instance) UIManagerScript.Instance.WinPanelActive();
            },false);
        }

        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void PanelInstanceFun(int panelCount)
    {
        if (previousPanelObj != null)
        {
            previousPanelObj = panelObj;
            StartCoroutine(previousPanelObj.GetComponent<EmojiClick>().ParentAnimDecrese());
        }
        else
        {
            if (panelObj != null)
            {
                previousPanelObj = panelObj;
                //previousPanelObj.SetActive(false);
                StartCoroutine(previousPanelObj.GetComponent<EmojiClick>().ParentAnimDecrese());
            }
        }

        panelObj = Instantiate(emojiDataScript.detailsLists[panelCount].panel,
            emojiDataScript.detailsLists[panelCount].panel.GetComponent<RectTransform>().position,
            emojiDataScript.detailsLists[panelCount].panel.GetComponent<RectTransform>().rotation);

        if (GetListNumbers() == 0)
        {
            if(hand1.activeInHierarchy)
                hand1.SetActive(false);
            if(!hand1.activeInHierarchy)
                hand1.SetActive(true);
        }
        if (panelObj.GetComponent<EmojiClick>().wrongList.Count > 0)
            panelObj.GetComponent<EmojiClick>().wrongList.Clear();
        if (panelObj.GetComponent<EmojiClick>().correctList.Count > 0)
            panelObj.GetComponent<EmojiClick>().correctList.Clear();
        panelObj.transform.parent = parentObj.transform;

        panelObj.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        panelObj.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);

        panelObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        panelObj.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
        panelObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        StartCoroutine(StartFunCall());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator StartFunCall()
    {
        yield return new WaitForSeconds(0.4f);
        PanelDataAssign(panelObj, listNumber);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            OneBYOneFun();
            panelObj.GetComponent<EmojiClick>().levelStarted = true;
        },false);
    }

    public void ButtonsDisable()
    {
       // hintButton.interactable = false;
        //shuffle.interactable = false;
        //button5050.interactable = false;
    }

    public void OneBYOneFun()
    {
        UIManagerScript.Instance.shuffleButton.transform.DOLocalMove(shuffleDummy.transform.localPosition, 0.25f);
        UIManagerScript.Instance.specialHintButton.transform.DOLocalMove(hintButtonDummy.transform.localPosition, 0.25f);
        UIManagerScript.Instance.fiftyFiftyButton.transform.DOLocalMove(button5050Dummy.transform.localPosition, 0.25f);
        
    }

    public void PanelDataAssign(GameObject panelObj, int listNumber)
    {
        //print("Panel detect");
        var panelObject = panelObj.GetComponent<EmojiClick>();

        panelObject.options = emojiDataScript.detailsLists[listNumber].options.ToList();
        panelObject.correctList = emojiDataScript.detailsLists[listNumber].correctList.ToList();
        panelObject.wrongColorlist = emojiDataScript.detailsLists[listNumber].wrongColorList.ToList();
        panelObject.emojiName.text = emojiDataScript.detailsLists[listNumber].emojiName;
        panelObject.winCount = emojiDataScript.detailsLists[listNumber].correctList.ToList().Count;
        panelObject.StartFun();

        if (GetListNumbers() > 2)
        {
            tutorialText.enabled = false;
        }
        else
        {
            tutorialText.enabled = true;
        }

        _levelCompletemain = false;
        HintButtonActiveFun();
        ShuffleButtonActive();
        if (panelObj.GetComponent<EmojiClick>().options.Count <= 5)
        {
            //button5050.interactable = false;
            //FiftyFiftyButtonDeActiveFun();
            UIManagerScript.Instance.fiftyFiftyButton.GetComponent<Image>().gameObject.SetActive(false);
        }
        else
        {
            // button5050.interactable = true;
            FiftyFiftyButtonActiveFun();
            //UIManagerScript.Instance.fiftyFiftyButton.gameObject.GetComponent<Image>().enabled = true;
        }
    }

    public void FunShuffle()
    {
        if (CoinManager.instance.GetCoinsCount() >= 25)
        {
            CoinManager.instance.ShuffleReduce(25);
            Shuffle_CallBack();
        }
        else
        {
            GameEssentials.RvType = RewardType.Shuffle;
            GameEssentials.ShowRewardedAds("Shuffle");
            if(LionStudiosManager.instance)
                LionStudiosManager.AdsEvents(true, AdsEventState.Start,UIManagerScript.Instance.GetSpecialLevelNumber(),"Applovin","Shuffle",CoinManager.instance.GetCoinsCount());
            /////--------------addcall
        }
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
    }

    public void Shuffle_CallBack()
    {
        if(LionStudiosManager.instance)
        {
            LionStudiosManager.
                Shuffle(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),_shuffleCount);
            _shuffleCount++;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ShuffleButtonActive()
    {
        UIManagerScript.Instance.shuffleButton.interactable = true;
        var shuffleText = UIManagerScript.Instance.shuffleButton.image.rectTransform.GetChild(0).gameObject;
        var coinsTxt = UIManagerScript.Instance.shuffleButton.image.rectTransform.GetChild(1).gameObject;
        var rvIcon = UIManagerScript.Instance.shuffleButton.image.rectTransform.GetChild(2).gameObject;
        var loadingIcon = UIManagerScript.Instance.shuffleButton.image.rectTransform.GetChild(3).gameObject;
			
        shuffleText.SetActive(CoinManager.instance.GetCoinsCount()>=25);
        coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
        rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<25 && GameEssentials.IsRvAvailable());
        loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<25 && !GameEssentials.IsRvAvailable());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ShuffleButtonDeActiveFun()
    {
        UIManagerScript.Instance.shuffleButton.interactable = false;
        var shuffleText = UIManagerScript.Instance.shuffleButton.image.rectTransform.GetChild(0).gameObject;
        var coinsTxt = UIManagerScript.Instance.shuffleButton.image.rectTransform.GetChild(1).gameObject;
        var rvIcon = UIManagerScript.Instance.shuffleButton.image.rectTransform.GetChild(2).gameObject;
        var loadingIcon = UIManagerScript.Instance.shuffleButton.image.rectTransform.GetChild(3).gameObject;
			
        shuffleText.SetActive(CoinManager.instance.GetCoinsCount()>=25);
        coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
        rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<25 && GameEssentials.IsRvAvailable());
        loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<25 && !GameEssentials.IsRvAvailable());
        
    }
    [HideInInspector] public int hintDisplayNum;
    public void FunHint()
    {
        hintCounter++;
        /*ByteBrewManager.instance.EmojiEvent(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),
            hintCounter.ToString(), "EmojiMode", GetListNumbers().ToString(), "Hints");*/
        if (CoinManager.instance.GetCoinsCount() >= 50)
        {
            CoinManager.instance.HintReduce(50);
            SpecialHint_CallBack();
        }
        else
        {
            GameEssentials.RvType = RewardType.Hint;
            GameEssentials.ShowRewardedAds("Hint");
            if(LionStudiosManager.instance)
                LionStudiosManager.AdsEvents(true, AdsEventState.Start,UIManagerScript.Instance.GetSpecialLevelNumber(),"Applovin","Hint",CoinManager.instance.GetCoinsCount());
            /////----------------ad call
        }
        HintButtonDeActiveFun();
        //ShuffleButtonActive();
        //FiftyFiftyButtonActiveFun();
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
    }
    public void SpecialHint_CallBack()
    {
        if (panelObj.GetComponent<EmojiClick>().hintPos.Count <= 0) return;
        
        //var num = UnityEngine.Random.Range(0, panelObj.GetComponent<EmojiClick>().hintPos.Count);
        for (int i = 0; i < panelObj.GetComponent<EmojiClick>().hintPos.Count; i++)
        {
            if (panelObj.GetComponent<EmojiClick>().hintPos[i].GetComponent<Image>().gameObject
                .activeInHierarchy)
            {
                var temp = panelObj.GetComponent<EmojiClick>().hintPos[i].GetComponent<Image>();
                temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, .65f);
                temp.enabled = true;
                temp.rectTransform.DOScale(1.3f, 0.25f).SetEase(Ease.Linear)
                    .SetLoops(2, LoopType.Yoyo);
                hintDisplayNum = i;
                break;
            }
        }
        
        if(!LionStudiosManager.instance) return;
        LionStudiosManager.Hint(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),UIManagerScript._hintCount.ToString());
        UIManagerScript._hintCount++;
    }
    public void HintButtonActiveFun()
    {
        if (panelObj.GetComponent<EmojiClick>().hintPos.Count > 0)
        {
            for (int i = 0; i < panelObj.GetComponent<EmojiClick>().hintPos.Count; i++)
            {
                if (panelObj.GetComponent<EmojiClick>().hintPos[i].GetComponent<Image>().gameObject
                    .activeInHierarchy)
                {
                    UIManagerScript.Instance.specialHintButton.interactable = true;
                    break;
                }
                   
            }
        }
        var specialHintText = UIManagerScript.Instance.specialHintButton.image.rectTransform.GetChild(0).gameObject;
        var coinsTxt = UIManagerScript.Instance.specialHintButton.image.rectTransform.GetChild(1).gameObject;
        var rvIcon = UIManagerScript.Instance.specialHintButton.image.rectTransform.GetChild(2).gameObject;
        var loadingIcon = UIManagerScript.Instance.specialHintButton.image.rectTransform.GetChild(3).gameObject;
			
        specialHintText.SetActive(CoinManager.instance.GetCoinsCount()>=50);
        coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
        rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && GameEssentials.IsRvAvailable());
        loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<50 && !GameEssentials.IsRvAvailable());
    }

    public void HintButtonDeActiveFun()
    {
        UIManagerScript.Instance.specialHintButton.interactable = false;
        var specialHintText = UIManagerScript.Instance.specialHintButton.image.rectTransform.GetChild(0).gameObject;
        var coinsTxt = UIManagerScript.Instance.specialHintButton.image.rectTransform.GetChild(1).gameObject;
        var rvIcon = UIManagerScript.Instance.specialHintButton.image.rectTransform.GetChild(2).gameObject;
        var loadingIcon = UIManagerScript.Instance.specialHintButton.image.rectTransform.GetChild(3).gameObject;
			
        specialHintText.SetActive(CoinManager.instance.GetCoinsCount()>=100);
        coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
        rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<100 && GameEssentials.IsRvAvailable());
        loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<100 && !GameEssentials.IsRvAvailable());
    }

    //private bool _fiftyFifty;
    public void Fun5050()
    {
        if (CoinManager.instance.GetCoinsCount() >= 25)
        {
            CoinManager.instance.FiftyFiftyReduce(25);
            FiftyFifty_CallBack();
        }
        else
        {
            /////--------add call
            GameEssentials.RvType = RewardType.FiftyFifty;
            GameEssentials.ShowRewardedAds("FiftyFifty");
            if(LionStudiosManager.instance)
                LionStudiosManager.AdsEvents(true, AdsEventState.Start,UIManagerScript.Instance.GetSpecialLevelNumber(),"Applovin","FiftyFifty",CoinManager.instance.GetCoinsCount());
        }

        //_fiftyFifty = true;
        //FiftyFiftyButtonDeActiveFun();
        //ShuffleButtonActive();
        //HintButtonDeActiveFun();
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
    }

    public void FiftyFifty_CallBack()
    {
        FiftyFiftyButtonDeActiveFun();
        CoinManager.instance.FiftyFiftyReduce(25);
        int num = (panelObj.GetComponent<EmojiClick>().optionBtn.Count / 2);
        for (int i = 0; i < num; i++)
        {
            panelObj.GetComponent<EmojiClick>().optionBtn.RemoveAt(i);
            panelObj.GetComponent<EmojiClick>().optionBtn[i].GetComponent<Button>().interactable = false;
            panelObj.GetComponent<EmojiClick>().optionBtn[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
            panelObj.GetComponent<EmojiClick>().optionBtn[i].GetComponent<Image>().raycastTarget = false;
            //panelObj.GetComponent<EmojiClick>().optionBtn[i].gameObject.SetActive(false);
        }
        
        if(!LionStudiosManager.instance) return;
        LionStudiosManager.FiftyFifty(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),_fiftyFiftyCount);
        _fiftyFiftyCount++;
    }
    public void FiftyFiftyButtonActiveFun()
    {
        UIManagerScript.Instance.fiftyFiftyButton.interactable = true;
        var fun5050Text = UIManagerScript.Instance.fiftyFiftyButton.image.rectTransform.GetChild(0).gameObject;
        var coinsTxt = UIManagerScript.Instance.fiftyFiftyButton.image.rectTransform.GetChild(1).gameObject;
        var rvIcon = UIManagerScript.Instance.fiftyFiftyButton.image.rectTransform.GetChild(2).gameObject;
        var loadingIcon = UIManagerScript.Instance.fiftyFiftyButton.image.rectTransform.GetChild(3).gameObject;
			
        fun5050Text.SetActive(CoinManager.instance.GetCoinsCount()>=25);
        coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
        rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<25 && GameEssentials.IsRvAvailable());
        loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<25 && !GameEssentials.IsRvAvailable());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void FiftyFiftyButtonDeActiveFun()
    {
        UIManagerScript.Instance.fiftyFiftyButton.interactable = false;
        var fun5050Text = UIManagerScript.Instance.fiftyFiftyButton.image.rectTransform.GetChild(0).gameObject;
        var coinsTxt = UIManagerScript.Instance.fiftyFiftyButton.image.rectTransform.GetChild(1).gameObject;
        var rvIcon = UIManagerScript.Instance.fiftyFiftyButton.image.rectTransform.GetChild(2).gameObject;
        var loadingIcon = UIManagerScript.Instance.fiftyFiftyButton.image.rectTransform.GetChild(3).gameObject;
			
        fun5050Text.SetActive(CoinManager.instance.GetCoinsCount()>=25);
        coinsTxt.SetActive(CoinManager.instance.GetCoinsCount()>=100);
			
        rvIcon.SetActive(CoinManager.instance.GetCoinsCount()<25 && GameEssentials.IsRvAvailable());
        loadingIcon.SetActive(CoinManager.instance.GetCoinsCount()<25 && !GameEssentials.IsRvAvailable());
    }
   
    
    public void NextButton()
    {
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
        //UIManagerScript.Instance.NextMoveFun();
        SetListNumber(GetListNumbers() + 1);
        SetPanelsDone(GetPanelsDone() + 1);
        nextButton.interactable = false;
    }

    public void RetryButton()
    {
        retryButton.interactable = false;
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BarFilling()
    {
        if (GetPanelsDone() == 1 && GetPanelsDone() != _previousValue)
        {
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
            circle1.DOFillAmount(1, 0.015f).OnComplete(() =>
            {
                circle1.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            });
        }
        else if (GetPanelsDone() == 2 && GetPanelsDone() != _previousValue)
        {
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
            circle1.fillAmount = 1;
            circle1.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            bar.DOFillAmount(0.25f, 0.05f).OnComplete(() =>
            {
                circle2.DOFillAmount(1, 0.015f).OnComplete(() =>
                {
                    circle2.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                });
            });
        }
        else if (GetPanelsDone() == 3 && GetPanelsDone() != _previousValue)
        {
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
            circle1.fillAmount = 1;
            circle2.fillAmount = 1;
            circle1.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            circle2.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            bar.DOFillAmount(0.5f, 0.05f).OnComplete(() =>
            {
                circle3.DOFillAmount(1, 0.015f).OnComplete(() =>
                {
                    circle3.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                });
            });
        }
        else if (GetPanelsDone() == 4 && GetPanelsDone() != _previousValue)
        {
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
            circle1.fillAmount = 1;
            circle2.fillAmount = 1;
            circle3.fillAmount = 1;
            circle1.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            circle2.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            circle3.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            bar.DOFillAmount(0.75f, 0.05f).OnComplete(() =>
            {
                circle4.DOFillAmount(1, 0.015f).OnComplete(() =>
                {
                    circle4.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                });
            });
        }
        else if (GetPanelsDone() == 0)
        {
            bar.DOFillAmount(0, 0.005f);
            circle1.DOFillAmount(0, 0.005f);
            circle1.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            circle2.DOFillAmount(0, 0.005f);
            circle2.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            circle3.DOFillAmount(0, 0.005f);
            circle3.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            circle4.DOFillAmount(0, 0.005f);
            circle4.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            circle5.DOFillAmount(0, 0.005f);
            circle5.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
            circle1.fillAmount = 1;
            circle2.fillAmount = 1;
            circle3.fillAmount = 1;
            circle4.fillAmount = 1;
            circle1.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            circle2.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            circle3.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            circle4.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            bar.DOFillAmount(1f, 0.05f).OnComplete(() =>
            {
                circle5.DOFillAmount(1, 0.015f).OnComplete(() =>
                {
                    circle5.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                });
            });
        }

        _previousValue = GetPanelsDone();
    }

    private int _previousValue;


    public int GetPanelsDone() => PlayerPrefs.GetInt("Panels Done", 0);
    public void SetPanelsDone(int num) => PlayerPrefs.SetInt("Panels Done", num);

    public int GetListNumbers() => PlayerPrefs.GetInt("Lists Number", 0);
    public void SetListNumber(int val) => PlayerPrefs.SetInt("Lists Number", val);
}