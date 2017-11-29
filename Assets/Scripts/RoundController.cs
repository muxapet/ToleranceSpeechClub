using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoundController : MonoBehaviour
{
    public MainMenuController MainMenu;
    public bool IsAllowToSameCharacters;
    public int OpponentsCount = 3;
    public int VariantsCount = 3;
    public CharacterObject PlayerCharacter;
    public CharacterObject[] Opponents;

    public Action<ICharacter[]> OnRoundStarted;
    public Action<ICharacter, ICharacter, SentenceObject[]> OnActivePlayerChanged;
    public Action<ICharacter, SentenceObject> OnActivePlayerAnswered;

    private LinkedList<ICharacter> playersInRoom;
    private LinkedListNode<ICharacter> currentTurn;

    public AudioClip GameMusic;
    public AudioClip FinishSound;

    private const float MaxRoundTime = 300;
    private bool isLoose = true;

    public bool IsGameLoose
    {
        get { return isLoose; }
    }

    public bool FailIfAllPlayersZero = true;

    public float RoundTime { get; private set; }

    public void StartRound()
    {
        playersInRoom = CreatePlayers(ChooseOpponents());

        if (OnRoundStarted != null)
        {
            OnRoundStarted(playersInRoom.ToArray());
        }

        SoundController.Play(GameMusic, true);

        CancelInvoke();
        Invoke("NextTurn", 3f);
        isLoose = false;
        RoundTime = 0;
    }

    public void StopGame(float time)
    {
        CancelInvoke();
        if (playersInRoom != null)
        {
            playersInRoom.Clear();
        }
        currentTurn = null;
        MainMenu.Show(time);
        SoundController.Stop(GameMusic);
        SoundController.Play(FinishSound);
    }

    private void NextTurn()
    {
        ICharacter previousCharacter = currentTurn == null ? null : currentTurn.Value;
        if (currentTurn == null || currentTurn.Next == null)
        {
            currentTurn = playersInRoom.First;
        }
        else
        {
            currentTurn = currentTurn.Next;
        }
        Debug.Log("Start turn " + currentTurn.Value.GetCharacter().Title);

        if (IsLoose())
        {
            isLoose = true;
        }

        var variants = currentTurn.Value.GetVariants(VariantsCount);
        if (OnActivePlayerChanged != null)
        {
            OnActivePlayerChanged(previousCharacter, currentTurn.Value, variants);
        }

        if (isLoose)
        {
            Invoke("OnPlayerLose", 10f);

            SoundController.Stop(GameMusic);
            return;
        }


        if (currentTurn.Value.IsHuman())
        {
            currentTurn.Value.OnStartTurn();
        }
        else
        {
            Invoke("GenerateAnswer", 3f);
        }
    }

    private void GenerateAnswer()
    {
        currentTurn.Value.OnStartTurn();
    }

    public void Answer(ICharacter from, SentenceObject sentence)
    {
        if (currentTurn == null) return;
        if (!ReferenceEquals(currentTurn.Value, from)) return;

        foreach (var player in playersInRoom)
        {
            player.OnSaid(from, sentence);
        }

        if (OnActivePlayerAnswered != null)
        {
            OnActivePlayerAnswered(currentTurn.Value, sentence);
        }

        currentTurn.Value.OnEndTurn();

        NextTurn();
    }

    private bool IsLoose()
    {
        bool hasNotFailed = false;
        bool hasFailed = false;
        foreach (var player in playersInRoom)
        {
            if (player.GetPatience() > 0)
            {
                hasNotFailed = true;
            }
            else
            {
                hasFailed = true;
            }
        }
        if (FailIfAllPlayersZero)
        {
            if (!hasNotFailed)
            {
                return true;
            }
        }
        else
        {
            if (hasFailed)
            {
                return true;
            }
        }
        return false;
    }

    private void OnPlayerLose()
    {
        StopGame(RoundTime);
    }

    private CharacterObject[] ChooseOpponents()
    {
        if (Opponents.Length < OpponentsCount)
        {
            throw new Exception("Не хватает оппонентов: " + Opponents.Length + "/" + OpponentsCount);
        }

        List<CharacterObject> choosed = new List<CharacterObject>();

        int cantGenerateCounter = 0;
        while (choosed.Count < OpponentsCount && cantGenerateCounter < 1000)
        {
            int id = Random.Range(0, Opponents.Length);
            if (IsAllowToSameCharacters || !choosed.Contains(Opponents[id]))
            {
                choosed.Add(Opponents[id]);
            }
            cantGenerateCounter++;
        }
        return choosed.ToArray();
    }

    private LinkedList<ICharacter> CreatePlayers(CharacterObject[] characters)
    {
        LinkedList<ICharacter> players = new LinkedList<ICharacter>();
        string queue = "";

        int humanTurn = 0;//Random.Range(0, characters.Length + 1);
        for (int i = 0, offset = 0; i < characters.Length + 1; i++)
        {
            if (i == humanTurn)
            {
                players.AddLast(new HumanPlayer(PlayerCharacter));
                queue += PlayerCharacter.Title + " ";
                offset++;
            }
            else
            {
                players.AddLast(new CpuPlayer(characters[i - offset]));
                queue += characters[i - offset].Title + " ";
            }
            players.Last.Value.OnAnswered += Answer;
        }

        Debug.Log("Turn queue: " + queue);
        return players;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopGame(RoundTime);
        }

        if (MaxRoundTime - RoundTime <= 0)
        {
            StopGame(RoundTime);
        }

        if (isLoose == false && currentTurn != null && !currentTurn.Value.IsHuman())
        {
            RoundTime += Time.deltaTime;
        }
    }
}