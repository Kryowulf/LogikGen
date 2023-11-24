using LogikGenAPI.Resolution.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.ViewModels
{
    public class MainViewModel
    {
        public DefinitionGridViewModel Definitions { get; private set; }
        public IList<StrategyViewModel> StrategyList { get; private set; }
        public ConstraintTargetsViewModel ConstraintTargets { get; private set; }
        public bool IsGenerateUnsolvableChecked { get; set; }

        public MainViewModel()
        {
            this.Definitions = new DefinitionGridViewModel();
            this.StrategyList = new List<StrategyViewModel>();
            this.ConstraintTargets = new ConstraintTargetsViewModel();
            this.IsGenerateUnsolvableChecked = false;

            this.StrategyList = new DefaultStrategyList()
                                .Select(s => new StrategyViewModel(s))
                                .ToList();
        }
    }
}
