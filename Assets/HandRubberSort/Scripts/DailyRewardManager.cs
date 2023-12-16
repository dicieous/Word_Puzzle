using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardManager : MonoBehaviour
{
    public Canvas mainCanvas;
    public ScrollRect scrollRect;
    public List<DayContent> days;
    public DateTime currentDateTime;
    
    private DayContent _currentDayContent;
   
    private static int GetSavedValue(int day, int month, int year) =>
        PlayerPrefs.GetInt("SavedDate_" + day + "" + month + "" + year);
    private void Awake()
    {
        currentDateTime = DateTime.Now;
        scrollRect.vertical = false;
        /*var height = GetComponent<RectTransform>().rect.height * 0.8f;
        var size = new Vector2(0, height <= 1835 ? 1835 : height); 
        scrollRect.transform.parent.GetComponent<RectTransform>().sizeDelta = size ;*/
    }

    private void Start()
    {
        for (var i = 0; i < days.Count; i++)
        {
            var date = i + 1;
            days[i].SetClassData(date, currentDateTime.Month,currentDateTime.Year);
            if (date == currentDateTime.Day)
            {
                _currentDayContent = days[i];
                days[i].SetButtonData(date,
                    GetSavedValue(days[i].dayInt, days[i].monthInt, days[i].yearInt) == 0
                        ? DayState.Claim
                        : DayState.Claimed);
            }
            else if(date > currentDateTime.Day)
            {
                days[i].SetButtonData(date, DayState.Lock);
            }
            else if(date < currentDateTime.Day - 1)
            {
                days[i].SetButtonData(date, DayState.Missed);
            }
            else if(date == currentDateTime.Day - 1)
            {
                print(date+"::"+(currentDateTime.Day - 1));
                days[i].SetButtonData(date,
                    GetSavedValue(days[i].dayInt, days[i].monthInt, days[i].yearInt) == 0
                        ? DayState.ClaimAd
                        : DayState.Claimed);
            }
        }
        
        MoveScrollToCurrentDate(_currentDayContent.GetComponent<RectTransform>(), () =>
        {
            scrollRect.vertical = true;
        });
    }


    private void MoveScrollToCurrentDate(RectTransform currentDay, Action callback)
    {
        scrollRect.content.DOLocalMove(GetSnapToPositionToBringChildIntoView(scrollRect, currentDay),0.25f).SetEase(Ease.Flash).OnComplete(()=>callback?.Invoke());
    }

    private static Vector2 GetSnapToPositionToBringChildIntoView(ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        var viewportLocalPosition = instance.viewport.localPosition;
        var childLocalPosition   = child.localPosition;
        var result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }
}
