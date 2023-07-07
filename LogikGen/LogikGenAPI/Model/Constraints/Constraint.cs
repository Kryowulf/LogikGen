namespace LogikGenAPI.Model.Constraints
{
    public abstract class Constraint
    {
        public abstract ConstraintCheckResult Check(IGrid grid);
        public abstract override int GetHashCode();
        public abstract override bool Equals(object obj);
        public abstract override string ToString();
    }
}
