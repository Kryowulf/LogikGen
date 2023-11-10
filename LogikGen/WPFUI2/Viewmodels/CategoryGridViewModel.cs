using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.Viewmodels
{
    public class CategoryGridViewModel
    {
        private CategoryDefinitionViewModel[] _categories;

        public static int MaxCategoryCount { get; } = 8;
        public static int MaxCategorySize { get; } = 8;
        public int SelectedCategoryCount { get; set; } = 4;
        public int SelectedCategorySize { get; set; } = 4;

        public CategoryDefinitionViewModel[] Categories { get { return _categories; } }

        public CategoryGridViewModel()
        {
            _categories = new CategoryDefinitionViewModel[MaxCategoryCount];

            for (int i = 0; i < MaxCategoryCount; i++)
                _categories[i] = new CategoryDefinitionViewModel();
        }
    }
}
