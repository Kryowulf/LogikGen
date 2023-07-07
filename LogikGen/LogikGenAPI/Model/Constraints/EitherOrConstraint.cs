using System;

namespace LogikGenAPI.Model.Constraints
{
    public class EitherOrConstraint : Constraint
    {
        public Property Key { get; private set; }
        public Property X { get; private set; }
        public Property Y { get; private set; }

        public EitherOrConstraint(Property key, Property x, Property y)
        {
            this.Key = key;
            this.X = x;
            this.Y = y;
        }

        public override ConstraintCheckResult Check(IGrid grid)
        {
            bool equalToX = grid[Key, X.Category] == X.Singleton;
            bool equalToY = grid[Key, Y.Category] == Y.Singleton;

            bool distinctFromX = !grid[Key, X.Category].ContainsSubset(X.Singleton);
            bool distinctFromY = !grid[Key, Y.Category].ContainsSubset(Y.Singleton);

            if (distinctFromX && distinctFromY)
            {
                return ConstraintCheckResult.Contradicts;
            }
            else if (equalToX && distinctFromY || equalToY && distinctFromX)
            {
                return ConstraintCheckResult.Satisfied;
            }
            else
            {
                return ConstraintCheckResult.Unsatisfied;
            }
        }

        public override bool Equals(object obj)
        {
            EitherOrConstraint other = obj as EitherOrConstraint;

            if (other == null)
                return false;

            return this.Key == other.Key &&
                   (this.X == other.X && this.Y == other.Y ||
                    this.X == other.Y && this.Y == other.X);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.GetType(), Key, X.GetHashCode() ^ Y.GetHashCode());
        }

        public override string ToString()
        {
            return $"EitherOr({Key}, {X}, {Y})";
        }
    }
}
