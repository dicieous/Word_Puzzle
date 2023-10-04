using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

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

    [Header("Hint Details")] public Button hintButton;
    public GameObject hintButtonDummy;
    public TextMeshProUGUI hintNumberText;
    private int hintCounter;
    [Header("50/50 Details")] public Button button5050;
    public GameObject button5050Dummy;
    [Header("Shuffle")] public Button shuffle;
    public GameObject shuffleDummy;
    [Header("Bar Filling Details")] public Image bar;
    public Image circle1;
    public Image circle2;
    public Image circle3;
    public Image circle4;
    public Image circle5;

    [Header("next and retry")] public Button nextButton;
    public Button retryButton;

    [Header("Win And Lose Panels")] public GameObject winPanel;
    public GameObject losePanel;

    public UIParticle popperBlast;

    public TextMeshProUGUI tutorialText;


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

        CoinManager.instance.coinCountText.text = CoinManager.instance.GetCoinsCount().ToString();
        CoinManager.instance.hintText.text = CoinManager.instance.GetHintCount().ToString();
        if (CoinManager.instance.GetHintCount() > 0)
            hintButton.interactable = true;
        else 
            hintButton.interactable = false;

        BarFilling();
        //SetPanelsDone(GetPanelsDone() + 1);
    }

    private void Update()
    {
        /*if (CoinManager.instance.GetHintCount() == 0 && hintButton.interactable)
        {
            hintButton.interactable = false;
            //hintNumberText.text = CoinManager.instance.GetHintCount().ToString();
        }
        else if (CoinManager.instance.GetHintCount() > 0 && !hintButton.interactable)
        {
            hintButton.interactable = true;
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
            DOVirtual.DelayedCall(1f, () => { if(UIManagerScript.Instance) UIManagerScript.Instance.WinPanelActive(); });
        }

        ButtonsDisable();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void PanelInstanceFun(int panelCount)
    {
        /*if (panelObj != null)
        {
            panelObj = null;
        }*/

        if (previousPanelObj != null)
        {
            previousPanelObj = panelObj;
            //previousPanelObj.SetActive(false);
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
        });
    }

    public void ButtonsDisable()
    {
        hintButton.interactable = false;
        shuffle.interactable = false;
        button5050.interactable = false;
    }

    public void OneBYOneFun()
    {
        shuffle.transform.DOLocalMove(shuffleDummy.transform.localPosition, 0.25f).OnComplete(
            () => { shuffle.interactable = true; });
        hintButton.transform.DOLocalMove(hintButtonDummy.transform.localPosition, 0.25f).OnComplete(
            () => {if(CoinManager.instance.GetHintCount() > 0) hintButton.interactable = true; });
        button5050.transform.DOLocalMove(button5050Dummy.transform.localPosition, 0.25f).OnComplete(
            () => { button5050.interactable = true; });
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
        if (panelObj.GetComponent<EmojiClick>().options.Count <= 5)
        {
            button5050.interactable = false;
            button5050.GetComponent<Image>().enabled = false;
        }
        else
        {
            button5050.interactable = true;
            button5050.gameObject.GetComponent<Image>().enabled = true;
        }

        if (GetListNumbers() > 2)
        {
            tutorialText.enabled = false;
        }
        else
        {
            tutorialText.enabled = true;
        }
        /*if (GetListNumbers() == 0 || GetListNumbers() == 1)
        {
            tutorialText.gameObject.SetActive(true);
            FunHint();
            /*DOVirtual.DelayedCall(2f, () =>
            {
                print("Hint call");
            });#1#
        }*/
    }

    public void FunShuffle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
    }

    public void FunHint()
    {
        //print("Hint create");
        hintCounter++;
        ByteBrewManager.instance.EmojiEvent(UIManagerScript.Instance.GetSpecialLevelNumber().ToString(),
            hintCounter.ToString(), "EmojiMode", GetListNumbers().ToString(), "Hints");
        if (CoinManager.instance.GetHintCount() > 0)
        {
            /*SetHintCount(GetHintCount()-1);
            SetCoinCount(GetCoinsCount()-20);
            coinCountText.text = GetCoinsCount().ToString();
            hintText.text = GetHintCount().ToString();*/
            for (int i = 0; i < panelObj.GetComponent<EmojiClick>().hintPos.Count; i++)
            {
                var temp = panelObj.GetComponent<EmojiClick>().hintPos[i].GetComponent<Image>();
                temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, .65f);
                temp.enabled = true;
                temp.sprite = panelObj.GetComponent<EmojiClick>().correctList[i];
                temp.rectTransform.DOScale(1.3f, 0.25f).SetEase(Ease.Linear)
                    .SetLoops(2, LoopType.Yoyo);
            }

            //print("Function decall");
            hintButton.interactable = false;
            CoinManager.instance.SetHintCount(CoinManager.instance.GetHintCount() - 1);
            CoinManager.instance.SetCoinCount(CoinManager.instance.GetCoinsCount() - 20);
            //CoinManager.instance.SetHintCount((int)( CoinManager.instance.GetCoinsCount()/20));
            CoinManager.instance.coinCountText.text = CoinManager.instance.GetCoinsCount().ToString();
            CoinManager.instance.hintText.text = CoinManager.instance.GetHintCount().ToString();
            /*if (CoinManager.instance.GetHintCount() == 0)
            {
                hintButton.interactable = false;
            }*/
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
        }
    }

    public void Fun5050()
    {
        /*var j = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        j.interactable = false;*/
        button5050.interactable = false;
        int num = (panelObj.GetComponent<EmojiClick>().optionBtn.Count / 2);
        for (int i = 0; i < num; i++)
        {
            panelObj.GetComponent<EmojiClick>().optionBtn.RemoveAt(i);
            panelObj.GetComponent<EmojiClick>().optionBtn[i].gameObject.SetActive(false);
        }

        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
    }

    public void NextButton()
    {
        UIManagerScript.Instance.NextMoveFun();
        SetListNumber(GetListNumbers() + 1);
        SetPanelsDone(GetPanelsDone() + 1);
        nextButton.interactable = false;
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        retryButton.interactable = false;
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
    }

    public void BarFilling()
    {
        if (GetPanelsDone() == 1 && GetPanelsDone() != _previousValue)
        {
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Coins");
            //if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
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

    public int GetListNumbers() => PlayerPrefs.GetInt("Lists Number", 50);
    public void SetListNumber(int val) => PlayerPrefs.SetInt("Lists Number", val);
}