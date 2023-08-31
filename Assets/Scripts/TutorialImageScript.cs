using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialImageScript : MonoBehaviour
{
	public Image tutHand;

	[SerializeField] private Vector2 targetPos;
	void Start()
	{
		tutHand.rectTransform.DOAnchorPos(targetPos, 2f).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Restart);
	}

	public void HelpHand()
	{
		tutHand.enabled = false;
	}
}
