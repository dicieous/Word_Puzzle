using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenjinManager : MonoBehaviour
{
    public static string sdkKey = "GSPTCKSIBO7QSXNH4YQNYVWVWZ4BRK2D";
    void Start()
    {
        Init();
    }

    private void Init()
    {
        BaseTenjin instance = Tenjin.getInstance(sdkKey);

        instance.SetAppStoreType(AppStoreType.googleplay);
        instance.Connect();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            Init();
        }
    }

    public static void Events(String eventName, string levelNo)
    {
        BaseTenjin instance = Tenjin.getInstance(sdkKey);

        instance.SendEvent(eventName, levelNo);
    }

}
