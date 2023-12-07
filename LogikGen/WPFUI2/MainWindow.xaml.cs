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

        // SizeToContent almost does what I want but not completely.
        // It requires the strategy list to have a hard-coded MaxHeight, which looks ugly
        // when the user manually resizes the window.

        // As a hack, if the user manually resizes the window while the generation tab is open,
        // their preferred height will be remembered and the strategy list's MaxHeight will be eliminated.
        // All other tabs will automatically switch back to SizeToContent = Height.

        private double? _generationTabDesiredHeight = null;

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

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            RunGenerataor();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }

        private void ViewResultsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenResultsWindow();
        }

        private void OpenResultsWindow()
        {
            if (_resultWindow == null)
                _resultWindow = new ResultsWindow(this, _viewmodel.ProgressModel);

            _resultWindow.Show();
        }

        private async void RunGenerataor()
        {
            if (_viewmodel.IsRunning)
                return;

            _viewmodel.IsRunning = true;

            PropertySet pset;
            SolutionGrid solution;

            try
            {
                _viewmodel.BuildDefinitionModel(out pset, out solution);
            }
            catch (InvalidDefinitionException)
            {
                MainWindowTabs.SelectedIndex = 0;
                MessageBox.Show("Every category & property must have a unique name.", "Incomplete Definition", MessageBoxButton.OK, MessageBoxImage.Error);
                _viewmodel.IsRunning = false;
                return;
            }

            IList<StrategyTarget> strategyTargets = _viewmodel.BuildStrategyTargets();
            IList<ConstraintTarget> constraintTargets = _viewmodel.BuildConstraintTargets();
            int maxTotalConstraints = _viewmodel.ConstraintTargets.MaxTotalConstraints ?? int.MaxValue;

            PuzzleGenerator pgen = new PuzzleGenerator(solution, strategyTargets, constraintTargets, maxTotalConstraints);
            MultithreadedPuzzleGenerator mpgen = new MultithreadedPuzzleGenerator(pgen);
            _cts = new CancellationTokenSource();

            _viewmodel.ProgressModel.Generator = pgen;

            OpenResultsWindow();

            try
            {

                if (_viewmodel.IsGenerateUnsolvableChecked)
                {
                    UnsolvableAnalysisReport report = await Task.Run(() => mpgen.FindUnsolvablePuzzle(
                            _viewmodel.ProgressModel.UpdateSearchProgress, 
                            _cts.Token));

                    if (report.IsCancelled)
                    {
                        _viewmodel.ProgressModel.ShowMessage("Cancelled");
                    }
                    else
                    {
                        _viewmodel.ProgressModel.UpdateUnsolvableResult(report.Constraints);
                    }
                }
                else
                {
                    GenerationAnalysisReport finalReport = await Task.Run(() => mpgen.FindSatisfyingPuzzle(
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

            _viewmodel.IsRunning = false;
        }

        private void MainWindowTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainWindowTabs.SelectedItem == GenerationTab && _generationTabDesiredHeight.HasValue)
            {
                // Setting SizeToContent will immediately fire Window_SizeChanged, 
                // thereby causing _generationTabDesiredHeight itself to change.
                // Thus, its current value needs to be saved.

                double desiredHeight = _generationTabDesiredHeight.Value;
                this.SizeToContent = SizeToContent.Manual;
                this.Height = desiredHeight;
            }
            else
            {
                this.SizeToContent = SizeToContent.Height;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MainWindowTabs.SelectedItem == GenerationTab && this.SizeToContent == SizeToContent.Manual)
            {
                _generationTabDesiredHeight = this.Height;
                strategyGrid.MaxHeight = double.MaxValue;
            }
        }

        private void EasyButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Strategies now configured for an overall easy puzzle.", "Difficulty Preset", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MediumButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Strategies now configured for an overall medium puzzle.", "Difficulty Preset", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void HardButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Strategies now configured for an overall difficult puzzle.", "Difficulty Preset", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
