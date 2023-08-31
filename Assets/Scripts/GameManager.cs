using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
	
	public static GameManager Instance;
	
	public List<string> answers;
	public List<Color> rowColor;

	public List<WordData> wordList;
	
	[Space(10)]
	public int rowsInGrid;

	public int colInGrid;

	[HideInInspector] public int rowfilled;
	[HideInInspector] public List<GameObject> fXRowWords;
	
	[HideInInspector]
	public bool grabwords = false;

	[Space(10)]
	//private List<List<string>> Word = new List<List<string>>();
	private List<List<GameObject>> letterCubeWord = new List<List<GameObject>>();

	public List<GameObject> Cube_Groups;

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
	public float yMaxLimit;

	public float xMinLimit;
	public float yMinLimit;
	public float xMaxLimit;

	private bool[] wordCompleted;
	private bool levelCompleted = false;

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
		UI.endScreen.SetActive(false);

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
	}

#region Initialize Grid Words

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

#endregion


	void Update()
	{
		MakeAndCheckWord();

		instTime -= Time.deltaTime;
		if (instTime < 0)
		{
			canInstantiate = true;
		}
	}


#region Make And Check Words

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
			//letterCubeWord[row][i].transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.white;
			wordList[row].wordsDataLists[i].transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.white;
		}

		DOVirtual.DelayedCall(0.1f, () =>
		{
			for (int i = 0; i < colInGrid; i++)
			{
				wordList[row].wordsDataLists.RemoveAt(0);
			}
			
		});
	}
	public void MovingSeq(int row, int columCount = 0)
	{
		var seq = DOTween.Sequence();
		seq.AppendCallback(() =>
		{
			letterCubeWord[row][columCount].transform.GetChild(1).transform.DOScale(new Vector3(30f, 30f, 15f), 0.2f).SetEase(Ease.InOutBounce).SetLoops(2, LoopType.Yoyo);
			letterCubeWord[row][columCount].transform.GetChild(1).GetComponent<MeshRenderer>().material.color = rowColor[row];
			letterCubeWord[row][columCount].transform.GetChild(0).transform.DOScale(new Vector3(1.75f, 1.75f, 2f), 0.2f).SetEase(Ease.InOutBounce).SetLoops(2, LoopType.Yoyo);
			if (wordList.Count != 0)
			{
				var gm = letterCubeWord[row][columCount].gameObject;
				wordList[row].wordsDataLists.Add(gm);
			}

			columCount++;
		});
		seq.AppendInterval(0.1f);
		seq.SetLoops(colInGrid);
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
					Debug.Log(madeword + " The word you made");
					if (canInstantiate)
					{
						Instantiate(complementPrefab, instPos.position, Quaternion.identity);
						instTime = 1f;
						canInstantiate = false;
					}

					
					MovingSeq(row);
					print("row number" + row);
					wordCompleted[row] = true;
					wordsMade++;
					Debug.Log("WordsMade " + wordsMade);
				}
			}
			else if (!IsRowFull(row) && wordCompleted[row])
			{
				print("row number"+ row);
				RearangeValues(row);
				wordCompleted[row] = false;
			}
		}

		if (wordsMade >= answers.Count && IsGridFull() && !levelCompleted)
		{
			print("Win");
			levelCompleted = true;

			DOVirtual.DelayedCall(2f, () =>
			{
				UI.endScreen.SetActive(true);
				Debug.Log("LevelComplete");
			});
		}
	}

#endregion


	public void ResetScreen()
	{
		// foreach (var cube in Cube_Groups)
		// {
		// 	cube.GetComponent<CubesGroupScript>().ResetPosition();
		// }

		var loadedScene = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(loadedScene);
	}

#region HintLogic

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
				}

				for (int j = 0; j < count; j++)
				{
					var obj = cube;
					obj.GetComponentsInChildren<TextMeshPro>()[j].DOFade(.2f, 2f);
					
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

#endregion
[System.Serializable]
public class WordData
{
	public List<GameObject> wordsDataLists;
}
}