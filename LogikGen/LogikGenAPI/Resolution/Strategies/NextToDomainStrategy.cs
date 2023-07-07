using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Resolution.Strategies
{
    public class NextToDomainStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.SimpleDomain;
        public override bool AutoRepeat => true;
        public override Difficulty Difficulty => Difficulty.Medium;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int initial = grid.TotalUnresolvedAssociations;

            foreach (Category orderingCategory in grid.PropertySet.OrderedCategories)
            {
                foreach (NextToConstraint ntc in cset.NextToConstraints(orderingCategory))
                {
                    int lastCount;

                    do
                    {
                        SubsetKey<Property> leftPosition = grid[ntc.Left, ntc.OrderingCategory];
                        SubsetKey<Property> rightPosition = grid[ntc.Right, ntc.OrderingCategory];

                        SubsetKey<Property> rightDomain = (leftPosition << 1) | (leftPosition >> 1);
                        SubsetKey<Property> leftDomain = (rightPosition << 1) | (rightPosition >> 1);

                        lastCount = grid.TotalUnresolvedAssociations;

                        if (grid.Update(ntc.Right, rightDomain))
                            Logger.LogInfo($"{ntc} -> {ntc.Right} = {grid[ntc.Right, orderingCategory]}");

                        if (grid.Update(ntc.Left, leftDomain))
                            Logger.LogInfo($"{ntc} -> {ntc.Left} = {grid[ntc.Left, orderingCategory]}");
                    }
                    while (grid.TotalUnresolvedAssociations < lastCount);
                }
            }

            int final = grid.TotalUnresolvedAssociations;
            return final < initial;
        }
    }
}
