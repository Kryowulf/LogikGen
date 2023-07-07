using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Utilities;
using System;

namespace LogikGenAPI.Generation.Patterns
{
    public class DistinctConstraintPattern : ConstraintPattern
    {
        public override Type ConstraintType => typeof(DistinctConstraint);

        public DistinctConstraintPattern(int maxCount = int.MaxValue) 
            : base(maxCount)
        {
        }

        public override Constraint RandomConstraint(SolutionGrid solution, Random rgen)
        {
            Property row = rgen.Select(solution.PropertySet.Properties);
            Category category = rgen.Select(solution.PropertySet.Categories);

            while (row.Category == category)
                category = rgen.Select(solution.PropertySet.Categories);

            Property column = rgen.Select(~solution[row, category]);

            return new DistinctConstraint(row, column);
        }
    }
}
