using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Resolution.Strategies
{
    /*  
     *      NextToCheckCompatibilityStrategy
     *      
     *      A more relaxed version of ImmediateLessThanCheckCompatibilityStrategy.
     *      
     *      NextTo(x, y)
     *      
     *      For every candidate position xpos of x
     *      
     *          Let leftOfX  be the position immediately left  of xpos.
     *              rightOfX be the position immediately right of xpos.
     * 
     *          if neither of the sets { leftOfX, xpos } and { rightOfX, xpos }
     *          can be assigned to the properties x and y - either due 
     *          to leftOfX and rightOfX being empty, or due to a conflict 
     *          with the available positions of some other distinct property z 
     *              
     *              then disassociate xpos from x
     *              
     *      Do the same thing for the candidate positions ypos of y.
     */

    public class NextToCompatibilityCheckStrategy : CompatibilityStrategy
    {
        public NextToCompatibilityCheckStrategy(bool sameCategoryOnly) 
            : base(sameCategoryOnly)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            int initial = grid.TotalUnresolvedAssociations;

            foreach (Category orderingCategory in cset.OrderedCategories)
            {
                foreach (NextToConstraint ntc in cset.NextToConstraints(orderingCategory))
                {
                    Property x = ntc.Left;
                    Property y = ntc.Right;

                    foreach (Property xpos in grid[x, orderingCategory])
                    {
                        SubsetKey<Property> leftOfX = (xpos.Singleton << 1) & grid[y, orderingCategory];
                        SubsetKey<Property> rightOfX = (xpos.Singleton >> 1) & grid[y, orderingCategory];

                        if ((leftOfX.IsEmpty || !AreAssociationsCompatible(grid, comparer, xpos.Singleton | leftOfX, x, y)) &&
                            (rightOfX.IsEmpty || !AreAssociationsCompatible(grid, comparer, xpos.Singleton | rightOfX, x, y)))
                        {
                            if (grid.Disassociate(x, xpos))
                                Logger.LogInfo($"{ntc} -> {x} != {xpos}.");
                        }
                    }

                    foreach (Property ypos in grid[y, orderingCategory])
                    {
                        SubsetKey<Property> leftOfY = (ypos.Singleton << 1) & grid[x, orderingCategory];
                        SubsetKey<Property> rightOfY = (ypos.Singleton >> 1) & grid[x, orderingCategory];

                        if ((leftOfY.IsEmpty || !AreAssociationsCompatible(grid, comparer, ypos.Singleton | leftOfY, x, y)) && 
                            (rightOfY.IsEmpty || !AreAssociationsCompatible(grid, comparer, ypos.Singleton | rightOfY, x, y)))
                        {
                            if (grid.Disassociate(y, ypos))
                                Logger.LogInfo($"{ntc} -> {y} != {ypos}.");
                        }
                    }
                }
            }

            return grid.TotalUnresolvedAssociations < initial;
        }
    }
}
