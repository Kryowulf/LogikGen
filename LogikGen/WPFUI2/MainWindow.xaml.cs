using LogikGenAPI.Examples;
using LogikGenAPI.Generation;
using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private MainViewModel _viewmodel;
        private CancellationTokenSource? _cts;
        private ResultsWindow? _resultWindow;

        public MainWindow()
        {
            InitializeComponent();

            _viewmodel = new MainViewModel();
            _cts = null;
            
            defgridControl.ViewModel = _viewmodel.Definitions;
            solgridControl.ViewModel = _viewmodel.Definitions;
            defgridControl.Refresh();
            solgridControl.Refresh();

            this.DataContext = _viewmodel;
        }

        private void SampleDataButton_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.Definitions.PopulateWithSampleData();
        }

        private void ShuffleDefinitionsButton_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.Definitions.Shuffle();
        }

        private void ClearDefinitionsButton_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.Definitions.Clear();
        }

        private void RandomizeSolutionButton_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.Definitions.RandomizeSolution();
        }

        private void ResetSolutionButton_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.Definitions.ResetSolution();
        }

        

        bool _isRunning = false;
        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning == false)
            {
                _isRunning = true;
                GenerateButton.Content = "Cancel";

                if (_resultWindow == null)
                    _resultWindow = new ResultsWindow(this, _viewmodel.ProgressModel);

                _resultWindow.Show();
                
                await Task.Run(() => RunGenerataor());
            }
            else
            {
                _cts?.Cancel();
                _isRunning = false;
                GenerateButton.Content = "Generate";
            }
        }

        private void ViewResultsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_resultWindow == null)
                _resultWindow = new ResultsWindow(this, _viewmodel.ProgressModel);

            _resultWindow.Show();
        }

        private async void RunGenerataor()
        {
            PropertySet pset;
            SolutionGrid solution;

            try
            {
                _viewmodel.BuildDefinitionModel(out pset, out solution);
            }
            catch (InvalidDefinitionException)
            {
                mainWindowTabs.SelectedIndex = 0;
                MessageBox.Show("Every category & property must have a unique name.", "Incomplete Definition", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IList<StrategyTarget> strategyTargets = _viewmodel.BuildStrategyTargets();
            IList<ConstraintTarget> constraintTargets = _viewmodel.BuildConstraintTargets();
            int maxTotalConstraints = _viewmodel.ConstraintTargets.MaxTotalConstraints ?? int.MaxValue;

            PuzzleGenerator pgen = new PuzzleGenerator(solution, strategyTargets, constraintTargets, maxTotalConstraints);
            MultithreadedPuzzleGenerator mpgen = new MultithreadedPuzzleGenerator(pgen);
            _cts = new CancellationTokenSource();

            _viewmodel.ProgressModel.Generator = pgen;

            try
            {

                if (_viewmodel.IsGenerateUnsolvableChecked)
                {
                    _viewmodel.ProgressModel.ShowMessage("Unsolvable Search Not Implemented Yet.");
                }
                else
                {
                    AnalysisReport finalReport = await Task.Run(() => mpgen.FindSatisfyingPuzzle(
                        _viewmodel.ProgressModel.UpdateReport, 
                        _viewmodel.ProgressModel.UpdateSearchProgress, 
                        _cts?.Token));

                    _viewmodel.ProgressModel.UpdateFinalReport(finalReport);
                }

            }
            catch(Exception ex)
            {
                // Print the error information and cancel the search.
                _viewmodel.ProgressModel.ShowMessage(ex.Message + "\n" + ex.StackTrace);
                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;
        }
    }
}
