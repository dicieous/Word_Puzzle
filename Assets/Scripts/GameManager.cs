using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<string> answers;
    public List<Color> rowColor;
    public string question;
    public List<WordData> wordList;
    public List<StickingAreaCheckingScript> stickingCubes;
    public List<GameObject> spriteRevealImages;
    public bool scriptOff;

    [Space(10)] public int rowsInGrid;
    public int colInGrid;

    [Header("Opposite Words Level Columns")]
    public List<int> numberOfColumns;

    [Space(10)] [Header("AutoComplete Section")] [SerializeField]
    public List<CompleteWordCubes> completeWordCubesList;

    [SerializeField] public List<CompleteWordPosition> completeWordPositionsList;

    private int wordNoToComplete = 0;

    public bool canClickNow = true;
    public bool canPlaceNow = true;

    [HideInInspector] public int rowfilled;

    [Space(20)] public int movesCount;
    [HideInInspector] public List<GameObject> fXRowWords;
    [HideInInspector] public bool downCheck;

    [Space(10)] [Header("Change Level Style")]
    public bool levelTypeChanged = false;

    [Space(10)] private List<List<GameObject>> letterCubeWord = new List<List<GameObject>>();

    public List<GameObject> Cube_Groups;

    public event EventHandler OnPartComplete;


    [SerializeField] public List<GameObject> _allCubeObjects;
    [Space(10)] public List<GameObject> hintCubesHolder;

    [Space(10)] [SerializeField] private GameObject starFX;

    [SerializeField] private GameObject complementPrefab;

    [SerializeField] private Transform instPos;

    private bool canInstantiate = true;

    [SerializeField] private List<int> wordsAfterWhichToMoveCam;

    private float instTime = 1f;

    [Space(10)] private bool[] wordCompleted;

    [HideInInspector] public bool levelCompleted = false;
    public bool levelFail;
    [SerializeField] private int wordsMade;

    [Serializable]
    public struct CompleteWordCubes
    {
        public List<GameObject> completeWordCubeGroup;
    }

    [Serializable]
    public struct CompleteWordPosition
    {
        public List<Vector3> completeWordCubePositionGroup;
    }

    [SerializeField] private int totalAutoWordsNumber;
    private UIManagerScript UI;


    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    private void Start()
    {
        canClickNow = true;
        canPlaceNow = true;
        Application.targetFrameRate = 120;
        InitializeGrid();

        UI = UIManagerScript.Instance;
        //Debug.Log($"Words Made {wordsMade}");
        wordCompleted = new bool[rowsInGrid];
        InitializeWordComplete();
        if (!levelTypeChanged)
        {
            for (int i = 0; i < rowsInGrid; i++)
            {
                var wordData = new WordData
                {
                    wordsDataLists = new List<GameObject>()
                };
                wordList.Add(wordData);
            }
        }

        totalAutoWordsNumber = completeWordCubesList.Count;
        if (levelTypeChanged) AssignAnswersToStickingCubes();
        /*for (int var = 0, i = 0; i < rowsInGrid; i++)
        {
            var s = answers[i];
            for (int j = 0; j < s.Length; j++)
            {
                UI.hintsWordList[var].transform.GetChild(0).GetComponent<TextMeshPro>().text = s[j].ToString();
                Debug.Log($"{var} + var value + {s[j].ToString()}");
                var++;
            }
        }*/

        starFX = UI.starparticleEffect;
        var num = hintCubesHolder.Count * 2;
        movesCount = num;
        UIManagerScript.Instance.movesText.text = "Moves: " + movesCount;
    }

//#region Initialize Grid Words

    void InitializeWordComplete()
    {
        for (int i = 0; i < rowsInGrid; i++)
        {
            wordCompleted[i] = false;
        }
    }

    void InitializeGrid()
    {
        for (int row = 0; row < rowsInGrid; row++)
        {
            List<GameObject> newRow = new List<GameObject>();
            if (numberOfColumns.Count == 0)
            {
                for (int col = 0; col < colInGrid; col++)
                {
                    newRow.Add(null);
                }
            }
            else
            {
                for (int col = 0; col < numberOfColumns[row]; col++)
                {
                    newRow.Add(null);
                }
            }


            letterCubeWord.Add(newRow);
        }
    }

    public void AddWords(int row, int col, GameObject value)
    {
        letterCubeWord[row][col] = value;
    }

    public void RemoveWords(int row, int col)
    {
        letterCubeWord[row][col] = null;
    }

    private void AssignAnswersToStickingCubes()
    {
        for (int i = 0; i < answers.Count; i++)
        {
            stickingCubes[i].answerString = answers[i];
        }
    }

    private bool _checkAllWordsClear;
//#endregion

