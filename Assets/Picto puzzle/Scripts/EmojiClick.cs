using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class EmojiClick : MonoBehaviour
{
    public TextMeshProUGUI emojiName;
    public List<Sprite> options;
    public List<Sprite> correctList;
    public List<Image> optionBtn;
    public List<Sprite> wrongColorlist;
    public GameObject emojiIcon;
    public List<GameObject> wrongList;
    public List<Image> correctPos;
    public List<GameObject> correctCheckList;
    private bool _levelfail = false;
    public List<Image> wrongImage;
    private bool _levelComplete = false;
    public List<Image> hintPos;
    public int winCount;

    public int index;
    public Sprite crossMark;

    // Gamepanel Animation Components...
    public List<GameObject> gamePanel;
    public List<GameObject> parentGamePanel;

    public bool levelStarted;
    
    // Start is called before the first frame update

    public void StartFun()
    {
        foreach (var t in gamePanel)
        {
            t.transform.localScale = Vector3.zero;
        }
        options.Sort((a, b) => 1 - 2 * Random.Range(0, options.Count));
        for (int i = 0; i < options.Count; i++)
        {
            optionBtn[i].GetComponent<Image>().sprite = options[i];
            optionBtn[i].gameObject.tag = "Wrong";
        }

        PickRandomCorrect();
        // Gamepanel Animations...
        StartCoroutine(ParentOneByOne());
    }
    void Update()
    {
        if (correctCheckList.Count == winCount && levelStarted)
        {
            if (_levelComplete == false)
            {
                //AudioManager.instance.Play("Won");
                //Debug.Log("Win");
                _levelComplete = true;
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    EmojiManager.Instance.PanelAndListUpdate();
                    if (EmojiManager.Instance.GetPanelsDone() == 3)
                    {
                        EmojiManager.Instance.popperBlast.Play();
                    }
                });
                ///FindObjectOfType<Button>().enabled = false;
                //UIManager.instance.WinPanel();
                /////hintBtn.transform.DOScale(Vector3.zero, 0.25f);
            }
        }

        if (wrongList.Count == 3)
        {
            if (_levelfail == false)
            {
                //UIManager.instance.LosePanel();
                //AudioManager.instance.Play("Lose");
                _levelfail = true;
                EmojiManager.Instance.losePanel.SetActive(true);
                Debug.Log("GameOver");
                for (int i = 0; i < optionBtn.Count; i++)
                {
                    optionBtn[i].GetComponent<Button>().enabled = false;
                }
                FindObjectOfType<Button>().enabled = false;
            }
        }
    }

    //GameObject 1-1 Animation...
    IEnumerator ParentOneByOne()
    {
        for (int i = 0; i < parentGamePanel.Count; i++)
        {
            yield return new WaitForSeconds(0.015f);
            parentGamePanel[i].transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear);
        }

        yield return new WaitForSeconds(0.05f);
        StartCoroutine(OneByOne());
    }

    public IEnumerator ParentAnimDecrese()
    {
        for (int i = 0; i < parentGamePanel.Count; i++)
        {
            yield return new WaitForSeconds(0.015f);
            parentGamePanel[i].transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear);
        }
        for (int i = 0; i < gamePanel.Count; i++)
        {
            yield return new WaitForSeconds(0.015f);
            gamePanel[i].transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear);
        }
    }
    IEnumerator OneByOne()
    {
        for (int i = 0; i < gamePanel.Count; i++)
        {
            yield return new WaitForSeconds(0.015f);
            gamePanel[i].transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator RemoveCrossMark(Image obj)
    {
        yield return new WaitForSeconds(.5f);
        obj.GetComponent<Image>().enabled = false;
    }

    public void SelectEmoji()
    {
        //AudioManager.instance.Play("Tap");
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        var temp = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
        temp.GetComponent<Button>().enabled = false;
        // UIManager.instance.SmokeOn();
        if (temp.CompareTag("Wrong"))
        {
            temp.GetComponent<Image>().sprite = crossMark;
            temp.GetComponent<Image>().transform.DOScale(.5f, .25f).SetEase(Ease.Linear);
            StartCoroutine(RemoveCrossMark(temp.GetComponent<Image>()));
            // temp.GetComponent<Image>().enabled = false;
            wrongList.Add(temp.rectTransform.gameObject);

            wrongImage[index].sprite = wrongColorlist[index];
            wrongImage[index].rectTransform.DOScale(Vector3.one * 1.3f, .25f).SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.Flash);
            index++;
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ResetPositionMG");
            //Vibration.Vibrate(20);
        }

        if (temp.CompareTag("Correct"))
        {
            if (temp.GetComponent<Image>().sprite == correctList[0])
            {
                temp.GetComponent<Image>().rectTransform.DOMove(correctPos[0].rectTransform.position, .25f)
                    .OnComplete(() =>
                    {
                        correctPos[0].GetComponent<Image>().enabled = true;
                        correctPos[0].GetComponent<Image>().sprite = temp.GetComponent<Image>().sprite;
                        // correctPos[0].GetComponent<Image>().rectTransform.DOScale(1.3f, 0.25f).SetEase(Ease.Linear)
                        //     .SetLoops(2, LoopType.Yoyo);
                        temp.GetComponent<Image>().enabled = false;
                        var colorTemp = correctPos[0].GetComponent<Image>();
                        colorTemp.color = new Color(colorTemp.color.r, colorTemp.color.g, colorTemp.color.b, 1f);
                    });
               
            }

            else if (temp.GetComponent<Image>().sprite == correctList[1])
            {
                temp.GetComponent<Image>().rectTransform.DOMove(correctPos[1].rectTransform.position, .25f)
                    .OnComplete(() =>
                    {
                        correctPos[1].GetComponent<Image>().enabled = true;
                        correctPos[1].GetComponent<Image>().sprite = temp.GetComponent<Image>().sprite;
                        // correctPos[1].GetComponent<Image>().rectTransform.DOScale(1.3f, 0.25f).SetEase(Ease.Linear)
                        //     .SetLoops(2, LoopType.Yoyo);
                        temp.GetComponent<Image>().enabled = false;
                        var colorTemp = correctPos[1].GetComponent<Image>();
                        colorTemp.color = new Color(colorTemp.color.r, colorTemp.color.g, colorTemp.color.b, 1f);
                    });
               
            }
            else if (temp.GetComponent<Image>().sprite == correctList[2])
            {
                temp.GetComponent<Image>().rectTransform.DOMove(correctPos[2].rectTransform.position, .25f)
                    .OnComplete(() =>
                    {
                        correctPos[2].GetComponent<Image>().enabled = true;
                        correctPos[2].GetComponent<Image>().sprite = temp.GetComponent<Image>().sprite;
                        // correctPos[1].GetComponent<Image>().rectTransform.DOScale(1.3f, 0.25f).SetEase(Ease.Linear)
                        //     .SetLoops(2, LoopType.Yoyo);
                        temp.GetComponent<Image>().enabled = false;
                        var colorTemp = correctPos[2].GetComponent<Image>();
                        colorTemp.color = new Color(colorTemp.color.r, colorTemp.color.g, colorTemp.color.b, 1f);
                    });
            }

            correctCheckList.Add(temp.gameObject);
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Pop");
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
            //Vibration.Vibrate(20);
        }
    }

    public void PickRandomCorrect()
    {
        for (int i = 0; i < correctList.Count; i++)
        {
            int ran = Random.Range(0, optionBtn.Count);
            optionBtn[ran].GetComponent<Image>().sprite = correctList[i];
            optionBtn[ran].gameObject.tag = "Correct";
            optionBtn.Remove(optionBtn[ran]);
        }
    }

    
    public void Reshuffle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}