using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Resolution.Strategies
{
    public class EitherOrImpliesDistinctStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.DistinctEquivalent;
        public override bool AutoRepeat => false;

        public override Difficulty Difficulty => Difficulty.Easy;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (EitherOrConstraint eoc in cset.EitherOrConstraints)
            {
                bool updated = grid.Disassociate(eoc.X, eoc.Y);

                if (eoc.X.Category == eoc.Y.Category)
                {
                    SubsetKey<Property> xy = grid.PropertySet.ToSubset(eoc.X, eoc.Y);
                    updated |= grid.Update(eoc.Key, xy);
                }

                if (updated)
                    Logger.LogInfo(eoc);
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
