using System;

namespace LogikGenAPI.Model.Constraints
{
    public class DistinctConstraint : BinaryConstraint
    {
        public DistinctConstraint(Property left, Property right) 
            : base(left, right)
        {
        }

        public override ConstraintCheckResult Check(IGrid grid)
        {
            return CheckEqual(grid) ? ConstraintCheckResult.Contradicts :
                   CheckDistinct(grid) ? ConstraintCheckResult.Satisfied :
                   ConstraintCheckResult.Unsatisfied;
        }

        public override bool Equals(object obj)
        {
            DistinctConstraint other = obj as DistinctConstraint;

            if (other == null)
                return false;

            return (this.Left == other.Left && this.Right == other.Right) ||
                   (this.Left == other.Right && this.Right == other.Left);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.GetType(), this.Left.GetHashCode() ^ this.Right.GetHashCode());
        }

        public override string ToString()
        {
            return $"Distinct({Left}, {Right})";
        }
    }
}
