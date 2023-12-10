using LogikGenAPI.Examples;
using LogikGenAPI.Generation;
using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            _viewmodel.IsCancelling = true;
            _cts?.Cancel();
            OpenResultsWindow();
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
            _resultWindow.WindowState = WindowState.Normal;
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
                        _viewmodel.ProgressModel.UpdateUnsolvableReport(report);
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
            catch(UnderspecifiedGenerationException)
            {
                _viewmodel.ProgressModel.ShowMessage(
                    "It is not possible to generate a puzzle fulfilling the selected settings.\n" + 
                    "Please review the chosen constraint targets and enabled strategies.");

                _cts.Cancel();
            }
            catch(GenerationException ex)
            {
                _viewmodel.ProgressModel.ShowMessage(
                    "An unexpected error occurred in the generation process:\n" + 
                    ex.Message + "\n\n" + 
                    "Adjusting which strategies are enabled or disabled may resolve this.");

                _cts.Cancel();
            }
            catch(Exception ex)
            {
                _viewmodel.ProgressModel.ShowMessage("An unexpected error occurred. Debugging information is printed below:\n\n" +
                    ex.Message + "\n\n" + 
                    ex.StackTrace);

                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;

            _viewmodel.IsRunning = false;
            _viewmodel.IsCancelling = false;
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
            Dictionary<string, StrategyViewModel> svmByName = new Dictionary<string, StrategyViewModel>();

            foreach (StrategyViewModel svm in _viewmodel.StrategyList)
            {
                svmByName[svm.Name] = svm;
                svm.IsEnabled = false;
                svm.MinimumApplications = 0;
                svm.MaximumApplications = null;
            }

            // Enable only the most basic strategies.
            // Don't require anything.

            svmByName["SynchronizeStrategy"].IsEnabled = true;

            svmByName["DistinctConstraintStrategy"].IsEnabled = true;
            svmByName["EqualConstraintStrategy"].IsEnabled = true;
            svmByName["IdentityConstraintStrategy"].IsEnabled = true;

            svmByName["EitherOrImpliesDistinctStrategy"].IsEnabled = true;
            svmByName["LessThanImpliesDistinctStrategy"].IsEnabled = true;
            svmByName["NextToImpliesDistinctStrategy"].IsEnabled = true;

            svmByName["EitherOrDomainStrategy"].IsEnabled = true;
            svmByName["LessThanDomainStrategy"].IsEnabled = true;
            svmByName["NextToDomainStrategy"].IsEnabled = true;

            _viewmodel.ConstraintTargets.MaxTotalConstraints = null;
            _viewmodel.ConstraintTargets.MaxEqualConstraints = null;
            _viewmodel.ConstraintTargets.MaxDistinctConstraints = null;
            _viewmodel.ConstraintTargets.MaxIdentityConstraints = 1;
            _viewmodel.ConstraintTargets.MaxLessThanConstraints = null;
            _viewmodel.ConstraintTargets.MaxNextToConstraints = null;
            _viewmodel.ConstraintTargets.MaxEitherOrConstraints = null;

            MessageBox.Show("Strategies now configured for an overall easy puzzle.", "Difficulty Preset", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MediumButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, StrategyViewModel> svmByName = new Dictionary<string, StrategyViewModel>();

            foreach (StrategyViewModel svm in _viewmodel.StrategyList)
            {
                svmByName[svm.Name] = svm;
                svm.IsEnabled = false;
                svm.MinimumApplications = 0;
                svm.MaximumApplications = null;
            }

            // Remove the "Equal" constraints since they give too much information.
            // Enable all the typical strategies seen in "Zebra" puzzles.
            // Require BlockCrossout and PropertyPairAnalysis.

            svmByName["SynchronizeStrategy"].IsEnabled = true;

            svmByName["DistinctConstraintStrategy"].IsEnabled = true;
            svmByName["IdentityConstraintStrategy"].IsEnabled = true;

            svmByName["EitherOrImpliesDistinctStrategy"].IsEnabled = true;
            svmByName["LessThanImpliesDistinctStrategy"].IsEnabled = true;
            svmByName["NextToImpliesDistinctStrategy"].IsEnabled = true;

            svmByName["EitherOrArgumentUnionStrategy"].IsEnabled = true;
            svmByName["EitherOrArgumentUnionStrategy"].MinimumApplications = 1;

            svmByName["EitherOrDomainStrategy"].IsEnabled = true;
            svmByName["LessThanDomainStrategy"].IsEnabled = true;
            svmByName["NextToDomainStrategy"].IsEnabled = true;

            svmByName["BlockCrossoutStrategy"].IsEnabled = true;
            svmByName["BlockCrossoutStrategy"].MinimumApplications = 1;

            svmByName["PropertyPairAnalysisStrategy"].IsEnabled = true;
            svmByName["PropertyPairAnalysisStrategy"].MinimumApplications = 1;

            svmByName["LessThanManyDomainStrategy/Direct"].IsEnabled = true;
            svmByName["DoubleNextToImpliesBetweenStrategy/Direct"].IsEnabled = true;
            svmByName["DoubleNextToImpliesEqualStrategy/Direct"].IsEnabled = true;

            _viewmodel.ConstraintTargets.MaxTotalConstraints = null;
            _viewmodel.ConstraintTargets.MaxEqualConstraints = 0;
            _viewmodel.ConstraintTargets.MaxDistinctConstraints = null;
            _viewmodel.ConstraintTargets.MaxIdentityConstraints = 1;
            _viewmodel.ConstraintTargets.MaxLessThanConstraints = null;
            _viewmodel.ConstraintTargets.MaxNextToConstraints = null;
            _viewmodel.ConstraintTargets.MaxEitherOrConstraints = null;

            MessageBox.Show("Strategies now configured for an overall medium puzzle.", "Difficulty Preset", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void HardButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, StrategyViewModel> svmByName = new Dictionary<string, StrategyViewModel>();

            foreach (StrategyViewModel svm in _viewmodel.StrategyList)
            {
                svmByName[svm.Name] = svm;
                svm.IsEnabled = true;
                svm.MinimumApplications = 0;
                svm.MaximumApplications = null;
            }

            // Enable all strategies, except for the rather experimental constraint generation ones 
            // since they drastically slow down the generator and can mess up difficulty measurements.

            foreach (StrategyViewModel svm in _viewmodel.StrategyList)
                if (svm.Name.Contains("ConstraintGenerationStrategy"))
                    svm.IsEnabled = false;

            // It's counterintuitive, but the best technique for generating the absolute hardest puzzles is to 
            // give the generator free reign over puzzle selection. Don't force its hand by making certain
            // strategies required.

            _viewmodel.ConstraintTargets.MaxTotalConstraints = null;
            _viewmodel.ConstraintTargets.MaxEqualConstraints = 0;
            _viewmodel.ConstraintTargets.MaxDistinctConstraints = null;
            _viewmodel.ConstraintTargets.MaxIdentityConstraints = 1;
            _viewmodel.ConstraintTargets.MaxLessThanConstraints = null;
            _viewmodel.ConstraintTargets.MaxNextToConstraints = null;
            _viewmodel.ConstraintTargets.MaxEitherOrConstraints = null;

            MessageBox.Show("Strategies now configured for an overall hard puzzle.", "Difficulty Preset", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
