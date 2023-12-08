using LogikGenAPI.Generation;
using LogikGenAPI.Generation.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.ViewModels
{
    public class ConstraintTargetsViewModel : ViewModel
    {
        private int? _maxTotalConstraints;
        public int? MaxTotalConstraints 
        { 
            get { return _maxTotalConstraints; }
            set { SetValue(ref _maxTotalConstraints, value); }
        }

        private int? _maxEqualConstraints;
        public int? MaxEqualConstraints 
        { 
            get { return _maxEqualConstraints; }
            set { SetValue(ref _maxEqualConstraints, value); }
        }

        private int? _maxDistinctConstraints;
        public int? MaxDistinctConstraints 
        { 
            get { return _maxDistinctConstraints; }
            set { SetValue(ref _maxDistinctConstraints, value); }
        }

        private int? _maxIdentityConstraints;
        public int? MaxIdentityConstraints 
        { 
            get { return _maxIdentityConstraints; }
            set { SetValue(ref _maxIdentityConstraints, value); }
        }

        private int? _maxLessThanConstraints;
        public int? MaxLessThanConstraints 
        { 
            get { return _maxLessThanConstraints; }
            set { SetValue(ref _maxLessThanConstraints, value); }
        }

        private int? _maxNextToConstraints;
        public int? MaxNextToConstraints 
        { 
            get { return _maxNextToConstraints; }
            set { SetValue(ref _maxNextToConstraints, value); }
        }

        private int? _maxEitherOrConstraints;
        public int? MaxEitherOrConstraints 
        { 
            get { return _maxEitherOrConstraints; }
            set { SetValue(ref _maxEitherOrConstraints, value); }
        }

        public IList<ConstraintTarget> BuildConstraintTargets()
        {
            return new List<ConstraintTarget>() {
                new ConstraintTarget(new DistinctConstraintPattern(), this.MaxDistinctConstraints ?? int.MaxValue),
                new ConstraintTarget(new EitherOrConstraintPattern(), this.MaxEitherOrConstraints ?? int.MaxValue),
                new ConstraintTarget(new EqualConstraintPattern(), this.MaxEqualConstraints ?? int.MaxValue),
                new ConstraintTarget(new IdentityConstraintPattern(), this.MaxIdentityConstraints ?? int.MaxValue),
                new ConstraintTarget(new LessThanConstraintPattern(), this.MaxLessThanConstraints ?? int.MaxValue),
                new ConstraintTarget(new NextToConstraintPattern(), this.MaxNextToConstraints ?? int.MaxValue)
            };
        }
    }
}
