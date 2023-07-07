using LogikGenAPI.Model;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Resolution.Strategies
{
    /*  
     *  LessThanManyIncompatibilityStrategy
     *  
     *      LessThan(x, a1) & LessThan(x, a2) & ... & LessThan(x, an)
     *      
     *      Let apos_set be the set of candidate positions for a1, a2, ..., an.
     *      Let apos_set_right be the last n positions of apos_set.
     *      
     *      If assigning all of apos_set_right to a1, a2, ..., an
     *      conflicts with the available positions of some other distinct property z
     *      
     *          then we know at least one of a1, a2, ..., an must be positioned somewhere
     *          before apos_set_right[0], and so xpos must come before that.
     *          
     * 
     * This is "Tom's Deduction" from 2019.
     * 
     *                 | 1 2 3 4
     *                 |
     *      Englishman |     . .
     *                 |
     *             Cat | .
     *             Dog | .     .
     *            Fish | . .
     *           Birth | O . . .
     * 
     *      The Englishman lives to the left of
     *      both the Cat owner and Dog owner.
     *      
     *      It cannot be that the cat/dog lives in houses 3/4.
     *      One of them has to live in house 2, otherwise
     *      the fish would have no place to live.
     *      
     *      Therefore, the Englishman must live in the first house.
     * 
     *      -------------------------------------------------------
     *      
     *      The LessThan constraints referred to above create a group:
     *      
     *          LessThanGroup(Key=Englishman, Value={Cat, Dog})
     *      
     *          valuePositions = { 2, 3, 4 }    the union of the positions of the cat & dog
     *          
     *          endPositions = { 3, 4 }         the rightmost possible positions for the cat & dog
     *          
     *          if assigning {cat, dog} to {3, 4} is not compatible with some other property in the grid,
     *          then we know that the position of Englishman is strictly less than 2.
     *      
     *      
     */


    public class LessThanManyCompatibilityCheckStrategy : CompatibilityStrategy
    {
        public LessThanManyCompatibilityCheckStrategy(bool sameCategoryOnly) 
            : base(sameCategoryOnly)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            int initialCount = grid.TotalUnresolvedAssociations;

            IList<OrderedGroup> lessThanGroups = cset.LessThanGroups(comparer);
            IList<OrderedGroup> greaterThanGroups = cset.GreaterThanGroups(comparer);

            UpdateKeyPositions(grid, comparer, lessThanGroups, "less than",
                (g, values) => values.Subtract(values[values.Count - g.Values.Count].LessThan),
                (g, values) => values[values.Count - g.Values.Count - 1].LessThan);

            UpdateKeyPositions(grid, comparer, greaterThanGroups, "greater than",
                (g, values) => values.Subtract(values[g.Values.Count - 1].GreaterThan),
                (g, values) => values[g.Values.Count].GreaterThan);

            return grid.TotalUnresolvedAssociations < initialCount;
        }

        private void UpdateKeyPositions(
            PuzzleGrid grid,
            IPropertyComparer comparer,
            IList<OrderedGroup> groups,
            string loggingGroupType,
            Func<OrderedGroup, SubsetKey<Property>, SubsetKey<Property>> selectEndPositions,
            Func<OrderedGroup, SubsetKey<Property>, SubsetKey<Property>> selectKeyPositions)
        {
            foreach (OrderedGroup g in groups)
            {
                SubsetKey<Property> valuePositions = g.Values.Aggregate(
                    g.OrderingCategory.Empty, (sk, p) => sk | grid[p, g.OrderingCategory]);

                if (valuePositions.Count < g.Values.Count)
                {
                    grid.FlagContradiction();
                    return;
                }

                SubsetKey<Property> endPositions = selectEndPositions(g, valuePositions);

                if (!AreAssociationsCompatible(grid, comparer, endPositions, g.Values.ToArray()))
                {
                    if (valuePositions.Count == endPositions.Count)
                    {
                        grid.FlagContradiction();
                        return;
                    }

                    SubsetKey<Property> keyPositions = selectKeyPositions(g, valuePositions);

                    if (grid.Update(g.Key, keyPositions))
                    {
                        Category orderingCategory = keyPositions.Source.Full[0].Category;
                        Logger.LogInfo($"Found {g.Key} is {loggingGroupType} the set {{{string.Join(", ", g.Values)}}}");
                        Logger.LogInfo($"      {g.Key}:{orderingCategory} = {grid[g.Key, orderingCategory]}");
                    }
                }
            }
        }
    }
}
