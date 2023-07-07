using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Resolution.Strategies
{
    /*
     *  NextTo(X, Y)
     *  
     *  If a distinct property Z has a position P such that,
     *  for all possible positions of X and Y, P is not available to Z,
     *  then we can disassociate P from Z.
     *  
     *  Seems I made this much more complicated than need be.
     *  It completely ignores the fact that it only applies when there are 
     *  at most 3 possible positions for X and Y, all consecutive.
     *  
     *  In the case that there are 3 positions, P would necessarily 
     *  be the middle one if it existed.
     *  
     *  In the case that there are 2 positions... well, 
     *  the new Binary Constraint Analysis strategy would've taken care of that anyway.
     *  
     *  I could simplify it, but it works and I may want a variation
     *  of this algorithm for a different strategy, so keep it for now.
     */

    public class NextToIncompatibilitySearchStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.CompatibilityCheck;
        private bool _sameCategoryOnly;

        public override bool AutoRepeat => true;

        public override Difficulty Difficulty =>
            _sameCategoryOnly ? Difficulty.Medium 
                              : Difficulty.Harder;

        public override string Name =>
            _sameCategoryOnly ? nameof(NextToIncompatibilitySearchStrategy) + "/SameCategory"
                              : nameof(NextToIncompatibilitySearchStrategy) + "/General";

        public NextToIncompatibilitySearchStrategy(bool sameCategoryOnly)
        {
            _sameCategoryOnly = sameCategoryOnly;
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int initial = grid.TotalUnresolvedAssociations;

            IndirectionLevel level = _sameCategoryOnly ? IndirectionLevel.Direct
                                                       : IndirectionLevel.IndirectBoth;

            StrategicPropertyComparer comparer = new StrategicPropertyComparer(level, grid);

            PropertySet pset = grid.PropertySet;

            foreach (Category orderingCategory in cset.OrderedCategories)
            {
                foreach (NextToConstraint ntc in cset.NextToConstraints(orderingCategory))
                {
                    var table = new Dictionary<Property, List<SubsetKey<Property>>>();

                    foreach (Property p in pset)
                    {
                        if (comparer.ProvenDistinct(p, ntc.Left) && comparer.ProvenDistinct(p, ntc.Right))
                        {
                            table[p] = new List<SubsetKey<Property>>();
                            table[p].Add(grid[p, orderingCategory]);
                        }
                    }

                    SubsetKey<Property> testPositions = grid[ntc.Left, orderingCategory] | grid[ntc.Right, orderingCategory];

                    for (int i = 0; i < testPositions.Count - 1; i++)
                    {
                        if (testPositions[i + 1].Index == testPositions[i].Index + 1)
                        {
                            SubsetKey<Property> testPair = testPositions[i].Singleton | testPositions[i + 1].Singleton;

                            foreach (Property p in table.Keys)
                                table[p].Add(table[p][0].Subtract(testPair));
                        }
                    }

                    foreach (Property p in table.Keys)
                    {
                        SubsetKey<Property> union = orderingCategory.Empty;

                        for (int i = 1; i < table[p].Count; i++)
                            union |= table[p][i];

                        if (grid.Update(p, union))
                            Logger.LogInfo($"{ntc} -> {p}:{orderingCategory} = {grid[p,orderingCategory]}");
                    }
                }
            }

            return grid.TotalUnresolvedAssociations < initial;
        }
    }
}
