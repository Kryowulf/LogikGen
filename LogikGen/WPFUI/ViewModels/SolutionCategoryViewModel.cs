using LogikGenAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogikGenAPI.Utilities;

namespace WPFUI.ViewModels
{
    public class SolutionCategoryViewModel : ViewModel
    {
        private bool _performingSwap;

        public Category Category { get; private set; }
        public IReadOnlyList<SolutionPropertyViewModel> Properties { get; private set; }

        public SolutionCategoryViewModel(Category category)
        {
            this.Category = category;

            this.Properties = category.Properties.Select(
                p => new SolutionPropertyViewModel(this, p)).ToList().AsReadOnly();

            _performingSwap = false;
        }


        public Property Swap(Property oldValue, Property newValue)
        {
            if (!_performingSwap)
            {
                _performingSwap = true;

                int oldValueIndex = Properties.FindIndex(spvm => spvm.SelectedValue == oldValue);
                int newValueIndex = Properties.FindIndex(spvm => spvm.SelectedValue == newValue);

                this.Properties[oldValueIndex].SelectedValue = newValue;
                this.Properties[newValueIndex].SelectedValue = oldValue;

                _performingSwap = false;
            }

            return newValue;
        }
    }
}
