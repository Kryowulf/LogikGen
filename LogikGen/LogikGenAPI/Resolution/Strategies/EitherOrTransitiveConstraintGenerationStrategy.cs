using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution.Terms;

namespace LogikGenAPI.Resolution.Strategies
{
    public class EitherOrTransitiveConstraintGenerationStrategy : ConstraintGenerationStrategy
    {
        public override StrategyClassification Classification => StrategyClassification.ConstraintGeneration;
        public override bool AutoRepeat => true;

        public EitherOrTransitiveConstraintGenerationStrategy(IndirectionLevel level) 
            : base(level)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            bool updated = false;

            PatternMatcher m = new PatternMatcher(cset, comparer);
            Variable x = new(), y = new(), a = new(), b = new();

            m.EitherOr(x, a, b).EitherOr(y, a, b).Distinct(x, y);

            while (m.Match())
            {
                EitherOrConstraint eitherOrAXY = new EitherOrConstraint(a.Value, x.Value, y.Value);
                EitherOrConstraint eitherOrBXY = new EitherOrConstraint(b.Value, x.Value, y.Value);

                if (cset.Add(eitherOrAXY))
                {
                    updated = true;
                    Logger.LogInfo($"Added constraint {eitherOrAXY}");
                }

                if (cset.Add(eitherOrBXY))
                {
                    updated = true;
                    Logger.LogInfo($"Added constraint {eitherOrBXY}");
                }
            }

            return updated;
        }
    }
}
