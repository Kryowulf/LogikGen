using LogikGenAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.ViewModels
{
    public class SolutionPropertyViewModel : ViewModel
    {
        public SolutionCategoryViewModel Parent { get; private set; }

        private Property _selectedValue;
        public Property SelectedValue
        {
            get { return _selectedValue; }
            set { SetValue(ref _selectedValue, Parent.Swap(_selectedValue, value)); }
        }

        public SolutionPropertyViewModel(SolutionCategoryViewModel parent, Property selectedValue)
        {
            this.Parent = parent;
            _selectedValue = selectedValue;
        }
    }
}
