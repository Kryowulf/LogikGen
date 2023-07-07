using LogikGenAPI.Examples;
using LogikGenAPI.Model;
using LogikGenAPI.Resolution;
using System;

namespace LogikGenTests
{
    public class PuzzleTestBase
    {
        protected PropertySet PSet { get; private set; }
        protected ConstraintSet CSet { get; private set; }
        protected PuzzleGrid Grid { get; private set; }
        

        protected void Setup(int totalCategories, int categorySize)
        {
            PSet = ZebraPuzzleBuilder.MakePropertySet(totalCategories, categorySize);
            CSet = new ConstraintSet(PSet);
            Grid = new PuzzleGrid(PSet);
        }

        protected bool GridMatch(IGrid other)
        {
            if (this.PSet != other.PropertySet)
                throw new ArgumentException("Test grid not based on the same property set.");

            foreach (Property p in PSet)
            {
                foreach (Category c in PSet.Categories)
                {
                    if (Grid[p, c] != other[p, c])
                        return false;
                }
            }

            return true;
        }

        protected bool CSetMatch(ConstraintSet other)
        {
            return this.CSet.SetEquals(other);
        }
    }
}
