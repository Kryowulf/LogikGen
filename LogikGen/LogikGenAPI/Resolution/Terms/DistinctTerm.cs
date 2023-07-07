using LogikGenAPI.Model;
using System;

namespace LogikGenAPI.Resolution.Terms
{
    public class DistinctTerm : Term
    {
        private IPropertyComparer _comparer;
        private Variable _x;
        private Variable _y;
        private bool _matched;

        public DistinctTerm(IPropertyComparer comparer, Variable x, Variable y)
        {
            if (x.Owner == null || y.Owner == null)
                throw new ArgumentException("DistinctTerm cannot accept ownerless variables.");

            _comparer = comparer;
            _x = x;
            _y = y;
            _matched = false;
        }

        public override bool Match()
        {
            if (_matched)
                return false;

            _matched = true;
            return _comparer.ProvenDistinct(_x.Value, _y.Value);
        }

        public override void Reset()
        {
            _matched = false;
        }

        public override string ToString()
        {
            return $"Distinct({_x}, {_y})";
        }
    }
}
