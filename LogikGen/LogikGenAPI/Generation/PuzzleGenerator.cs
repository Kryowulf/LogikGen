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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation
{
    public class PuzzleGenerator
    {
        private const int USELESS_CONSTRAINT_THRESHOLD = 999;
        private const int PUZZLE_REGEN_THRESHOLD = 5;

        public PropertySet PropertySet { get; private set; }
        public SolutionGrid Solution { get; private set; }
        public IReadOnlyList<StrategyTarget> StrategyTargets { get; private set; }
        public IReadOnlyList<ConstraintTarget> ConstraintTargets { get; private set; }
        public IReadOnlyList<ConstraintPattern> AvailablePatterns { get; private set; }
        public int MaxTotalConstraints { get; private set; }

        public PuzzleGenerator(
            PropertySet pset, 
            int solutionSeed = -1)
        {
            IReadOnlyList<ConstraintPattern> defaultPatterns = new DefaultConstraintPatternList();

            this.PropertySet = pset;
            this.Solution = new PuzzleSolver(pset).Randomize(solutionSeed);
            this.StrategyTargets = new DefaultStrategyList().Select(s => new StrategyTarget(s)).ToList().AsReadOnly();
            this.ConstraintTargets = defaultPatterns.Select(p => new ConstraintTarget(p)).ToList().AsReadOnly();
            this.AvailablePatterns = defaultPatterns;
            this.MaxTotalConstraints = int.MaxValue;
        }

        public PuzzleGenerator(
            SolutionGrid solution, 
            IEnumerable<StrategyTarget> strategies, 
            IEnumerable<ConstraintTarget> constraints,
            int maxTotalConstraints)
        {
            List<ConstraintPattern> availablePatterns = new DefaultConstraintPatternList().ToList();

            this.PropertySet = solution.PropertySet;
            this.Solution = solution;
            this.StrategyTargets = strategies.ToList().AsReadOnly();
            this.ConstraintTargets = constraints.ToList().AsReadOnly();

            foreach (ConstraintTarget t in this.ConstraintTargets.Where(t => t.MaxCount == 0))
                availablePatterns.Remove(t.Pattern);

            this.AvailablePatterns = availablePatterns.AsReadOnly();
            this.MaxTotalConstraints = maxTotalConstraints;
        }

        public IList<Constraint> RandomPuzzle(Random rgen, IEnumerable<Constraint> initialConstraints = null, IReadOnlyList<ConstraintPattern> selectedPatterns = null)
        {
            initialConstraints = initialConstraints ?? Enumerable.Empty<Constraint>();
            selectedPatterns = selectedPatterns ?? this.AvailablePatterns;

            PuzzleSolver solver = new PuzzleSolver(this.PropertySet, this.StrategyTargets.Select(s => s.Strategy));
            solver.AddConstraints(initialConstraints);
            solver.Resolve();

            int uselessConstraintCount = 0;

            // Keep adding constraints until the puzzle is solved.
            while (!solver.Grid.Solved)
            {
                Constraint constraint = rgen.Select(selectedPatterns).RandomConstraint(this.Solution, rgen);

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

                // Counting "useless" constraints is a hack to make sure we're not stuck in an infinite loop.
                // This can happen if, e.g., we're only generating EitherOr constraints for some reason 
                // but none of the strategies for processing those are enabled. 
                
                // Unlikely to ever happen unless the generator was given stupid settings.
                if (uselessConstraintCount >= USELESS_CONSTRAINT_THRESHOLD)
                    throw new GenerationException("Unable to create a random puzzle due to insufficient generation parameters.");
            }


            // Prune all redundant constraints.

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

        public bool TrimConstraints(IEnumerable<Constraint> constraints, out List<Constraint> trimmedList, out List<ConstraintPattern> availablePatternsList)
        {
            bool wasTrimmed = false;

            trimmedList = new List<Constraint>();
            Dictionary<Type, int> limitsByConstraintType = new Dictionary<Type, int>();
            Dictionary<Type, int> countsByConstraintType = new Dictionary<Type, int>();
            Dictionary<Type, ConstraintPattern> patternsByConstraintType = new Dictionary<Type, ConstraintPattern>();
            HashSet<ConstraintPattern> enabledPatterns = new HashSet<ConstraintPattern>();

            foreach (ConstraintPattern p in new DefaultConstraintPatternList())
            {
                limitsByConstraintType[p.ConstraintType] = int.MaxValue;
                countsByConstraintType[p.ConstraintType] = 0;
                patternsByConstraintType[p.ConstraintType] = p;
                enabledPatterns.Add(p);
            }

            foreach (ConstraintTarget t in this.ConstraintTargets)
            {
                limitsByConstraintType[t.Pattern.ConstraintType] = t.MaxCount;

                if (t.MaxCount == 0)
                    enabledPatterns.Remove(t.Pattern);
            }

            foreach(Constraint c in constraints)
            {
                countsByConstraintType[c.GetType()]++;

                if (countsByConstraintType[c.GetType()] > limitsByConstraintType[c.GetType()])
                {
                    enabledPatterns.Remove(patternsByConstraintType[c.GetType()]);
                    wasTrimmed = true;
                }
                else
                {
                    trimmedList.Add(c);
                }
            }

            availablePatternsList = enabledPatterns.ToList();

            return wasTrimmed;
        }

        public GenerationAnalysisReport FindSatisfyingPuzzle(
            Action<GenerationAnalysisReport> newPuzzleCallback = null,
            Action<int> searchProgressCallback = null,
            CancellationToken? cancelToken = null,
            int seed = -1)
        {
            Random rgen = seed >= 0 ? new Random(seed) : new Random();
            PuzzleSolver solver = new PuzzleSolver(this.PropertySet, this.StrategyTargets.Select(t => t.Strategy));
            
            IList<Constraint> constraints = RandomPuzzle(rgen);     
            solver.AddConstraints(constraints);                     
                                                                    
            GenerationAnalysisReport bestAnalysis = MakeGenerationReport(solver.FullAnalysis(), false);
            int totalPuzzlesSearched = 1;

            searchProgressCallback?.Invoke(totalPuzzlesSearched);
            newPuzzleCallback?.Invoke(bestAnalysis);

            int regenCounter = 0;

            while (!cancelToken.GetValueOrDefault().IsCancellationRequested)
            {
                // First, try modifying the generated puzzle to achieve our constraint targets,
                // if we can and if we have any targets. 
                if (this.ConstraintTargets.Count > 0 &&
                    regenCounter < PUZZLE_REGEN_THRESHOLD && 
                    TrimConstraints(constraints, out List<Constraint> trimmedList, out List<ConstraintPattern> availablePatternsList) && 
                    availablePatternsList.Count > 0)
                {
                    constraints = RandomPuzzle(rgen, trimmedList, availablePatternsList);
                    regenCounter++;
                }

                // otherwise, just create a totally fresh puzzle ignoring the targets.
                else
                {
                    constraints = RandomPuzzle(rgen);
                    regenCounter = 0;
                }

                // Try out the generated constraints and compare with the best found so far. 
                solver.ClearConstraints();
                solver.ResetAll();
                solver.AddConstraints(constraints);

                GenerationAnalysisReport candidateAnalysis = MakeGenerationReport(solver.QuickAnalysis(), false);

                if (candidateAnalysis.CompareTo(bestAnalysis) > 0)
                {
                    candidateAnalysis = MakeGenerationReport(solver.FullAnalysis(), false);

                    if (candidateAnalysis.CompareTo(bestAnalysis) > 0)
                    {
                        bestAnalysis = candidateAnalysis;
                        newPuzzleCallback?.Invoke(bestAnalysis);
                    }
                }

                totalPuzzlesSearched++;
                searchProgressCallback?.Invoke(totalPuzzlesSearched);
            }

            return MakeGenerationReport(bestAnalysis.ResolutionReport, true);
        }

        public UnsolvableAnalysisReport FindUnsolvablePuzzle(
            Action<int> searchProgressCallback = null,
            CancellationToken? cancelToken = null, 
            int unsolvableDepth = -1,
            int seed = -1)
        {
            Random rgen = seed >= 0 ? new Random(seed) : new Random();
            PuzzleSolver solver = new PuzzleSolver(this.PropertySet, this.StrategyTargets.Select(t => t.Strategy));
            int totalPuzzlesSearched = 0;
            bool puzzleFound = false;

            do
            {
                // If we don't start over, we can get "stuck".
                // We can end up with a set of constraints for which there is no 
                // space in which puzzles have unique solutions but are still unsolvable.
                // Clear the constraints to get out of this situation.
                solver.ClearConstraints();
                solver.ResetAll();

                // Keep adding constraints until there is a unique solution.
                do solver.AddConstraint(rgen.Select(this.AvailablePatterns).RandomConstraint(this.Solution, rgen));
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
                        puzzleFound = true;
                    }
                }

                totalPuzzlesSearched++;
                searchProgressCallback?.Invoke(totalPuzzlesSearched);
            }
            while (!puzzleFound &&
                    !cancelToken.GetValueOrDefault().IsCancellationRequested);

            if (!puzzleFound)
                return new UnsolvableAnalysisReport(solver.Strategies, this.Solution);
            else
                return new UnsolvableAnalysisReport(solver.Strategies, solver.Constraints.ToList(), this.Solution);
        }

        private GenerationAnalysisReport MakeGenerationReport(ResolutionAnalysisReport resolutionReport, bool isFinal)
        {
            return new GenerationAnalysisReport(resolutionReport, this.StrategyTargets, this.ConstraintTargets, this.MaxTotalConstraints, isFinal);
        }
    }
}
