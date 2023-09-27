using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using System;

namespace LogikGenAPI.Generation.Patterns
{
    public abstract class ConstraintPattern
    {
        public abstract Type ConstraintType { get; }
        public abstract Constraint RandomConstraint(SolutionGrid solution, Random rgen);
    }
}
