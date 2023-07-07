using System.Linq;
using LogikGenAPI.Model;
using System.Collections.Generic;

namespace WPFUI.ViewModels
{
    public class CategoryDefinitionViewModel : ViewModel
    {
        private string _categoryName;
        public string CategoryName
        {
            get { return _categoryName; }
            set { SetValue(ref _categoryName, value); }
        }

        private bool _isOrdered;
        public bool IsOrdered
        {
            get { return _isOrdered; }
            set { SetValue(ref _isOrdered, value); }
        }

        private bool _isVisible;
        public bool IsVisible
        { 
            get { return _isVisible; }
            set { SetValue(ref _isVisible, value); }
        }

        public IReadOnlyList<PropertyDefinitionViewModel> PropertyDefinitions { get; private set; }

        public CategoryDefinitionViewModel(int maximumCategorySize)
        {
            this.CategoryName = "";
            this.IsOrdered = false;
            this.IsVisible = true;
            this.PropertyDefinitions = Enumerable.Range(0, maximumCategorySize).Select(i =>
                new PropertyDefinitionViewModel()).ToList().AsReadOnly();
        }

        public CategoryDefinition MakeDefinition()
        {
            IEnumerable<string> properties = this.PropertyDefinitions
                .Where(pdvm => pdvm.IsVisible)
                .Select(pdvm => pdvm.PropertyName);

            return new CategoryDefinition(this.CategoryName, this.IsOrdered, properties);
        }
    }
}
