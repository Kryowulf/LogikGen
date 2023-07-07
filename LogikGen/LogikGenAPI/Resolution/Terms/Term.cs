namespace LogikGenAPI.Resolution.Terms
{
    public abstract class Term
    {
        public abstract bool Match();
        public abstract void Reset();
        public abstract override string ToString();
    }
}
