using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Utilities;
using System;

namespace LogikGenAPI.Generation.Patterns
{
    public class EqualConstraintPattern : ConstraintPattern
    {
        public override Type ConstraintType => typeof(EqualConstraint);

        public EqualConstraintPattern(int maxCount = int.MaxValue)
            : base(maxCount)
        {
        }

        public override Constraint RandomConstraint(SolutionGrid solution, Random rgen)
        {
            Property row = rgen.Select(solution.PropertySet.Properties);
            Category category = rgen.Select(solution.PropertySet.Categories);

            while (row.Category == category)
                category = rgen.Select(solution.PropertySet.Categories);

            Property column = solution[row, category][0];

            return new EqualConstraint(row, column);
        }
    }
}
