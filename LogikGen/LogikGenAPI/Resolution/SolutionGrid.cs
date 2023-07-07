using LogikGenAPI.Model;
using LogikGenAPI.Utilities;
using System;

namespace LogikGenAPI.Resolution
{
    public class SolutionGrid : IGrid
    {
        private SubsetKey<Property>[,] _grid;

        public PropertySet PropertySet { get; private set; }
        public SubsetKey<Property> this[Property row, Category column] => _grid[row.Index, column.Index];
        public int TotalUnresolvedAssociations => 0;
        public bool Contradiction => false;
        public bool Solved => true;
        public bool Complete => true;


        public SolutionGrid(IGrid grid)
        {
            if (!grid.Solved)
                throw new ArgumentException("SolutionGrid can only be constructed from a solved IGrid.");

            PropertySet pset = grid.PropertySet;

            _grid = new SubsetKey<Property>[pset.Properties.Count, pset.Categories.Count];
            
            foreach (Property row in pset.Properties)
                foreach (Category column in pset.Categories)
                    _grid[row.Index, column.Index] = grid[row, column];

            this.PropertySet = pset;
        }
    }
}
