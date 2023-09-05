using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	
	public static GameManager Instance;
	
	public List<string> answers;
	public List<Color> rowColor;

	public List<WordData> wordList;
    
    [HideInInspector]
	public bool ScriptOff;
	[Space(10)]
	public int rowsInGrid;
	public int colInGrid;

	[HideInInspector] public int rowfilled;
	[HideInInspector] public List<GameObject> fXRowWords;
    [HideInInspector] public bool downCheck;
	[Space(10)]
	private List<List<GameObject>> letterCubeWord = new List<List<GameObject>>();

	public List<GameObject> Cube_Groups;

	[SerializeField]
	private List<GameObject> _allCubeObjects;
	[Space(10)]
	[SerializeField] private List<GameObject> hintCubesHolder;

	[Space(10)]
	[SerializeField] private GameObject starFX;

	[SerializeField] private GameObject complementPrefab;

	[SerializeField] private Transform instPos;

	private bool canInstantiate = true;

	private float instTime = 1f;

	[Space(10)]

	//The min and max value of the Grid where cube are Attached
	// public float yMaxLimit;
	//
	// public float xMinLimit;
	// public float yMinLimit;
	// public float xMaxLimit;

	private bool[] wordCompleted;
	public bool levelCompleted = false;

	private int wordsMade = 0;

	private UIManagerScript UI;

	private void Awake()
	{
		if (!Instance) Instance = this;
	}

	private void Start()
	{
		Application.targetFrameRate = 120;
		InitializeGrid();

		UI = UIManagerScript.Instance;
		
		wordCompleted = new bool[rowsInGrid];
		InitializeWordComplete();
		for (int i = 0; i < rowsInGrid; i++)
		{
			var wordData = new WordData
			{
				wordsDataLists = new List<GameObject>()
			};
			wordList.Add(wordData);
		}

		starFX = UI.starparticleEffect;
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
			for (int col = 0; col < colInGrid; col++)
			{
				newRow.Add(null);
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

//#endregion

// ReSharper disable Unity.PerformanceAnalysis
	void Update()
	{
		MakeAndCheckWord();

		instTime -= Time.deltaTime;
		if (instTime < 0)
		{
			canInstantiate = true;
		}
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
	public void MovingSeq(int row, int columCount=0)
	{
        if (!removing)
        {
            for (int i = 0; i < colInGrid; i++)
            {
                if (!wordList[row].wordsDataLists.Contains(letterCubeWord[row][i].gameObject) && wordList[row].wordsDataLists.Count != colInGrid+1)
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
                    /*if (!ScriptOff)
                    {
                        ScriptOff = true;
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
		if (ScriptOff)
		{
			DOVirtual.DelayedCall(.7f, () =>
			{
				//print("FunctionCall");
				ScriptOff = false;
			});
		}
	}
	/*public void collidercheck(int rownum)
	{
		for (int col = 0; col < colInGrid; col++)
		{
			letterCubeWord[rownum][col].GetComponent<Collider>().enabled = false;
		}

		DOVirtual.DelayedCall(0.5f, () =>
		{
			for (int col = 0; col < colInGrid; col++)
			{
				letterCubeWord[rownum][col].GetComponent<Collider>().enabled = true;
			}
		});
	}*/

	private void MakeAndCheckWord()
	{
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
						Instantiate(complementPrefab, instPos.position, Quaternion.identity);
						instTime = 1f;
						canInstantiate = false;
					}
					
					if (!ScriptOff)
					{
						//ScriptOff = true;
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
					//print("row number" + row);
					
//					Debug.Log("WordsMade " + wordsMade);
				}
			}
			else if (!IsRowFull(row) && wordCompleted[row] && !levelCompleted)
			{
				if (!removing)
				{
					//removing = true;
					RearangeValues(row);
					print("Removing one");
				}
				
				wordCompleted[row] = false;
				wordsMade--;
				//print("row number"+ row);
			}
		}

		if (wordsMade >= answers.Count && IsGridFull() && !levelCompleted)
		{
//			print("Win");
			levelCompleted = true;
			UI.restartButton.interactable = false;
			UI.hintButton.interactable = false;
			//DestroyBlocks();
			DOVirtual.DelayedCall(.75f, () =>
			{
				UI.WinPanelActive();
                BlockSeqCall();
                //Debug.Log("LevelComplete");
            });
		}
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
    public void AllBlocksColoredAtaTimeFun(int row,int columCount = 0)
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
		// foreach (var cube in Cube_Groups)
		// {
		// 	cube.GetComponent<CubesGroupScript>().ResetPosition();
		// }

		var loadedScene = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(loadedScene);
	}
	public void ShowTheText()
	{
		foreach (var cube in hintCubesHolder)
		{
			int count = cube.GetComponentsInChildren<TextMeshPro>().Length;
			if (count != 0)
			{
				if (!IsInstantiated(cube, count))
				{
					Instantiate(starFX, cube.transform.position, Quaternion.identity);
					if (PlayerPrefs.GetInt("Level", 1) > 1)
					{
						CoinManager.instance.HintReduce();
					}
					if (PlayerPrefs.GetInt("Level", 1) == 1) UIManagerScript.Instance.HelpHand();
//					print("instantiated");
				}

				for (int j = 0; j < count; j++)
				{
					var obj = cube;
					obj.GetComponentsInChildren<TextMeshPro>()[j].DOFade(217f/255f, 2f);
					
					obj.GetComponentsInChildren<HighlightTextScript>()[j].isVisible = true;
				}

				break;
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
			Time.timeScale = 2f;
			BlockSeq();
		});
	}

	private int blocknum;
	public void BlockSeq()
	{
		var seq = DOTween.Sequence();
		seq.AppendCallback(() =>
		{
			if (blocknum < _allCubeObjects.Count)
			{
				_allCubeObjects[blocknum].AddComponent<Rigidbody>();
				_allCubeObjects[blocknum].GetComponent<Collider>().enabled = false;
				_allCubeObjects[blocknum].transform
					.DOScale(
						new Vector3(_allCubeObjects[blocknum].transform.localScale.x + .5f,
							_allCubeObjects[blocknum].transform.localScale.y + .7f,
							_allCubeObjects[blocknum].transform.localScale.z + 0.7f), 1f);
				_allCubeObjects[blocknum].transform.GetChild(0).GetComponent<TextMeshPro>().enabled = false;
				_allCubeObjects[blocknum].transform.GetChild(2).GetComponent<TextMeshPro>().enabled = true;
				_allCubeObjects[blocknum].transform.GetComponent<Rigidbody>().AddForce(new Vector3(0, Random.Range(350, 400), -150));
				_allCubeObjects[blocknum].transform.DORotate(new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180)), 0.05f)
					.SetEase(Ease.InFlash);
				_allCubeObjects[blocknum].transform.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50)));
				if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("CubesBlast");
				
				blocknum++;
			}
			else
			{
				DOVirtual.DelayedCall(1.25f, () =>
				{
					UI.NextMoveFun();
				});

			}
		});
		seq.AppendInterval(0.1f);
		seq.SetLoops(_allCubeObjects.Count+1);
	}
//#endregion
[System.Serializable]
public class WordData
{
	public List<GameObject> wordsDataLists;
}
}