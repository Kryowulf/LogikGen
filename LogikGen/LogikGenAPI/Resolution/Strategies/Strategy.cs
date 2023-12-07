using LogikGenAPI.Model;
using LogikGenAPI.Utilities;

namespace LogikGenAPI.Resolution.Strategies
{
    public abstract class Strategy
    {
        private static DummyLogger _dummyLogger = new DummyLogger();
        private Logger _currentLogger = _dummyLogger;

        public virtual string Name => this.GetType().Name;
        public virtual string GroupName => this.GetType().Name;
        public override string ToString() => this.Name;
        public abstract StrategyClassification Classification { get; }
        public abstract bool AutoRepeat { get; }
        public abstract Difficulty Difficulty { get; }
        public Logger Logger
        {
            get { return _currentLogger; }
            set { _currentLogger = value ?? _dummyLogger; }
        }
        public bool LogGridOnUpdate { get; set; }

        protected abstract bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset);
        public bool Apply(PuzzleGrid grid, ConstraintSet cset)
        {
            this.Logger.SetTag(this.Name);

            bool updated = ApplyOnce(grid, cset);

            if (this.AutoRepeat && updated)
            {
                while (ApplyOnce(grid, cset)) ;
            }

            if (updated && this.LogGridOnUpdate)
                this.Logger.LogInfo("\n" + GridPrinter.BuildGridString(grid), false);

            return updated;
        }

        private class DummyLogger : Logger
        {
            public override void LogInfo(object data, bool includeTag = true)
            {
            }

            public override void SetTag(string name)
            {
            }
        }
    }
}
