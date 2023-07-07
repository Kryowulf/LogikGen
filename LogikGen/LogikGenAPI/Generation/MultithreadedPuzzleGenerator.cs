using LogikGenAPI.Generation.Patterns;
using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<AnalysisReport> FindSatisfyingPuzzle(
            Action<AnalysisReport> newPuzzleCallback = null,
            Action<int, int> searchProgressCallback = null,
            CancellationToken? cancelToken = null,
            int seed = -1,
            int nTasks = -1)
        {
            Random rgen = seed >= 0 ? new Random(seed) : new Random();
            nTasks = nTasks > 0 ? nTasks : Environment.ProcessorCount;
            List<Task<AnalysisReport>> tasks = new List<Task<AnalysisReport>>(nTasks);
            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken.GetValueOrDefault());

            object bestLock = new object();
            AnalysisReport globalBest = null;

            for (int i = 0; i < nTasks; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(() => this.Prototype.FindSatisfyingPuzzle(
                    (newReport) =>
                    {
                        lock (bestLock)
                        {
                            if (globalBest == null || this.Prototype.SelectPreferred(globalBest, newReport) == newReport)
                            {
                                globalBest = newReport;
                                newPuzzleCallback(globalBest);
                            }
                        }
                    },
                    (totalReports) => searchProgressCallback(taskId, totalReports),
                    cts.Token,
                    rgen.Next())));
            }

            AnalysisReport[] reports = await Task.WhenAll(tasks);
            AnalysisReport best = reports[0];

            foreach (AnalysisReport report in reports)
                best = this.Prototype.SelectPreferred(best, report);

            cts.Dispose();

            return best;
        }

        public async Task<IList<Constraint>> FindUnsolvablePuzzle(
            Action<int, int> searchProgressCallback = null,
            CancellationToken? cancelToken = null,
            int unsolvableDepth = - 1,
            int seed = -1, 
            int nTasks = -1)
        {
            Random rgen = seed >= 0 ? new Random(seed) : new Random();
            nTasks = nTasks > 0 ? nTasks : Environment.ProcessorCount;
            List<Task<IList<Constraint>>> tasks = new List<Task<IList<Constraint>>>(nTasks);
            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken.GetValueOrDefault());

            for (int i = 0; i < nTasks; i++)
            {
                int taskId = i;

                tasks.Add(Task.Run(() => this.Prototype.FindUnsolvablePuzzle(
                    (totalReports) => searchProgressCallback(taskId, totalReports),
                    cts.Token,
                    unsolvableDepth,
                    rgen.Next())));
            }

            Task<IList<Constraint>> completedTask = await Task.WhenAny(tasks);
            IList<Constraint> constraints = await completedTask;

            cts.Cancel();

            await Task.WhenAll(tasks);

            cts.Dispose();

            return constraints;
        }
    }
}
