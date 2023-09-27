using LogikGenAPI.Resolution.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation
{
    public class StrategyTarget
    {
        public Strategy Strategy { get; private set; }
        public int MinApplications { get; private set; }
        public int MaxApplications { get; private set; }

        public StrategyTarget(Strategy strategy, int minApplications = 0, int maxApplications = int.MaxValue)
        {
            this.Strategy = strategy;
            this.MinApplications = minApplications;
            this.MaxApplications = maxApplications;
        }
    }
}
