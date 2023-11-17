using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace WPFUI2.Viewmodels
{
    public class CategoryDefinitionViewModel : ViewModel
    {
        private string _name = "";
        public string Name {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }

        private bool _isOrdered = false;
        public bool IsOrdered { 
            get { return _isOrdered; }
            set { SetValue(ref _isOrdered, value); }
        }

        public IReadOnlyList<PropertyDefinitionViewModel> Properties { get; private set; }

        public CategoryDefinitionViewModel()
        {
            var properties = new List<PropertyDefinitionViewModel>();

            for (int i = 0; i < DefinitionGridViewModel.MaximumCategorySize; i++)
                properties.Add(new PropertyDefinitionViewModel());

            this.Properties = properties.AsReadOnly();
        }
    }
}
