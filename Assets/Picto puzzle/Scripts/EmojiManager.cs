using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    
    [Header("Hint Details")]
    public Button hintButton;
    public GameObject hintButtonDummy;
    [Header("50/50 Details")]
    public Button button5050;
    public GameObject button5050Dummy;
    [Header("Shuffle")] 
    public Button shuffle;
    public GameObject shuffleDummy;
    [Header("Bar Filling Details")] 
    public Image bar;
    public Image circle1;
    public Image circle2;
    public Image circle3;

    [Header("Win And Lose Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    

    // public Button
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //PanelInstanceFun(GetListNumbers());
        
        if (GetListNumbers() > emojiDataScript.detailsLists.Count-1)
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
        if (GetPanelsDone() == 3)
        {
            SetPanelsDone(0);
        }
        
        BarFilling();
        //SetPanelsDone(GetPanelsDone() + 1);
    }

    ///////----- panels and list updates-----------
    public void PanelAndListUpdate()
    {
        if (GetPanelsDone() != 2)
        {
            SetListNumber(GetListNumbers() + 1);
            SetPanelsDone(GetPanelsDone() + 1);
            BarFilling();
            if (GetListNumbers() > emojiDataScript.detailsLists.Count-2)
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
            winPanel.SetActive(true);
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
            
        panelObj = Instantiate(emojiDataScript.detailsLists[panelCount].panel, emojiDataScript.detailsLists[panelCount].panel.GetComponent<RectTransform>().position, 
            emojiDataScript.detailsLists[panelCount].panel.GetComponent<RectTransform>().rotation);
        
        if (panelObj.GetComponent<EmojiClick>().wrongList.Count > 0)
            panelObj.GetComponent<EmojiClick>().wrongList.Clear();
        if(panelObj.GetComponent<EmojiClick>().correctList.Count > 0)
            panelObj.GetComponent<EmojiClick>().correctList.Clear();
        panelObj.transform.parent = parentObj.transform;
        
        
        panelObj.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,0,0);
        panelObj.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom,0,0);
        
        panelObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        panelObj.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
        panelObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        StartCoroutine(StartFunCall());
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator StartFunCall()
    {
        yield return new WaitForSeconds(0.4f);
        PanelDataAssign(panelObj , listNumber);
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
            () =>
            {
                shuffle.interactable = true;
            });
        hintButton.transform.DOLocalMove(hintButtonDummy.transform.localPosition, 0.25f).OnComplete(
            () =>
            {
                hintButton.interactable = true;
            });
        button5050.transform.DOLocalMove(button5050Dummy.transform.localPosition, 0.25f).OnComplete(
            () =>
            {
                button5050.interactable = true;
            });
    }
    public void PanelDataAssign(GameObject panelObj,int listNumber)
    {
        //print("Panel detect");
        var panelObject = panelObj.GetComponent<EmojiClick>();
        
        panelObject.options = emojiDataScript.detailsLists[listNumber].options.ToList();
        panelObject.correctList = emojiDataScript.detailsLists[listNumber].correctList.ToList();
        panelObject.wrongColorlist = emojiDataScript.detailsLists[listNumber].wrongColorList.ToList();
        panelObject.emojiName.text = emojiDataScript.detailsLists[listNumber].emojiName;
        panelObject.winCount = emojiDataScript.detailsLists[listNumber].correctList.ToList().Count;
        panelObject.StartFun();
        
    }
    public void FunShuffle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void FunHint()
    {
        hintButton.interactable = false;
        for (int i = 0; i < panelObj.GetComponent<EmojiClick>().hintPos.Count; i++)
        {
            var temp = panelObj.GetComponent<EmojiClick>().hintPos[i].GetComponent<Image>();
            temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, .5f);
            temp.enabled = true;
            temp.sprite = panelObj.GetComponent<EmojiClick>().correctList[i];
            temp.rectTransform.DOScale(1.3f, 0.25f).SetEase(Ease.Linear)
                .SetLoops(2, LoopType.Yoyo);
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
    }

    public void NextButton()
    {
        UIManagerScript.Instance.NextMoveFun();
        
        SetListNumber(GetListNumbers() + 1);
        SetPanelsDone(GetPanelsDone() + 1);
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void BarFilling()
    {
        if (GetPanelsDone() == 1 && GetPanelsDone() != _previousValue)
        {
            circle1.DOFillAmount(1, 0.015f);
        }
        else if (GetPanelsDone() == 2 && GetPanelsDone() != _previousValue)
        {
            circle1.fillAmount = 1;
            bar.DOFillAmount(0.5f, 0.05f).OnComplete(() =>
            {
                circle2.DOFillAmount(1, 0.015f);
            });
        }
        else /*if(GetPanelsDone() == 0)*/
        {
            bar.DOFillAmount(0, 0.005f);
            circle1.DOFillAmount(0, 0.005f);
            circle2.DOFillAmount(0, 0.005f);
            circle3.DOFillAmount(0, 0.005f);
        }
        /*else
        {
            circle1.fillAmount = 1;
            circle2.fillAmount = 1;
            bar.DOFillAmount(1f, 0.05f).OnComplete(() =>
            {
                circle3.DOFillAmount(1, 0.015f);
            });
        }*/
        _previousValue = GetPanelsDone();
    }

    private int _previousValue;
    
    public  int GetPanelsDone() => PlayerPrefs.GetInt("Panels Done", 0);
    public  void SetPanelsDone(int num) => PlayerPrefs.SetInt("Panels Done", num);
    
    public  int GetListNumbers() => PlayerPrefs.GetInt("Lists Number", 0);
    public  void SetListNumber(int val) => PlayerPrefs.SetInt("Lists Number", val);
    
}
