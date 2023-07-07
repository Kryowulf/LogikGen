using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;

namespace LogikGenAPI.Resolution.Strategies
{
    public class LessThanImpliesDistinctStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.DistinctEquivalent;
        public override bool AutoRepeat => false;
        public override Difficulty Difficulty => Difficulty.Easy;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (Category orderingCategory in grid.PropertySet.OrderedCategories)
            {
                foreach (LessThanConstraint ltc in cset.LessThanConstraints(orderingCategory))
                {
                    if (grid.Disassociate(ltc.Left, ltc.Right))
                        Logger.LogInfo(ltc);
                }
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
