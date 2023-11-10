using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.Viewmodels
{
    public class CategoryDefinitionViewModel
    {
        private PropertyDefinitionViewModel[] _properties;

        public string Name { get; set; } = "";
        public bool IsOrdered { get; set; } = false;
        public PropertyDefinitionViewModel[] Properties { get { return _properties; } }

        public CategoryDefinitionViewModel()
        {
            _properties = new PropertyDefinitionViewModel[CategoryGridViewModel.MaxCategorySize];

            for (int i = 0; i < CategoryGridViewModel.MaxCategorySize; i++)
                _properties[i] = new PropertyDefinitionViewModel();
        }
    }
}
