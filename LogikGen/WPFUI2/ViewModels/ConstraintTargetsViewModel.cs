using LogikGenAPI.Generation;
using LogikGenAPI.Generation.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.ViewModels
{
    public class ConstraintTargetsViewModel
    {
        public int? MaxTotalConstraints { get; set; }
        public int? MaxEqualConstraints { get; set; }
        public int? MaxDistinctConstraints { get; set; }
        public int? MaxIdentityConstraints { get; set; }
        public int? MaxLessThanConstraints { get; set; }
        public int? MaxNextToConstraints { get; set; }
        public int? MaxEitherOrConstraints { get; set; }

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
