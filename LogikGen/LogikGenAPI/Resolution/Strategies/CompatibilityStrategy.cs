using LogikGenAPI.Model;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Resolution.Strategies
{
    /*  The different indirection levels created an undesireable phenomenon with all of the "compatibility check" strategies.
     *  Consider the ImmediateLessThanCompatibilityCheckStrategy on the EqualOnly variant, with the clues:
     *  
     *  LessThan(Red, Yellow) & NextTo(German, Yellow), 
     *  where the German lives in the Red house, and some candidate pair of locations conflicts with White.
     *  
     *  Consider the following pattern
     *  LessThan(X, Y) & NextTo(X, Y)       In this case, the matcher will bind X=Red, Y=Yellow
     *                                      The strategy will work as expected, identifying White as incompatible.
     *  
     *  If we instead use the pattern
     *  NextTo(X, Y) & LessThan(Y, X)       In this case, the matcher will bind X=German, Y=Yellow
     *                                      The strategy will fail to identify White as distinct from X and Y,
     *  
     *  A similar situation arises if we switch up the clues to be 
     *  LessThan(German, Yellow) & NextTo(Red, Yellow) instead.
     *  
     *  Thus, whether the EqualOnly variant can apply depends on the order in which the patterns are matched - 
     *  a confusing situation that's best to avoid entirely.
     *  
     *  For this reason, require all strategies which use AreAssociationsCompatible to 
     *  have an indirection level of either Direct or IndirectBoth only.
     * 
     */

    public abstract class CompatibilityStrategy : MultipleConstraintStrategy
    {
        public override StrategyClassification Classification => StrategyClassification.CompatibilityCheck;
        public override bool AutoRepeat => true;

        public override Difficulty Difficulty => 
            this.IndirectionLevel == IndirectionLevel.Direct ? Difficulty.Medium
                                                             : Difficulty.Harder;

        public override string Name => 
            this.IndirectionLevel == IndirectionLevel.Direct ? this.GetType().Name + "/SameCategory" 
                                                             : this.GetType().Name + "/General";

        protected CompatibilityStrategy(bool sameCategoryOnly)
            : base(sameCategoryOnly ? IndirectionLevel.Direct : IndirectionLevel.IndirectBoth)
        {
        }

        // Returns True if the given associations can be exclusively assigned to the given list of properties,
        // without causing a contradiction with some other property p in the property set.

        // Returns False if there exists some property p, distinct from all those in the properties array,
        // for which there would be a contradiction if none of the given associations are available to it.
        protected bool AreAssociationsCompatible(PuzzleGrid grid, IPropertyComparer comparer, SubsetKey<Property> associations, params Property[] properties)
        {
            if (associations.Count != properties.Length)
                throw new ArgumentException("Associations & properties collections must have the same length.");

            Category catetory = associations.Source.Full[0].Category;

            foreach (Property p in grid.PropertySet)
            {
                if (grid[p, catetory].Subtract(associations).IsEmpty)
                {
                    if (properties.All(q => comparer.ProvenDistinct(p, q)))
                        return false;
                }
            }

            return true;
        }
    }
}
