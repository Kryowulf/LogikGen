using LogikGenAPI.Utilities;

namespace LogikGenAPI.Model.Constraints
{
    public abstract class BinaryConstraint : Constraint
    {
        public Property Left { get; private set; }
        public Property Right { get; private set; }

        public BinaryConstraint(Property left, Property right)
        {
            this.Left = left;
            this.Right = right;
        }

        public bool CheckEqual(IGrid grid)
        {
            return grid[this.Left, this.Right.Category] == this.Right.Singleton;
        }

        public bool CheckDistinct(IGrid grid)
        {
            return (grid[this.Left, this.Right.Category] & this.Right.Singleton).IsEmpty;
        }
    }
}
