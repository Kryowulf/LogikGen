using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.Viewmodels
{
    // Implementing the solution grid is a bit of a challenge.
    // Each row represents a category.
    // Each column represents an entity.

    // Each cell contains a SelectedPropertyIndex, which represents the index
    // of the property that belongs to that cell's corresponding entity.

    // We can't make the SolutionMatrix refer to PropertyDefinitionViewModel objects directly,
    // because for data-binding purposes shuffling properties in a category will only
    // shuffle the names, not the PropertyDefinitionViewModel objects themselves.

    public class SolutionMatrixViewModel
    {
        private SolutionMatrixCellViewModel[,] _cells;

        public int TotalCategories { get; private set; }
        public int TotalEntities { get; private set; }
        public SolutionMatrixCellViewModel this[int cat, int ent] => _cells[cat, ent];

        public SolutionMatrixViewModel(int nCategories, int nEntities)
        {
            _cells = new SolutionMatrixCellViewModel[nCategories, nEntities];

            for (int i = 0; i < nCategories; i++)
                for (int j = 0; j < nEntities; j++)
                    _cells[i, j] = new SolutionMatrixCellViewModel(this, i, j);

            this.TotalCategories = nCategories;
            this.TotalEntities = nEntities;
        }

        public void Reset()
        {
            for (int i = 0; i < TotalCategories; i++)
                for (int j = 0; j < TotalEntities; j++)
                    _cells[i, j].SelectedPropertyIndex = j;
        }
    }
}
