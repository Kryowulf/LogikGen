using LogikGenAPI.Model;
using LogikGenAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogikGenAPI.Resolution
{
    public class PuzzleGrid : IGrid
    {
        private SubsetKey<Property>[,] _grid;

        private DisjointSet<Property>[] _unions;

        public PropertySet PropertySet { get; private set; }
        public SubsetKey<Property> this[Property row, Category column] => _grid[row.Index, column.Index];

        public int TotalUnresolvedAssociations { get; private set; }
        public bool Contradiction { get; private set; }
        public bool Solved => this.TotalUnresolvedAssociations == 0 && !this.Contradiction;
        public bool Complete => this.TotalUnresolvedAssociations == 0 || this.Contradiction;

        public PuzzleGrid(PropertySet pset)
        {
            _grid = new SubsetKey<Property>[pset.Properties.Count, pset.Categories.Count];
            _unions = new DisjointSet<Property>[pset.Count];

            this.PropertySet = pset;
            this.Clear();
        }

        public PuzzleGrid(PuzzleGrid template)
        {
            PropertySet pset = template.PropertySet;

            _grid = new SubsetKey<Property>[pset.Properties.Count, pset.Categories.Count];

            foreach (Property p in pset)
                foreach (Category c in pset.Categories)
                    _grid[p.Index, c.Index] = template[p, c];

            _unions = new DisjointSet<Property>[pset.Count];

            foreach (Property p in pset)
                _unions[p.Index] = new DisjointSet<Property>(p);

            foreach (Property p in pset)
            {
                foreach (Property q in template._unions[p.Index])
                    _unions[p.Index].UnionWith(_unions[q.Index]);
            }

            this.PropertySet = pset;
            this.TotalUnresolvedAssociations = template.TotalUnresolvedAssociations;
            this.Contradiction = template.Contradiction;
        }

        public SolutionGrid AsSolution()
        {
            if (!this.Solved)
                throw new InvalidOperationException("Cannot construct solution from unsolved grid.");

            return new SolutionGrid(this);
        }

        public PuzzleGrid Clone()
        {
            return new PuzzleGrid(this);
        }

        public IEnumerable<(Property, Property)> GetUnresolvedAssociations()
        {
            foreach (Property p in this.PropertySet.Properties)
                foreach (Category c in this.PropertySet.Categories)
                    if (this[p, c].Count > 1)
                        foreach (Property q in this[p, c])
                            yield return (p, q);
        }

        public IEnumerable<Property> GetUnionsOf(Property root)
        {
            return _unions[root.Index];
        }

        public void Clear()
        {
            PropertySet pset = this.PropertySet;

            foreach (Property p in pset)
                foreach (Category c in pset.Categories)
                    _grid[p.Index, c.Index] = c.Full;

            foreach (Property p in pset)
                _grid[p.Index, p.Category.Index] = p.Singleton;

            this.TotalUnresolvedAssociations = pset.Count * (pset.Count - pset.CategorySize);
            this.Contradiction = false;

            foreach (Property p in pset)
                _unions[p.Index] = new DisjointSet<Property>(p);
        }

        public void FlagContradiction() { this.Contradiction = true; }
        public bool Associate(Property row, Property column) => this.Update(row, column.Singleton);
        public bool Disassociate(Property row, Property column) => this.Update(row, ~column.Singleton);

        public bool Update(Property row, SubsetKey<Property> mask)
        {
            int initialUnresolvedCount = this.TotalUnresolvedAssociations;

            Category category = mask.Source.Full[0].Category;
            SubsetKey<Property> originalField = _grid[row.Index, category.Index];
            SubsetKey<Property> updatedField = originalField & mask;
            SubsetKey<Property> crossedValues = originalField.Subtract(updatedField);

            if (originalField == updatedField)
                return false;

            if (updatedField.Count == 0)
            {
                this.Contradiction = true;
                return false;
            }

            _grid[row.Index, category.Index] = updatedField;

            this.TotalUnresolvedAssociations -= crossedValues.Count;

            if (updatedField.Count == 1)
                this.TotalUnresolvedAssociations--;

            if (updatedField.Count == 1)
                _unions[row.Index].UnionWith(_unions[updatedField[0].Index]);

            foreach (Property q in crossedValues)
                Update(q, ~row.Singleton);

            if (updatedField.Count == 1)
                Update(updatedField[0], row.Singleton);

            return this.TotalUnresolvedAssociations < initialUnresolvedCount;
        }

        public void Synchronize()
        {
            PropertySet pset = this.PropertySet;
            int initialUnresolvedCount;

            do
            {
                initialUnresolvedCount = this.TotalUnresolvedAssociations;

                foreach (Property p in pset)
                {
                    foreach (Property q in _unions[p.Index].Where(q => q.Index > p.Index))
                    {
                        foreach (Category c in pset.Categories)
                        {
                            Update(p, this[q, c]);
                            Update(q, this[p, c]);
                        }
                    }
                }
            }
            while (this.TotalUnresolvedAssociations < initialUnresolvedCount);
        }
    }
}
