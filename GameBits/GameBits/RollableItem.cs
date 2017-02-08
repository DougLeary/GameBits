namespace GameBits
{
    public abstract class RollableItem : IResolver
    {
        public int LowRoll;
        public int HighRoll;
        public IResolver Item;
    }
}
