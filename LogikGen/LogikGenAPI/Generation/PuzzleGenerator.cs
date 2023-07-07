using LogikGenAPI.Generation.Patterns;
using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation
{
    public class PuzzleGenerator
    {
        private const int USELESS_CONSTRAINT_THRESHOLD = 999;

        public PropertySet PropertySet { get; private set; }
        public SolutionGrid Solution { get; private set; }
        public IReadOnlyList<Strategy> Strategies { get; private set; }
        public IReadOnlyList<StrategyAnalysis> MinTargets { get; private set; }
        public IReadOnlyList<StrategyAnalysis> MaxTargets { get; private set; }
        public int MaxTotalConstraints { get; private set; }
        public IReadOnlyList<ConstraintPattern> Patterns { get; private set; }

        // +1 is for whether max constraints is reached.
        // Each pattern has its own "MaximumCount" target.
        public int MaxTargetScore => this.MinTargets.Count + this.MaxTargets.Count + this.Patterns.Count + 1;

        public PuzzleGenerator(
            PropertySet pset, 
            int minApplications = 1,
            int maxApplications = int.MaxValue,
            int maxTotalConstraints = int.MaxValue,
            int solutionSeed = -1)
        {
            this.PropertySet = pset;
            this.Solution = new PuzzleSolver(pset).Randomize(solutionSeed);
            this.Strategies = new DefaultStrategyList();
            this.MinTargets = this.Strategies.Select(s => new StrategyAnalysis(s, minApplications)).ToList().AsReadOnly();
            this.MaxTargets = this.Strategies.Select(s => new StrategyAnalysis(s, maxApplications)).ToList().AsReadOnly();            
            this.MaxTotalConstraints = maxTotalConstraints;

            List<ConstraintPattern> patterns = new List<ConstraintPattern>()
            {
                new EqualConstraintPattern(),
                new DistinctConstraintPattern(),
                new IdentityConstraintPattern(),
                new LessThanConstraintPattern(),
                new NextToConstraintPattern(),
                new EitherOrConstraintPattern()
            };
            
            this.Patterns = patterns.AsReadOnly();
        }

        public PuzzleGenerator(
            SolutionGrid solution, 
            IEnumerable<ConstraintPattern> patterns, 
            IEnumerable<StrategyAnalysis> minTargets,
            IEnumerable<StrategyAnalysis> maxTargets,   
            int maxTotalConstraints)
        {
            this.PropertySet = solution.PropertySet;
            this.Solution = solution;
            this.Patterns = patterns.ToList().AsReadOnly();
            this.MinTargets = minTargets.ToList().AsReadOnly();
            this.MaxTargets = maxTargets.ToList().AsReadOnly();

            this.Strategies = this.MinTargets.Select(a => a.Strategy).Union(
                this.MaxTargets.Select(a => a.Strategy)).ToList().AsReadOnly();

            this.MaxTotalConstraints = maxTotalConstraints;
        }

        public Constraint RandomConstraint(Random rgen)
        {
            return rgen.Select(this.Patterns).RandomConstraint(this.Solution, rgen);
        }

        public IList<Constraint> RandomPuzzle(Random rgen)
        {
            PuzzleSolver solver = new PuzzleSolver(this.PropertySet, this.Strategies);
            int uselessConstraintCount = 0;

            while (!solver.Grid.Solved)
            {
                Constraint constraint = this.RandomConstraint(rgen);
                
                if (solver.AddConstraint(constraint))
                {
                    if (solver.Resolve() == 0)
                    {
                        solver.RemoveConstraint(constraint);
                        uselessConstraintCount++;
                    }
                    else
                    {
                        uselessConstraintCount = 0;
                    }

                    if (solver.Grid.Contradiction)
                        throw new GenerationException("Random puzzle generation produced a contradiction. ");
                }
                else
                {
                    uselessConstraintCount++;
                }

                if (uselessConstraintCount >= USELESS_CONSTRAINT_THRESHOLD)
                    throw new GenerationException("Unable to create a random puzzle due to insufficient generation parameters.");
            }

            List<Constraint> candidates = solver.Constraints.ToList();

            foreach (Constraint c in candidates)
            {
                solver.RemoveConstraint(c);
                solver.ResetAll();
                solver.Resolve();

                if (!solver.Grid.Solved)
                    solver.AddConstraint(c);
            }

            return solver.Constraints.ToList().AsReadOnly();
        }

        public AnalysisReport FindSatisfyingPuzzle(
            Action<AnalysisReport> newPuzzleCallback = null, 
            Action<int> searchProgressCallback = null, 
            CancellationToken? cancelToken = null, 
            int seed = -1)
        {
            Random rgen = seed >= 0 ? new Random(seed) : new Random();
            IList<Constraint> constraints = RandomPuzzle(rgen);
            PuzzleSolver solver = new PuzzleSolver(this.PropertySet, this.Strategies);
            solver.AddConstraints(constraints);

            AnalysisReport bestAnalysis = solver.FullAnalysis();
            int totalPuzzlesSearched = 1;

            newPuzzleCallback?.Invoke(bestAnalysis);
            searchProgressCallback?.Invoke(totalPuzzlesSearched);

            while (!cancelToken.GetValueOrDefault().IsCancellationRequested)
            {
                constraints = RandomPuzzle(rgen);
                solver.ClearConstraints();
                solver.ResetAll();
                solver.AddConstraints(constraints);

                AnalysisReport candidateAnalysis = solver.QuickAnalysis();

                if (SelectPreferred(bestAnalysis, candidateAnalysis) == candidateAnalysis)
                {
                    candidateAnalysis = solver.FullAnalysis();

                    if (SelectPreferred(bestAnalysis, candidateAnalysis) == candidateAnalysis)
                    {
                        bestAnalysis = candidateAnalysis;
                        newPuzzleCallback?.Invoke(bestAnalysis);
                    }
                }

                totalPuzzlesSearched++;
                searchProgressCallback?.Invoke(totalPuzzlesSearched);
            }

            return bestAnalysis;
        }

        public IList<Constraint> FindUnsolvablePuzzle(
            Action<int> searchProgressCallback = null,
            CancellationToken? cancelToken = null, 
            int unsolvableDepth = -1,
            int seed = -1)
        {
            Random rgen = seed >= 0 ? new Random(seed) : new Random();
            PuzzleSolver solver = new PuzzleSolver(this.PropertySet, this.Strategies);
            int totalPuzzlesSearched = 0;
            bool puzzleFound = false;
            IList<Constraint> result = null;

            do
            {
                // If we don't start over, we can get "stuck".
                // We can end up with a set of constraints for which there is no 
                // space in which puzzles have unique solutions but are still unsolvable.
                // Clear the constraints to get out of this situation.
                solver.ClearConstraints();
                solver.ResetAll();

                // Keep adding constraints until there is a unique solution.
                do solver.AddConstraint(RandomConstraint(rgen));
                while (!solver.FindUniqueSolution(out _));

                IList<Constraint> constraints = solver.Constraints.ToList();

                // Pare down the constraints to a minimal list.
                foreach (Constraint c in constraints)
                {
                    solver.RemoveConstraint(c);
                    solver.ResetAll();

                    if (!solver.FindUniqueSolution(out SolutionGrid solution))
                        solver.AddConstraint(c);
                }

                // Try to solve the puzzle via ordinary strategies.
                solver.ResetAll();
                solver.Resolve();

                // Sanity check. Should never happen.
                if (solver.Grid.Contradiction)
                    throw new GenerationException("Unsolvable puzzle search produced an unexpected contradiction.");

                // If the ordinary strategies didn't solve the puzzle, we've found an unsolvable one.
                if (!solver.Grid.Solved)
                {
                    if (unsolvableDepth < 0 || unsolvableDepth <= solver.FindUniqueSolutionSearchDepth())
                    {
                        result = solver.Constraints.ToList();
                        puzzleFound = true;
                    }
                }

                totalPuzzlesSearched++;
                searchProgressCallback?.Invoke(totalPuzzlesSearched);
            }
            while (!puzzleFound &&
                   !cancelToken.GetValueOrDefault().IsCancellationRequested);

            return result;
        }

        public AnalysisReport SelectPreferred(AnalysisReport reportA, AnalysisReport reportB)
        {
            int scoreA = CalculateTargetScore(reportA);
            int scoreB = CalculateTargetScore(reportB);

            if (scoreA > scoreB)
                return reportA;

            if (scoreB > scoreA)
                return reportB;

            if (reportB.CompareTo(reportA) > 0)
                return reportB;

            return reportA;
        }

        public bool SatisfiesTargets(AnalysisReport report)
        {
            return CalculateTargetScore(report) == this.MaxTargetScore;
        }

        public int CalculateTargetScore(AnalysisReport report)
        {
            int score = 0;

            foreach (StrategyAnalysis target in this.MinTargets)
                if (report[target.Strategy].ApplicationsNeeded >= target.ApplicationsNeeded)
                    score++;

            foreach (StrategyAnalysis target in this.MaxTargets)
                if (report[target.Strategy].ApplicationsNeeded <= target.ApplicationsNeeded)
                    score++;

            if (report.Constraints.Count <= this.MaxTotalConstraints)
                score++;

            Dictionary<Type, int> countsByConstraintType = new Dictionary<Type, int>();

            foreach (Constraint c in report.Constraints)
            {
                Type t = c.GetType();

                if (countsByConstraintType.ContainsKey(t))
                    countsByConstraintType[t]++;
                else
                    countsByConstraintType[t] = 1;
            }

            foreach (ConstraintPattern p in this.Patterns)
            {
                if (countsByConstraintType.GetValueOrDefault(p.ConstraintType, 0) <= p.MaximumCount)
                    score++;
            }

            return score;
        }
    }
}
