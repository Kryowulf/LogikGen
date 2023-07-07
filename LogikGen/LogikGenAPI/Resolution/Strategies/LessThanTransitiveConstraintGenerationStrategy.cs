using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution.Terms;
using System;

namespace LogikGenAPI.Resolution.Strategies
{
    public class LessThanTransitiveConstraintGenerationStrategy : ConstraintGenerationStrategy
    {
        public override StrategyClassification Classification => StrategyClassification.ConstraintGeneration;
        public override bool AutoRepeat => true;

        public LessThanTransitiveConstraintGenerationStrategy(bool useIndirectEqual)
            : base(useIndirectEqual ? IndirectionLevel.IndirectEqualOnly : IndirectionLevel.Direct)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            bool updated = false;

            foreach (Category orderingCategory in cset.OrderedCategories)
            {
                Variable a = new Variable();
                Variable b = new Variable();
                Variable c = new Variable();

                PatternMatcher m = new PatternMatcher(cset, comparer, orderingCategory);
                m.LessThan(a, b).LessThan(b, c);

                while (m.Match())
                {
                    LessThanConstraint ltc = new LessThanConstraint(a.Value, c.Value, orderingCategory);

                    if (cset.Add(ltc))
                    {
                        updated = true;
                        Logger.LogInfo($"LessThan({a}, {b}) & LessThan({b}, {c}) -> {ltc}");
                    }
                }
            }

            return updated;
        }
    }
}
