using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.ViewModels
{
    public class SolutionMatrixCellViewModel : ViewModel
    {
        public SolutionMatrixViewModel Matrix { get; private set; }
        public int CategoryIndex { get; private set; }
        public int EntityIndex { get; private set; }

        private int _selectedPropertyIndex;
        public int SelectedPropertyIndex 
        { 
            get { return _selectedPropertyIndex; }
            set { Swap(value); }
        }

        public SolutionMatrixCellViewModel(SolutionMatrixViewModel matrix, int cat, int ent)
        {
            this.Matrix = matrix;
            this.CategoryIndex = cat;
            this.EntityIndex = ent;
            this.SelectedPropertyIndex = ent;
        }

        private static bool _swapping = false;
        private void Swap(int newPropIdx)
        {
            if (!_swapping)
            {
                _swapping = true;

                SolutionMatrixCellViewModel? other = Enumerable.Range(0, Matrix.TotalEntities)
                                                     .Select(ent => Matrix[this.CategoryIndex, ent])
                                                     .Where(cell => cell.SelectedPropertyIndex == newPropIdx)
                                                     .SingleOrDefault();

                other?.SetValue(ref other._selectedPropertyIndex, this._selectedPropertyIndex, nameof(SelectedPropertyIndex));
                this.SetValue(ref this._selectedPropertyIndex, newPropIdx, nameof(SelectedPropertyIndex));

                _swapping = false;
            }
        }
    }
}
