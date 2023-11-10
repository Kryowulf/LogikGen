using LogikGenAPI.Examples;
using LogikGenAPI.Model;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFUI2.Controls;
using WPFUI2.Viewmodels;

namespace WPFUI2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SampleDataButton_Click(object sender, RoutedEventArgs e)
        {
            int categoryCount = CategoryGridViewModel.MaxCategoryCount;
            int categorySize = CategoryGridViewModel.MaxCategorySize;

            for (int catIndex = 0; catIndex < categoryCount; catIndex++)
            {
                CategoryDefinition def = ZebraPuzzleBuilder.AvailableCategories[catIndex];
                CategoryDefinitionViewModel defVM = catgrid.ViewModel.Categories[catIndex];

                defVM.Name = def.CategoryName;
                defVM.IsOrdered = def.IsOrdered;

                for (int propIndex = 0; propIndex < categorySize; propIndex++)
                {
                    defVM.Properties[propIndex].Name = def.PropertyNames[propIndex];
                }
            }

            catgrid.Refresh();
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Random rgen = new Random();

            // Only shuffle what's visible, not the whole 8x8 grid.
            int categoryCount = catgrid.ViewModel.SelectedCategoryCount;
            int categorySize = catgrid.ViewModel.SelectedCategorySize;

            for (int catIndex = 0; catIndex < categoryCount; catIndex++)
            {
                CategoryDefinitionViewModel defVM = catgrid.ViewModel.Categories[catIndex];

                // Ordered categories shouldn't be shuffled.
                if (!defVM.IsOrdered)
                {
                    List<string> shuffledProperties = defVM.Properties.Take(categorySize).Select(p => p.Name).ToList();
                    rgen.Shuffle(shuffledProperties);
                    
                    for (int propIndex = 0; propIndex < categorySize; propIndex++)
                    {
                        defVM.Properties[propIndex].Name = shuffledProperties[propIndex];
                    }
                }
            }

            catgrid.Refresh();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            int categoryCount = CategoryGridViewModel.MaxCategoryCount;
            int categorySize = CategoryGridViewModel.MaxCategorySize;

            for (int catIndex = 0; catIndex < categoryCount; catIndex++)
            {
                CategoryDefinitionViewModel defVM = catgrid.ViewModel.Categories[catIndex];

                defVM.Name = "";
                defVM.IsOrdered = false;

                for (int propIndex = 0; propIndex < categorySize; propIndex++)
                {
                    defVM.Properties[propIndex].Name = "";
                }
            }

            catgrid.Refresh();
        }
    }
}
