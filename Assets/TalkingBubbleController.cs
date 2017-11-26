using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TalkingBubbleController : MonoBehaviour
{
	public Text Label;
	private Sequence _sequence;
	private Transform _cameraTransform;

	public void Say(Vector3 position, SentenceObject sentence, Transform cameraTransform)
	{
		_cameraTransform = cameraTransform;
		gameObject.SetActive(true);
		Label.text = sentence.Id;
		Label.color = Color.white;
		transform.position = position + new Vector3(0, 0.5f, 0);
		transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward,
			_cameraTransform.rotation * Vector3.up);

		if (_sequence != null)
		{
			_sequence.Kill();
		}
		
		_sequence = DOTween.Sequence();
		_sequence.Append(transform.DOMoveY(transform.position.y + 0.5f, 7f));
		_sequence.Join(Label.DOFade(0, 7f));
		_sequence.Play();
	}

	private void Update()
	{
		if(_cameraTransform == null) return;
		
		transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward,
			_cameraTransform.rotation * Vector3.up);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
