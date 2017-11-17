using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DummyValueCounter : MonoBehaviour
{
	public Image ProgressImage;
	public void SetValue(float val)
	{
		StopAllCoroutines();
		StartCoroutine(UpdateValue(val));
	}

	IEnumerator UpdateValue(float target)
	{
		Debug.Log("Set " + target);
		WaitForSeconds delay = new WaitForSeconds(0.02f);
		target = Mathf.Clamp(target, 0, 1);
		while (Mathf.Abs(ProgressImage.fillAmount - target) > 0.02f)
		{
			ProgressImage.fillAmount = Mathf.Lerp(ProgressImage.fillAmount, target, 0.05f);
			yield return delay;
		}
	}
}
