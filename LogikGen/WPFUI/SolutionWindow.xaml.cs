using LogikGenAPI.Examples;
using LogikGenAPI.Model;
using LogikGenAPI.Resolution;
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
    /// Interaction logic for SolutionWindow.xaml
    /// </summary>
    public partial class SolutionWindow : Window
    {
        private SolutionWindowViewModel _viewmodel;

        public SolutionWindow(PropertySet pset, double left = -1, double top = -1)
        {
            InitializeComponent();

            _viewmodel = new SolutionWindowViewModel(pset);
            this.DataContext = _viewmodel;

            if (left >= 0)
                this.Left = left;

            if (top >= 0)
                this.Top = top;
        }
        
        private void RandomizeButton_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.Randomize();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            DefinitionWindow window = new DefinitionWindow(_viewmodel.PropertySet, this.Left, this.Top);
            window.Show();
            this.Close();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SolutionGrid solution = _viewmodel.MakeSolution();
            GenerationWindow window = new GenerationWindow(solution, this.Left, this.Top);
            window.Show();
            this.Close();
        }
    }
}
