using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Utilities
{
    public class IndexedPowerSet<T> : IEnumerable<SubsetKey<T>>
    {
        private IReadOnlyList<T>[] _subsets;
        private Dictionary<T, int> _singletonSubsetNumbers;

        public int MaximumItemCount => 24;
        public IReadOnlyList<T> Items => _subsets[_subsets.Length - 1];
        public int Size => _subsets.Length;
        public SubsetKey<T> Full => new SubsetKey<T>(this, _subsets.Length - 1);
        public SubsetKey<T> Empty => new SubsetKey<T>(this, 0);
        public SubsetKey<T> Singleton(T item) => new SubsetKey<T>(this, _singletonSubsetNumbers[item]);
        public IReadOnlyList<T> Lookup(int subsetNumber) => _subsets[subsetNumber];

        public IEnumerable<SubsetKey<T>> Subsets
        {
            get
            {
                for (int subsetNumber = 0; subsetNumber < _subsets.Length; subsetNumber++)
                    yield return new SubsetKey<T>(this, subsetNumber);
            }
        }

        public SubsetKey<T> ToSubset(IEnumerable<T> items)
        {
            int combinedSubsetNumber = 0;

            foreach (T item in items)
                combinedSubsetNumber |= _singletonSubsetNumbers[item];
            
            return new SubsetKey<T>(this, combinedSubsetNumber);
        }

        public SubsetKey<T> ToSubset(params T[] items) => ToSubset((IEnumerable<T>)items);
        

        public IEnumerator<SubsetKey<T>> GetEnumerator()
        {
            return this.Subsets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Subsets.GetEnumerator();
        }

        public IndexedPowerSet(IEnumerable<T> items)
        {
            List<T> itemList = items.ToList();

            if (itemList.Count > this.MaximumItemCount)
                throw new ArgumentException("Number of items exceeds maximum item count.");

            int itemCount = itemList.Count;
            int subsetCount = 1 << itemCount;

            _subsets = new IReadOnlyList<T>[subsetCount];
            _singletonSubsetNumbers = new Dictionary<T, int>();

            for (int subsetNumber = 0; subsetNumber < subsetCount; subsetNumber++)
            {
                List<T> subset = new List<T>(itemCount);

                for (int i = 0; i < itemCount; i++)
                {
                    if (((subsetNumber >> i) & 1) == 1)
                        subset.Add(itemList[i]);
                }

                _subsets[subsetNumber] = subset.AsReadOnly();

                if (subset.Count == 1)
                    _singletonSubsetNumbers[subset[0]] = subsetNumber;
            }
        }
    }
}
