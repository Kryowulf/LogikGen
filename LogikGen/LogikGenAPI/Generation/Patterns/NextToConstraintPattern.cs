using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Generation.Patterns
{
    public class NextToConstraintPattern : ConstraintPattern
    {
        public override Type ConstraintType => typeof(NextToConstraint);

        public override Constraint RandomConstraint(SolutionGrid solution, Random rgen)
        {
            // Pick one of the ordered categories at random.
            List<Category> orderedCategories = solution.PropertySet.Categories.Where(c => c.IsOrdered).ToList();

            if (orderedCategories.Count == 0)
                throw new ArgumentException("No ordered categories to select from.");

            Category orderingCategory = rgen.Select(orderedCategories);

            // Grab a random property.
            Property leftPosition = rgen.Select(orderingCategory.Properties);

            // Randomly select the property that's either to the left or to the right.
            Property rightPosition = rgen.Select( (leftPosition.Singleton << 1) | (leftPosition.Singleton >> 1) );

            // Don't use the ordering category for either of the arguments,
            // lest we get something stupid like NextTo(1st, Englishman)
            List<Category> availableCategories = solution.PropertySet.Categories
                .Where(c => c != orderingCategory).ToList();

            // Choose categories for our "left" and "right" properties.
            Category leftCategory = rgen.Select(availableCategories);
            Category rightCategory = rgen.Select(availableCategories);

            // Get the properties which are associated with our 
            // leftPosition and rightPosition in the selected categories.
            Property left = solution[leftPosition, leftCategory][0];
            Property right = solution[rightPosition, rightCategory][0];

            return new NextToConstraint(left, right, orderingCategory);
        }
    }
}
