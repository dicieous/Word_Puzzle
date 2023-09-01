using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CubesGroupScript : MonoBehaviour
{
	private Vector3 _initPos;

	//private PlayerCubeScript _playerCubeScript;

	[SerializeField] private List<GameObject> childObjects;
	[SerializeField] private LayerMask mask;
	[SerializeField] private ParticleSystem dustFX;

	private Vector3[] initialPos;
	private Vector3[] initialDir;
	private Vector3 _offset;

	private bool isFilledC;
	private bool canMove = true;
	private bool canReset = true;

	void Start()
	{
		_initPos = transform.position;

		initialPos = new Vector3[childObjects.Count];
		for (int i = 0; i < childObjects.Count; i++)
		{
			initialPos[i] = childObjects[i].transform.position;
			//Debug.Log("Initial Position get " + initialPos[i]);
		}

		// if ((PlayerPrefs.GetInt("Level", 1) == 1))
		// {
		// 	GameManager.Instance.ShowTheText();
		// }
	}


	private void OnMouseDown()
	{
		if (UIManagerScript.Instance.endScreen.activeInHierarchy) return;

		var position1 = transform.position;
		var position = new Vector3(position1.x, position1.y, position1.z);
		_offset = position - MouseWorldPosition();

		if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		//Debug.Log("Vibrate on mouse Down");


		//_oldPos = position;
		/*var newScale = new Vector3(1f, 1f, 1f);
		transform.DOScale(newScale, 0.05f).SetEase(Ease.OutBounce);*/
	}

	private void OnMouseDrag()
	{
		if (canMove)
		{
			if (UIManagerScript.Instance.endScreen.activeInHierarchy) return;
		
			var position1 = transform.position;
			position1 = MouseWorldPosition() + _offset;
			position1 = new Vector3(position1.x, position1.y, -3.5f);
			transform.position = position1;

			if (!CheckIfHittingCubeGroup()) return;
			foreach (var child in childObjects)
			{
				var position = child.transform.position;
				position = new Vector3(position.x, position.y, -3.5f);
				child.GetComponent<PlayerCubeScript>().isPlaced = false;
				child.transform.position = position;
			}
		}
		

		//Raycast to check if you're hitting the CubeGroup Collider and take the childObjects back in the cubeGroup Collider

		// RaycastHit hit;
		// var ray = Camera.main!.ScreenPointToRay (Input.mousePosition);
		// if (Physics.Raycast (ray, out hit)) {
		// 	if (hit.transform.CompareTag("Cube_Group"))
		// 	{
		// 		foreach (var child in childObjects)
		// 		{
		// 			var position = child.transform.position;
		// 			position = new Vector3(position.x, position.y, -2f);
		// 			child.GetComponent<PlayerCubeScript>().isPlaced = false;
		// 			child.transform.position = position;
		// 		}
		// 		
		// 		//Debug.Log("PushUp");
		// 	}
		// }


		
	}

	private void OnMouseUp()
	{
		/*if ((transform.position.y > GameManager.Instance.yMaxLimit ||
			 transform.position.y < GameManager.Instance.yMinLimit) ||
			(transform.position.x > GameManager.Instance.xMaxLimit ||
			 transform.position.x < GameManager.Instance.xMinLimit))
		{
			ResetPosition();
		}*/


		/*var newScale = new Vector3(0.9f, .9f, 1f);
		//transform.localScale = (newScale);
		transform.DOScale(newScale, 0f);
		*/
	}

	Vector3 MouseWorldPosition()
	{
		var mouseScreenPos = Input.mousePosition;
		mouseScreenPos.z = Camera.main!.WorldToScreenPoint(transform.position).z;
		return Camera.main.ScreenToWorldPoint(mouseScreenPos);
	}

	private void Update()
	{
		//Debug.Log("isFilledC Value "+ isFilledC);
		if (!Input.GetMouseButton(0) && isFilledC)
		{
			ResetPosition();
		}
		CondToAttachCubesInGrid();
	}

	//To reset the Position of the Objects
	private void ResetPosition()
	{
		//transform.position = _initPos;
		if (canReset)
		{
			canReset = false;
			transform.DOMove(_initPos, 0.2f).SetEase(Ease.Flash).OnComplete(() =>
			{
				canReset = true;
			});
			
			for (int i = 0; i < childObjects.Count; i++)
			{
				childObjects[i].transform.DOMove(initialPos[i], 0.2f).SetEase(Ease.Flash);
				//childObjects[i].transform.position = initialPos[i];
				//Debug.Log($"Initial Position set {initialPos[i]}");
			}
		}
		

		/*var newScale = new Vector3(0.9f, .9f, .9f);
		transform.DOScale(newScale, 0.2f);*/

		//Debug.Log("ResetPos");
		//Debug.Log("isFilled Value "+ PlayerCubeScript.instance.isFilled);
		//if(SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ResetPositionMG");
		//if(SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);

		
		// for (int i = 0; i < childObjects.Count; i++)
		// {
		// 	//childObjects[i].transform.DOMove(initialPos[i], 0.2f);
		// 	childObjects[i].transform.position = initialPos[i];
		// 	//Debug.Log($"Initial Position set {initialPos[i]}");
		// }
	}

	//To check if you are hitting letters Group Collider
	private bool CheckIfHittingCubeGroup()
	{
		var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hit))
		{
			if (hit.transform.CompareTag("Cube_Group"))
			{
				return true;
			}
		}

		return false;
	}

	//To Check if you're hitting the Grid Cube
	private bool CheckIfHitting(Transform child)
	{
		var rayOrigin = child.transform.position;
		var rayDirection = child.transform.TransformDirection(Vector3.forward);

		var rayCheck = Physics.Raycast(rayOrigin, rayDirection, Mathf.Infinity, mask);

		if (rayCheck)
		{
			return true;
		}

		return false;
	}
	
	private bool CheckIfAllHitting()
	{
		for (int i = 0; i < childObjects.Count; i++)
		{
			var child = childObjects[i].transform;
			if(!CheckIfHitting(child)) return false;
			var hitInfo = RayCastInfo(child);
			if (hitInfo.collider.GetComponent<HolderCubeScript>().isFilled) return false;
		}

		print("all are hitting");
		return true;
	}

	//To get position of the Cube Grid Cubes
	private RaycastHit RayCastInfo(Transform child)
	{
		var rayOrigin = child.transform.position;
		var rayDirection = child.transform.TransformDirection(Vector3.forward);

		var rayCheck = Physics.Raycast(rayOrigin, rayDirection, out var hitInfo, Mathf.Infinity, mask);
		return hitInfo;
	}

	//To Attach the Cube to the Grid Cube
	private void AttachTheObj(RaycastHit hitInfo, Transform child)
	{
		isFilledC = hitInfo.collider.GetComponent<HolderCubeScript>().isFilled;

		child.GetComponent<PlayerCubeScript>().isPlaced = true;
		Debug.Log("isFilled Value " + isFilledC);

		if (!isFilledC)
		{
			Debug.Log("Check if hitting");

			if (Input.GetMouseButtonUp(0))
			{
				var position = hitInfo.transform.position;
				child.transform.position = position;
				var vector3 = transform.position;
				vector3.z = position.z;
				
				Instantiate(dustFX, position, Quaternion.identity);
				
				if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
				if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Pop");

				DOVirtual.DelayedCall(.05f, () =>
				{
					// if (PlayerPrefs.GetInt("Level", 1) == 1)
					// {
					// 	GameManager.Instance.ShowTheText();
					// }
				});
			}
		}
	}

	// ReSharper disable Unity.PerformanceAnalysis
	private void CondToAttachCubesInGrid()
	{
		if (CheckIfAllHitting())
		{
			for (int i = 0; i < childObjects.Count; i++)
			{
				var child = childObjects[i].transform;
				if (CheckIfHitting(child))
				{
					var hitInfo = RayCastInfo(child);
					if (Input.GetMouseButtonUp(0))
					{
						Debug.Log("Show");
						AttachTheObj(hitInfo, child);
					}

				}
			}
			
			/*// for (int i = 0; i < childObjects.Count; i++)
			// {
			// 	var child = childObjects[i].transform;
			// 	if (CheckIfHitting(child))
			// 	{
			// 		var hitInfo = RayCastInfo(child);
			// 		if (Input.GetMouseButtonUp(0))
			// 		{
			// 			Debug.Log("Show");
			// 			AttachTheObj(hitInfo, child);
			// 		}
			// 	
			// 	}
			// 	else if (!CheckIfHitting(child) && !child.GetComponent<PlayerCubeScript>().isPlaced)
			// 	{
			// 		if (Input.GetMouseButtonUp(0))
			// 		{
			// 			ResetPosition();
			// 		}
			// 	}
			// }

			// for (int i = 0; i < childObjects.Count; i++)
			// {
			// 	var child = childObjects[i].transform;
			// 	if (!CheckIfHitting(child) && !child.GetComponent<PlayerCubeScript>().isPlaced)
			// 	{
			//
			// 		if (Input.GetMouseButtonUp(0))
			// 		{
			// 			ResetPosition();
			// 		}
			//
			// 	}
			//
			// 	if (CheckIfHitting(child))
			// 	{
			// 		
			// 		var hitInfo = RayCastInfo(child);
			// 		Debug.Log("Show");
			// 		AttachTheObj(hitInfo, child);
			// 	}*/
		}
		else
		{
			for (int i = 0; i < childObjects.Count; i++)
			{
				var child = childObjects[i].transform;
				if (!child.GetComponent<PlayerCubeScript>().isPlaced)
				{
					if (Input.GetMouseButtonUp(0))
						ResetPosition();
				}

			}
		}
	}
}