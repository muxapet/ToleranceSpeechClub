using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
	public TalkingBubbleController TalkingBubble;
	public CameraController Camera;
	public RoundController Game;
	public BallController Ball;
	public CharacterAvatarController[] CharacterAvatars;
	private ICharacter[] _characters;
	private int _humanPosOffset = 0;
	private readonly Dictionary<ICharacter, CharacterAvatarController> _charactersDict = new Dictionary<ICharacter, CharacterAvatarController>();
	
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
		_charactersDict.Clear();
		for (int i = 0; i < CharacterAvatars.Length; i++)
		{
			int chairId = i - _humanPosOffset;
			if (chairId < 0) chairId += CharacterAvatars.Length;
			_charactersDict.Add(_characters[i], CharacterAvatars[chairId]);
			CharacterAvatars[chairId].Init(_characters[i], Game);
		}
		
		TalkingBubble.Hide();
	}

	private void OnNewTurn(ICharacter from, ICharacter to, SentenceObject[] variants)
	{
//		Camera.SetControlsEnabled(!to.IsHuman());

		CharacterAvatarController fromChair = null, toChair = null;
		foreach (var character in _charactersDict)
		{
			if (ReferenceEquals(character.Key, from))
			{
				fromChair = character.Value;
			}
			if (ReferenceEquals(character.Key, to))
			{
				toChair = character.Value;
			}
		}
		if (fromChair != null && toChair != null)
		{
			Ball.Throw(fromChair.GetHandsTransform(), toChair.GetHandsTransform());
		}
		
	}

	private void OnPlayerAnswered(ICharacter player, SentenceObject sentence)
	{
//		Camera.SetControlsEnabled(true);
		if (!player.IsHuman())
		{
			TalkingBubble.Say(_charactersDict[player].GetHandsTransform().position, sentence, Camera.gameObject.transform);
		}
	}
}
