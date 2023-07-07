using LogikGenAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Resolution.Strategies
{
    public class PigeonholeStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.GridOnly;

        public override bool AutoRepeat => true;

        public override Difficulty Difficulty => Difficulty.Hardest;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (Category c in grid.PropertySet.Categories) 
            {
                foreach (Property p in grid.PropertySet)
                {
                    if (grid[p, c].Count == 2)
                    {
                        // Build a set of all properties Q distinct from p,
                        // for which grid[q, c] = grid[p, c] for all q in Q.
                        //
                        // Assert those properties as all equal to each other.

                        List<Property> Q = (from q in grid.PropertySet
                                            where (grid[p, q.Category] & q.Singleton).IsEmpty
                                               && grid[p, c] == grid[q, c]
                                            select q).ToList();

                        for (int i = 1; i < Q.Count; i++)
                        {
                            if (grid.Associate(Q[0], Q[i]))
                                Logger.LogInfo($"{Q[0]} = {Q[i]}");
                        }
                    }
                }
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
