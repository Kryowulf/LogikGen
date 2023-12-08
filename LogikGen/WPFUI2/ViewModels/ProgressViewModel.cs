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
    public class ProgressViewModel : ViewModel
    {
        private int _lastTotalGenerated = 0;
        private DateTime _lastProgressUpdate = DateTime.Now;
        private int _progressUpdateLock = 0;

        public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMilliseconds(200);

        // It is allowed to bind to properties that get updated from another thread.
        // Due to the update lock, only one thread at a time will update these & raise OnPropertyChanged.
        // Thus, binding to these should be okay but any codebehind shouldn't access them directly.
        // There should be a better solution that doesn't involve blocking locks.

        private int _totalGenerated;
        public int TotalGenerated
        {
            get { return _totalGenerated; }
            set { SetValue(ref _totalGenerated, value); }
        }


        private int _generationSpeed;
        public int GenerationSpeed
        {
            get { return _generationSpeed; }
            set { SetValue(ref _generationSpeed, value); }
        }


        private string _resultDisplay = "";
        public string ResultDisplay
        {
            get { return _resultDisplay; }
            set { SetValue(ref _resultDisplay, value); }
        }

        public void UpdateSearchProgress(int totalProgress, int taskId, int taskProgress)
        {
            // Non-blocking lock. If the current thread can't obtain the lock to do
            // a thread-safe progress update, then don't bother with it.
            if (Interlocked.Exchange(ref _progressUpdateLock, 1) == 0)
            {
                if (DateTime.Now - _lastProgressUpdate >= UpdateInterval)
                {
                    if (_lastTotalGenerated < totalProgress)
                    {
                        int progressDelta = totalProgress - _lastTotalGenerated;
                        int timeDelta = (int)(DateTime.Now - _lastProgressUpdate).TotalMilliseconds;
                        this.GenerationSpeed = timeDelta == 0 ? -1 : (1000 * progressDelta / timeDelta);
                        this.TotalGenerated = totalProgress;
                    }

                    _lastTotalGenerated = totalProgress;
                    _lastProgressUpdate = DateTime.Now;
                }

                Interlocked.Exchange(ref _progressUpdateLock, 0);
            }
        }

        public void UpdateReport(GenerationAnalysisReport report)
        {
            // No need to lock here.
            // MultithreadedPuzzleGenerator runs UpdateReport within a lock already.

            this.ResultDisplay = report.PrintBrief();
        }

        public void UpdateFinalReport(GenerationAnalysisReport report)
        {
            this.ResultDisplay = report.PrintComplete();
        }

        public void UpdateUnsolvableReport(UnsolvableAnalysisReport report)
        {
            this.ResultDisplay = report.Print();
        }

        public void ShowMessage(string message)
        {
            this.ResultDisplay = message;
        }
    }
}
