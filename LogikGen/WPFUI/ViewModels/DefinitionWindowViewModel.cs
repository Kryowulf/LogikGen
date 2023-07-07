using LogikGenAPI.Examples;
using LogikGenAPI.Model;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.ViewModels
{
    public class DefinitionWindowViewModel : ViewModel
    {
        public int MinimumCategoryCount => 3;
        public int MaximumCategoryCount => 8;
        public int DefaultSelectedCount => 4;
        public int MinimumCategorySize => 3;
        public int MaximumCategorySize => 8;
        public int DefaultSelectedSize => 4;

        public IReadOnlyList<int> AvailableCategoryCounts { get; private set; }
        public IReadOnlyList<int> AvailableCategorySizes { get; private set; }


        private int _selectedCategoryCount;
        public int SelectedCategoryCount 
        { 
            get 
            { 
                return _selectedCategoryCount; 
            }
            set 
            { 
                SetValue(ref _selectedCategoryCount, value);
                UpdateVisibilities();
            }
        }

        private int _selectedCategorySize;
        public int SelectedCategorySize 
        { 
            get
            {
                return _selectedCategorySize;
            }
            set
            {
                SetValue(ref _selectedCategorySize, value);
                UpdateVisibilities();
            }
        }


        public IReadOnlyList<CategoryDefinitionViewModel> CategoryDefinitions { get; private set; }

        public DefinitionWindowViewModel()
        {
            this.AvailableCategoryCounts = Enumerable.Range(
                    this.MinimumCategoryCount, 
                    this.MaximumCategoryCount - this.MinimumCategoryCount + 1)
                .ToList().AsReadOnly();

            this.AvailableCategorySizes = Enumerable.Range(
                    this.MinimumCategorySize,
                    this.MaximumCategorySize - this.MinimumCategorySize + 1)
                .ToList().AsReadOnly();

            this.CategoryDefinitions = Enumerable.Range(0, this.MaximumCategoryCount).Select(i =>
                new CategoryDefinitionViewModel(this.MaximumCategorySize)).ToList().AsReadOnly();

            this.SelectedCategoryCount = this.DefaultSelectedCount;
            this.SelectedCategorySize = this.DefaultSelectedSize;
        }

        public DefinitionWindowViewModel(PropertySet pset)
            : this()
        {
            this.SelectedCategoryCount = pset.Categories.Count;
            this.SelectedCategorySize = pset.CategorySize;

            for (int i = 0; i < this.MaximumCategoryCount; i++)
            {
                CategoryDefinitionViewModel cdef = this.CategoryDefinitions[i];

                if (i < pset.Categories.Count)
                {
                    cdef.CategoryName = pset.Categories[i].Name;
                    cdef.IsOrdered = pset.Categories[i].IsOrdered;

                    for (int j = 0; j < this.MaximumCategorySize; j++)
                    {
                        if (j < pset.CategorySize)
                            cdef.PropertyDefinitions[j].PropertyName = pset.Categories[i][j].Name;
                        else
                            cdef.PropertyDefinitions[j].PropertyName = "";
                    }
                }
                else
                {
                    cdef.CategoryName = "";
                    cdef.IsOrdered = false;

                    foreach (PropertyDefinitionViewModel pdef in cdef.PropertyDefinitions)
                        pdef.PropertyName = "";
                }
            }
        }

        public bool TryMakePropertySet(out PropertySet pset)
        {
            try
            {
                IEnumerable<CategoryDefinition> definitions = this.CategoryDefinitions
                    .Where(cdvm => cdvm.IsVisible)
                    .Select(csvm => csvm.MakeDefinition());

                pset = new PropertySet(definitions);
                return true;
            }
            catch (ArgumentException)
            {
                pset = null;
                return false;
            }
        }

        public void PopulateWithSampleData()
        {
            PropertySet pset = ZebraPuzzleBuilder.MakePropertySet(MaximumCategoryCount, MaximumCategorySize);

            int nCategories = Math.Min(pset.Categories.Count, this.MaximumCategoryCount);
            int categorySize = Math.Min(pset.CategorySize, this.MaximumCategorySize);

            for (int i = 0; i < nCategories; i++)
            {
                CategoryDefinitionViewModel cdef = this.CategoryDefinitions[i];
                cdef.CategoryName = pset.Categories[i].Name;
                cdef.IsOrdered = pset.Categories[i].IsOrdered;

                for (int j = 0; j < categorySize; j++)
                    cdef.PropertyDefinitions[j].PropertyName = pset.Categories[i][j].Name;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < this.MaximumCategoryCount; i++)
            {
                CategoryDefinitionViewModel cdef = this.CategoryDefinitions[i];
                cdef.CategoryName = "";
                cdef.IsOrdered = false;

                for (int j = 0; j < this.MaximumCategorySize; j++)
                    cdef.PropertyDefinitions[j].PropertyName = "";
            }
        }

        public void Randomize()
        {
            Random rgen = new Random();

            foreach (CategoryDefinitionViewModel cdef in this.CategoryDefinitions)
            {
                if (cdef.IsVisible && !cdef.IsOrdered)
                {
                    List<string> properties = cdef.PropertyDefinitions.Where(pdef => pdef.IsVisible)
                                                                      .Select(pdef => pdef.PropertyName)
                                                                      .ToList();

                    rgen.Shuffle(properties);

                    for (int i = 0; i < properties.Count; i++)
                        cdef.PropertyDefinitions[i].PropertyName = properties[i];
                }
            }
        }

        private void UpdateVisibilities()
        {
            for (int i = 0; i < this.MaximumCategoryCount; i++)
            {
                CategoryDefinitionViewModel cdef = this.CategoryDefinitions[i];
                cdef.IsVisible = i < this.SelectedCategoryCount;

                for (int j = 0; j < this.MaximumCategorySize; j++)
                    cdef.PropertyDefinitions[j].IsVisible = j < this.SelectedCategorySize;
            }
        }
    }
}
