using System;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Model.Constraints
{
    public class LessThanConstraint : OrderedBinaryConstraint
    {
        public LessThanConstraint(Property left, Property right, Category orderingCategory) 
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

            Property leftMin = leftPositions[0];
            Property leftMax = leftPositions[leftPositions.Count - 1];

            Property rightMin = rightPositions[0];
            Property rightMax = rightPositions[rightPositions.Count - 1];

            if (leftMin.Index >= rightMax.Index)
                return ConstraintCheckResult.Contradicts;

            if (areDistinct && leftMax.Index < rightMin.Index)
                return ConstraintCheckResult.Satisfied;
            
            return ConstraintCheckResult.Unsatisfied;
        }

        public override bool Equals(object obj)
        {
            LessThanConstraint other = obj as LessThanConstraint;

            if (other == null)
                return false;

            if (this.OrderingCategory != other.OrderingCategory)
                return false;

            return this.Left == other.Left && this.Right == other.Right;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                this.GetType(), 
                this.OrderingCategory, 
                this.Left, 
                this.Right);
        }

        public override SubsetKey<Property> LeftDomainFrom(SubsetKey<Property> p)
        {
            if (p.Source != this.OrderingCategory.Full.Source)
                throw new ArgumentException();

            if (p.IsEmpty)
                return this.OrderingCategory.Empty;

            return p[p.Count - 1].LessThan;
        }

        public override SubsetKey<Property> RightDomainFrom(SubsetKey<Property> p)
        {
            if (p.Source != this.OrderingCategory.Full.Source)
                throw new ArgumentException();

            if (p.IsEmpty)
                return this.OrderingCategory.Empty;

            return p[0].GreaterThan;
        }

        public override string ToString()
        {
            return $"LessThan:{OrderingCategory}({Left}, {Right})";
        }
    }
}
