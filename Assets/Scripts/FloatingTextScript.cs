using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class FloatingTextScript : MonoBehaviour
{
	public TMP_Text text;

	[SerializeField]
	private Vector3 scale;
	
	//private float yMoveValue = 2.3f;
	
	[SerializeField]
	private List<string> complementWords;

	[SerializeField] private Ease _ease;
	private void Awake()
	{
		// text = transform.GetComponent<TMP_Text>();
	}

	private void Start()
	{
		/*transform.DOScale(scale, 1f).SetEase(_ease);
		//transform.DOMoveY(yMoveValue, 1f) ;	
		

		_text.DOFade(0, 2f).OnComplete((() =>
		{
			Destroy(gameObject);
		}));

		_text.text = complementWords[Random.Range(0,complementWords.Count)];*/
		// TextExpression();

	}
	
	public void TextExpression()
	{
		StartCoroutine(AnimateText(complementWords[Random.Range(0, complementWords.Count)], text));
	}
	private IEnumerator AnimateText(string text, TMP_Text textObj)
	{
		textObj.text = text.ToUpper();
		textObj.ForceMeshUpdate(true, true);
        
		/*var effects = textObj.GetComponentsInChildren<ParticleImage>();

		foreach (var t in effects)
		{
		    t.Play();
		}*/
        
		var animator = new DOTweenTMPAnimator(textObj);
        
		for (int i = 0; i < animator.textInfo.characterCount; i++)
		{
			textObj.maxVisibleCharacters = i + 1; // Reveal the next character
			textObj.ForceMeshUpdate(true, true);
			var currentChar = animator.textInfo.characterInfo[i].character;
			if (char.IsSymbol(currentChar) || char.IsPunctuation(currentChar))
			{
				animator.DOPunchCharScale(i, 0.35f, 0.15f)
					.SetEase(Ease.InOutBounce)
					.SetLoops(1, LoopType.Yoyo);
			}
			else
			{
				animator.DOPunchCharScale(i, 1.5f, 0.15f)
					.SetEase(Ease.InOutBounce)
					.SetLoops(1, LoopType.Yoyo);
			}
			yield return new WaitForSeconds(0.125f); 
		}
        
		/*foreach (var t in effects)
		{
		    t.Stop();
		}*/
        
		yield return new WaitForSeconds(0.22f); 
        
		for (int i = animator.textInfo.characterCount; i >= 0; i--)
		{
			textObj.maxVisibleCharacters = i;
			yield return new WaitForSeconds(0.025f); 
		}
	}
	
}
