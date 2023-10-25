using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CubesGroupScript : MonoBehaviour
{
	private Vector3 _initPos;


	[SerializeField] private List<GameObject> childObjects;
	[SerializeField] private LayerMask mask;
	[SerializeField] private ParticleSystem dustFX;

	private Vector3[] initialPos;
	private Vector3[] initialDir;
	private Vector3 _offset;

	private bool isFilledC;
	//private bool canMove = true;
	private bool canReset = true;
    private bool canCheckForPlacement = true;

	public int number;
	void Start()
	{
		_initPos = transform.position;

		initialPos = new Vector3[childObjects.Count];
		for (int i = 0; i < childObjects.Count; i++)
		{
			initialPos[i] = childObjects[i].transform.position;
			//Debug.Log("Initial Position get " + initialPos[i]);
		}
		
		if(GameManager.Instance.levelTypeChanged)
        {
            transform.GetChild(0).GetComponent<PlayerCubeScript>().anim = true;
        }
	}

    public void AnimSeq()
    {
        var num = transform.GetComponents<Collider>().Length;
        for (int i = 0; i < num; i++)
        {
            transform.GetComponents<Collider>()[i].enabled = false;
        }
        CrctAnimSeqCall();
    }

    private int countnum;
    private void CrctAnimSeqCall()
    {
        countnum = 0;
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            childObjects[countnum].transform.GetChild(1).GetComponent<MeshRenderer>().materials[0]
                .color = CoinManager.instance.greenColor; 
            childObjects[countnum].transform.GetChild(1).GetComponent<MeshRenderer>().materials[1]
                .color = CoinManager.instance.greenColor;
            childObjects[countnum].transform.GetChild(0).transform
                .DOScale(new Vector3(1.75f, 1.75f, 2f), 0.1f)
                .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            childObjects[countnum].transform.GetChild(1).transform
                .DOScale(new Vector3(20f, 30f, 15f), 0.1f)
                .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            countnum++;
        });
        seq.AppendInterval(0.15f);
        seq.SetLoops(childObjects.Count);
    }
    
    public void WrongAnimSeq()
    {
        countnum = 0;
        var num = transform.GetComponents<Collider>().Length;
        for (int i = 0; i < num; i++)
        {
            transform.GetComponents<Collider>()[i].enabled = false;
        }
        DOVirtual.DelayedCall(0.15f, () =>
        {
            WrongBackAnimSeq();
        });
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            childObjects[countnum].transform.GetChild(1).GetComponent<MeshRenderer>().materials[0]
                .color = CoinManager.instance.redColor; 
            childObjects[countnum].transform.GetChild(1).GetComponent<MeshRenderer>().materials[1]
                .color = CoinManager.instance.redColor;
            childObjects[countnum].transform.GetChild(0).transform
                .DOScale(new Vector3(1.75f, 1.75f, 2f), 0.1f)
                .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            childObjects[countnum].transform.GetChild(1).transform
                .DOScale(new Vector3(20f, 30f, 15f), 0.1f)
                .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            countnum++;
        });
        seq.AppendInterval(0.05f);
        seq.SetLoops(childObjects.Count);
        /*for (int i = 0; i < childObjects.Count; i++)
        {
            childObjects[i].transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.red;
        }
        DOVirtual.DelayedCall(0.15f, () =>
        {
            for (int i = 0; i < childObjects.Count; i++)
            {
                childObjects[i].transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.white;
                //childObjects[i].GetComponent<MeshRenderer>().material.
            }
        });*/
    }

    private int countBackNum;
    public void WrongBackAnimSeq()
    {
        countBackNum = 0;
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            if (countBackNum == childObjects.Count)
            {
                var num = transform.GetComponents<Collider>().Length;
                for (int i = 0; i < num; i++)
                {
                    transform.GetComponents<Collider>()[i].enabled = true;
                }
            }
            else
            {
                childObjects[countBackNum].transform.GetChild(1).GetComponent<MeshRenderer>().materials[0]
                    .color = Color.white; 
                childObjects[countBackNum].transform.GetChild(1).GetComponent<MeshRenderer>().materials[1]
                    .color = Color.white;
            }
            countBackNum++;
        });
        seq.AppendInterval(0.075f);
        seq.SetLoops(childObjects.Count + 1);
    }
	private bool _once;
	private void OnMouseDown()
	{
		if (!GameManager.Instance.downCheck)
		{
			if (UIManagerScript.Instance.endScreen.activeInHierarchy) return;

			var position1 = transform.position;
			var position = new Vector3(position1.x, position1.y + 3f, position1.z + 2.5f);
			_offset = position - MouseWorldPosition();
            
            if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("ButtonClickMG");
			if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
		}
	}

	private void OnMouseDrag()
	{
		if (!GameManager.Instance.downCheck)
		{
			if (GameManager.Instance.levelCompleted || GameManager.Instance.scriptOff) return;
			
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
	}

	/*private void OnMouseUp()
	{
		if (!GameManager.Instance.downCheck)
		{
			GameManager.Instance.downCheck = true;
			DOVirtual.DelayedCall(0.5f, () =>
			{
				GameManager.Instance.downCheck = false;
			});
		}
		
	}*/

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
        if(canCheckForPlacement) CondToAttachCubesInGrid();
	}

	//To reset the Position of the Objects
	private void ResetPosition()
	{
		//transform.position = _initPos;
		if (canReset && !
				GameManager.Instance.levelCompleted)
		{
			canReset = false;
			transform.DOMove(_initPos, 0.2f).SetEase(Ease.Flash).OnStart(() =>
            {
                foreach (var childCol in childObjects.Select(t => t.transform.GetComponent<Collider>()))
                {
                    childCol.enabled = false;
                }
                canCheckForPlacement = false;
                //Debug.Log("Check Stop");
            }).OnComplete(() =>
            {
                foreach (var childCol in childObjects.Select(t => t.transform.GetComponent<Collider>()))
                {
                    childCol.enabled = true;
                }
                canCheckForPlacement = true;
				canReset = true;
                //Debug.Log("Check Start");
			});
			
			for (int i = 0; i < childObjects.Count; i++)
            {
                childObjects[i].transform.DOMove(initialPos[i], 0.2f).SetEase(Ease.Flash);
            }
		}
		

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

//		print("all are hitting");
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

	
		//Debug.Log("isFilled Value " + isFilledC);

		if (!isFilledC)
		{
			//Debug.Log("Check if hitting");

			//if (Input.GetMouseButtonUp(0))
			{
				var position = hitInfo.transform.position;
				child.transform.position = position;
				var vector3 = transform.position;
				vector3.z = position.z;
				
				child.GetComponent<PlayerCubeScript>().isPlaced = true;
				//Debug.Log("Placed");
				Instantiate(dustFX, position, Quaternion.identity);
                //GameManager.Instance.canPlaceNow = false;
				if (SoundHapticManager.Instance) SoundHapticManager.Instance.Vibrate(30);
				if (SoundHapticManager.Instance) SoundHapticManager.Instance.Play("Pop");

				if (PlayerPrefs.GetInt("Level", 1) == 1)
				{
					DOVirtual.DelayedCall(0.3f, () =>
					{
						if (!check2done)
						{
//							print("checkingObj");
							GameManager.Instance.ShowTheText();
							check2done = true;
						}
					});
				}
				
			}
		}
	}

	public bool check1done;
	public bool check2done; 
	public bool check3done;
	
	// ReSharper disable Unity.PerformanceAnalysis
	private void CondToAttachCubesInGrid()
	{
		if (canCheckForPlacement && CheckIfAllHitting())
		{
            
			for (int i = 0; i < childObjects.Count; i++)
			{
				var child = childObjects[i].transform;
				if (CheckIfHitting(child))
				{
					var hitInfo = RayCastInfo(child);
					if (Input.GetMouseButtonUp(0) || GameManager.Instance.canPlaceNow)
					{
						//Debug.Log("Show");
						AttachTheObj(hitInfo, child);
					}

				}
			}

            //GameManager.Instance.canPlaceNow = false;
            
            if (Input.GetMouseButtonUp(0))
            {
                GameManager.Instance.movesCount--;
                UIManagerScript.Instance.movesText.text = "Moves: " + GameManager.Instance.movesCount;
            }
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