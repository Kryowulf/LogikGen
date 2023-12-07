using LogikGenAPI.Resolution.Strategies;

namespace LogikGenAPI.Resolution
{
    public class StrategyAnalysis
    {
        public Strategy Strategy { get; private set; }
        public int ApplicationsNeeded { get; private set; }

        public StrategyAnalysis(Strategy strategy, int applicationsNeeded)
        {
            this.Strategy = strategy;
            this.ApplicationsNeeded = applicationsNeeded;
        }
    }
}
