using LogikGenAPI.Generation;
using LogikGenAPI.Generation.Patterns;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFUI.ViewModels
{
    public class GenerationWindowViewModel : ViewModel
    {
        private Dictionary<string, StrategyViewModel> _strategyVMsByName;

        public IList<StrategyViewModel> StrategyList { get; private set; }
        public SolutionGrid Solution { get; private set; }
        public bool IsGenerateUnsolvableChecked { get; set; }
        public int? UnsolvableDepth { get; set; }
        public int? MaxTotalConstraints { get; set; }
        public int? MaxEqualConstraints { get; set; }
        public int? MaxDistinctConstraints { get; set; }
        public int? MaxIdentityConstraints { get; set; }
        public int? MaxLessThanConstraints { get; set; }
        public int? MaxNextToConstraints { get; set; }
        public int? MaxEitherOrConstraints { get; set; }
        public int? Seed { get; set; }
        public int? NThreads { get; set; }

        public GenerationWindowViewModel(SolutionGrid solution)
        {
            this.StrategyList = new DefaultStrategyList().Select(s =>
                new StrategyViewModel(s)).ToList();

            (this.StrategyList as List<StrategyViewModel>).Sort((s1, s2) => s1.Name.CompareTo(s2.Name));

            this.Solution = solution;
            this.IsGenerateUnsolvableChecked = false;
            this.MaxTotalConstraints = null;
            this.Seed = null;
            this.NThreads = null;
            
            _strategyVMsByName = new Dictionary<string, StrategyViewModel>();

            foreach (StrategyViewModel svm in this.StrategyList)
                _strategyVMsByName[svm.Name] = svm;
        }

        public PuzzleGenerator MakeGenerator()
        {
            IEnumerable<StrategyAnalysis> minTargets =
                this.StrategyList.Where(s => s.IsEnabled)
                .Select(s => new StrategyAnalysis(s.Strategy, s.MinimumApplications.GetValueOrDefault(0)));

            IEnumerable<StrategyAnalysis> maxTargets =
                this.StrategyList.Where(s => s.IsEnabled)
                .Select(s => new StrategyAnalysis(s.Strategy, s.MaximumApplications.GetValueOrDefault(int.MaxValue)));

            List<ConstraintPattern> patterns = new List<ConstraintPattern>();

            int maxTotal = this.MaxTotalConstraints ?? int.MaxValue;
            int maxEqual = this.MaxEqualConstraints ?? int.MaxValue;
            int maxDistinct = this.MaxDistinctConstraints ?? int.MaxValue;
            int maxIdentity = this.MaxIdentityConstraints ?? int.MaxValue;
            int maxLessThan = this.MaxLessThanConstraints ?? int.MaxValue;
            int maxNextTo = this.MaxNextToConstraints ?? int.MaxValue;
            int maxEitherOr = this.MaxEitherOrConstraints ?? int.MaxValue;
            
            if (maxEqual > 0) patterns.Add(new EqualConstraintPattern(maxEqual));
            if (maxDistinct > 0) patterns.Add(new DistinctConstraintPattern(maxDistinct));
            if (maxIdentity > 0) patterns.Add(new IdentityConstraintPattern(maxIdentity));
            if (maxLessThan > 0) patterns.Add(new LessThanConstraintPattern(maxLessThan));
            if (maxNextTo > 0) patterns.Add(new NextToConstraintPattern(maxNextTo));
            if (maxEitherOr > 0) patterns.Add(new EitherOrConstraintPattern(maxEitherOr));

            return new PuzzleGenerator(this.Solution, patterns, minTargets, maxTargets, maxTotal);
        }
    }
}
