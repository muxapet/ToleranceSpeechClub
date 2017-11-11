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
                        int catInfluence = sentenceCategory.Value * characterCaresOf.Value;
                        influence += catInfluence;
                        if (catInfluence != 0)
                        {
                            DummyGUI.Log("To " + GetCharacter().Title + " in " + sentenceCategory.CategoryName + " " +
                                         influence);
                        }
                    }
                }
            }

            _patience += influence;
        }
    }
}