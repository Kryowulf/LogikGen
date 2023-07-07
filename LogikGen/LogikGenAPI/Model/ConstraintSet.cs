using System;
using System.Linq;
using System.Collections.Generic;
using LogikGenAPI.Model.Constraints;

namespace LogikGenAPI.Model
{
    public class ConstraintSet
    {
        private HashSet<Constraint> _constraints;

        public int Count => _constraints.Count;

        public IEnumerable<Constraint> All => _constraints;

        public IEnumerable<EqualConstraint> EqualConstraints =>
            _constraints.Where(c => c is EqualConstraint)
                        .Select(c => c as EqualConstraint);

        public IEnumerable<DistinctConstraint> DistinctConstraints =>
            _constraints.Where(c => c is DistinctConstraint)
                        .Select(c => c as DistinctConstraint);

        public IEnumerable<IdentityConstraint> IdentityConstraints =>
            _constraints.Where(c => c is IdentityConstraint)
                        .Select(c => c as IdentityConstraint);

        public IEnumerable<EitherOrConstraint> EitherOrConstraints =>
            _constraints.Where(c => c is EitherOrConstraint)
                        .Select(c => c as EitherOrConstraint);

        public IEnumerable<LessThanConstraint> LessThanConstraints(Category orderingCategory) =>
            _constraints.Where(c => c is LessThanConstraint)
                        .Select(c => c as LessThanConstraint)
                        .Where(c => c.OrderingCategory == orderingCategory);

        public IEnumerable<NextToConstraint> NextToConstraints(Category orderingCategory) =>
            _constraints.Where(c => c is NextToConstraint)
                        .Select(c => c as NextToConstraint)
                        .Where(c => c.OrderingCategory == orderingCategory);

        public IEnumerable<OrderedBinaryConstraint> OrderedBinaryConstraints(Category orderingCategory) =>
            _constraints.Where(c => c is OrderedBinaryConstraint)
                        .Select(c => c as OrderedBinaryConstraint)
                        .Where(c => c.OrderingCategory == orderingCategory);

        public IReadOnlyList<Category> OrderedCategories { get; private set; }

        public ConstraintSet(PropertySet pset)
        {
            _constraints = new HashSet<Constraint>();
            this.OrderedCategories = pset.OrderedCategories;
        }

        public ConstraintSet(ConstraintSet template)
        {
            _constraints = new HashSet<Constraint>(template._constraints);
            this.OrderedCategories = template.OrderedCategories;
        }

        public void Clear() => _constraints.Clear();
        public ConstraintSet Clone() => new ConstraintSet(this);
        public bool Add(Constraint constraint) => _constraints.Add(constraint);
        public void UnionWith(IEnumerable<Constraint> constraints) => _constraints.UnionWith(constraints);
        public bool Remove(Constraint constraint) => _constraints.Remove(constraint);
        public bool Contains(Constraint constraint) => _constraints.Contains(constraint);
        public bool SetEquals(ConstraintSet other) => _constraints.SetEquals(other._constraints);

        public IList<OrderedGroup> LessThanGroups(IPropertyComparer comparer)
        {
            return CreateOrderedGroups(comparer, (ltc) => ltc.Left, (ltc) => ltc.Right);
        }

        public IList<OrderedGroup> GreaterThanGroups(IPropertyComparer comparer)
        {
            return CreateOrderedGroups(comparer, (ltc) => ltc.Right, (ltc) => ltc.Left);
        }

        private IList<OrderedGroup> CreateOrderedGroups(
            IPropertyComparer comparer, 
            Func<LessThanConstraint, Property> getKey, 
            Func<LessThanConstraint, Property> getValue)
        {
            List<OrderedGroup> groups = new List<OrderedGroup>();

            foreach (Category category in this.OrderedCategories)
            {
                List<LessThanConstraint> ltcs = LessThanConstraints(category).ToList();

                for (int i = 0; i < ltcs.Count; i++)
                {
                    if (ltcs[i] != null)
                    {
                        Property key = getKey(ltcs[i]);
                        OrderedGroup group = new OrderedGroup(key, category);
                        group.Values.Add(getValue(ltcs[i]));

                        for (int j = i + 1; j < ltcs.Count; j++)
                        {
                            if (ltcs[j] != null && comparer.ProvenEqual(key, getKey(ltcs[j])))
                            {
                                if (group.Values.All(p => comparer.ProvenDistinct(p, getValue(ltcs[j]))))
                                    group.Values.Add(getValue(ltcs[j]));

                                ltcs[j] = null;
                            }
                        }

                        groups.Add(group);
                        ltcs[i] = null;
                    }
                }
            }

            return groups;
        }
    }
}
