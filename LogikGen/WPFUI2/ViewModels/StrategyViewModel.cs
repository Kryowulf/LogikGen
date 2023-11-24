using LogikGenAPI.Generation.Patterns;
using LogikGenAPI.Resolution.Strategies;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WPFUI2.ViewModels
{
    public class StrategyViewModel : ViewModel
    {
        public Strategy Strategy { get; private set; }
        public string Name => Strategy.Name;
        public StrategyClassification Classification => Strategy.Classification;
        public string ClassificationDescription => Strategy.Classification.GetDescriptionAttribute();
        
        public Difficulty Difficulty => Strategy.Difficulty;

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetValue(ref _isEnabled, value); }
        }

        private int? _minimumApplications;
        public int? MinimumApplications
        {
            get { return _minimumApplications; }
            set 
            { 
                SetValue(ref _minimumApplications, value);

                if (_minimumApplications > _maximumApplications)
                    this.MaximumApplications = _minimumApplications;
            }
        }

        private int? _maximumApplications;
        public int? MaximumApplications
        {
            get { return _maximumApplications; }
            set
            {
                SetValue(ref _maximumApplications, value);

                if (_minimumApplications > _maximumApplications)
                    this.MinimumApplications = _maximumApplications;
            }
        }

        public StrategyViewModel(Strategy strategy)
        {
            this.Strategy = strategy;
            this.IsEnabled = true;
            this.MinimumApplications = 0;
            this.MaximumApplications = null;
        }
    }
}
