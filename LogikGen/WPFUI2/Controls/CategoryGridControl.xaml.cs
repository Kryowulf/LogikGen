using LogikGenAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFUI2.Viewmodels;

namespace WPFUI2.Controls
{
    /// <summary>
    /// Interaction logic for PropertyDefinitionGridControl.xaml
    /// </summary>
    public partial class CategoryGridControl : UserControl
    {
        private bool _initialized = false;

        public CategoryGridViewModel ViewModel { get; } = new CategoryGridViewModel();
        
        public CategoryGridControl()
        {
            InitializeComponent();
            _initialized = true;
            Refresh();
        }

        private void categorySizeInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;

            ComboBoxItem selectedItem = (ComboBoxItem)categorySizeInput.SelectedItem;
            string selectedValue = (string)selectedItem.Content;
            ViewModel.SelectedCategorySize = int.Parse(selectedValue);
            Refresh();
        }

        private void categoryCountInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;

            ComboBoxItem selectedItem = (ComboBoxItem)categoryCountInput.SelectedItem;
            string selectedValue = (string)selectedItem.Content;
            ViewModel.SelectedCategoryCount = int.Parse(selectedValue);
            Refresh();
        }

        public void Refresh()
        {
            gridPanel.Children.Clear();
            gridPanel.RowDefinitions.Clear();
            gridPanel.ColumnDefinitions.Clear();

            // + 1 for the heading at row 0.
            int nrows = ViewModel.SelectedCategoryCount + 1;

            // + 1 for the category name at column 0
            // + 1 for the "is ordered" column on the end
            int ncols = ViewModel.SelectedCategorySize + 2;

            for (int i = 0; i < nrows; i++)
            {
                RowDefinition rdef = new RowDefinition();
                gridPanel.RowDefinitions.Add(rdef);
            }
            
            for (int j = 0; j < ncols; j++)
            {
                ColumnDefinition cdef = new ColumnDefinition();
                gridPanel.ColumnDefinitions.Add(cdef);
            }

            TextBlock heading1 = new TextBlock();
            heading1.Text = "Category Name";
            heading1.SetValue(Grid.RowProperty, 0);
            heading1.SetValue(Grid.ColumnProperty, 0);
            gridPanel.Children.Add(heading1);

            TextBlock heading2 = new TextBlock();
            heading2.Text = "Property Names";
            heading2.SetValue(Grid.RowProperty, 0);
            heading2.SetValue(Grid.ColumnProperty, 1);
            gridPanel.Children.Add(heading2);

            TextBlock heading3 = new TextBlock();
            heading3.Text = "Is Ordered?";
            heading3.SetValue(Grid.RowProperty, 0);
            heading3.SetValue(Grid.ColumnProperty, ncols - 1);
            gridPanel.Children.Add(heading3);

            for (int rowIndex = 1, categoryIndex = 0; 
                rowIndex <= ViewModel.SelectedCategoryCount; 
                rowIndex++, categoryIndex++)
            {
                TextBox categoryNameInput = new TextBox();
                categoryNameInput.SetValue(Grid.RowProperty, rowIndex);
                categoryNameInput.SetValue(Grid.ColumnProperty, 0);
                gridPanel.Children.Add(categoryNameInput);

                categoryNameInput.DataContext = ViewModel.Categories[categoryIndex];
                categoryNameInput.SetBinding(TextBox.TextProperty, nameof(CategoryDefinitionViewModel.Name));

                for (int colIndex = 1, propertyIndex = 0; 
                    colIndex <= ViewModel.SelectedCategorySize; 
                    colIndex++, propertyIndex++)
                {
                    TextBox propertyNameInput = new TextBox();
                    propertyNameInput.SetValue(Grid.RowProperty, rowIndex);
                    propertyNameInput.SetValue(Grid.ColumnProperty, colIndex);
                    gridPanel.Children.Add(propertyNameInput);

                    propertyNameInput.DataContext = ViewModel.Categories[categoryIndex].Properties[propertyIndex];
                    propertyNameInput.SetBinding(TextBox.TextProperty, nameof(PropertyDefinitionViewModel.Name));
                }

                CheckBox isOrderedInput = new CheckBox();
                isOrderedInput.SetValue(Grid.RowProperty, rowIndex);
                isOrderedInput.SetValue(Grid.ColumnProperty, ncols - 1);
                gridPanel.Children.Add(isOrderedInput);

                isOrderedInput.DataContext = ViewModel.Categories[categoryIndex];
                isOrderedInput.SetBinding(ToggleButton.IsCheckedProperty, nameof(CategoryDefinitionViewModel.IsOrdered));
            }
        }
    }
}
