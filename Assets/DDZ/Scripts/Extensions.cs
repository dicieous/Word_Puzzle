using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public static class Extensions
{
    public static void ClampedAngle(float min,float max, GameObject obj)
    {
        var charAngle = obj.transform.localEulerAngles;
        charAngle.y = (charAngle.y > 180) ? charAngle.y-360 : charAngle.y;
        charAngle.y = Mathf.Clamp(charAngle.y, min, max);
        obj.transform.localEulerAngles = charAngle;
    }
    
    public static Transform GetChildObjectOfName(this Transform target, string name)
    {
        if (target.name == name)
            return target;

        for (var i = 0; i < target.childCount; ++i)
        {
            var result = GetChildObjectOfName(target.GetChild(i), name);

            if (result != null)
                return result;
        }

        return null;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2, bool isClamped = false)
    {
        if (isClamped)
        {
            value = Mathf.Clamp(value, from1, to1);
        }
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    public static int Remap(this int value, int from1, int to1, int from2, int to2, bool isClamped = false)
    {
        if (isClamped)
        {
            value = Mathf.Clamp(value, from1, to1);
        }
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static bool IsInRange(this float value, float bottom, float top)
    {
        return (value >= bottom && value <= top);
    }

    public static string ScoreShow(int score)
    {
        float tempScore = score;
        string result;
        var scoreNames = new string[] { "", "k", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
        int i;

        for (i = 0; i < scoreNames.Length; i++)
            if (tempScore < 900)
                break;
            else tempScore = Mathf.Floor(tempScore / 100f) / 10f;

        if (tempScore == Mathf.Floor(tempScore))
            result = tempScore + scoreNames[i];
        else result = tempScore.ToString("F1") + scoreNames[i];
        return result;
    }
    
    public static List<T> RandomizeList<T>(List<T> allObjects)
    {
        for (int i = 0; i < allObjects.Count; i++)
        {
            var temp = allObjects[i];
            var randomIndex = UnityEngine.Random.Range(i, allObjects.Count);
            allObjects[i] = allObjects[randomIndex];
            allObjects[randomIndex] = temp;
        }

        return allObjects;
    }

    public static Vector3 GetRandomPointInBoxCollider(BoxCollider box)
    {
        var tempTransform = box.transform;
        var bLocalScale = tempTransform.localScale;
        var boxPosition = tempTransform.position;
        boxPosition += new Vector3(bLocalScale.x * box.center.x, bLocalScale.y * box.center.y, bLocalScale.z * box.center.z);

        var dimensions = new Vector3(bLocalScale.x * box.size.x,
            bLocalScale.y * box.size.y,
            bLocalScale.z * box.size.z);

        var newPos = new Vector3(UnityEngine.Random.Range(boxPosition.x - (dimensions.x / 2), boxPosition.x + (dimensions.x / 2)),
            UnityEngine.Random.Range(boxPosition.y - (dimensions.y / 2), boxPosition.y + (dimensions.y / 2)),
            UnityEngine.Random.Range(boxPosition.z - (dimensions.z / 2), boxPosition.z + (dimensions.z / 2)));
        return newPos;
    }
    
    public static T GetOneFromArray<T>(T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length - 1)];
    }

    public static T GetOneFromList<T>(List<T> array)
    {
        return array[UnityEngine.Random.Range(0, array.Count)];
    }

    public static IEnumerator WaitThenCallback(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }
    
    public static T UniqueRandomInt<T>( List<T> list)
    {
        const int minIndex = 0;
        var maxIndex = list.Count;
        var val = Random.Range(minIndex, maxIndex);
        while(!list.Contains(list[val]))
        {
            val = Random.Range(minIndex, maxIndex);
        }
        return list[val];
    } 


}
