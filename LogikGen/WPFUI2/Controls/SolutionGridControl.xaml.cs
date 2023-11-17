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
    /// Interaction logic for SolutionGridControl.xaml
    /// </summary>
    public partial class SolutionGridControl : UserControl
    {
        private DefinitionGridViewModel? _viewmodel;
        public DefinitionGridViewModel? ViewModel 
        { 
            get { return _viewmodel; }
            set
            {
                if (_viewmodel != null)
                    _viewmodel.PropertyChanged -= ViewModel_PropertyChanged;

                _viewmodel = value;

                if (_viewmodel != null)
                    _viewmodel.PropertyChanged += ViewModel_PropertyChanged;
            }
        }       

        public SolutionGridControl()
        {
            InitializeComponent();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DefinitionGridViewModel.SelectedCategoryCount) || 
                e.PropertyName == nameof(DefinitionGridViewModel.SelectedCategorySize))
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            gridPanel.Children.Clear();
            gridPanel.RowDefinitions.Clear();
            gridPanel.ColumnDefinitions.Clear();

            if (ViewModel == null)
                return;

            int nrows = ViewModel.SelectedCategoryCount;
            int ncols = ViewModel.SelectedCategorySize;

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

            // Every row of combo boxes represents a different category.
            for (int catIndex = 0; catIndex < nrows; catIndex++)
            {
                // Every column of combo boxes represents a different 'entity'
                // (combination of all associated properties across every category).
                for (int entIndex = 0; entIndex < ncols; entIndex++)
                {
                    ComboBox selection = new ComboBox();
                    
                    // Simply setting the .ItemSource of our combo box won't work, since the item names
                    // should automatically update when they're changed via the definitions tab.
                    for (int propIndex = 0; propIndex < ViewModel.SelectedCategorySize; propIndex++)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.DataContext = ViewModel.Categories[catIndex].Properties[propIndex];
                        item.SetBinding(ContentProperty, nameof(PropertyDefinitionViewModel.Name));
                        selection.Items.Add(item);
                    }

                    selection.SetValue(Grid.RowProperty, catIndex);
                    selection.SetValue(Grid.ColumnProperty, entIndex);

                    selection.DataContext = ViewModel.SolutionMatrix[catIndex, entIndex];
                    selection.SetBinding(Selector.SelectedIndexProperty, nameof(SolutionMatrixCellViewModel.SelectedPropertyIndex));

                    gridPanel.Children.Add(selection);
                }
            }
        }
    }
}
