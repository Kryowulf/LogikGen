using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using System;

namespace LogikGenAPI.Generation.Patterns
{
    public abstract class ConstraintPattern
    {
        public abstract Type ConstraintType { get; }
        public int MaximumCount { get; private set; }
        public abstract Constraint RandomConstraint(SolutionGrid solution, Random rgen);

        public ConstraintPattern(int maxCount)
        {
            this.MaximumCount = maxCount;
        }
    }
}
