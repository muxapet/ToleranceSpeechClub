using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public SmoothMouseLook Looker;
	private Transform _transform;
	public Transform PlayerEyes;

	private void Awake()
	{
		_transform = transform;
		StartCoroutine(UpdateTime(0.5f));
	}

	private IEnumerator UpdateTime(float delayInSeconds)
	{
		WaitForSeconds delay = new WaitForSeconds(delayInSeconds);
		while (true)
		{
			if (PlayerEyes != null)
			{
				_transform.position = PlayerEyes.position;
			}
			yield return delay;
		}
	}

	public void SetControlsEnabled(bool enabled)
	{
		Looker.enabled = enabled;
	}

	public void LookAt(Transform to)
	{
		
	}
}
