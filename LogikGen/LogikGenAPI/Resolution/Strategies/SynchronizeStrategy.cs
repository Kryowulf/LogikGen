using LogikGenAPI.Model;

namespace LogikGenAPI.Resolution.Strategies
{
    public class SynchronizeStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.GridOnly;
        public override bool AutoRepeat => false;
        public override Difficulty Difficulty => Difficulty.Easy;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int initial = grid.TotalUnresolvedAssociations;

            grid.Synchronize();

            int final = grid.TotalUnresolvedAssociations;

            if (final < initial)
                Logger.LogInfo($"Resolved {initial - final} associations.");

            return final < initial;
        }
    }
}
