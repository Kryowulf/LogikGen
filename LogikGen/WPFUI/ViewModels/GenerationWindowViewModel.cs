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
            IEnumerable<StrategyTarget> strategyTargets =
                this.StrategyList.Where(s => s.IsEnabled)
                .Select(s => new StrategyTarget(
                    s.Strategy, 
                    s.MinimumApplications ?? 0, 
                    s.MaximumApplications ?? int.MaxValue));


            List<ConstraintTarget> constraintTargets = new List<ConstraintTarget>() {
                new ConstraintTarget(new DistinctConstraintPattern(), this.MaxDistinctConstraints ?? int.MaxValue),
                new ConstraintTarget(new EitherOrConstraintPattern(), this.MaxEitherOrConstraints ?? int.MaxValue),
                new ConstraintTarget(new EqualConstraintPattern(), this.MaxEqualConstraints ?? int.MaxValue),
                new ConstraintTarget(new IdentityConstraintPattern(), this.MaxIdentityConstraints ?? int.MaxValue),
                new ConstraintTarget(new LessThanConstraintPattern(), this.MaxLessThanConstraints ?? int.MaxValue),
                new ConstraintTarget(new NextToConstraintPattern(), this.MaxNextToConstraints ?? int.MaxValue)
            };

            int maxTotal = this.MaxTotalConstraints ?? int.MaxValue;

            return new PuzzleGenerator(this.Solution, strategyTargets, constraintTargets, maxTotal);
        }
    }
}
