using LogikGenAPI.Model;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Resolution.Strategies
{
    public class BlockCrossoutStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.GridOnly;
        public override bool AutoRepeat => true;
        public override Difficulty Difficulty => Difficulty.Hard;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int initial = grid.TotalUnresolvedAssociations;

            PropertySet pset = grid.PropertySet;

            foreach (Category c1 in pset.Categories)
            {
                foreach (Category c2 in pset.Categories)
                {
                    if (c1 != c2)
                    {
                        for (int n = 2; n <= pset.CategorySize - 2; n++)
                        {
                            for (int prime_index = 0; prime_index < c1.Count - n + 1; prime_index++)
                            {
                                Property prime = c1[prime_index];

                                if (grid[prime, c2].Count == n)
                                {
                                    SubsetKey<Property> grouping = prime.Singleton;

                                    for (int j = prime_index + 1; j < c1.Count; j++)
                                    {
                                        Property pj = c1[j];

                                        if (grid[prime, c2] == grid[pj, c2])
                                            grouping |= pj.Singleton;
                                    }

                                    if (grouping.Count >= n)
                                    {
                                        foreach (Property q in ~grouping)
                                        {
                                            if (grid.Update(q, ~grid[prime, c2]))
                                                Logger.LogInfo($"{q}:{c2} = {grid[q, c2]}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return grid.TotalUnresolvedAssociations < initial;
        }
    }
}
