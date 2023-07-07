using System;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Model.Constraints
{
    public class NextToConstraint : OrderedBinaryConstraint
    {
        public NextToConstraint(Property left, Property right, Category orderingCategory)
            : base(left, right, orderingCategory)
        {
        }

        public override ConstraintCheckResult Check(IGrid grid)
        {
            bool areEqual = CheckEqual(grid);
            bool areDistinct = CheckDistinct(grid);

            if (areEqual)
                return ConstraintCheckResult.Contradicts;

            SubsetKey<Property> leftPositions = grid[this.Left, this.OrderingCategory];
            SubsetKey<Property> rightPositions = grid[this.Right, this.OrderingCategory];

            if (leftPositions.IsEmpty || rightPositions.IsEmpty)
                return ConstraintCheckResult.Contradicts;

            if ((leftPositions & (rightPositions << 1 | rightPositions >> 1)).IsEmpty)
                return ConstraintCheckResult.Contradicts;

            Property leftMin = leftPositions[0];
            Property leftMax = leftPositions[leftPositions.Count - 1];

            Property rightMin = rightPositions[0];
            Property rightMax = rightPositions[rightPositions.Count - 1];

            int min = Math.Min(leftMin.Index, rightMin.Index);
            int max = Math.Max(leftMax.Index, rightMax.Index);

            if (areDistinct && max - min == 1)
                return ConstraintCheckResult.Satisfied;

            return ConstraintCheckResult.Unsatisfied;
        }

        public override bool Equals(object obj)
        {
            NextToConstraint other = obj as NextToConstraint;

            if (other == null)
                return false;

            if (this.OrderingCategory != other.OrderingCategory)
                return false;

            return (this.Left == other.Left && this.Right == other.Right) ||
                   (this.Left == other.Right && this.Right == other.Left);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                this.GetType(), 
                this.OrderingCategory, 
                this.Left.GetHashCode() ^ this.Right.GetHashCode());
        }

        public override SubsetKey<Property> LeftDomainFrom(SubsetKey<Property> p)
        {
            if (p.Source != this.OrderingCategory.Full.Source)
                throw new ArgumentException();

            return (p << 1) | (p >> 1);
        }

        public override SubsetKey<Property> RightDomainFrom(SubsetKey<Property> p)
        {
            if (p.Source != this.OrderingCategory.Full.Source)
                throw new ArgumentException();

            return (p << 1) | (p >> 1);
        }

        public override string ToString()
        {
            return $"NextTo:{OrderingCategory}({Left}, {Right})";
        }
    }
}
