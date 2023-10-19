using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsPicInstantiator : MonoBehaviour
{

    [SerializeField] private GameObject levelImages;
    
    
    void Start()
    {
        SetLevelImages();
    }

    //To show the level Which you need to play now
    void ShowTheActiveLevel()
    {
        Debug.Log("2");
        var activeLevel = PlayerPrefs.GetInt("Level");
        Debug.Log("Active Level " + activeLevel);
        
        var child = transform.GetChild(activeLevel);
       
        
        if(child!=null) child.GetComponent<Image>().color = Color.blue;
        
        MainMenuUIManagerScript.Instance.MoveToActivePlayableLevel();
    }

    //To instantiate the Level Images
    void SetLevelImages()
    {
        Debug.Log("1");
        var imagesNoToInstantiate = SceneManager.sceneCountInBuildSettings - 1;
        //Debug.Log("StartCalled " + imagesNoToInstantiate);
        for (int i = 0; i < imagesNoToInstantiate; i++)
        {
            if (this == null) continue;
            var image  = Instantiate(levelImages, transform, true);
            image.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
            //Debug.Log("Called");
        }
        
        ShowTheActiveLevel();
    }
    
}
