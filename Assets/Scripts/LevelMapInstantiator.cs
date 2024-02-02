
using System;
using System.Collections;
using System.Collections.Generic;
using DDZ;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class LevelMapInstantiator : MonoBehaviour
{
    public GameObject levelImages;

    public GameObject normalLevel;
    public GameObject prevLevel;
    public GameObject currLevel;
    public GameObject bossLevel;
    public GameObject giftLevel;
    public GameObject powerUpRef;
    
    public List<Sprite> powerUpsSprites;
    public List<int> refLevels;
    
    public Sprite prevSprite;
    public Sprite currSprite;
    public Color prevCol;

    public bool isNewLevel = false;

    public int totalLevelCount=0;
    public int level = 0;
    public int startLevel = 0;
    public Vector3 firstPos;
    public Vector3 firstPos02;


    public Vector3 secondPos;
    public Vector3 secondPos02;

    Vector3 startPos;
    Vector3 endPos;
    // Start is called before the first frame update
    void Start()
    {
        isNewLevel= false;
        //level = PlayerPrefs.GetInt("Level", 1);
        level = UIManagerScript.Instance.GetSpecialLevelNumber();
        
        if (!PlayerPrefs.HasKey("RoadLevel"))
        {
            PlayerPrefs.SetInt("RoadLevel", 1);
        }
        if (level != PlayerPrefs.GetInt("RoadLevel"))
        {
            isNewLevel = true;
            PlayerPrefs.SetInt("RoadLevel", level);
        }

        
        if (level < 21)
        {
            startLevel = 1;
            totalLevelCount += level;

        }
        else
        {
            startLevel = level - 20;
            totalLevelCount += 20;
        }
        totalLevelCount += 60;
        
        SetContentHeight();
        CheckCalendarIndicator();
    }

    private void CheckCalendarIndicator()
    {
        var calenderIndicator = UIManagerScript.Instance.calenderButton.transform.GetChild(2).GetComponent<Image>();
        var todayDateCheck =  PlayerPrefs.GetInt("DailyChallenges_" + GameEssentials.GameStartTime.Day + GameEssentials.GameStartTime.Month + GameEssentials.GameStartTime.Year, 0);
        calenderIndicator.enabled = UIManagerScript.Instance.GetSpecialLevelNumber() >= 30 && todayDateCheck == 0;
    }

    private void SetContentHeight()
    {
        var totalContentCount = (float)totalLevelCount;
       if(totalContentCount>75.0f)
        {
            totalContentCount = 75;
        }
        if (level < 16)
        {
            totalContentCount -= 1.5f;
        }
      

        var vert = GetComponent<VerticalLayoutGroup>();
        
        var child = levelImages.transform as RectTransform;

        float scrollHeight = 0f;
        if (child != null) scrollHeight = (child.rect.height + vert.spacing) * (totalContentCount);
        

        var rect = GetComponent<RectTransform>().sizeDelta;
        GetComponent<RectTransform>().sizeDelta = new Vector2(rect.x, scrollHeight);
        //Debug.Log("ScrollHeight of Objects " + scrollHeight);
        //Debug.Log("rectHeight of Objects " + rect.y);

        if (level < 21)
        {
            GetComponent<RectTransform>().anchoredPosition = firstPos;
            startPos = firstPos02;
            endPos = firstPos;
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = secondPos;
            startPos = secondPos02;
            endPos = secondPos;
        }

        SetLevelImages();
    }
    void ShowTheActiveLevel()
    {
        Debug.Log("2");
        var activeLevel = 5;
        //Debug.Log("Active Level " + activeLevel);

        var child = transform.GetChild(activeLevel);


        if (child != null) child.GetComponent<Image>().color = Color.blue;

        //MainMenuUIManagerScript.Instance.MoveToActivePlayableLevel();
    }

    //To instantiate the Level Images
    void SetLevelImages()
    {
        Transform oldLevel = null;
        Debug.Log("1");
        var imagesNoToInstantiate = totalLevelCount;
        //Debug.Log("StartCalled " + imagesNoToInstantiate);
        for (int i = 0; i < imagesNoToInstantiate; i++)
        {
            if((i+startLevel)%10==0)
            {
                levelImages = bossLevel;
            }
            else if((i + startLevel) % 5 == 0 && (i + startLevel != 5))
            {
                levelImages = giftLevel;
            }
            else
            {
                levelImages = normalLevel;
            }


            if (this == null) continue;
            var image = Instantiate(levelImages, transform, true);
            image.transform.localScale = Vector3.one * 2.5f;
            image.GetComponentInChildren<TextMeshProUGUI>().text = (i + startLevel).ToString();

            //image.transform.GetChild(1).GetComponent<Image>().color = Color.gray ;
            //if ((i + startLevel) %10==0)
            //{
            //    image.transform.GetChild(1).GetComponent <Image>().color=Color.red;

            //}else
            if (((i + startLevel) % 5 == 0 && (i + startLevel) != 5) && ( (i + startLevel) % 10 != 0) && (i + startLevel) < level)
            {
               image.transform.GetChild(2).gameObject.SetActive(false);
            }
            if (level == i + startLevel)
            {
                if(level%10 != 0)
                {
                    image.transform.GetChild(1).GetComponent<Image>().sprite = currSprite;
                }
                if(isNewLevel)
                {
                    StartCoroutine(NextLevelAnimation(oldLevel, image.transform));
                }
                else
                {
                    image.transform.GetChild(1).GetComponent<RectTransform>().localScale = Vector3.one * 2f;
                }
                
                
            }
            if (i + startLevel < level)
            {
                image.transform.GetChild(1).GetComponent<Image>().sprite = prevSprite;
                image.transform.GetChild(0).GetComponent<Image>().color = prevCol;
            }
            oldLevel = image.transform;
            
            if (i + startLevel >=level)
            {
                for (int j = 0; j < refLevels.Count; j++)
                {
                    if (refLevels[j] == i + startLevel)
                    {
                        var refPrefab = Instantiate(powerUpRef, image.transform, false);
                        refPrefab.transform.GetChild(0).GetComponent<Image>().sprite = powerUpsSprites[j];
                        refPrefab.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        refPrefab.GetComponent<RectTransform>().localScale = Vector3.one;
                    }
                }
            }
            //Debug.Log("Called");
        }

        //ShowTheActiveLevel();
        
    }
    IEnumerator NextLevelAnimation(Transform oldLevel,Transform newLevel)
    {
        GetComponent<RectTransform>().anchoredPosition = startPos;
        oldLevel.GetChild(1).GetComponent<RectTransform>().localScale = Vector3.one * 2f;
        newLevel.GetChild(1).GetComponent<RectTransform>().localScale = Vector3.one * 1.4f;
        yield return new WaitForSeconds(0.4f);
        float elapsedTime = 0f;
        
        while (elapsedTime < 1f)
        {
            elapsedTime+= Time.deltaTime;
            oldLevel.GetChild(1).GetComponent<RectTransform>().localScale =
                Vector3.Lerp(Vector3.one * 2f, Vector3.one * 1.4f, elapsedTime);
            newLevel.GetChild(1).GetComponent<RectTransform>().localScale=
                Vector3.Lerp( Vector3.one * 1.4f, Vector3.one * 2f, elapsedTime);
            GetComponent<RectTransform>().anchoredPosition=Vector3.Lerp(startPos,endPos,elapsedTime);

            yield return null;
        }
        GetComponent<RectTransform>().anchoredPosition = endPos;
    }
}
