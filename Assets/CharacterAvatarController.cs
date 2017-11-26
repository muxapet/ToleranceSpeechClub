using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAvatarController : MonoBehaviour
{
	public Transform Hands;
	public Animator CharacterAnimator;
	private ICharacter _character;
	private RoundController _game;
	public SpriteRenderer Emoticon;
	public Sprite[] EmoticonSprites;
	public TextMesh PatienceLabel;

	private int _lastPatience = 0;
	private bool _isSubscribed = false;

	public void Init(ICharacter character, RoundController game)
	{
		_character = character;
		_game = game;

		if (!_isSubscribed)
		{
			_isSubscribed = true;
			_game.OnActivePlayerChanged += OnNewTurn;
			_game.OnActivePlayerAnswered += OnPlayerAnswered;
		}

		_lastPatience = _character.GetPatience();
		SetEmoticon();
	}

	public Transform GetHandsTransform()
	{
		return Hands;
	}

	private void OnNewTurn(ICharacter from, ICharacter to, SentenceObject[] variants)
	{
		SetEmoticon();
	}

	private void OnPlayerAnswered(ICharacter player, SentenceObject sentence)
	{
		if (!ReferenceEquals(_character, player))
		{
			if (_lastPatience < _character.GetPatience())
			{
				CharacterAnimator.Play("Thumb");
			}
		}
		else
		{
			float patienceValue = _character.GetPatience() / (float)_character.GetCharacter().StartPatience;
			AudioClip saySound = player.GetCharacter().GetSaySound();
			if (!player.IsHuman() && patienceValue < 1f / EmoticonSprites.Length)
			{
				saySound = player.GetCharacter().GetSayRageSound();
			}
			if (saySound != null)
			{
				SoundController.Play(saySound);
			}
		}
		_lastPatience = _character.GetPatience();
	}

	private void SetEmoticon()
	{
		if(_character.IsHuman()) return;
		PatienceLabel.text = _lastPatience.ToString();
		float patienceValue = _lastPatience / (float)_character.GetCharacter().StartPatience;
		float size = 1f / EmoticonSprites.Length;
		for (int i = 0; i < EmoticonSprites.Length; i++)
		{
			if (patienceValue >= i * size && patienceValue < (i + 1) * size)
			{
				Emoticon.sprite = EmoticonSprites[i];
				break;
			}
		}
	}
	
}
