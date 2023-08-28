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

	[Space(10)]
	public int rowsInGrid;

	public int colInGrid;

	[Space(10)]
	private List<List<string>> Word = new List<List<string>>();

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
		if(!Instance) Instance = this;
	}

	private void Start()
	{
	InitializeGrid();

		UI = UIManagerScript.Instance;
		UI.endScreen.SetActive(false);

		wordCompleted = new bool[rowsInGrid];
		InitializeWordComplete();
		
		
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
			List<string> newRow = new List<string>();
			for (int col = 0; col < colInGrid; col++)
			{
				newRow.Add("NV");
			}

			Word.Add(newRow);
		}
	}

	public void AddWords(int row, int col, string value)
	{
		Word[row][col] = value;
	}

	public void RemoveWords(int row, int col)
	{
		Word[row][col] = "NV";
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
				if (Word[rowIndex][col] == "NV")
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
					if (Word[row][col] == "NV")
					{
						return false;
					}
				}
			}

			return true;
		}

	

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
						madeword += Word[row][col];
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

						wordCompleted[row] = true;
						wordsMade++;
						Debug.Log("WordsMade " + wordsMade);
					}
				}
			}

			if (wordsMade >= answers.Count && IsGridFull()&&!levelCompleted)
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
					obj.GetComponentsInChildren<TextMeshPro>()[j].DOFade(.5f, 2f);
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
}