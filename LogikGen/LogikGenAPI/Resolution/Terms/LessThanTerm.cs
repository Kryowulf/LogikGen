using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Resolution.Terms
{
    public class LessThanTerm : BindingTerm
    {
        private IEnumerator<LessThanConstraint> _enumerator;

        private Variable X => Parameters[0];
        private Variable Y => Parameters[1];

        public LessThanTerm(ConstraintSet cset, IPropertyComparer comparer, Variable x, Variable y, Category orderingCategory) 
            : base(comparer, x, y)
        {
            _enumerator = cset.LessThanConstraints(orderingCategory).ToList().GetEnumerator();
        }

        public override bool Match()
        {
            while (_enumerator.MoveNext())
            {
                LessThanConstraint constraint = _enumerator.Current;

                if (BindArguments(constraint.Left, constraint.Right))
                    return true;
            }

            return false;
        }

        public override void Reset()
        {
            _enumerator.Reset();
        }

        public override string ToString()
        {
            return $"LessThan({X}, {Y})";
        }
    }
}
