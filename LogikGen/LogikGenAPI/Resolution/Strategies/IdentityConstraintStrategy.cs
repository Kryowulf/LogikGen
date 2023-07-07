using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Resolution.Strategies
{
    public class IdentityConstraintStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.BasicAssertion;
        public override bool AutoRepeat => false;

        public override Difficulty Difficulty => Difficulty.Easiest;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (IdentityConstraint ic in cset.IdentityConstraints)
            {
                bool updated = false;

                for (int i = 0; i < ic.PairwiseDistinctProperties.Count - 1; i++)
                {
                    for (int j = i + 1; j < ic.PairwiseDistinctProperties.Count; j++)
                    {
                        Property a = ic.PairwiseDistinctProperties[i];
                        Property b = ic.PairwiseDistinctProperties[j];

                        updated |= grid.Disassociate(a, b);
                    }
                }

                if (updated)
                    Logger.LogInfo(ic);
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
