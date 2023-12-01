using LogikGenAPI.Generation;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPFUI2.ViewModels
{
    public class ThreadSafeProgressViewModel : ViewModel
    {
        private int _lastTotalProgress = 0;
        private DateTime _lastProgressUpdate = DateTime.Now;
        private int _updateLock = 0;

        public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMilliseconds(100);


        // A stupid hack for now. 
        // In order to tell whether an analysis report satisfies all generation targets,
        // the instance of the generator that produced the report is needed.
        // In the future, there'll be a ResolutionAnalysisReport and a GenerationAnalysisReport,
        // where the latter will include the former along with information about
        // which generation targets were satisfied.
        private bool _lastReportSatisfied = false;
        public PuzzleGenerator? Generator { get; set; }


        // According to https://stackoverflow.com/questions/61119856/bindings-and-updating-from-another-thread
        // it's legal to bind to properties that are updated from another thread.
        // No need to fuss about with a Dispatcher.

        private string _resultDisplay = "";
        public string ResultDisplay
        {
            get { return _resultDisplay; }
            set { SetValue(ref _resultDisplay, value); }
        }


        private string _progressDisplay = "";
        public string ProgressDisplay
        {
            get { return _progressDisplay; }
            set { SetValue(ref _progressDisplay, value); }
        }


        public void UpdateSearchProgress(int totalProgress, int taskId, int taskProgress)
        {
            // Non-blocking lock. If the current thread can't obtain the lock to do
            // a thread-safe progress update, then don't bother with it.
            if (Interlocked.Exchange(ref _updateLock, 1) == 0)
            {
                if (DateTime.Now - _lastProgressUpdate >= UpdateInterval)
                {
                    if (_lastTotalProgress < totalProgress)
                    {
                        int progressDelta = totalProgress - _lastTotalProgress;
                        int timeDelta = (int)(DateTime.Now - _lastProgressUpdate).TotalMilliseconds;
                        int speed = timeDelta == 0 ? -1 : (1000 * progressDelta / timeDelta);

                        this.ProgressDisplay = $"Puzzles Searched: {totalProgress}\n" +
                                               $"Speed: {speed}/sec\n" + 
                                               (_lastReportSatisfied ? "[SATISFIED]" : "[UNSATISFIED]");
                    }

                    _lastTotalProgress = totalProgress;
                    _lastProgressUpdate = DateTime.Now;
                }

                Interlocked.Exchange(ref _updateLock, 0);
            }
        }

        public void UpdateReport(AnalysisReport report)
        {
            _lastReportSatisfied = Generator?.SatisfiesTargets(report) ?? false;
            string heading = _lastReportSatisfied ? "[SATISFIED]" : "[UNSATISFIED]";

            this.ResultDisplay = heading + "\n" + report.Print();
        }

        public void UpdateFinalReport(AnalysisReport report)
        {
            _lastReportSatisfied = Generator?.SatisfiesTargets(report) ?? false;
            string heading = _lastReportSatisfied ? "[SATISFIED]" : "[UNSATISFIED]";
            
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(heading);
            sb.AppendLine(report.Print());

            PuzzleSolver solver = new PuzzleSolver(report.Solution.PropertySet, report.Analyses.Select(a => a.Strategy));
            solver.AddConstraints(report.Constraints);

            foreach (string step in solver.Explain(true))
                sb.AppendLine(step);

            this.ResultDisplay = sb.ToString();
        }

        public void ShowMessage(string message)
        {
            this.ResultDisplay = message;
        }

        internal void UpdateUnsolvableResult(IList<Constraint> constraints)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(constraints.Count + " total constraints.");
            sb.AppendLine(string.Join("\n", constraints.Select(c => c.ToString())));
            sb.AppendLine();

            if (this.Generator != null)
            {
                PuzzleSolver solver = new PuzzleSolver(Generator.PropertySet, Generator.StrategyTargets.Select(s => s.Strategy));
                solver.AddConstraints(constraints);
                solver.Resolve();

                sb.AppendLine(GridPrinter.BuildGridString(solver.Grid));
                sb.AppendLine();
                sb.AppendLine(GridPrinter.BuildGridString(Generator.Solution));
            }

            this.ResultDisplay = sb.ToString();
        }
    }
}
