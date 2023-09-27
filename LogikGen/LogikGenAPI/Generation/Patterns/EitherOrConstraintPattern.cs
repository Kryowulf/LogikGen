using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Generation.Patterns
{
    public class EitherOrConstraintPattern : ConstraintPattern
    {
        public override Type ConstraintType => typeof(EitherOrConstraint);

        public override Constraint RandomConstraint(SolutionGrid solution, Random rgen)
        {
            Property key = rgen.Select(solution.PropertySet);

            List<Category> argcats = solution.PropertySet.Categories.Except(key.Category).ToList();

            Category xcat = rgen.Select(argcats);
            Category ycat = rgen.Select(argcats);

            Property x = solution[key, xcat].Single();
            Property y = rgen.Select(solution[key, ycat].Complement());

            Constraint constraint = rgen.Next(2) == 0 ? new EitherOrConstraint(key, x, y)
                                                      : new EitherOrConstraint(key, y, x);

            return constraint;
        }
    }
}
