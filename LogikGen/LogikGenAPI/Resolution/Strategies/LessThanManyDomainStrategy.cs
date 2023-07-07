using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Resolution.Strategies
{
    public class LessThanManyDomainStrategy : MultipleConstraintStrategy
    {
        public override StrategyClassification Classification => StrategyClassification.SimpleDomain;
        public override bool AutoRepeat => true;

        public LessThanManyDomainStrategy(IndirectionLevel level)
            : base(level)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            IList<OrderedGroup> lessThanGroups = cset.LessThanGroups(comparer);
            UpdateKeyPositions(grid, lessThanGroups, "less than", (n, vpos) => vpos.TakeLast(n).First().LessThan);

            IList<OrderedGroup> greaterThanGroups = cset.GreaterThanGroups(comparer);
            UpdateKeyPositions(grid, greaterThanGroups, "greater than", (n, vpos) => vpos.Take(n).Last().GreaterThan);

            return grid.TotalUnresolvedAssociations < originalCount;
        }

        private void UpdateKeyPositions(
            PuzzleGrid grid, 
            IList<OrderedGroup> groups, 
            string loggingGroupType,
            Func<int, SubsetKey<Property>, SubsetKey<Property>> selectKeyPositions)
        {
            foreach (OrderedGroup g in groups)
            {
                SubsetKey<Property> valuePositions = g.Values.Aggregate(
                    g.OrderingCategory.Empty, (sk, p) => sk | grid[p, g.OrderingCategory]);

                if (valuePositions.Count < g.Values.Count)
                    grid.FlagContradiction();

                SubsetKey<Property> keyPositions = selectKeyPositions(g.Values.Count, valuePositions);

                if (grid.Update(g.Key, keyPositions))
                {
                    Category orderingCategory = keyPositions.Source.Full[0].Category;
                    Logger.LogInfo($"Found {g.Key} is {loggingGroupType} the set {{{string.Join(", ", g.Values)}}}");
                    Logger.LogInfo($"      {g.Key}:{orderingCategory} = {grid[g.Key, orderingCategory]}");
                }
            }
        }
    }
}
