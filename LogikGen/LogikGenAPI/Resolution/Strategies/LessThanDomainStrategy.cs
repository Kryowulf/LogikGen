using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Utilities;
using System.Linq;

namespace LogikGenAPI.Resolution.Strategies
{
    public class LessThanDomainStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.SimpleDomain;
        public override bool AutoRepeat => true;
        public override Difficulty Difficulty => Difficulty.Medium;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (Category orderingCategory in grid.PropertySet.OrderedCategories)
            {
                foreach (LessThanConstraint ltc in cset.LessThanConstraints(orderingCategory))
                {
                    int lastCount;

                    do
                    {
                        SubsetKey<Property> leftPosition = grid[ltc.Left, ltc.OrderingCategory];
                        SubsetKey<Property> rightPosition = grid[ltc.Right, ltc.OrderingCategory];

                        if (leftPosition.Count == 0 || rightPosition.Count == 0)
                        {
                            grid.FlagContradiction();
                            return true;
                        }

                        lastCount = grid.TotalUnresolvedAssociations;

                        if (grid.Update(ltc.Right, leftPosition.First().GreaterThan))
                            Logger.LogInfo($"{ltc} -> {ltc.Right} = {grid[ltc.Right, orderingCategory]}");

                        if (grid.Update(ltc.Left, rightPosition.Last().LessThan))
                            Logger.LogInfo($"{ltc} -> {ltc.Left} = {grid[ltc.Left, orderingCategory]}");
                    }
                    while (grid.TotalUnresolvedAssociations < lastCount);
                }
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
