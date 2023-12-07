using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution.Strategies;
using LogikGenAPI.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Resolution
{
    public class PuzzleSolver
    {
        private PuzzleGrid _grid;
        private List<Strategy> _strategies;
        private HashSet<Constraint> _primaryConstraints;
        private ConstraintSet _cset;

        public IGrid Grid => _grid;
        public IReadOnlyList<Strategy> Strategies => _strategies;
        public IReadOnlyCollection<Constraint> Constraints => _primaryConstraints;

        public PuzzleSolver(PropertySet pset)
            : this(pset, new DefaultStrategyList())
        {
        }
        
        public PuzzleSolver(PropertySet pset, IEnumerable<Strategy> strategies)
        {
            _grid = new PuzzleGrid(pset);
            _strategies = strategies.ToList();
            _strategies.Sort((s1, s2) => s1.Difficulty.CompareTo(s2.Difficulty));

            _primaryConstraints = new HashSet<Constraint>();
            _cset = new ConstraintSet(pset);
        }

        public bool AddConstraint(Constraint constraint)
        {
            _cset.Add(constraint);
            return _primaryConstraints.Add(constraint);
        }

        public void AddConstraints(IEnumerable<Constraint> constraints)
        {
            _primaryConstraints.UnionWith(constraints);
            _cset.UnionWith(constraints);
        }

        public void AddConstraints(params Constraint[] constraints)
        {
            AddConstraints((IEnumerable<Constraint>)constraints);
        }

        public void RemoveConstraint(Constraint constraint)
        {
            // If .Resolve() was run, some strategies may have created new constraints based on this one.
            // The only way to fully remove it is to rebuild the cset. 

            _primaryConstraints.Remove(constraint);
            _cset.Clear();
            _cset.UnionWith(_primaryConstraints);
        }

        public void ClearConstraints()
        {
            _primaryConstraints.Clear();
            _cset.Clear();
        }

        public void ResetAll()
        {
            _grid.Clear();
            _cset.Clear();
            _cset.UnionWith(_primaryConstraints);
        }

        public SolutionGrid Randomize(int solutionSeed = -1)
        {
            _primaryConstraints.Clear();
            _cset.Clear();
            _grid.Clear();

            Random rgen = solutionSeed >= 0 ? new Random(solutionSeed) : new Random();
            Category primaryCategory = _grid.PropertySet.Categories[0];

            for (int i = 1; i < _grid.PropertySet.Categories.Count; i++)
            {
                List<Property> secondaryProperties = _grid.PropertySet.Categories[i].ToList();
                rgen.Shuffle(secondaryProperties);

                for (int j = 0; j < _grid.PropertySet.CategorySize; j++)
                    _grid.Associate(primaryCategory[j], secondaryProperties[j]);
            }

            _grid.Synchronize();

            return new SolutionGrid(_grid);
        }

        public int Resolve()
        {
            int initial = _grid.TotalUnresolvedAssociations;
            bool updated;

            do
            {
                updated = false;

                foreach (Strategy s in _strategies)
                {
                    updated = s.Apply(_grid, _cset);

                    // Contradictions here are expected and normal when running FindSolutions.

                    // Uncomment these only to find out the last strategy applied before
                    // a contradiction happens.

                    //if (_grid.Contradiction)
                    //    System.Diagnostics.Debugger.Break();

                    if (updated)
                        break;
                }
            }
            while (updated);

            if (_grid.Solved)
            {
                if (_primaryConstraints.Any(c => c.Check(_grid) == ConstraintCheckResult.Contradicts))
                    _grid.FlagContradiction();
            }

            int final = _grid.TotalUnresolvedAssociations;
            return initial - final;
        }

        public IReadOnlyList<string> Explain(bool includeProgressGraphs = false)
        {
            Logger logger = new Logger();

            foreach (Strategy s in _strategies)
            {
                s.Logger = logger;
                s.LogGridOnUpdate = includeProgressGraphs;
            }

            ResetAll();
            Resolve();

            foreach (Strategy s in _strategies)
            {
                s.Logger = null;
                s.LogGridOnUpdate = false;
            }

            return logger.GetCurrentLog();
        }

        public ResolutionAnalysisReport QuickAnalysis()
        {
            this.ResetAll();

            Dictionary<Strategy, int> applications = new Dictionary<Strategy, int>(
                this.Strategies.Select(s => new KeyValuePair<Strategy, int>(s, 0)));

            bool updated;

            do
            {
                updated = false;

                foreach (Strategy s in _strategies)
                {
                    updated = s.Apply(_grid, _cset);

                    if (updated)
                    {
                        applications[s]++;
                        break;
                    }
                }
            }
            while (!_grid.Complete && updated);

            if (_primaryConstraints.Any(c => c.Check(_grid) == ConstraintCheckResult.Contradicts))
                _grid.FlagContradiction();

            if (!_grid.Solved)
                throw new InvalidOperationException("Analysis cannot be performed on an unsolvable puzzle.");

            IEnumerable<StrategyAnalysis> analyses = this.Strategies.Select(
                s => new StrategyAnalysis(s, applications[s]));

            return new ResolutionAnalysisReport(_grid.AsSolution(), _primaryConstraints, analyses);
        }

        public ResolutionAnalysisReport FullAnalysis()
        {
            this.ResetAll();
            this.Resolve();

            if (!this.Grid.Solved)
                throw new InvalidOperationException("Analysis cannot be performed on an unsolvable puzzle.");

            Dictionary<Strategy, int> applications = new Dictionary<Strategy, int>(
                this.Strategies.Select(s => new KeyValuePair<Strategy, int>(s, 0)));

            var strategyGroups = _strategies.GroupBy(s => s.GroupName);

            bool updated;

            foreach (var testGroup in strategyGroups)
            {
                this.ResetAll();
                
                do
                {
                    updated = false;

                    foreach (Strategy s in _strategies.Where(s => s.GroupName != testGroup.Key))
                        updated |= s.Apply(_grid, _cset);

                    if (!updated)
                    {
                        Strategy testStrategy = testGroup.FirstOrDefault(s => s.Apply(_grid, _cset));

                        if (testStrategy == null)
                        {
                            throw new ResolutionAnalysisException("Test strategy was expected to produce an update. " +
                            "Failure of a test strategy to produce an update indicates that the solvability " +
                            "of a puzzle depends on the order in which strategies are applied, which shouldn't happen. " +
                            "Check that strategies are not producing false/contradictory information.");
                        }
                        else
                        {
                            applications[testStrategy]++;
                        }
                    }
                }
                while (!_grid.Solved);
            }

            IEnumerable<StrategyAnalysis> analyses = this.Strategies.Select(
                s => new StrategyAnalysis(s, applications[s]));

            return new ResolutionAnalysisReport(_grid.AsSolution(), _primaryConstraints, analyses);
        }

        public bool FindUniqueSolution(out SolutionGrid solution)
        {
            List<SolutionGrid> solutions = FindSolutions(2);

            if (solutions.Count == 1)
            {
                solution = solutions[0];
                return true;
            }
            else
            {
                solution = null;
                return false;
            }
        }

        public List<SolutionGrid> FindSolutions(int limit = 2)
        {
            List<SolutionGrid> solutions = new List<SolutionGrid>();
            FindSolutions(limit, solutions);
            return solutions;
        }

        private void FindSolutions(int limit, List<SolutionGrid> solutions)
        {
            Resolve();

            if (_grid.Solved)
            {
                solutions.Add(_grid.AsSolution());
            }
            else if (!_grid.Complete)
            {
                (Property p, Property q) = _grid.GetUnresolvedAssociations().First();
                PuzzleGrid savedGrid = _grid.Clone();
                ConstraintSet savedCset = _cset.Clone();

                _grid.Update(p, q.Singleton);
                FindSolutions(limit, solutions);

                if (solutions.Count < limit)
                {
                    _grid = savedGrid.Clone();
                    _cset = savedCset.Clone();

                    _grid.Update(p, ~q.Singleton);
                    FindSolutions(limit, solutions);
                }

                _grid = savedGrid;
                _cset = savedCset;
            }
        }

        public int FindUniqueSolutionSearchDepth()
        {
            ResetAll();

            if (!FindUniqueSolution(out SolutionGrid solution))
                throw new InvalidOperationException("Finding the search depth requires the puzzle be uniquely solvable.");

            ResetAll();
            Resolve();

            return FindUniqueSolutionSearchDepth(solution);
        }

        private int FindUniqueSolutionSearchDepth(SolutionGrid solution)
        {
            // Precondition: The puzzle has been resolved.
            // It's either solved, or we're ready to make a guess.

            if (_grid.Solved)
            {
                return 0;
            }
            else if (!_grid.Complete)
            {
                PuzzleGrid savedGrid = _grid.Clone();
                ConstraintSet savedCset = _cset.Clone();
                int minDepth = int.MaxValue;

                HashSet<(Property, Property)> unresolvedAssociations =
                    new HashSet<(Property, Property)>(_grid.GetUnresolvedAssociations());

                while (unresolvedAssociations.Count > 0 && minDepth > 1)
                {
                    (Property p, Property q) = unresolvedAssociations.First();

                    if (solution[p, q.Category] == q.Singleton)
                        _grid.Associate(p, q);
                    else
                        _grid.Disassociate(p, q);

                    Resolve();

                    // If guess X directly implies association Y, then there's no need to 
                    // ever guess association Y since it can't result in a smaller search depth.
                    unresolvedAssociations.IntersectWith(_grid.GetUnresolvedAssociations());

                    // The above Resolve() and pruning operation mean that the puzzle is fully resolved
                    // anyway at the start of every recursive call, hence the precondition.
                    minDepth = Math.Min(FindUniqueSolutionSearchDepth(solution) + 1, minDepth);

                    // Restore for the next iteration of the loop.
                    _grid = savedGrid;
                    _cset = savedCset;
                }

                if (minDepth == int.MaxValue)
                    throw new UnexpectedContradictionException("No unresolved associations available on incomplete puzzle.");

                return minDepth;
            }
            else
            {
                throw new UnexpectedContradictionException("Unexpected contradiction encountered measuring search depth.");
            }
        }
    }
}
