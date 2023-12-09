using LogikGenAPI.Generation;
using LogikGenAPI.Model;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public ProgressViewModel ProgressModel { get; private set; }
        public DefinitionGridViewModel Definitions { get; private set; }
        public IList<StrategyViewModel> StrategyList { get; private set; }
        public ConstraintTargetsViewModel ConstraintTargets { get; private set; }
        public bool IsGenerateUnsolvableChecked { get; set; }


        private bool _isRunning;
        public bool IsRunning 
        { 
            get { return _isRunning; }
            set { SetValue(ref _isRunning, value); }
        }


        private bool _isCancelling;
        public bool IsCancelling
        {
            get { return _isCancelling; }
            set { SetValue(ref _isCancelling, value); }
        }


        public MainViewModel()
        {
            this.ProgressModel = new ProgressViewModel();
            this.Definitions = new DefinitionGridViewModel();
            this.StrategyList = new List<StrategyViewModel>();
            this.ConstraintTargets = new ConstraintTargetsViewModel();
            this.IsGenerateUnsolvableChecked = false;
            this.IsRunning = false;
            this.IsCancelling = false;

            this.StrategyList = new DefaultStrategyList()
                                .Select(s => new StrategyViewModel(s))
                                .ToList();
        }

        public void BuildDefinitionModel(out PropertySet pset, out SolutionGrid solution)
        {
            this.Definitions.BuildModel(out pset, out solution);
        }

        public IList<StrategyTarget> BuildStrategyTargets()
        {
            return StrategyList
                    .Where(s => s.IsEnabled)
                    .Select(s => new StrategyTarget(
                        s.Strategy, 
                        s.MinimumApplications ?? 0, 
                        s.MaximumApplications ?? int.MaxValue))
                    .ToList();
        }

        public IList<ConstraintTarget> BuildConstraintTargets()
        {
            return this.ConstraintTargets.BuildConstraintTargets();
        }
    }
}
