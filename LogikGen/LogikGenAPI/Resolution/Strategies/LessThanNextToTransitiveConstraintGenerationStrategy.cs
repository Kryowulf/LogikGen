using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution.Terms;
using System;

namespace LogikGenAPI.Resolution.Strategies
{
    /*
     *      LessThanNextToTransitiveConstraintGenerationStrategy
     * 
     *      LessThan(x, a) & LessThan(a, b) & NextTo(y, b) -> LessThan(x, y)
     *      
     *      NextTo(x, a) & LessThan(a, b) & LessThan(b, y) -> LessThan(x, y)
     * 
     * 
     */

    public class LessThanNextToTransitiveConstraintGenerationStrategy : ConstraintGenerationStrategy
    {
        public override StrategyClassification Classification => StrategyClassification.ConstraintGeneration;
        public override bool AutoRepeat => true;

        public LessThanNextToTransitiveConstraintGenerationStrategy(bool useIndirectEqual)
            : base(useIndirectEqual ? IndirectionLevel.IndirectEqualOnly : IndirectionLevel.Direct)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            bool updated = false;

            foreach (Category orderingCategory in cset.OrderedCategories)
            {
                Variable x = new Variable();
                Variable y = new Variable();
                Variable a = new Variable();
                Variable b = new Variable();

                PatternMatcher m = new PatternMatcher(cset, comparer, orderingCategory);
                m.LessThan(x, a).LessThan(a, b).NextTo(y, b);

                while (m.Match())
                {
                    LessThanConstraint ltc = new LessThanConstraint(x.Value, y.Value, orderingCategory);

                    if (cset.Add(ltc))
                    {
                        updated = true;
                        Logger.LogInfo($"LessThan({x}, {a}) & LessThan({a}, {b}) & NextTo({y}, {b}) -> {ltc}");
                    }
                }

                m.Clear();

                m.NextTo(x, a).LessThan(a, b).LessThan(b, y);

                while (m.Match())
                {
                    LessThanConstraint ltc = new LessThanConstraint(x.Value, y.Value, orderingCategory);

                    if (cset.Add(ltc))
                    {
                        updated = true;
                        Logger.LogInfo($"NextTo({x}, {a}) & LessThan({a}, {b}) & LessThan({b}, {y}) -> {ltc}");
                    }
                }
            }

            return updated;
        }
    }
}
