using System;

namespace LogikGenAPI.Model.Constraints
{
    public class EqualConstraint : BinaryConstraint
    {
        public EqualConstraint(Property left, Property right) 
            : base(left, right)
        {
        }

        public override ConstraintCheckResult Check(IGrid grid)
        {
            return CheckEqual(grid) ? ConstraintCheckResult.Satisfied :
                   CheckDistinct(grid) ? ConstraintCheckResult.Contradicts :
                   ConstraintCheckResult.Unsatisfied;
        }

        public override bool Equals(object obj)
        {
            EqualConstraint other = obj as EqualConstraint;

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
            return $"Equal({Left}, {Right})";
        }
    }
}
