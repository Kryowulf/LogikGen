using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;

namespace LogikGenAPI.Resolution.Strategies
{
    public class DistinctConstraintStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.BasicAssertion;
        public override bool AutoRepeat => false;
        public override Difficulty Difficulty => Difficulty.Easiest;


        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (DistinctConstraint dc in cset.DistinctConstraints)
            {
                if (grid.Disassociate(dc.Left, dc.Right))
                    this.Logger.LogInfo(dc);
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
