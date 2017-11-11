namespace Model
{
    public class HumanPlayer : BasePlayer
    {
        public HumanPlayer(CharacterObject character) : base(character)
        {
        }

        public void OnStartTurn()
        {
            throw new System.NotImplementedException();
        }

        public void OnEndTurn()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsHuman()
        {
            return true;
        }

        public void Answer(SentenceObject sentence)
        {
            InternalAnswer(sentence);
        }
    }
}