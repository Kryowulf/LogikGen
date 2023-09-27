using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation.Patterns
{
    public class IdentityConstraintPattern : ConstraintPattern
    {
        public override Type ConstraintType => typeof(IdentityConstraint);

        public override Constraint RandomConstraint(SolutionGrid solution, Random rgen)
        {
            PropertySet pset = solution.PropertySet;

            // TODO: If there are more entities than categories, then we can't 
            // select one unique property from each category. Figure out what
            // IdentityConstraintPattern should do in that case.
            int n = Math.Min(pset.Categories.Count, pset.CategorySize);

            List<Category> categories = pset.Categories.ToList();
            rgen.Shuffle(categories);
            categories.RemoveRange(n, categories.Count - n);

            Category key = categories[0];

            List<Property> pairwiseDistinctProperties =
                Enumerable.Range(0, n).Select(i => solution[key[i], categories[i]][0]).ToList();

            return new IdentityConstraint(pairwiseDistinctProperties);          
        }
    }
}
