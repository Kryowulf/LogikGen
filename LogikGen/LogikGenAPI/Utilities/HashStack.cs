using System.Collections;
using System.Collections.Generic;

namespace LogikGenAPI.Utilities
{
    public class HashStack<T> : IReadOnlyCollection<T>
    {
        private Stack<T> _stack;
        private HashSet<T> _hashset;

        public HashStack()
        {
            _stack = new Stack<T>();
            _hashset = new HashSet<T>();
        }

        public HashStack(int capacity)
        {
            _stack = new Stack<T>(capacity);
            _hashset = new HashSet<T>(capacity);
        }

        public HashStack(IEnumerable<T> collection)
        {
            _stack = new Stack<T>();
            _hashset = new HashSet<T>();

            foreach (T item in collection)
            {
                if (_hashset.Add(item))
                    _stack.Push(item);
            }
        }

        public HashStack(IEqualityComparer<T> comparer)
        {
            _stack = new Stack<T>();
            _hashset = new HashSet<T>(comparer);
        }

        public HashStack(int capacity, IEqualityComparer<T> comparer)
        {
            _stack = new Stack<T>(capacity);
            _hashset = new HashSet<T>(capacity, comparer);
        }

        public HashStack(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _stack = new Stack<T>();
            _hashset = new HashSet<T>(comparer);

            foreach (T item in collection)
            {
                if (_hashset.Add(item))
                    _stack.Push(item);
            }
        }

        public int Count => _stack.Count;

        public IEnumerator<T> GetEnumerator() => _stack.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _stack.GetEnumerator();

        public void Clear()
        {
            _stack.Clear();
            _hashset.Clear();
        }

        public bool Contains(T item) => _hashset.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _stack.CopyTo(array, arrayIndex);
        public T Peek() => _stack.Peek();

        public T Pop()
        {
            T item = _stack.Pop();
            _hashset.Remove(item);
            return item;
        }

        public void Push(T item)
        {
            if (_hashset.Add(item))
                _stack.Push(item);
        }

        public T[] ToArray() => _stack.ToArray();

        public void TrimExcess()
        {
            _stack.TrimExcess();
            _hashset.TrimExcess();
        }
    }
}
