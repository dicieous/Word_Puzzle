using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardManager : SingletonInstance<DailyRewardManager>
{
    public Canvas mainCanvas;
    public ScrollRect scrollRect;
    public List<DayContent> days;
    public DateTime currentDateTime;
    
    public DayContent selectedDayContent;
    private DayContent _currentDayContent;
   
    public GameObject dailyRewardPanel, dairlyRewardBtr, spinWheelBtr;
    private static int GetSavedValue(int day, int month, int year) =>
        PlayerPrefs.GetInt("SavedDate_" + day + "" + month + "" + year);

    public void DailyCallBack()
    {
        selectedDayContent.SaveDay();
    }
    
    
    private void Start()
    {
        currentDateTime = DateTime.Now;
        scrollRect.vertical = false;
        
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
    
    public void OpenPanel()
    {
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        dailyRewardPanel.transform.DOScale(1, 1);
        dailyRewardPanel.SetActive(true);

        dairlyRewardBtr.SetActive(false);
        spinWheelBtr.SetActive(false);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            MoveScrollToCurrentDate(_currentDayContent.GetComponent<RectTransform>(), () =>
            {
                scrollRect.vertical = true;
            });
        });
    }

    public void ClosePanel()
    {
        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
        dailyRewardPanel.transform.DOScale(0.9f, 0.2f).OnComplete((() =>
        {
            dailyRewardPanel.SetActive(false);
        }));
        dairlyRewardBtr.SetActive(true);
        spinWheelBtr.SetActive(true);
    }
}
