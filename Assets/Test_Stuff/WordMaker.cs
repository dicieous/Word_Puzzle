using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordMaker : MonoBehaviour
{
    [SerializeField] private WordsData_TestSO WordsDataTestSO;

    private void Start()
    {
    }

    public void CommonWords()
    {
        var wordList = WordsDataTestSO.wordsToUseListSO;
        for (var wordNo = 0; wordNo < wordList.Count; wordNo++)
        {
            for (int j = 0; j < wordList.Count; j++)
            {
                if (wordNo != j)
                { 
                    string commonCharacters = SimilarCharacters(wordList[wordNo], wordList[j]);
                }
            }
        }
    }

    private string SimilarCharacters(string word, string s)
    {
        string commonWords = "";
        foreach (var character in s)
        {
            if (word.Contains(character))
            {
                commonWords += character.ToString();
            }
        }
        return commonWords;
    }
}