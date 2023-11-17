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
using WPFUI2.ViewModels;

namespace WPFUI2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DefinitionGridViewModel _definitionsVM;

        public MainWindow()
        {
            InitializeComponent();

            _definitionsVM = new DefinitionGridViewModel();

            definitionGrid.ViewModel = _definitionsVM;
            solutionGrid.ViewModel = _definitionsVM;
            definitionGrid.Refresh();
            solutionGrid.Refresh();
        }

        private void SampleDataButton_Click(object sender, RoutedEventArgs e)
        {
            _definitionsVM.PopulateWithSampleData();
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            _definitionsVM.Shuffle();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _definitionsVM.Clear();
        }

        private void RandomizeSolutionButton_Click(object sender, RoutedEventArgs e)
        {
            _definitionsVM.RandomizeSolution();
        }

        private void ResetSolutionButton_Click(object sender, RoutedEventArgs e)
        {
            _definitionsVM.ResetSolution();
        }
    }
}
