using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;

namespace LogikGenAPI.Resolution.Strategies
{
    public class EqualConstraintStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.BasicAssertion;
        public override bool AutoRepeat => false;
        public override Difficulty Difficulty => Difficulty.Easiest;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (EqualConstraint ec in cset.EqualConstraints)
            {
                if (grid.Associate(ec.Left, ec.Right))
                    Logger.LogInfo(ec);
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
