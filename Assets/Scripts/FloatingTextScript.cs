using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class FloatingTextScript : MonoBehaviour
{
	private TextMeshPro _text;

	[SerializeField]
	private Vector3 scale;
	
	private float yMoveValue = 2.3f;
	
	[SerializeField]
	private List<string> complementWords;

	[SerializeField] private Ease _ease;
	private void Awake()
	{
		_text = transform.GetComponent<TextMeshPro>();
	}

	private void Start()
	{
		transform.DOScale(scale, 1f).SetEase(_ease);
		//transform.DOMoveY(yMoveValue, 1f) ;	
		

		_text.DOFade(0, 2f).OnComplete((() =>
		{
			Destroy(gameObject);
		}));

		_text.text = complementWords[Random.Range(0,complementWords.Count)];

	}
	
}
