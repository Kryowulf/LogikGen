using LogikGenAPI.Model;
using LogikGenAPI.Resolution.Terms;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Resolution.Strategies
{
    public class DoubleNextToImpliesBetweenStrategy : MultipleConstraintStrategy
    {
        public override StrategyClassification Classification => StrategyClassification.DoubleNextTo;
        public override bool AutoRepeat => true;

        public DoubleNextToImpliesBetweenStrategy(IndirectionLevel level)
            : base(level)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (Category orderingCategory in grid.PropertySet.OrderedCategories)
            {
                Variable x = new Variable();
                Variable a = new Variable();
                Variable b = new Variable();

                PatternMatcher m = new PatternMatcher(cset, comparer, orderingCategory);
                m.NextTo(a, x).NextTo(x, b).Distinct(a, b);

                while (m.Match())
                {
                    SubsetKey<Property> neighborsOfX = grid[a.Value, orderingCategory] | grid[b.Value, orderingCategory];

                    if (neighborsOfX.Count < 2)
                    {
                        grid.FlagContradiction();
                    }
                    else
                    {
                        Property firstPosition = neighborsOfX[0];
                        Property lastPosition = neighborsOfX[neighborsOfX.Count - 1];
                        
                        if (grid.Update(x.Value, firstPosition.GreaterThan & lastPosition.LessThan))
                        {
                            Logger.LogInfo($"NextTo({a}, {x}) & NextTo({x}, {b}) & Distinct({a}, {b}) -> ");
                            Logger.LogInfo($"    {x}:{orderingCategory} = {grid[x.Value, orderingCategory]}");
                        }
                    }
                }
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