// ReSharper disable Unity.PerformanceAnalysis
    void Update()
    {
        if (!levelTypeChanged)
            MakeAndCheckWord();
        else
            MakeAndCheckWordNew();

        instTime -= Time.deltaTime;
        if (instTime < 0)
        {
            canInstantiate = true;
        }

        if (!levelCompleted && completeWordCubesList.Count == 0 && !_checkAllWordsClear)
        {
            ButtonsTurnOffFun();
            _checkAllWordsClear = true;
            DOVirtual.DelayedCall(3f, () =>
            {
                if (!levelCompleted)
                {
                    levelCompleted = true;
                    //ButtonsTurnOffFun();

                    DOVirtual.DelayedCall(.75f, () =>
                    {
                        CoinManager.instance.confettiFx.Play();
                        CoinManager.instance.confettiFx1.Play();
                        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("BlastPopper");
                        UI.WinPanelActive();
                        // BlockSeqCall();
                        //Debug.Log("LevelComplete");
                    });
                }
            });
        }

        if (SavedData.GetSpecialLevelNumber() > 10)
        {
            if (movesCount <= 0 && !levelCompleted && !levelFail)
            {
                print("Failed1");
                if (!scriptOff)
                    scriptOff = true;
                ButtonsTurnOffFun();
                print("Failed2");
                DOVirtual.DelayedCall(1f, () =>
                {
                    if (!levelCompleted)
                    {
                        //if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("BlastPopper");
                        if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Fail");
                        UI.FailPanelActive();
                        print("Failed");
                    }
                }, false);
                levelFail = true;
            }
        }
        /*if (Input.GetKeyDown(KeyCode.A))
        {
            print("Nextfuncall");
            UI.NextMoveFun();
        }*/
    }


    //#region Make And Check Words

    bool IsRowFull(int rowIndex)
    {
        for (int col = 0; col < colInGrid; col++)
        {
            if (letterCubeWord[rowIndex][col] == null)
            {
                //Debug.Log($"Row {rowIndex+1} is not full");
                return false;
            }
        }

        //Debug.Log($"Row {rowIndex+1} is full");
        return true;
    }


    bool IsGridFull()
    {
        for (int row = 0; row < rowsInGrid; row++)
        {
            for (int col = 0; col < colInGrid; col++)
            {
                if (letterCubeWord[row][col] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    //private int colnum = 0;
    public void RearangeValues(int row)
    {
        for (int i = 0; i < colInGrid; i++)
        {
            if (i < colInGrid)
            {
                if (wordList[row].wordsDataLists[0] != null)
                {
                    /*wordList[row].wordsDataLists[0].transform.GetChild(1).GetComponent<MeshRenderer>().material.color = UI.originalColor.color;
                    wordList[row].wordsDataLists[0].transform.GetChild(1).GetComponent<MeshRenderer>().materials[0].color =  UI.originalColor.color;
                    wordList[row].wordsDataLists[0].transform.GetChild(1).GetComponent<MeshRenderer>().materials[1].color =  UI.originalColor.color;*/
                    wordList[row].wordsDataLists.RemoveAt(0);
                }
            }
            /*else if (i >= colInGrid )
            {
                if (removing)
                {
                    removing = false;
                }
                else
                {
                    print(i);
                }
            }*/
        }
    }

    public bool removing;

    public void MovingSeq(int row, int columCount = 0)
    {
        if (!removing)
        {
            for (int i = 0; i < colInGrid; i++)
            {
                if (!wordList[row].wordsDataLists.Contains(letterCubeWord[row][i].gameObject) &&
                    wordList[row].wordsDataLists.Count != colInGrid + 1)
                {
                    wordList[row].wordsDataLists.Add(letterCubeWord[row][i].gameObject);
                }
            }
            /*var seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                if (columCount < colInGrid)
                {
                    if (!wordList[row].wordsDataLists.Contains(letterCubeWord[row][columCount].gameObject))
                    {
                        wordList[row].wordsDataLists.Add(letterCubeWord[row][columCount].gameObject);
                    }

                    //print(columCount);
                    /*letterCubeWord[row][columCount].transform.GetChild(1).GetComponent<MeshRenderer>().materials[0]
                        .color = rowColor[row];
                    letterCubeWord[row][columCount].transform.GetChild(1).GetComponent<MeshRenderer>().materials[1]
                        .color = rowColor[row];
                    letterCubeWord[row][columCount].transform.GetChild(0).transform
                        .DOScale(new Vector3(1.75f, 1.75f, 2f), 0.1f)
                        .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
                    letterCubeWord[row][columCount].transform.GetChild(1).transform
                        .DOScale(new Vector3(20f, 30f, 15f), 0.1f)
                        .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);#1#
                    /*if (!scriptOff)
                    {
                        scriptOff = true;
                    }#1#
                }
                /*if (columCount >= colInGrid)
                {
                    scriptonfun();
                }#1#
                columCount++;
            });
            seq.AppendInterval(0.09f);
            seq.SetLoops(colInGrid );*/
        }
    }

    public void scriptonfun()
    {
        if (scriptOff)
        {
            DOVirtual.DelayedCall(.7f, () =>
            {
                //print("FunctionCall");
                scriptOff = false;
            });
        }
    }

    private void MakeAndCheckWord()
    {
        if (levelCompleted) return;
        for (int row = 0; row < rowsInGrid; row++)
        {
            if (IsRowFull(row) && !wordCompleted[row])
            {
                //Debug.Log("entered");
                string madeword = "";
                for (int col = 0; col < colInGrid; col++)
                {
                    madeword += letterCubeWord[row][col].GetComponentInChildren<TextMeshPro>().text;
                }

                if (answers.Contains(madeword) && row == answers.IndexOf(madeword))
                {
                    //Debug.Log(madeword + " The word you made");
                    if (canInstantiate)
                    {
                        // Instantiate(complementPrefab, instPos.position, Quaternion.identity);
                        UIManagerScript.Instance.GetComponent<FloatingTextScript>().TextExpression();
                        instTime = 1f;
                        canInstantiate = false;
                    }

                    if (!scriptOff)
                    {
                        //scriptOff = true;
                        MovingSeq(row);
                        //rownumadded = row;
                    }

                    /*else
                    {
                        if(rownumadded != row)
                        {
                            MovingSeq(row);
                        }
                    }*/
                    wordCompleted[row] = true;
                    wordsMade++;
                    //wordNoToComplete++;
                    //print("row number" + row);

                    //Debug.Log("WordsMade " + wordsMade);
                }
            }
            /*if (!IsRowFull(row) && wordCompleted[row] && !levelCompleted)
            {
                if (!removing)
                {
                    //removing = true;
                    //RearangeValues(row);
                    print("Removing one");
                }

                wordCompleted[row] = false;
               // wordsMade--;
                //print("row number"+ row);
            }*/
        }

        if (wordsMade >= answers.Count && IsGridFull() && !levelCompleted)
        {
            print("Win");

            ButtonsTurnOffFun();
            levelCompleted = true;
            //DestroyBlocks();

            DOVirtual.DelayedCall(1.25f, () =>
            {
                CoinManager.instance.confettiFx.Play();
                if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("BlastPopper");
                CoinManager.instance.confettiFx1.Play();
                UI.WinPanelActive();
                BlockSeqCall();
                //Debug.Log("LevelComplete");
            }, false);
        }
    }

    public void MakeAndCheckWordNew()
    {
        if (levelCompleted) return;
        for (int i = 0; i < stickingCubes.Count; i++)
        {
            var s = stickingCubes[i];
            if (!s.correctWordMade && s.IsAllPlacesFullCheck() && s.CheckForAnswer())
            {
                if (!stickingCubes[i].countDone)
                {
                    stickingCubes[i].countDone = true;
                    wordsMade++;
                }
                /*if (wordsMade != wordsMade + 1 && wordsMade < stickingCubes.Count)
                {
                    wordsMade++;
                }*/

                s.correctWordMade = true;
                //Debug.Log("Correct Word");
                Debug.Log("Words Made " + wordsMade);
                if (SavedData.GetSpecialLevelNumber() == 1)
                {
                    UIManagerScript.Instance.HelpHand();
                }

                if (wordsAfterWhichToMoveCam.Count > 0)
                {
                    foreach (var wordNo in wordsAfterWhichToMoveCam)
                    {
                        if (wordNo == wordsMade /*|| wordsDeleteNumber == wordNo*/)
                        {
                            print(":::::" + wordNo + ":::::" + wordsMade);
                            OnPartComplete?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }

                if (canInstantiate && wordsAfterWhichToMoveCam.Count <= 0)
                {
                    // Instantiate(complementPrefab, instPos.position, Quaternion.identity);
                    UIManagerScript.Instance.GetComponent<FloatingTextScript>().TextExpression();
                    instTime = 1f;
                    canInstantiate = false;
                }
                //wordNoToComplete++;
                //Do anything after making the word
            }

            if (s.correctWordMade && !stickingCubes[i].IsAllPlacesFullCheck())
            {
                s.correctWordMade = false;
                //print("worddata::::::::::");
                //wordsMade--;
                /*var tempword = wordsMade;
                if(words)
                wordsMade--;*/
            }

            if (wordsMade == stickingCubes.Count && !levelCompleted /*&& CheckIfAllBlocksFullNew()*/)
            {
                //do anything after all words are made
                levelCompleted = true;
                Debug.Log("LevelComplete");

                ButtonsTurnOffFun();

                DOVirtual.DelayedCall(1.5f, () =>
                {
                    CoinManager.instance.confettiFx.Play();
                    CoinManager.instance.confettiFx1.Play();
                    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("BlastPopper");
                    UI.WinPanelActive();
                    // UI.NextSceneLoader();
                    WordsFallingAfterWinFun();
                    // BlockSeqCall();
                    //Debug.Log("LevelComplete");
                }, false);
            }
        }

        /*if (wordsMade == stickingCubes.Count && CheckIfAllBlocksFullNew())
        {
            //do anything after all words are made
            levelCompleted = true;
            Debug.Log("LevelComplete");

            ButtonsTurnOffFun();

            DOVirtual.DelayedCall(.75f, () =>
            {
                CoinManager.instance.confettiFx.Play();
                CoinManager.instance.confettiFx1.Play();
                if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("BlastPopper");
                UI.WinPanelActive();
                // BlockSeqCall();
                //Debug.Log("LevelComplete");
            });
        }*/
        /*if (movesCount == 0 && !levelCompleted && !levelFail && UIManagerScript.Instance.GetSpecialLevelNumber() > 10)
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                if (!levelCompleted && movesCount == 0)
                {
                    //if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("BlastPopper");
                    if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Fail");
                    UI.FailPanelActive();
                    //Debug.Log("Failed");
                    if (!scriptOff)
                        scriptOff = true;
                }
            },false);
            if (!scriptOff)
                scriptOff = true;
            levelFail = true;
        }*/
    }

    public DOTween vardo;

    private int rownumadded;

    //private int rowNumberDeleted;
    public void BlockSeqCall()
    {
        ///// FOr All rows moving at a time
        for (int i = 0; i < rowsInGrid; i++)
        {
            AllBlocksColoredAtaTimeFun(i);
        }
        //for single row moving at a time
        //BlocksColorRowByRowFun();
    }

    private void AllBlocksColoredAtaTimeFun(int row, int columCount = 0)
    {
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            if (columCount < colInGrid)
            {
                //print(columCount);
                letterCubeWord[row][columCount].transform.GetChild(1).GetComponent<MeshRenderer>().materials[0]
                    .color = rowColor[row];
                letterCubeWord[row][columCount].transform.GetChild(1).GetComponent<MeshRenderer>().materials[1]
                    .color = rowColor[row];
                letterCubeWord[row][columCount].transform.GetChild(0).transform
                    .DOScale(new Vector3(1.75f, 1.75f, 2f), 0.1f)
                    .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
                letterCubeWord[row][columCount].transform.GetChild(1).transform
                    .DOScale(new Vector3(20f, 30f, 15f), 0.1f)
                    .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            }

            columCount++;
        });
        seq.AppendInterval(0.13f);
        seq.SetLoops(colInGrid);
    }


    public void ResetScreen()
    {
        var loadedScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(loadedScene);
    }

    public GameObject hintSpawnObject;

    public void ShowTheText()
    {
        /*foreach (var cube in hintCubesHolder)
        {
            int count = cube.GetComponentsInChildren<TextMeshPro>().Length;
            if (count == 0) continue;
            if (!IsInstantiated(cube, count))
            {
                Instantiate(starFX, cube.transform.position, Quaternion.identity);
                hintSpawnObject = cube;
                /*if (SavedData.GetSpecialLevelNumber() > 1)
                {
                    CoinManager.instance.HintReduce(50);
                }#1#

                if (SavedData.GetSpecialLevelNumber() == 1) UIManagerScript.Instance.HelpHand();
                //print("instantiated");
            }

            for (int j = 0; j < count; j++)
            {
                var obj = cube;
                obj.GetComponentsInChildren<TextMeshPro>()[j].DOFade(217f / 255f, 2f);

                obj.GetComponentsInChildren<HighlightTextScript>()[j].isVisible = true;
            }

            break;
        }*/
        if (SavedData.GetSpecialLevelNumber() == 1)
        {
            foreach (var cube in hintCubesHolder)
            {
                int count = cube.GetComponentsInChildren<TextMeshPro>().Length;
                if (count == 0) continue;
                if (!IsInstantiated(cube, count))
                {
                    Instantiate(starFX, cube.transform.position, Quaternion.identity);
                    hintSpawnObject = cube;
                    // if (SavedData.GetSpecialLevelNumber() == 1) UIManagerScript.Instance.HelpHand();
                }

                for (int j = 0; j < count; j++)
                {
                    var obj = cube;
                    obj.GetComponentsInChildren<TextMeshPro>()[j].DOFade(217f / 255f, 2f);

                    obj.GetComponentsInChildren<HighlightTextScript>()[j].isVisible = true;
                }

                break;
            }
        }
        else
        {
            // if (hintCubesHolder.Count == 0) return;
            if (hintCubesHolder[0].gameObject.activeInHierarchy && !hintCubesHolder[0].GetComponent<HintParent>().doneObject)
            {
                var selectObj = hintCubesHolder[0];
                Instantiate(starFX, selectObj.transform.position, Quaternion.identity);
                for (int j = 0; j <= selectObj.transform.childCount; j++)
                {
                    if (j < selectObj.transform.childCount)
                    {
                        var obj = selectObj.transform.GetChild(j);
                        obj.GetChild(0).GetComponent<TextMeshPro>().DOFade(1, .5f).SetEase(Ease.Linear);
                    }
                    else
                    {
                        if (hintCubesHolder.Contains(selectObj))
                        {
                            //print(":::::::::::::::::::::::::::::::::::::::::::::");
                            hintCubesHolder.RemoveAt(0);
                        }
                    }

                    /*for (int i = 0; i < obj.childCount; i++)
                    {
                        obj.GetChild(i).GetComponent<TextMeshPro>().DOFade(217f / 255f, 2f);
                    }*/
                    //obj.GetComponentsInChildren<TextMeshPro>()[j].DOFade(217f / 255f, 2f);

                    //obj.GetComponentsInChildren<HighlightTextScript>()[j].isVisible = true;
                }
            }
        }
    }

    private bool IsInstantiated(GameObject obj, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var isVisible = obj.GetComponentsInChildren<HighlightTextScript>()[i].isVisible;
            if (!isVisible)
            {
                return false;
            }
        }

        return true;
    }


    private bool CheckIfAllBlocksFullNew()
    {
        return stickingCubes.All(hScript => hScript.IsAllPlacesFullCheck());
    }


    public void DestroyBlocks()
    {
        for (int row = 0; row < rowsInGrid; row++)
        {
            for (int col = 0; col < colInGrid; col++)
            {
                _allCubeObjects.Add(letterCubeWord[row][col]);
            }
        }

        DOVirtual.DelayedCall(0.5f, () =>
        {
            //Time.timeScale = 2f;
            //WordsFallingAfterWinFun();
        }, false);
    }

    public int wordsDeleteNumber;

    //To Remove the word from the AutoWordCompleteLists list after it's placed
    public void RemoveCompletedWord(Transform wordGroup)
    {
        for (var i = 0; i < completeWordCubesList.Count; i++)
        {
            var cubesGroups = completeWordCubesList[i].completeWordCubeGroup;
            var cubesPositions = completeWordPositionsList[i].completeWordCubePositionGroup;

            //For deleting element from completeWordCubeGroup
            for (var j = 0; j < cubesGroups.Count; j++)
            {
                if (cubesGroups[j].gameObject == wordGroup.gameObject)
                {
                    //cubesGroups[j] = null;
                    if (cubesGroups.Count == 1)
                    {
                        completeWordCubesList.RemoveAt(i);
                        completeWordPositionsList.RemoveAt(i);
                        wordsDeleteNumber++;
                    }
                    else
                    {
                        cubesGroups.RemoveAt(j);
                        cubesPositions.RemoveAt(j);
                    }
                }
            }
        }
    }


    private int _numberVal;
    public List<GameObject> numberCubeHolders;
    [SerializeField] private List<int> objNumbers;

    public void LetterObjectsCheck()
    {
        if (numberCubeHolders.Count > 0)
        {
            numberCubeHolders.Clear();
        }

        if (objNumbers.Count > 0)
        {
            objNumbers.Clear();
        }

        var firstAutoWordData = completeWordCubesList[0].completeWordCubeGroup;
        var firstAutoWordDataCount = completeWordCubesList[0].completeWordCubeGroup.Count;
        for (int i = 0; i < firstAutoWordDataCount; i++)
        {
            var childcountdetails = completeWordCubesList[0].completeWordCubeGroup[i].transform.childCount;
            for (int j = 0; j < childcountdetails; j++)
            {
                objNumbers.Add(completeWordCubesList[0].completeWordCubeGroup[i].transform.GetChild(j).GetComponent<PlayerCubeScript>().checknumber);
            }
        }
        /*if (levelTypeChanged)
        {
            var stickingCount = stickingCubes.Count;
            for (int i = 0; i < stickingCount; i++)
            {
                var stickingChildCount = stickingCubes[i].transform.childCount;
                for (int j = 0; j < stickingChildCount; j++)
                {
                    var holderCubeData = stickingCubes[i].transform.GetChild(j).GetComponent<HolderCubeScript>();
                    if (objNumbers.Contains(holderCubeData.checkNumberRef))
                    {
                        //var cubeobj = holderCubeData;
                        if (holderCubeData.isFilled)
                        {
                            if (!numberCubeHolders.Contains(holderCubeData.objRef.transform.parent.gameObject))
                            {
                                // numberCubeHolders.Add(objRef);
                                numberCubeHolders.Add(holderCubeData.objRef.transform.parent.gameObject);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            var stickingCubesObjs = FindObjectsOfType<HolderCubeScript>().ToList();
            var stickingCount = stickingCubesObjs.Count;
            for (int i = 0; i < stickingCount; i++)
            {
                var stickingChildCount = stickingCubesObjs[i].transform.childCount;
                for (int j = 0; j < stickingChildCount; j++)
                {
                    var holderCubeData = stickingCubesObjs[i];
                    if (objNumbers.Contains(holderCubeData.checkNumberRef))
                    {
                        //var cubeobj = holderCubeData;
                        if (holderCubeData.isFilled)
                        {
                            if (!numberCubeHolders.Contains(holderCubeData.objRef.transform.parent.gameObject))
                            {
                                // numberCubeHolders.Add(objRef);
                                numberCubeHolders.Add(holderCubeData.objRef.transform.parent.gameObject);
                            }
                        }
                    }
                }
            }
        }

        DOVirtual.DelayedCall(0.1f, () =>
        {
            print("oneObj");
            for (int i = 0; i < numberCubeHolders.Count; i++)
            {
                var holderCube = numberCubeHolders[i];
                for (int j = 0; j < firstAutoWordDataCount; j++)
                {
                    if (holderCube != completeWordCubesList[0].completeWordCubeGroup[j] && !holderCube.GetComponent<CubesGroupScript>().doneWithWord)
                    {
                        //FunWaitCall(holderCube,completeWordPositionsList[0].completeWordCubePositionGroup[j]);
                        ResetObjects(holderCube);
                    }
                }
            }
        },false);*/

        DOVirtual.DelayedCall(0.5f, () =>
        {
            /*for (int i = 0; i < firstAutoWordDataCount; i++)
            {
                FunWaitCall(completeWordCubesList[0].completeWordCubeGroup[i],
                    completeWordPositionsList[0].completeWordCubePositionGroup[i]);
            }*/
            var number = LetterGroupSet.instance.MagnetData();
            FunWaitCall(LetterGroupSet.instance.currentAutoWordSets[number],
                LetterGroupSet.instance.currentAutoWordPositions[number]);
            LetterGroupSet.instance.currentAutoWordSets.RemoveAt(number);
            LetterGroupSet.instance.currentAutoWordPositions.RemoveAt(number);
        }, false);
    }

    public void ResetObjects(GameObject resetObj)
    {
        var cubeVec = resetObj.transform.position;
        var cubePos = new Vector3(cubeVec.x, cubeVec.y, -5f);
        resetObj.transform.position = cubePos;

        for (int i = 0; i < resetObj.transform.childCount; i++)
        {
            if (resetObj.transform.GetChild(i).GetComponent<PlayerCubeScript>())
            {
                if (resetObj.transform.GetChild(i).transform.localPosition !=
                    resetObj.transform.GetChild(i).GetComponent<PlayerCubeScript>().startPos)
                {
                    resetObj.transform.GetChild(i).transform.localPosition =
                        resetObj.transform.GetChild(i).GetComponent<PlayerCubeScript>().startPos;
                }
            }
        }
        // if (holderCube.isFilled)
        //     holderCube.isFilled = false;

        for (int i = 0; i < resetObj.transform.childCount; i++)
        {
            var playerCubeScript = resetObj.transform.GetChild(i).GetComponent<PlayerCubeScript>();

            if (playerCubeScript.isPlaced) playerCubeScript.isPlaced = false;

            var holderCubeScript = playerCubeScript.stickingCubeObjRef.GetComponent<HolderCubeScript>();

            if (holderCubeScript.isFilled) holderCubeScript.isFilled = false;
            if (holderCubeScript.staying) holderCubeScript.staying = false;
            if (holderCubeScript.once) holderCubeScript.once = false;

            //print("I am being Called");
        }

        print("resetcalling");
        resetObj.GetComponent<CubesGroupScript>().AutoResetPositionFun();
    }

    public void FunWaitCall(GameObject obj, Vector3 posi)
    {
        ObjMoving(obj, posi);
        //ObjMovingFunCall(obj,posi);
    }

    public void AutoCompleteFunc()
    {
        _numberVal = 0;
        //autoFunCall = true;
        //if(wordNoToComplete >= completeWordCubesList.Count) return;
        // seqcall();
        //AutoFunCall();
        LetterObjectsCheck();
    }


    /*public void AutoFunCall()
    {
        //print("Function calling here");
        var cubesGroups = completeWordCubesList[wordNoToComplete].completeWordCubeGroup;
        ClearAutoWordArea(cubesGroups);
        DOVirtual.DelayedCall(0.25f, () =>
        {
            AutoFunCallMoving();
        });
        /*var cubeGroupsCount = cubesGroups.Count;
        //Debug.Log("CubeGroups Count " + cubesGroups.Count);
        //var seq = DOTween.Sequence();
        for (int i = 0; i < cubeGroupsCount; i++)
        {
            if (i < cubeGroupsCount)
            {
                print(_numberVal);
                var cube = cubesGroups[i];

                for (int k = 0; k < cube.transform.childCount; k++)
                {
                    var cubeChild = cube.transform.GetChild(k);
                    var position = cubeChild.transform.position;
                    position = new Vector3(position.x, position.y, -3.5f);
                    cubeChild.GetComponent<PlayerCubeScript>().isPlaced = false;
                    cubeChild.transform.position = position;
                }

                //Debug.Log(cube.transform.position + "Position");

                ObjMoving(cube,completeWordPositionsList[wordNoToComplete].completeWordCubePositionGroup[i]);
                //_numberVal++;
            }
            else
            {
                DOVirtual.DelayedCall(0.25f, () =>
                {
                    scriptOff = false;
                    /*autoWordClick = false;
                    UIManagerScript.Instance.AutoButtonActiveFun();#2#
                });

            }
        }#1#
    }*/
    /*public void AutoFunCallMoving()
    {
        var cubesGroups = completeWordCubesList[wordNoToComplete].completeWordCubeGroup;
        var cubeGroupsCount = cubesGroups.Count;
        for (int i = 0; i <= cubeGroupsCount; i++)
        {
            if (i < cubeGroupsCount)
            {
 //               print(_numberVal);
                var cube = cubesGroups[i];

                for (int k = 0; k < cube.transform.childCount; k++)
                {
                    var cubeChild = cube.transform.GetChild(k);
                    var position = cubeChild.transform.position;
                    position = new Vector3(position.x, position.y, -3.5f);
                    cubeChild.GetComponent<PlayerCubeScript>().isPlaced = false;
                    cubeChild.transform.position = position;
                }
                ObjMoving(cube,completeWordPositionsList[wordNoToComplete].completeWordCubePositionGroup[i]);
            }
            else
            {
                DOVirtual.DelayedCall(0.25f, () =>
                {
                    scriptOff = false;
                    /*autoWordClick = false;
                    UIManagerScript.Instance.AutoButtonActiveFun();#1#
                });

            }
        }
    }*/
    /*public void seqcall()
    {
        //scriptOff = true;
        print("Function calling here");
        var cubesGroups = completeWordCubesList[wordNoToComplete].completeWordCubeGroup;
        ClearAutoWordArea(cubesGroups);
        var cubeGroupsCount = cubesGroups.Count;
        Debug.Log("CubeGroups Count " + cubesGroups.Count);
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            //Debug.Log("CubeGroups Count " + cubesGroups.Count);
            if (_numberVal < cubeGroupsCount)
            {
                print(_numberVal);
                var cube = cubesGroups[0];

                for (int k = 0; k < cube.transform.childCount; k++)
                {
                    var cubeChild = cube.transform.GetChild(k);
                    var position = cubeChild.transform.position;
                    position = new Vector3(position.x, position.y, -3.5f);
                    cubeChild.GetComponent<PlayerCubeScript>().isPlaced = false;
                    cubeChild.transform.position = position;
                }

                //Debug.Log(cube.transform.position + "Position");

                ObjMoving(cube,completeWordPositionsList[wordNoToComplete].completeWordCubePositionGroup[0]);
                _numberVal++;
            }
            else
            {
                //print("call in sequence One");
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    scriptOff = false;
                });
                DOVirtual.DelayedCall(0.7f, () =>
                {
                    autoWordClick = false;
                    UIManagerScript.Instance.AutoButtonActiveFun();
                });

            }
        });
        seq.AppendInterval(1f);
        seq.SetLoops(cubeGroupsCount + 1);
    }*/

    /*public void ObjMoving(GameObject obj,Vector3 pos)
    {
        obj.transform.DOMove(pos, .5f).SetEase(Ease.Linear).OnStart(() =>
        {
            obj.GetComponent<CubesGroupScript>().canPlaceNow = false;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                obj.transform.GetChild(i).GetComponent<Collider>().enabled = false;
                if (obj.transform.GetChild(i).GetComponent<PlayerCubeScript>())
                {
                    if (obj.transform.GetChild(i).GetComponent<PlayerCubeScript>().stickingCubeObjRef)
                    {
                        var holdercubeObj = obj.transform.GetChild(i).GetComponent<PlayerCubeScript>()
                            .stickingCubeObjRef
                            .GetComponent<HolderCubeScript>();
                        holdercubeObj.FunCallingObj();
                    }

                }
            }
            var coll=obj.GetComponents<Collider>().ToList();
            if (coll.Count > 1)
            {
                for (int i = 0; i < coll.Count; i++)
                {
                    coll[i].enabled = false;
                    //coll[i].GetComponent<PlayerCubeScript>().letterDone = true;
                }
            }
            else
            {
                coll[0].enabled = false;
                //coll[0].GetComponent<PlayerCubeScript>().letterDone = true;
            }
        }).OnComplete(() =>
        {
            obj.GetComponent<CubesGroupScript>().canPlaceNow = true;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                obj.transform.GetChild(i).GetComponent<Collider>().enabled = true;
            }
            var coll=obj.GetComponents<Collider>().ToList();
            if (coll.Count > 1)
            {
                for (int i = 0; i < coll.Count; i++)
                {
                    coll[i].enabled = true;
                }
            }
            else
            {
                coll[0].enabled = true;
            }

            CollisionOff(obj);
        });
        //var posi=pos.transform.position
    }*/
    public void ObjMoving(GameObject obj, Vector3 pos)
    {
        obj.GetComponent<CubesGroupScript>().canPlaceNow = false;
        obj.GetComponent<CubesGroupScript>().canNotPlace = true;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).GetComponent<Collider>().enabled = false;
            if (obj.transform.GetChild(i).GetComponent<PlayerCubeScript>())
            {
                if (obj.transform.GetChild(i).GetComponent<PlayerCubeScript>().stickingCubeObjRef)
                {
                    var holdercubeObj = obj.transform.GetChild(i).GetComponent<PlayerCubeScript>()
                        .stickingCubeObjRef
                        .GetComponent<HolderCubeScript>();
                    holdercubeObj.FunCallingObj();
                }
            }

            if (obj.transform.GetChild(i).GetComponent<PlayerCubeScript>())
            {
                if (obj.transform.GetChild(i).transform.localPosition !=
                    obj.transform.GetChild(i).GetComponent<PlayerCubeScript>().startPos)
                {
                    obj.transform.GetChild(i).transform.localPosition =
                        obj.transform.GetChild(i).GetComponent<PlayerCubeScript>().startPos;
                }
            }
        }

        var coll = obj.GetComponents<Collider>().ToList();
        if (coll.Count > 1)
        {
            for (int i = 0; i < coll.Count; i++)
            {
                coll[i].enabled = false;
                //coll[i].GetComponent<PlayerCubeScript>().letterDone = true;
            }
        }
        else
        {
            coll[0].enabled = false;
            //coll[0].GetComponent<PlayerCubeScript>().letterDone = true;
        }

        var seq = DOTween.Sequence();
        seq.Append(obj.transform.DOMove(pos, 0.5f).SetEase(Ease.Linear));
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                obj.transform.GetChild(i).GetComponent<Collider>().enabled = true;
            }

            var coll = obj.GetComponents<Collider>().ToList();
            if (coll.Count > 1)
            {
                for (int i = 0; i < coll.Count; i++)
                {
                    coll[i].enabled = true;
                }
            }
            else
            {
                coll[0].enabled = true;
            }

            obj.GetComponent<CubesGroupScript>().canPlaceNow = true;
            obj.GetComponent<CubesGroupScript>().canNotPlace = false;
            CollisionOff(obj);
        });
        //var posi=pos.transform.position
    }

    public void CollisionOff(GameObject obj)
    {
        DOVirtual.DelayedCall(4.25f, () =>
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                if (obj.transform.GetChild(i).GetComponent<Collider>().enabled)
                    obj.transform.GetChild(i).GetComponent<Collider>().enabled = false;
            }
        }, false);
    }

    private bool autoFunCall = false;
    private int blocknum;

    private void WordsFallingAfterWinFun()
    {
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            if (blocknum < _allCubeObjects.Count)
            {
                _allCubeObjects[blocknum].AddComponent<Rigidbody>().mass = 0.6f;
                _allCubeObjects[blocknum].GetComponent<Collider>().enabled = false;
                /*_allCubeObjects[blocknum].transform
                    .DOScale(
                        new Vector3(_allCubeObjects[blocknum].transform.localScale.x + .5f,
                            _allCubeObjects[blocknum].transform.localScale.y + .7f,
                            _allCubeObjects[blocknum].transform.localScale.z + 0.7f), 1f);*/
                _allCubeObjects[blocknum].transform.GetChild(0).GetComponent<TextMeshPro>().enabled = false;
                _allCubeObjects[blocknum].transform.GetChild(2).GetComponent<TextMeshPro>().enabled = true;
                _allCubeObjects[blocknum].transform.GetChild(3).GetComponent<SpriteRenderer>().enabled = false;

                _allCubeObjects[blocknum].transform.GetComponent<Rigidbody>()
                    .AddForce(new Vector3(Random.Range(-200, 200), Random.Range(350, 400) * 2, 0), ForceMode.Acceleration);
                _allCubeObjects[blocknum].transform
                    .DORotate(new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180)),
                        0.05f)
                    .SetEase(Ease.InFlash);
                _allCubeObjects[blocknum].transform.GetComponent<Rigidbody>()
                    .AddTorque(new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50)));
                if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("CubesBlast");

                blocknum++;
            }
            /*else
            {
                DOVirtual.DelayedCall(1.25f, () => { UI.NextMoveFun(); },false);
            }*/
        });
        seq.AppendInterval(0.05f);
        seq.SetLoops(_allCubeObjects.Count + 1);
    }

    public void ButtonsTurnOffFun()
    {
        if (UI.restartButton.interactable)
            UI.restartButton.interactable = false;
        if (UI.hintButton.interactable)
            UI.hintButton.interactable = false;
        if (UI.autoWordButton.interactable)
            UI.autoWordButton.interactable = false;
        if (UI.emojiRevealButton.interactable)
            UI.emojiRevealButton.interactable = false;
        if (MonitizationScript.instance.giftImage.GetComponent<Button>().interactable)
            MonitizationScript.instance.giftImage.GetComponent<Button>().interactable = false;
        if (MonitizationScript.instance.giftImage.GetComponent<Button>().interactable)
            MonitizationScript.instance.giftImage.GetComponent<Button>().interactable = false;
    }

    public bool wordTouch;
    public bool autoWordClick;

    public void ClearAutoWordArea(List<GameObject> cubeGroups)
    {
        for (int i = 0; i < cubeGroups.Count; i++)
        {
            var childCount = cubeGroups[i].transform.childCount;
            for (int j = 0; j < childCount; j++)
            {
                var child = cubeGroups[i].transform.GetChild(j).GetComponent<PlayerCubeScript>();
                //refNumber.Add(child.checknumber);
                if (ClearWordResetSticking(child.checknumber)) break;
            }
        }
    }

    private bool ClearWordResetSticking(int refNumber)
    {
        for (int i = 0; i < stickingCubes.Count; i++)
        {
            var cubes = stickingCubes[i].transform;
            for (int j = 0; j < cubes.childCount; j++)
            {
                var holdersCube = cubes.GetChild(j);
                if (ResetCubeFunc(holdersCube, refNumber)) return true;
            }
        }

        return false;
    }

    private bool ResetCubeFunc(Transform holdersCube, int refNumber)
    {
        var holderCube = holdersCube.GetComponent<HolderCubeScript>();

        if (!holderCube.isFilled) return false;
        if (holderCube.checkNumberRef == refNumber)
        {
            var individualCube = holderCube.objRef.transform;
            var cubeToReset = individualCube.parent;
            cubeToReset.transform.GetComponent<CubesGroupScript>().canCheckForPlacement = false;

            if (individualCube.GetComponent<PlayerCubeScript>().checknumber != holderCube.checkNumberRef)
            {
                var cubeVec = cubeToReset.transform.position;
                var cubePos = new Vector3(cubeVec.x, cubeVec.y, -5f);
                cubeToReset.transform.position = cubePos;

                // if (holderCube.isFilled)
                //     holderCube.isFilled = false;

                for (int i = 0; i < cubeToReset.childCount; i++)
                {
                    var playerCubeScript = cubeToReset.transform.GetChild(i).GetComponent<PlayerCubeScript>();

                    if (playerCubeScript.isPlaced) playerCubeScript.isPlaced = false;

                    var holderCubeScript = playerCubeScript.stickingCubeObjRef.GetComponent<HolderCubeScript>();

                    if (holderCubeScript.isFilled) holderCubeScript.isFilled = false;
                    if (holderCubeScript.staying) holderCubeScript.staying = false;
                    if (holderCubeScript.once) holderCubeScript.once = false;

                    //print("I am being Called");
                }

                cubeToReset.GetComponent<CubesGroupScript>().AutoResetPositionFun();
                return true;
            }
        }

        return false;
    }

    public bool cameraMoving;

//#endregion
    [System.Serializable]
    public class WordData
    {
        public List<GameObject> wordsDataLists;
    }

    public List<GameObject> imageRevel;
}