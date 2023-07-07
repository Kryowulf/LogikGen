using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;

namespace LogikGenAPI.Resolution.Strategies
{
    public class NextToImpliesDistinctStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.DistinctEquivalent;
        public override bool AutoRepeat => false;
        public override Difficulty Difficulty => Difficulty.Easy;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (Category orderingCategory in grid.PropertySet.OrderedCategories)
            {
                foreach (NextToConstraint ntc in cset.NextToConstraints(orderingCategory))
                {
                    if (grid.Disassociate(ntc.Left, ntc.Right))
                        Logger.LogInfo(ntc);
                }
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
