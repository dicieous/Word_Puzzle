using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialImageScript : MonoBehaviour
{
	public Image tutHand;

	private Tween _tween;
	private bool imageDeleted = false;

	[SerializeField] private Vector2 targetPos;
	void Start()
	{
		_tween = tutHand.rectTransform.DOAnchorPos(targetPos, 2f).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Restart);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0)&&!imageDeleted)
		{
			imageDeleted = true;
			Destroy(gameObject);
			_tween.Kill();
		}
		

	}
}
