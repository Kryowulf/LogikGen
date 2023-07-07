using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Model.Constraints
{
    public class IdentityConstraint : Constraint
    {
        public IReadOnlyList<Property> PairwiseDistinctProperties { get; private set; }

        public IdentityConstraint(IEnumerable<Property> properties)
        {
            PairwiseDistinctProperties = properties.ToList().AsReadOnly();
        }

        public IdentityConstraint(params Property[] properties)
            : this((IEnumerable<Property>)properties)
        {
        }

        public override ConstraintCheckResult Check(IGrid grid)
        {
            List<Property> properties = PairwiseDistinctProperties.ToList();

            for (int i = 0; i < properties.Count - 1; i++)
            {
                for (int j = i + 1; j < properties.Count; j++)
                {
                    Property a = properties[i];
                    Property b = properties[j];

                    bool areEqual = grid[a, b.Category] == b.Singleton;
                    bool areDistinct = (grid[a, b.Category] & b.Singleton).IsEmpty;

                    if (areEqual)
                    {
                        return ConstraintCheckResult.Contradicts;
                    }
                    else if (!areDistinct)
                    {
                        return ConstraintCheckResult.Unsatisfied;
                    }
                }
            }

            return ConstraintCheckResult.Satisfied;
        }

        public override bool Equals(object obj)
        {
            IdentityConstraint other = obj as IdentityConstraint;
            return PairwiseDistinctProperties.SequenceEqual(other.PairwiseDistinctProperties);
        }

        public override int GetHashCode()
        {
            int properties_xor = 0;

            foreach (Property p in PairwiseDistinctProperties)
                properties_xor = properties_xor ^ p.GetHashCode();

            return HashCode.Combine(this.GetType(), properties_xor);
        }

        public override string ToString()
        {
            return "Identity(" + string.Join(", ", PairwiseDistinctProperties) + ")";
        }
    }
}
