using LogikGenAPI.Generation.Patterns;
using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation
{
    public class GenerationAnalysisReport : IComparable<GenerationAnalysisReport>
    {
        public DateTime Timestamp { get; }
        public ResolutionAnalysisReport ResolutionReport { get; }
        public IReadOnlyList<StrategyTarget> StrategyTargets { get; }
        public IReadOnlyList<ConstraintTarget> ConstraintTargets { get; }
        public int MaxTotalConstraints { get; }
        public bool IsFinalReport { get; }
        public int MaximumScore { get; }
        public int Score { get; }
        public bool AllTargetsSatisfied { get; }

        public GenerationAnalysisReport(
            ResolutionAnalysisReport resolutionReport,
            IReadOnlyList<StrategyTarget> strategyTargets,
            IReadOnlyList<ConstraintTarget> constraintTargets,
            int maxTotalConstraints,
            bool isFinal)
        {
            this.Timestamp = DateTime.Now;
            this.ResolutionReport = resolutionReport;
            this.StrategyTargets = strategyTargets;
            this.ConstraintTargets = constraintTargets;
            this.MaxTotalConstraints = maxTotalConstraints;
            this.IsFinalReport = isFinal;

            // Each strategy has a target min & max number of applications. 
            // Each constraint has a desired maximum count.
            // + 1 for meeting the desired maximum total constraints.
            this.MaximumScore = 2 * this.StrategyTargets.Count
                              + this.ConstraintTargets.Count
                              + 1;

            this.Score = CalculateScore();

            this.AllTargetsSatisfied = this.Score == this.MaximumScore;
        }

        public int CompareTo(GenerationAnalysisReport other)
        {
            // Prefer the report that satisfies more generation targets.

            // If they're equal in that regard, only then prefer the report
            // that resolution analysis says is better.

            // Harder puzzles aren't more desired if they exceed the user's
            // desired maximum number of constraints, or don't require a strategy
            // the user wants to have.

            if (this.Score < other.Score)
                return -1;
            else if (this.Score == other.Score)
                return this.ResolutionReport.CompareTo(other.ResolutionReport);
            else
                return 1;
        }

        private int CalculateScore()
        {
            int score = 0;

            foreach (StrategyTarget target in this.StrategyTargets)
            {
                StrategyAnalysis analysis = ResolutionReport[target.Strategy];

                if (analysis.ApplicationsNeeded >= target.MinApplications)
                    score++;

                if (analysis.ApplicationsNeeded <= target.MaxApplications)
                    score++;
            }

            if (ResolutionReport.Constraints.Count <= this.MaxTotalConstraints)
                score++;

            Dictionary<Type, int> countsByConstraintType = new Dictionary<Type, int>();

            foreach (Constraint c in ResolutionReport.Constraints)
            {
                Type t = c.GetType();

                if (countsByConstraintType.ContainsKey(t))
                    countsByConstraintType[t]++;
                else
                    countsByConstraintType[t] = 1;
            }

            foreach (ConstraintTarget t in this.ConstraintTargets)
            {
                if (countsByConstraintType.GetValueOrDefault(t.Pattern.ConstraintType, 0) <= t.MaxCount)
                    score++;
            }

            return score;
        }

        public string PrintBrief()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Report Generated " + this.Timestamp.ToLongTimeString());
            sb.AppendLine(this.AllTargetsSatisfied ? "All Targets Satisfied" : "Some Targets Unsatisfied");
            sb.AppendLine(this.IsFinalReport ? "Search Complete!" : "Search In Progress...");
            sb.AppendLine();
            PrintHeading(sb, "Constraints");
            sb.AppendLine();
            sb.AppendLine(this.ResolutionReport.Constraints.Count + " total constraints.");
            sb.AppendLine();

            foreach (Constraint c in this.ResolutionReport.Constraints.OrderBy(c => c.ToString()))
                sb.AppendLine(c.ToString());
            
            sb.AppendLine();
            PrintHeading(sb, "Strategies Required");
            sb.AppendLine();

            var appliedAnalyses = this.ResolutionReport.Analyses
                .Where(a => a.ApplicationsNeeded > 0)
                .OrderBy(a => a.Strategy.Name);

            foreach (StrategyAnalysis sa in appliedAnalyses)
            {
                sb.AppendLine($"{sa.Strategy.Name} ({sa.Strategy.Difficulty})");
                sb.AppendLine($"{sa.ApplicationsNeeded} application(s) needed.");
                sb.AppendLine();
            }
                
            return sb.ToString();
        }

        public string PrintComplete()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(PrintBrief());

            PrintHeading(sb, "Categories & Properties");
            sb.AppendLine();

            foreach (Category c in this.ResolutionReport.Solution.PropertySet.Categories)
            {
                sb.AppendLine(c.Name + ":");

                foreach (Property p in c.Properties)
                    sb.AppendLine("    " + p.Name);

                sb.AppendLine();
            }

            PrintHeading(sb, "Constraint Targets");
            sb.AppendLine();
            sb.AppendLine("Type                    Desired Maximum Count ");
            sb.AppendLine("--------------------    ------------------------");

            foreach (ConstraintTarget t in this.ConstraintTargets.OrderBy(t => t.Pattern.ConstraintType.Name))
            {
                sb.Append((t.Pattern.ConstraintType.Name + ":").PadRight(24));
                sb.AppendLine(t.MaxCount == int.MaxValue ? "No Limit" : t.MaxCount.ToString());
            }

            sb.AppendLine();
            sb.Append("Total:".PadRight(24));
            sb.AppendLine(this.MaxTotalConstraints == int.MaxValue ? "No Limit" : this.MaxTotalConstraints.ToString());
            sb.AppendLine();

            PrintHeading(sb, "Enabled Strategies");
            sb.AppendLine();

            foreach (StrategyTarget t in this.StrategyTargets.OrderBy(t => t.Strategy.Name))
            {
                sb.AppendLine($"{t.Strategy.Name} ({t.Strategy.Difficulty})");
                sb.Append("Applications Desired:   Minimum: ");
                sb.Append(t.MinApplications);
                sb.Append("      Maximum: ");
                sb.AppendLine(t.MaxApplications == int.MaxValue ? 
                    "No Limit" : t.MaxApplications.ToString());

                sb.AppendLine();
            }

            PrintHeading(sb, "Solution");

            sb.AppendLine();
            sb.AppendLine(GridPrinter.BuildGridString(this.ResolutionReport.Solution));
            sb.AppendLine();
            sb.AppendLine(GridPrinter.BuildSolutionTableString(this.ResolutionReport.Solution));
            sb.AppendLine();

            PrintHeading(sb, "How To Solve");
            sb.AppendLine();

            PuzzleSolver solver = new PuzzleSolver(
                this.ResolutionReport.Solution.PropertySet,
                this.StrategyTargets.Select(t => t.Strategy));

            solver.AddConstraints(this.ResolutionReport.Constraints);

            foreach (string step in solver.Explain(true))
                sb.AppendLine(step);

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
