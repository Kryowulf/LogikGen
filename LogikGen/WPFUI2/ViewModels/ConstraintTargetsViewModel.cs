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
    }
}
