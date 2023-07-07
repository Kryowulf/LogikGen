using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Resolution.Terms
{
    public class EitherOrTerm : BindingTerm
    {
        private IEnumerator<EitherOrConstraint> _enumerator;
        private bool _reverseNextMatch;

        private Variable Key => Parameters[0];
        private Variable X => Parameters[1];
        private Variable Y => Parameters[2];

        public EitherOrTerm(ConstraintSet cset, IPropertyComparer comparer, Variable key, Variable x, Variable y) 
            : base(comparer, key, x, y)
        {
            _enumerator = cset.EitherOrConstraints.ToList().GetEnumerator();
            _reverseNextMatch = false;
        }

        public override bool Match()
        {
            if (_reverseNextMatch)
            {
                EitherOrConstraint constraint = _enumerator.Current;
                _reverseNextMatch = false;

                if (!IsResolved(constraint) && BindArguments(constraint.Key, constraint.Y, constraint.X))
                    return true;
            }

            while (_enumerator.MoveNext())
            {
                EitherOrConstraint constraint = _enumerator.Current;

                if (IsResolved(constraint))
                    continue;

                _reverseNextMatch = true;

                if (BindArguments(constraint.Key, constraint.X, constraint.Y))
                    return true;

                _reverseNextMatch = false;

                if (BindArguments(constraint.Key, constraint.Y, constraint.X))
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
            return $"EitherOr({Key}, {X}, {Y})";
        }

        private bool IsResolved(EitherOrConstraint c)
        {
            return this.Comparer.ProvenEqual(c.Key, c.X) || this.Comparer.ProvenEqual(c.Key, c.Y);
        }
    }
}
