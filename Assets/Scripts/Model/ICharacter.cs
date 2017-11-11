using System;

public interface ICharacter
{
    event Action<ICharacter, SentenceObject> OnAnswered;
    
    SentenceObject[] GetVariants(int count);
    CharacterObject GetCharacter();
    bool IsHuman();

    void OnSaid(ICharacter from, SentenceObject sentence);
    int GetPatience();

    void OnStartTurn();
    void OnEndTurn();
}