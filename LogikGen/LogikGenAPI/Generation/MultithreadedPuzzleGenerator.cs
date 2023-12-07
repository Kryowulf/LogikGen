using LogikGenAPI.Generation.Patterns;
using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation
{
    public class MultithreadedPuzzleGenerator
    {
        public PuzzleGenerator Prototype { get; private set; }

        public MultithreadedPuzzleGenerator(PuzzleGenerator prototype)
        {
            this.Prototype = prototype;
        }

        public async Task<GenerationAnalysisReport> FindSatisfyingPuzzle(
            Action<GenerationAnalysisReport> newPuzzleCallback = null,
            Action<int, int, int> searchProgressCallback = null,
            CancellationToken? cancelToken = null,
            int seed = -1,
            int nTasks = -1)
        {
            Random rgen = seed >= 0 ? new Random(seed) : new Random();
            nTasks = nTasks > 0 ? nTasks : Environment.ProcessorCount;
            List<Task<GenerationAnalysisReport>> tasks = new List<Task<GenerationAnalysisReport>>(nTasks);
            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken.GetValueOrDefault());

            object bestLock = new object();
            GenerationAnalysisReport globalBest = null;

            int totalProgress = 0;
            
            for (int i = 0; i < nTasks; i++)
            {
                int taskId = i;

                tasks.Add(Task.Run(() => this.Prototype.FindSatisfyingPuzzle(
                    (newReport) =>
                    {
                        lock (bestLock)
                        {
                            if (globalBest == null || newReport.CompareTo(globalBest) > 0)
                            {
                                globalBest = newReport;
                                newPuzzleCallback(globalBest);
                            }
                        }
                    },
                    (taskProgress) => searchProgressCallback(Interlocked.Increment(ref totalProgress), taskId, taskProgress),
                    cts.Token,
                    rgen.Next())));
            }

            GenerationAnalysisReport[] reports = await Task.WhenAll(tasks);
            GenerationAnalysisReport best = reports[0];

            foreach (GenerationAnalysisReport report in reports)
                if (report.CompareTo(best) > 0)
                    best = report;

            cts.Dispose();

            return best;
        }

        public async Task<UnsolvableAnalysisReport> FindUnsolvablePuzzle(
            Action<int, int, int> searchProgressCallback = null,
            CancellationToken? cancelToken = null,
            int unsolvableDepth = - 1,
            int seed = -1, 
            int nTasks = -1)
        {
            Random rgen = seed >= 0 ? new Random(seed) : new Random();
            nTasks = nTasks > 0 ? nTasks : Environment.ProcessorCount;
            List<Task<UnsolvableAnalysisReport>> tasks = new List<Task<UnsolvableAnalysisReport>>(nTasks);
            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken.GetValueOrDefault());

            int totalProgress = 0;
            
            for (int i = 0; i < nTasks; i++)
            {
                int taskId = i;

                tasks.Add(Task.Run(() => this.Prototype.FindUnsolvablePuzzle(
                    (taskProgress) => searchProgressCallback(Interlocked.Increment(ref totalProgress), taskId, taskProgress),
                    cts.Token,
                    unsolvableDepth,
                    rgen.Next())));
            }

            Task<UnsolvableAnalysisReport> completedTask = await Task.WhenAny(tasks);
            UnsolvableAnalysisReport report = await completedTask;

            cts.Cancel();

            await Task.WhenAll(tasks);

            cts.Dispose();

            return report;
        }
    }
}
