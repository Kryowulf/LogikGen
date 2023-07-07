using LogikGenAPI.Generation;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using WPFUI.ViewModels;

namespace WPFUI
{
    public partial class GenerationWindow : Window
    {
        private GenerationWindowViewModel _viewmodel;
        private CancellationTokenSource _cts;
        private bool _isRunning;
        private List<int> _totalPuzzlesSearchedByTaskId;

        public GenerationWindow(SolutionGrid solution, double left = -1, double top = -1)
        {
            InitializeComponent();

            _viewmodel = new GenerationWindowViewModel(solution);
            _cts = null;
            _isRunning = false;
            _totalPuzzlesSearchedByTaskId = new List<int>();

            this.DataContext = _viewmodel;

            if (left >= 0)
                this.Left = left;

            if (top >= 0)
                this.Top = top;

            this.MaxHeight = SystemParameters.PrimaryScreenHeight * 0.9;
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isRunning)
            {
                _isRunning = true;
                difficultyPanel.IsEnabled = false;
                strategyGrid.IsEnabled = false;
                settingsGrid.IsEnabled = false;
                generateButton.IsEnabled = false;

                PuzzleGenerator pgen = _viewmodel.MakeGenerator();
                MultithreadedPuzzleGenerator mpgen = new MultithreadedPuzzleGenerator(pgen);
                _cts = new CancellationTokenSource();

                int unsolvableDepth = _viewmodel.UnsolvableDepth ?? -1;
                int seed = _viewmodel.Seed ?? -1;
                int nthreads = _viewmodel.NThreads ?? -1;

                try
                {
                    if (_viewmodel.IsGenerateUnsolvableChecked)
                    {
                        IList<Constraint> constraints = await Task.Run(() => mpgen.FindUnsolvablePuzzle(
                            (taskId, puzzlesSearched) => this.Dispatcher.BeginInvoke(new Action<int, int>(statusUpdate), taskId, puzzlesSearched),
                            _cts.Token, unsolvableDepth, seed, nthreads));

                        if (constraints == null)
                        {
                            outputTextbox.Text = "Cancelled";
                        }
                        else
                        {
                            PuzzleSolver solver = new PuzzleSolver(pgen.PropertySet, pgen.Strategies);
                            solver.AddConstraints(constraints);
                            solver.Resolve();

                            StringBuilder output = new StringBuilder();
                            output.AppendLine(constraints.Count + " total constraints.");
                            output.AppendLine(string.Join("\n", constraints.Select(c => c.ToString())));
                            output.AppendLine();
                            output.AppendLine(GridPrinter.BuildGridString(solver.Grid));
                            output.AppendLine();
                            output.AppendLine(GridPrinter.BuildGridString(pgen.Solution));

                            outputTextbox.Text = output.ToString();
                        }
                    }
                    else
                    {
                        AnalysisReport finalReport = await Task.Run(() => mpgen.FindSatisfyingPuzzle(
                            (report) => this.Dispatcher.BeginInvoke(new Action<PuzzleGenerator, AnalysisReport>(newPuzzleFound), pgen, report),
                            (taskId, puzzlesSearched) => this.Dispatcher.BeginInvoke(new Action<int, int>(statusUpdate), taskId, puzzlesSearched),
                            _cts.Token, seed, nthreads));

                        newPuzzleFound(pgen, finalReport);

                        PuzzleSolver solver = new PuzzleSolver(pgen.PropertySet, pgen.Strategies);
                        solver.AddConstraints(finalReport.Constraints);
                        IReadOnlyList<string> steps = solver.Explain(true);

                        outputTextbox.Text += "\n";

                        foreach (string step in steps)
                            outputTextbox.Text += step + "\n";
                    }
                }
                catch(Exception ex)
                {
                    outputTextbox.Text = ex.Message;
                    _cts.Cancel();
                }

                _cts.Dispose();
                _cts = null;
                difficultyPanel.IsEnabled = true;
                strategyGrid.IsEnabled = true;
                settingsGrid.IsEnabled = true;
                generateButton.IsEnabled = true;
                _isRunning = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }

        private void newPuzzleFound(PuzzleGenerator pgen, AnalysisReport report)
        {
            string heading = pgen.SatisfiesTargets(report) ? "[SATISFIED]" : "[UNSATISFIED]";
            outputTextbox.Text = heading + "\n" + report.Print();
        }

        private void statusUpdate(int taskId, int puzzlesSearched)
        {
            while (taskId >= threadStatusPanel.Children.Count)
            {
                threadStatusPanel.Children.Add(new TextBlock());
                _totalPuzzlesSearchedByTaskId.Add(0);
            }

            _totalPuzzlesSearchedByTaskId[taskId] = puzzlesSearched;
            int sum = _totalPuzzlesSearchedByTaskId.Sum();

            (threadStatusPanel.Children[taskId] as TextBlock).Text = $"{taskId}) {puzzlesSearched} Puzzles Searched.";
            overallStatusTextBlock.Text = $"{sum} Total Searched.";
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            SolutionWindow window = new SolutionWindow(_viewmodel.Solution.PropertySet, this.Left, this.Top);
            window.Show();
            this.Close();
        }
    }
}
