using LogikGenAPI.Model;

namespace LogikGenAPI.Resolution.Strategies
{
    public class PropertyPairAnalysisStrategy : Strategy
    {
        public override StrategyClassification Classification => StrategyClassification.GridOnly;
        public override bool AutoRepeat => true;
        public override Difficulty Difficulty => Difficulty.Hard;

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            int initial = grid.TotalUnresolvedAssociations;

            PropertySet pset = grid.PropertySet;

            foreach (Property p in pset)
            {
                foreach (Category c in pset.Categories)
                {
                    if (2 <= grid[p, c].Count && grid[p, c].Count <= pset.CategorySize - 2)
                    {
                        foreach (Property q in pset)
                        {
                            if ((grid[p, c] & grid[q, c]) == c.Empty)
                            {
                                if (grid.Disassociate(p, q))
                                    Logger.LogInfo($"{p} != {q}");
                            }
                        }
                    }
                }
            }

            return grid.TotalUnresolvedAssociations < initial;
        }
    }
}
