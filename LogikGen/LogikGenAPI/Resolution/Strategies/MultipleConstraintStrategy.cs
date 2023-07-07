using LogikGenAPI.Model;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Resolution.Strategies
{
    public abstract class MultipleConstraintStrategy : Strategy
    {
        private Dictionary<IndirectionLevel, Difficulty> _difficulties;

        public override string Name => $"{base.Name}/{this.IndirectionLevel}";
        public override Difficulty Difficulty => _difficulties[this.IndirectionLevel];
        public IndirectionLevel IndirectionLevel { get; private set; }

        public MultipleConstraintStrategy(IndirectionLevel level)
        {
            this.IndirectionLevel = level;

            _difficulties = new Dictionary<IndirectionLevel, Difficulty>();
            _difficulties[IndirectionLevel.Direct] = Difficulty.Medium;
            _difficulties[IndirectionLevel.IndirectEqualOnly] = Difficulty.Hard;
            _difficulties[IndirectionLevel.IndirectDistinctOnly] = Difficulty.Harder;
            _difficulties[IndirectionLevel.IndirectBoth] = Difficulty.Hardest;
        }

        protected void SetDifficulty(IndirectionLevel level, Difficulty difficulty)
        {
            _difficulties[level] = difficulty;
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset)
        {
            StrategicPropertyComparer comparer = new StrategicPropertyComparer(this.IndirectionLevel, grid);
            return ApplyOnce(grid, cset, comparer);
        }

        protected abstract bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer);
    }
}
