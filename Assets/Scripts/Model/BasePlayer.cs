using System;

namespace Model
{
    public class BasePlayer : ICharacter
    {
        public event Action<ICharacter, SentenceObject> OnAnswered;
        private CharacterObject _character;
        protected SentenceObject[] _lastVariants;
        
        public BasePlayer(CharacterObject character)
        {
            _character = character;
        }

        public CharacterObject GetCharacter()
        {
            return _character;
        }


        public SentenceObject[] GetVariants(int count)
        {
            _lastVariants = _character.Sentences.GetVariants(count);
            return _lastVariants;
        }

        public virtual bool IsHuman()
        {
            return false;
        }

        public virtual void OnSaid(ICharacter from, SentenceObject sentence)
        {
            
        }

        public virtual int GetPatience()
        {
            return _character.StartPatience;
        }

        public virtual void OnStartTurn()
        {
        }

        public virtual void OnEndTurn()
        {
        }

        protected void InternalAnswer(SentenceObject answer)
        {
            if (OnAnswered != null)
            {
                OnAnswered(this, answer);
            }
        }
    }
}