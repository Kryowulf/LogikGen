using LogikGenAPI.Model;
using LogikGenAPI.Resolution.Terms;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Resolution.Strategies
{
    /*
     *  ImmediateLessThanIncompatibilityStrategy
     *  
     *      LessThan(x, y) & NextTo(x, y)
     *  
     *      For every pair of candidate positions xypos for x & y
     *          
     *          if reserving xypos would cause another distinct property z
     *          to have no available positions left
     *          
     *              disassociate xypos[0] from x and xypos[1] from y.
     *              
     */

    public class ImmediateLessThanCompatibilityCheckStrategy : CompatibilityStrategy
    {
        public ImmediateLessThanCompatibilityCheckStrategy(bool sameCategoryOnly) 
            : base(sameCategoryOnly)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            int initial = grid.TotalUnresolvedAssociations;

            foreach (Category orderingCategory in cset.OrderedCategories)
            {
                Variable x = new Variable();
                Variable y = new Variable();

                PatternMatcher m = new PatternMatcher(cset, comparer, orderingCategory);
                m.LessThan(x, y).NextTo(x, y);

                while (m.Match())
                {
                    foreach (Property xpos in grid[x.Value, orderingCategory])
                    {
                        SubsetKey<Property> xypositions =
                            xpos.Singleton | (xpos.Singleton >> 1) & grid[y.Value, orderingCategory];

                        if (xypositions.Count == 2)
                        {
                            if (!AreAssociationsCompatible(grid, comparer, xypositions, x.Value, y.Value))
                            {
                                Property ypos = xypositions[1];

                                if (grid.Disassociate(x.Value, xpos))
                                    Logger.LogInfo($"LessThan({x}, {y}) & NextTo({x}, {y}) -> {x} != {xpos}");

                                if (grid.Disassociate(y.Value, ypos))
                                    Logger.LogInfo($"LessThan({x}, {y}) & NextTo({x}, {y}) -> {y} != {ypos}");
                            }
                        }
                    }
                }
            }

            return grid.TotalUnresolvedAssociations < initial;
        }
    }
}
