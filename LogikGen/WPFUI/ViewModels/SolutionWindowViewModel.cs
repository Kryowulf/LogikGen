using LogikGenAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogikGenAPI.Utilities;
using LogikGenAPI.Resolution;

namespace WPFUI.ViewModels
{
    public class SolutionWindowViewModel : ViewModel
    {
        public PropertySet PropertySet { get; private set; }

        public IReadOnlyList<SolutionCategoryViewModel> Categories { get; private set; }

        public SolutionWindowViewModel(PropertySet pset)
        {
            this.PropertySet = pset;

            this.Categories = pset.Categories.Select(c =>
                new SolutionCategoryViewModel(c)).ToList().AsReadOnly();
        }
        
        public void Randomize()
        {
            Random rgen = new Random();

            foreach (SolutionCategoryViewModel catvm in this.Categories)
            {
                List<Property> properties = catvm.Category.Properties.ToList();
                rgen.Shuffle(properties);

                for (int i = 0; i < properties.Count; i++)
                    catvm.Swap(catvm.Properties[i].SelectedValue, properties[i]);
            }
        }

        public SolutionGrid MakeSolution()
        {
            PropertySet pset = this.PropertySet;
            PuzzleGrid grid = new PuzzleGrid(pset);

            // Build a solved grid based on the user's selections.

            // For every column on the solution window.
            for (int j = 0; j < pset.CategorySize; j++)
            {
                // Grab the property at row 0 in this column.
                Property primary = this.Categories[0].Properties[j].SelectedValue;

                // For every row in the solution window after the first.
                for (int i = 1; i < pset.Categories.Count; i++)
                {
                    Property associated = this.Categories[i].Properties[j].SelectedValue;
                    grid.Update(primary, associated.Singleton);
                }
            }

            grid.Synchronize();

            return grid.AsSolution();
        }
    }
}
