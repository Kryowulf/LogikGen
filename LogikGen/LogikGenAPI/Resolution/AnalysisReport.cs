using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogikGenAPI.Resolution
{
    public class AnalysisReport : IComparable<AnalysisReport>
    {
        private Dictionary<Strategy, StrategyAnalysis> _analysesByStrategy;
        private Dictionary<string, StrategyAnalysis> _analysesByName;

        public SolutionGrid Solution { get; private set; }
        public IReadOnlyList<Constraint> Constraints { get; private set; }
        public IReadOnlyList<StrategyAnalysis> Analyses { get; private set; }
        
        public StrategyAnalysis this[Strategy s] => _analysesByStrategy[s];
        public StrategyAnalysis this[string name] => _analysesByName[name];

        public AnalysisReport(SolutionGrid solution, IEnumerable<Constraint> constraints, IEnumerable<StrategyAnalysis> analyses)
        {
            _analysesByStrategy = new Dictionary<Strategy, StrategyAnalysis>();
            _analysesByName = new Dictionary<string, StrategyAnalysis>();

            this.Solution = solution;
            this.Constraints = constraints.ToList().AsReadOnly();
            this.Analyses = analyses.ToList().AsReadOnly();

            foreach (StrategyAnalysis sa in this.Analyses)
            {
                _analysesByStrategy[sa.Strategy] = sa;
                _analysesByName[sa.Strategy.Name] = sa;
            }
        }

        public int CompareTo(AnalysisReport other)
        {
            // Get a list of all difficulty levels in reverse order, from hardest to easiest.
            List<Difficulty> difficulties = Enum.GetValues<Difficulty>().ToList();
            difficulties.Sort();
            difficulties.Reverse();

            Dictionary<Difficulty, int> difficultyCounts = new Dictionary<Difficulty, int>();

            foreach (Difficulty d in difficulties)
                difficultyCounts[d] = 0;

            // Sum up the total applications needed in each difficulty level for "this" report. 
            foreach (StrategyAnalysis analysis in this.Analyses)
                difficultyCounts[analysis.Strategy.Difficulty] += analysis.ApplicationsNeeded;

            // Subtract the total applications needed in each difficulty level for the "other" report. 
            foreach (StrategyAnalysis anaysis in other.Analyses)
                difficultyCounts[anaysis.Strategy.Difficulty] -= anaysis.ApplicationsNeeded;

            // Start from the hardest difficulty level and go down.
            // If one analysis requires more applications than the other at a particular 
            // difficulty level, then that analysis corresponds to the harder/better puzzle.
            foreach (Difficulty d in difficulties)
            {
                int count = difficultyCounts[d];

                if (count != 0)
                    return count;
            }

            return 0;
        }

        public string Print()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(this.Constraints.Count + " total constraints.");

            foreach (Constraint constraint in this.Constraints)
                sb.AppendLine(constraint.ToString());

            sb.AppendLine();

            foreach (StrategyAnalysis analysis in this.Analyses.OrderBy(a => a.Strategy.Name))
                sb.AppendLine(analysis.ToString());

            sb.AppendLine();
            sb.AppendLine(GridPrinter.BuildGridString(this.Solution));
            sb.AppendLine();
            sb.AppendLine(GridPrinter.BuildSolutionTableString(this.Solution));

            return sb.ToString();
        }
    }
}
