using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
	public CameraController Camera;
	public RoundController Game;
	public BallController Ball;
	public Transform[] PlayersHands;
	private ICharacter[] _characters;
	private int _humanPosOffset = 0;
	
	private void Awake()
	{
		Game.OnRoundStarted += OnRoundStarted;
		Game.OnActivePlayerChanged += OnNewTurn;
		Game.OnActivePlayerAnswered += OnPlayerAnswered;
	}
	
	private void OnRoundStarted(ICharacter[] characters)
	{
		_characters = characters;
		for (int i = 0; i < _characters.Length; i++)
		{
			if (_characters[i].IsHuman())
			{
				_humanPosOffset = i;
				break;
			}
		}
	}

	private void OnNewTurn(ICharacter from, ICharacter to, SentenceObject[] variants)
	{
		Camera.SetControlsEnabled(!to.IsHuman());
		try
		{
			Debug.Log(from.GetCharacter().Title + "->>" + to.GetCharacter().Title);
		} catch(Exception e)
		{}

		int fromId = -1, toId = -1;
		for (int i = 0; i < _characters.Length; i++)
		{
			if (ReferenceEquals(_characters[i], from))
			{
				fromId = i - _humanPosOffset;
				if (fromId < 0) fromId += PlayersHands.Length;
			}
			if (ReferenceEquals(_characters[i], to))
			{
				toId = i - _humanPosOffset;
				if (toId < 0) toId += PlayersHands.Length;
			}
		}
		if (fromId >= 0 && toId >= 0)
		{
			Ball.Throw(PlayersHands[fromId], PlayersHands[toId]);
		}
		
	}

	private void OnPlayerAnswered(ICharacter player, SentenceObject sentence)
	{
		Camera.SetControlsEnabled(true);
		AudioClip saySound = player.GetCharacter().GetSaySound();
		if (saySound != null)
		{
			SoundController.Play(saySound);
		}
	}
}
