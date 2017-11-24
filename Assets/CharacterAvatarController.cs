using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAvatarController : MonoBehaviour
{
	public Transform Hands;
	public Animator CharacterAnimator;
	private ICharacter _character;
	private RoundController _game;

	private int _lastPatience = 0;

	public void Init(ICharacter character, RoundController game)
	{
		_character = character;
		_game = game;
		_game.OnActivePlayerChanged += OnNewTurn;
		_game.OnActivePlayerAnswered += OnPlayerAnswered;

		_lastPatience = _character.GetPatience();
	}

	public Transform GetHandsTransform()
	{
		return Hands;
	}

	private void OnNewTurn(ICharacter from, ICharacter to, SentenceObject[] variants)
	{
	}

	private void OnPlayerAnswered(ICharacter player, SentenceObject sentence)
	{
		if(ReferenceEquals(_character, player)) return;
		if (_lastPatience < _character.GetPatience())
		{
			CharacterAnimator.Play("Thumb");
		}
		_lastPatience = _character.GetPatience();
	}
	
}
