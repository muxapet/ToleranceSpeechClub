using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TalkingBubbleController : MonoBehaviour
{
	public Text Label;
	private Sequence _sequence;

	public void Say(Vector3 position, SentenceObject sentence, Quaternion cameraRotaion)
	{
		gameObject.SetActive(true);
		Label.text = sentence.Id;
		Label.color = Color.white;
		transform.position = position + new Vector3(0, 0.5f, 0);
		transform.LookAt(transform.position + cameraRotaion * Vector3.forward,
			cameraRotaion * Vector3.up);

		if (_sequence != null)
		{
			_sequence.Kill();
		}
		
		_sequence = DOTween.Sequence();
		_sequence.Append(transform.DOMoveY(transform.position.y + 0.5f, 5f));
		_sequence.Join(Label.DOFade(0, 5f));
		_sequence.Play();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
