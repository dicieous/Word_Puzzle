using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManagerScript : MonoBehaviour
{
    [SerializeField] private ScrollRect levelsScrollRect;

    public static MainMenuUIManagerScript Instance;
    
    
    
    private void Awake()
    {
        Instance = this;
    }

    public void MoveToActivePlayableLevel()
    {
        Debug.Log("3");
        var levelImageIndex = PlayerPrefs.GetInt("Level")+1;

        VerticalLayoutGroup vert = levelsScrollRect.content.GetComponent<VerticalLayoutGroup>();
        levelImageIndex = Mathf.Clamp(levelImageIndex, 0, levelsScrollRect.content.transform.childCount-1);

        float scrollHeight = 0f; 
        for (int i = 0; i < levelImageIndex - 1; i++)
        {
            var childTransform = vert.transform.GetChild(i) as RectTransform;
            if (childTransform != null) scrollHeight += childTransform.rect.height + vert.spacing;
        }

        var viewportHeight = levelsScrollRect.viewport.rect.height;

        var normalizedPosition = scrollHeight / (levelsScrollRect.content.rect.height - viewportHeight);
        normalizedPosition = Mathf.Clamp01(normalizedPosition);
        levelsScrollRect.verticalNormalizedPosition = normalizedPosition;
        
        // Debug.Log("scrollHeight: " + scrollHeight);
        // Debug.Log("viewportHeight: " + viewportHeight);
        // Debug.Log("normalizedPosition: " + normalizedPosition);

    }
}
