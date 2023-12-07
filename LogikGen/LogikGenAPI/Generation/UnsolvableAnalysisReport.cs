using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation
{
    public class UnsolvableAnalysisReport
    {
        public DateTime Timestamp { get; }
        public IReadOnlyList<Strategy> EnabledStrategies { get; }
        public IReadOnlyList<Constraint> Constraints { get; }
        public SolutionGrid Solution { get; }
        public bool IsCancelled { get; }

        public UnsolvableAnalysisReport(IReadOnlyList<Strategy> enabledStrategies, IReadOnlyList<Constraint> constraints, SolutionGrid solution)
        {
            this.Timestamp = DateTime.Now;
            this.EnabledStrategies = enabledStrategies;
            this.Constraints = constraints;
            this.Solution = solution;
            this.IsCancelled = false;
        }

        public UnsolvableAnalysisReport(IReadOnlyList<Strategy> enabledStrategies, SolutionGrid solution)
        {
            this.Timestamp = DateTime.Now;
            this.EnabledStrategies = enabledStrategies;
            this.Constraints = new Constraint[0];
            this.Solution = solution;
            this.IsCancelled = true;
        }

        public IGrid BuildPartialSolution()
        {
            if (this.IsCancelled)
                throw new InvalidOperationException();

            PuzzleSolver solver = new PuzzleSolver(this.Solution.PropertySet, this.EnabledStrategies);
            solver.AddConstraints(this.Constraints);
            solver.Resolve();
            return solver.Grid;
        }

        public string Print()
        {
            if (this.IsCancelled)
                return "Cancelled";
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Unsolvable Puzzle Generated " + this.Timestamp.ToLongTimeString());
            sb.AppendLine();
            PrintHeading(sb, "Constraints");
            sb.AppendLine();
            sb.AppendLine(this.Constraints.Count + " total constraints.");
            sb.AppendLine();
            sb.AppendLine(string.Join("\n", this.Constraints.Select(c => c.ToString())));
            sb.AppendLine();
            PrintHeading(sb, "Partial Solution");
            sb.AppendLine();
            sb.AppendLine(GridPrinter.BuildGridString(BuildPartialSolution()));
            sb.AppendLine();
            PrintHeading(sb, "Complete Solution");
            sb.AppendLine();
            sb.AppendLine(GridPrinter.BuildGridString(this.Solution));
            sb.AppendLine();
            PrintHeading(sb, "Enabled Strategies");
            sb.AppendLine();
            sb.AppendLine(string.Join("\n", this.EnabledStrategies.Select(s => s.ToString()).OrderBy(s => s)));

            return sb.ToString();
        }

        private void PrintHeading(StringBuilder sb, string heading)
        {
            sb.AppendLine('+' + new string('-', 62) + '+');
            sb.AppendLine('|' + heading.PadCenter(62) + '|');
            sb.AppendLine('+' + new string('-', 62) + '+');
        }
    }
}
