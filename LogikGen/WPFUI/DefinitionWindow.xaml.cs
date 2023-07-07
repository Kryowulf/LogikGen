using LogikGenAPI.Model;
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
using System.Windows.Shapes;
using WPFUI.ViewModels;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for DefinitionWindow.xaml
    /// </summary>
    public partial class DefinitionWindow : Window
    {
        private DefinitionWindowViewModel _viewmodel;

        public DefinitionWindow()
        {
            InitializeComponent();

            _viewmodel = new DefinitionWindowViewModel();
            this.DataContext = _viewmodel;
        }

        public DefinitionWindow(PropertySet pset, double left = -1, double top = -1)
        {
            InitializeComponent();

            _viewmodel = new DefinitionWindowViewModel(pset);
            this.DataContext = _viewmodel;

            if (left >= 0)
                this.Left = left;

            if (top >= 0)
                this.Top = top;
        }

        private void CategoryCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void CategorySize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 1; i < headingsPanel.Children.Count - 1; i++)
            {
                if (i <= _viewmodel.SelectedCategorySize)
                    headingsPanel.Children[i].Visibility = Visibility.Visible;
                else
                    headingsPanel.Children[i].Visibility = Visibility.Collapsed;
            }

            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void SampleDataButton_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.PopulateWithSampleData();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.Clear();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewmodel.TryMakePropertySet(out PropertySet pset))
            {
                SolutionWindow nextWindow = new SolutionWindow(pset, this.Left, this.Top);
                nextWindow.Show();
                this.Close();
            }
            else
            {
                // BooleanToVisibilityConverter converts to "Collapsed" instead of "Hidden".
                // I could write a new converter to databind the visibility, but for now
                // it's easier to set it directly.
                duplicatesErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void RandomizeButton_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.Randomize();
        }
    }
}
