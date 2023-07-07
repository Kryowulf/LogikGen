using LogikGenAPI.Utilities;

namespace LogikGenAPI.Model.Constraints
{
    public abstract class OrderedBinaryConstraint : BinaryConstraint
    {
        public Category OrderingCategory { get; private set; }

        public OrderedBinaryConstraint(Property left, Property right, Category orderingCategory)
            : base(left, right)
        {
            this.OrderingCategory = orderingCategory;
        }

        public abstract SubsetKey<Property> LeftDomainFrom(SubsetKey<Property> p);
        public abstract SubsetKey<Property> RightDomainFrom(SubsetKey<Property> p);
    }
}
