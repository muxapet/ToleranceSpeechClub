using UnityEngine;

namespace Model
{
    public class CpuPlayer : BasePlayer
    {
        private int _patience;

        public CpuPlayer(CharacterObject character) : base(character)
        {
            _patience = character.StartPatience;
        }

        public override void OnStartTurn()
        {
            if (_lastVariants == null) return;

            int choosed = Random.Range(0, _lastVariants.Length);
            InternalAnswer(_lastVariants[choosed]);
        }

        public override int GetPatience()
        {
            return _patience;
        }

        public override void OnSaid(ICharacter from, SentenceObject sentence)
        {
            if (ReferenceEquals(from, this)) return;

            var influence = 0;
            for (int i = 0; i < sentence.Influence.Length; i++)
            {
                CategoryValue sentenceCategory = sentence.Influence[i];
                for (int j = 0; j < GetCharacter().CaresOf.Length; j++)
                {
                    CategoryValue characterCaresOf = GetCharacter().CaresOf[j];

                    if (Equals(sentenceCategory.CategoryName, characterCaresOf.CategoryName))
                    {
                        if (characterCaresOf.Value < -10)
                        {
                            characterCaresOf.Value = Random.Range(-10, 10);
                        }
                        int catInfluence = sentenceCategory.Value * characterCaresOf.Value;
                        if (catInfluence < 0) catInfluence = catInfluence * 20;
                        influence += catInfluence;
                    }
                }
            }

            _patience = Mathf.Clamp(_patience + influence, 0, GetCharacter().StartPatience);    
        }
    }
}