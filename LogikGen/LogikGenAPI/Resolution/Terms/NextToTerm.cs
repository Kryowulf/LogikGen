using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Resolution.Terms
{
    public class NextToTerm : BindingTerm
    {
        private IEnumerator<NextToConstraint> _enumerator;
        private bool _reverseNextMatch;

        private Variable X => Parameters[0];
        private Variable Y => Parameters[1];

        public NextToTerm(ConstraintSet cset, IPropertyComparer comparer, Variable x, Variable y, Category orderingCategory)
            : base(comparer, x, y)
        {
            _enumerator = cset.NextToConstraints(orderingCategory).ToList().GetEnumerator();
            _reverseNextMatch = false;
        }

        public override bool Match()
        {
            if (_reverseNextMatch)
            {
                NextToConstraint constraint = _enumerator.Current;

                _reverseNextMatch = false;
                
                if (BindArguments(constraint.Right, constraint.Left))
                    return true;
            }

            while (_enumerator.MoveNext())
            {
                NextToConstraint constraint = _enumerator.Current;

                _reverseNextMatch = true;

                if (BindArguments(constraint.Left, constraint.Right))
                    return true;

                _reverseNextMatch = false;

                if (BindArguments(constraint.Right, constraint.Left))
                    return true;
            }

            return false;
        }

        public override void Reset()
        {
            _enumerator.Reset();
            _reverseNextMatch = false;
        }

        public override string ToString()
        {
            return $"NextTo({X}, {Y})";
        }
    }
}
