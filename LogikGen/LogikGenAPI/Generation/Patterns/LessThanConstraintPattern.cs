using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Generation.Patterns
{
    public class LessThanConstraintPattern : ConstraintPattern
    {
        public override Type ConstraintType => typeof(LessThanConstraint);

        public override Constraint RandomConstraint(SolutionGrid solution, Random rgen)
        {
            // Pick one of the ordered categories at random.
            List<Category> orderedCategories = solution.PropertySet.Categories.Where(c => c.IsOrdered).ToList();

            if (orderedCategories.Count == 0)
                throw new ArgumentException("No ordered categories to select from.");

            Category orderingCategory = rgen.Select(orderedCategories);

            // Grab a random property except the largest.
            Property leftPosition = rgen.Select(orderingCategory.Last().Singleton.Complement());

            // Grab a random property greater than leftPosition.
            Property rightPosition = rgen.Select(leftPosition.GreaterThan);

            // Don't use the ordering category for either of the arguments,
            // lest we get something stupid like LessThan(Englishman, 2nd).
            List<Category> availableCategories =solution.PropertySet.Categories
                .Where(c => c != orderingCategory).ToList();

            // Choose categories for our "left" and "right" properties.
            Category leftCategory = rgen.Select(availableCategories);
            Category rightCategory = rgen.Select(availableCategories);

            // Get the properties which are associated with our 
            // leftPosition and rightPosition in the selected categories.
            Property left = solution[leftPosition, leftCategory][0];
            Property right = solution[rightPosition, rightCategory][0];

            return new LessThanConstraint(left, right, orderingCategory);
        }
    }
}
