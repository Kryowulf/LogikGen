using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;

namespace LogikGenAPI.Resolution.Strategies
{
    public class EitherOrDomainStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.SimpleDomain;
        public override bool AutoRepeat => true;

        public override Difficulty Difficulty => Difficulty.Medium;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (EitherOrConstraint eoc in cset.EitherOrConstraints)
            {
                Property key = eoc.Key;
                Property x = eoc.X;
                Property y = eoc.Y;
                Category xcat = eoc.X.Category;
                Category ycat = eoc.Y.Category;

                if (grid[key, xcat].Intersect(x.Singleton).IsEmpty)
                {
                    if (grid.Associate(key, y))
                        Logger.LogInfo($"{key} = {y}");
                }

                if (grid[key, ycat].Intersect(y.Singleton).IsEmpty)
                {
                    if (grid.Associate(key, x))
                        Logger.LogInfo($"{key} = {x}");
                }
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
