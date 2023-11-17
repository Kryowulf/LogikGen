using LogikGenAPI.Examples;
using LogikGenAPI.Model;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Media;

namespace WPFUI2.Viewmodels
{
    public class DefinitionGridViewModel : ViewModel
    {
        public static int MaximumCategoryCount { get; } = 8;
        public static int MaximumCategorySize { get; } = 8;


        private int _selectedCategoryCount = 4;
        public int SelectedCategoryCount {
            get { return _selectedCategoryCount; }
            set {
                ResetSolution();
                SetValue(ref _selectedCategoryCount, value);
            }
        }


        private int _selectedCategorySize = 4;
        public int SelectedCategorySize {
            get { return _selectedCategorySize; }
            set {
                ResetSolution();
                SetValue(ref _selectedCategorySize, value);
            }
        }
        
        public IReadOnlyList<CategoryDefinitionViewModel> Categories { get; private set; }
        public SolutionMatrixViewModel SolutionMatrix { get; private set; }

        public DefinitionGridViewModel()
        {
            var categories = new List<CategoryDefinitionViewModel>();

            for (int i = 0; i < MaximumCategoryCount; i++)
                categories.Add(new CategoryDefinitionViewModel());

            this.Categories = categories.AsReadOnly();
            this.SolutionMatrix = new SolutionMatrixViewModel(MaximumCategoryCount, MaximumCategorySize);
        }

        public void PopulateWithSampleData()
        {
            for (int catIndex = 0; catIndex < MaximumCategoryCount; catIndex++)
            {
                CategoryDefinition def = ZebraPuzzleBuilder.AvailableCategories[catIndex];
                CategoryDefinitionViewModel defVM = Categories[catIndex];

                defVM.Name = def.CategoryName;
                defVM.IsOrdered = def.IsOrdered;

                for (int propIndex = 0; propIndex < MaximumCategorySize; propIndex++)
                    defVM.Properties[propIndex].Name = def.PropertyNames[propIndex];
            }

            ResetSolution();
        }

        public void Shuffle()
        {
            Random rgen = new Random();

            // Shuffle the names of the properties along with their matching solution matrix cells.

            // The names of the properties are shuffled rather than the properties themselves in 
            // order to keep databinding a bit simpler, avoiding the need for an ObservableCollection.
            // (in retrospect, the ObservableCollection approach might actually have been simpler).

            // The cells are shuffled along with them in order to restore the original solution.

            for (int catIndex = 0; catIndex < SelectedCategoryCount; catIndex++)
            {
                CategoryDefinitionViewModel catVM = Categories[catIndex];

                // Don't shuffle ordered categories.
                // Doing so will mess up the LessThan/NextTo relationships.
                if (catVM.IsOrdered)
                    continue;

                var propertyInfo = new List<(string Name, SolutionMatrixCellViewModel Cell)>();

                for (int propIndex = 0; propIndex < SelectedCategorySize; propIndex++)
                {
                    PropertyDefinitionViewModel propVM = catVM.Properties[propIndex];

                    // The .Single() call shouldn't raise an exception if the matrix has been properly managed. 
                    SolutionMatrixCellViewModel cellVM = Enumerable.Range(0, SelectedCategorySize)
                                                         .Select(entIndex => SolutionMatrix[catIndex, entIndex])
                                                         .Where(cell => cell.SelectedPropertyIndex == propIndex)
                                                         .Single();

                    propertyInfo.Add((propVM.Name, cellVM));
                }

                rgen.Shuffle(propertyInfo);

                for (int propIndex = 0; propIndex < SelectedCategorySize; propIndex++)
                {
                    PropertyDefinitionViewModel propVM = catVM.Properties[propIndex];
                    propVM.Name = propertyInfo[propIndex].Name;
                    propertyInfo[propIndex].Cell.SelectedPropertyIndex = propIndex;
                }
            }
        }

        public void Clear()
        {
            foreach (var cat in Categories)
            {
                cat.Name = "";
                cat.IsOrdered = false;

                foreach (var prop in cat.Properties)
                    prop.Name = "";
            }

            ResetSolution();
        }

        public void ResetSolution()
        {
            SolutionMatrix.Reset();
        }

        public void RandomizeSolution()
        {
            Random rgen = new Random();
            var selections = Enumerable.Range(0, SelectedCategorySize).ToList();

            for (int i = 0; i < SelectedCategoryCount; i++)
            {
                rgen.Shuffle(selections);

                for (int j = 0; j < SelectedCategorySize; j++)
                    SolutionMatrix[i, j].SelectedPropertyIndex = selections[j];
            }
        }
    }
}
