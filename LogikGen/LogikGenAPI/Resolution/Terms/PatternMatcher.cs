using LogikGenAPI.Model;
using System;
using System.Collections.Generic;

namespace LogikGenAPI.Resolution.Terms
{
    public class PatternMatcher
    {
        private HashSet<Variable> _variables;
        private List<Term> _rule;
        private int _index;
        private ConstraintSet _cset;
        private IPropertyComparer _comparer;
        private Category _orderingCategory;

        public PatternMatcher(ConstraintSet cset, IPropertyComparer comparer, Category orderingCategory = null)
        {
            _variables = new HashSet<Variable>();
            _rule = new List<Term>();
            _index = 0;
            _cset = cset;
            _comparer = comparer;
            _orderingCategory = orderingCategory;
        }

        public bool Match()
        {
            while (0 <= _index && _index < _rule.Count)
            {
                if (_rule[_index].Match())
                {
                    _index++;
                }
                else
                {
                    _rule[_index].Reset();
                    _index--;
                }
            }

            if (_index < 0)
                return false;

            _index--;
            return true;
        }

        public void Reset()
        {
            _index = 0;

            foreach (Term t in _rule)
                t.Reset();
        }

        public void Clear()
        {
            foreach (Variable v in _variables)
                v.Clear();

            _variables.Clear();
            _rule.Clear();
            _index = 0;
        }

        public PatternMatcher LessThan(Variable x, Variable y)
        {
            if (_orderingCategory == null)
                throw new InvalidOperationException("Ordering category not given for this matcher.");

            _variables.Add(x);
            _variables.Add(y);
            _rule.Add(new LessThanTerm(_cset, _comparer, x, y, _orderingCategory));
            return this;
        }

        public PatternMatcher NextTo(Variable x, Variable y)
        {
            if (_orderingCategory == null)
                throw new InvalidOperationException("Ordering category not given for this matcher.");

            _variables.Add(x);
            _variables.Add(y);
            _rule.Add(new NextToTerm(_cset, _comparer, x, y, _orderingCategory));
            return this;
        }

        public PatternMatcher EitherOr(Variable key, Variable x, Variable y)
        {
            _variables.Add(key);
            _variables.Add(x);
            _variables.Add(y);
            _rule.Add(new EitherOrTerm(_cset, _comparer, key, x, y));
            return this;
        }

        public PatternMatcher Distinct(Variable x, Variable y)
        {
            _variables.Add(x);
            _variables.Add(y);
            _rule.Add(new DistinctTerm(_comparer, x, y));
            return this; 
        }
    }
}
