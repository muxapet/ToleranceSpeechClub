using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyVariantController : MonoBehaviour
{
	private DummyGUI _gui;
	private SentenceObject _sentence;
	public Text Label;

	public void Init(DummyGUI gui, SentenceObject sentence)
	{
		_gui = gui;
		_sentence = sentence;
		Label.text = sentence.Id;
	}

	public void OnClick()
	{
		_gui.OnAnswered(_sentence);
	}
}
