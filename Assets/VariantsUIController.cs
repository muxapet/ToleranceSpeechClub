using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;

public class VariantsUIController : MonoBehaviour {
	public RoundController Game;
	public BubbleController[] Bubbles;
	
	private ICharacter _current;
	
	private void Awake()
	{
		Game.OnRoundStarted += OnRoundStarted;
		Game.OnActivePlayerChanged += OnNewTurn;
		Game.OnActivePlayerAnswered += OnPlayerAnswered;
		HideBubbles();
	}
	
	private void OnRoundStarted(ICharacter[] characters)
	{
		HideBubbles();
	}

	private void OnNewTurn(ICharacter from, ICharacter to, SentenceObject[] variants)
	{
		_current = to;
		if (_current.IsHuman() && Bubbles != null)
		{
			for (int i = 0; i < Bubbles.Length; i++)
			{
				Bubbles[i].Show(this, variants[i]);
			}
		}
	}

	private void OnPlayerAnswered(ICharacter player, SentenceObject sentence)
	{
		HideBubbles();
	}
	
	public void OnAnswered(SentenceObject sentence)
	{
		HideBubbles();
		if (_current.IsHuman())
		{
			((HumanPlayer)_current).Answer(sentence);
		}
	}

	private void HideBubbles()
	{
		for (int i = 0; i < Bubbles.Length; i++)
		{
			Bubbles[i].Hide();
		}
	}
}
