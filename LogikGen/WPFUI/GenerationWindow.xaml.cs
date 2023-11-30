using LogikGenAPI.Generation;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFUI.ViewModels;

namespace WPFUI
{
    public partial class GenerationWindow : Window
    {
        private GenerationWindowViewModel _viewmodel;
        private CancellationTokenSource _cts;
        private bool _isRunning;

        public GenerationWindow(SolutionGrid solution, double left = -1, double top = -1)
        {
            InitializeComponent();

            _viewmodel = new GenerationWindowViewModel(solution);
            _cts = null;
            _isRunning = false;

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
                int nthreads = _viewmodel.NThreads ?? Environment.ProcessorCount;
                GenerationStatusUpdater updater = new GenerationStatusUpdater(nthreads, this.Dispatcher, searchProgressCallback);

                try
                {
                    if (_viewmodel.IsGenerateUnsolvableChecked)
                    {
                        IList<Constraint> constraints = await Task.Run(() => mpgen.FindUnsolvablePuzzle(
                            updater.UpdateSearchProgress, _cts.Token, unsolvableDepth, seed, nthreads));

                        if (constraints == null)
                        {
                            outputTextbox.Text = "Cancelled";
                        }
                        else
                        {
                            PuzzleSolver solver = new PuzzleSolver(pgen.PropertySet, pgen.StrategyTargets.Select(t => t.Strategy));
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
                            (report) => this.Dispatcher.BeginInvoke(new Action<PuzzleGenerator, AnalysisReport>(newPuzzleCallback), pgen, report),
                            updater.UpdateSearchProgress, _cts.Token, seed, nthreads));

                        newPuzzleCallback(pgen, finalReport);

                        PuzzleSolver solver = new PuzzleSolver(pgen.PropertySet, pgen.StrategyTargets.Select(t => t.Strategy));
                        solver.AddConstraints(finalReport.Constraints);
                        IReadOnlyList<string> steps = solver.Explain(true);

                        outputTextbox.Text += "\n";

                        foreach (string step in steps)
                            outputTextbox.Text += step + "\n";
                    }
                }
                catch(Exception ex)
                {
                    outputTextbox.Text = ex.Message + "\n" + ex.StackTrace;
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

        private void newPuzzleCallback(PuzzleGenerator pgen, AnalysisReport report)
        {
            string heading = pgen.SatisfiesTargets(report) ? "[SATISFIED]" : "[UNSATISFIED]";
            outputTextbox.Text = heading + "\n" + report.Print();
        }

        private void searchProgressCallback(int[] progress)
        {
            while (progress.Length > threadStatusPanel.Children.Count)
                threadStatusPanel.Children.Add(new TextBlock());

            int sum = progress.Sum();

            for (int i = 0; i < progress.Length; i++)
                (threadStatusPanel.Children[i] as TextBlock).Text = $"{i}) {progress[i]} Puzzles Searched.";

            overallStatusTextBlock.Text = $"{sum} Total Searched.";
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            SolutionWindow window = new SolutionWindow(_viewmodel.Solution.PropertySet, this.Left, this.Top);
            window.Show();
            this.Close();
        }

        private class GenerationStatusUpdater
        {
            private int _ntasks;
            private Dispatcher _dispatcher;
            private Action<int[]> _searchProgressCallback;

            private ConcurrentDictionary<int, int> _progressByTaskId;
            private int _checkingUpdate;
            private DateTime _lastUpdate;
            private TimeSpan _updateInterval;

            public GenerationStatusUpdater(int ntasks, Dispatcher dispatcher, Action<int[]> searchProgressCallback)
            {
                if (ntasks <= 0)
                    throw new ArgumentException("Invalid task count given to generation status updater.");

                _ntasks = ntasks;
                _dispatcher = dispatcher;
                _searchProgressCallback = searchProgressCallback;

                _progressByTaskId = new ConcurrentDictionary<int, int>();

                for (int i = 0; i < ntasks; i++)
                    _progressByTaskId[i] = 0;

                _checkingUpdate = 0;
                _lastUpdate = DateTime.Now;
                _updateInterval = TimeSpan.FromMilliseconds(100);
            }

            public void UpdateSearchProgress(int totalProgress, int taskId, int taskProgress)
            {
                _progressByTaskId[taskId] = taskProgress;

                // Using interlocked because I don't want to block any threads. 
                // It isn't important for the UI to be completely up-to-date on the number of puzzles searched,
                // we just need to slow down how often it's updated so it doesn't freeze.
                if (Interlocked.Exchange(ref _checkingUpdate, 1) == 0)
                {
                    if (DateTime.Now - _lastUpdate > _updateInterval)
                    {
                        int[] progress = new int[_ntasks];

                        for (int i = 0; i < _ntasks; i++)
                            progress[i] = _progressByTaskId[i];

                        _dispatcher.BeginInvoke(_searchProgressCallback, progress);

                        _lastUpdate = DateTime.Now;
                    }

                    Interlocked.Exchange(ref _checkingUpdate, 0);
                }
            }
        }
    }
}
