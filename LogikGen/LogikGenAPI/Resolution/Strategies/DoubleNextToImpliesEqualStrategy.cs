using LogikGenAPI.Model;
using LogikGenAPI.Resolution.Terms;
using LogikGenAPI.Utilities;
using System;

namespace LogikGenAPI.Resolution.Strategies
{
    /*
     *      DoubleNextToAsEqualStrategy
     *
     *      NextTo(Blue, Spaniard) & NextTo(Spaniard, Fox)
     *      If the Spaniard cannot fit between Blue and Fox, then that means Blue == Fox. 
     *      
     */

    public class DoubleNextToImpliesEqualStrategy : MultipleConstraintStrategy
    {
        public override StrategyClassification Classification => StrategyClassification.DoubleNextTo;
        public override bool AutoRepeat => true;

        public DoubleNextToImpliesEqualStrategy(bool useIndirectEqual) 
            : base(useIndirectEqual ? IndirectionLevel.IndirectEqualOnly : IndirectionLevel.Direct)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            int initial = grid.TotalUnresolvedAssociations;

            foreach (Category position in cset.OrderedCategories)
            {
                Variable a = new Variable();
                Variable b = new Variable();
                Variable x = new Variable();

                PatternMatcher m = new PatternMatcher(cset, comparer, position);
                m.NextTo(a, x).NextTo(x, b);

                while (m.Match())
                {
                    SubsetKey<Property> ablocations = grid[a.Value, position] | grid[b.Value, position];

                    if (ablocations.Count == 2 && ablocations[1].Index == ablocations[0].Index + 1)
                    {
                        if (grid.Associate(a.Value, b.Value))
                            this.Logger.LogInfo($"NextTo({a}, {x}) & NextTo({x}, {b}) -> {a} = {b}");
                    }
                }
            }

            return grid.TotalUnresolvedAssociations < initial;
        }
    }
}
