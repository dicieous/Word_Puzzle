using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EmojiDataFile",menuName = "EmojiDetails",order = 1)]
public class EmojiesData : ScriptableObject
{
    //public GameObject panelDetails;
    public BackGroundDetails backGroundDetails;
    
    public List<Details> detailsLists;
    
    [Serializable]
    public class Details
    {
        public string emojiName;
        public GameObject panel;
        
        //public GameObject emojiIcon;
        public Sprite[] options;
        public Sprite[] correctList;
        //public Image[] correctPos;
        //public GameObject[] correctCheckList;
        public Sprite[] wrongColorList;

    }
    [Serializable]
    public class BackGroundDetails
    {
        public Sprite[] backGrounds;
        public Sprite crossMark;
    }
}
