using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Resolution.Strategies
{
    public class EitherOrArgumentUnionStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.Other;
        public override bool AutoRepeat => true;

        public override Difficulty Difficulty => Difficulty.Medium;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (EitherOrConstraint eoc in cset.EitherOrConstraints)
            {
                foreach (Category cat in grid.PropertySet.Categories)
                {
                    SubsetKey<Property> xset = grid[eoc.X, cat];
                    SubsetKey<Property> yset = grid[eoc.Y, cat];
                    
                    if (grid.Update(eoc.Key, xset | yset))
                        Logger.LogInfo($"{eoc} -> {eoc.Key} = {grid[eoc.Key, cat]}");
                    
                }
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
