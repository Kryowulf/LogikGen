using System.Collections;
using System.Collections.Generic;

namespace LogikGenAPI.Utilities
{
    public class DisjointSet<T> : IEnumerable<T>
    {
        private T _content;
        private DisjointSet<T> _representativeNode;
        private int _subtreeSize;
        private DisjointSet<T> _next;

        public int Size => this.FindRootNode()._subtreeSize;
        public T Representative => this.FindRootNode()._content;

        public DisjointSet(T content)
        {
            _content = content;
            _representativeNode = this;
            _subtreeSize = 1;
            _next = this;
        }

        private DisjointSet<T> FindRootNode()
        {
            if (this == _representativeNode)
                return this;

            // Path compression.
            _representativeNode = _representativeNode.FindRootNode();

            return _representativeNode;
        }

        public bool UnionWith(DisjointSet<T> set)
        {
            DisjointSet<T> thisRoot = this.FindRootNode();
            DisjointSet<T> otherRoot = set.FindRootNode();

            if (thisRoot == otherRoot)
                return false;

            // Point the smaller set to the root of the larger set.
            if (thisRoot._subtreeSize <= otherRoot._subtreeSize)
            {
                thisRoot._representativeNode = otherRoot;
                otherRoot._subtreeSize += thisRoot._subtreeSize;
            }
            else
            {
                otherRoot._representativeNode = thisRoot;
                thisRoot._subtreeSize += otherRoot._subtreeSize;
            }

            // Swap the "next" links on the the two nodes to maintain a circular queue over the whole set.
            DisjointSet<T> X = thisRoot._next;
            DisjointSet<T> Y = otherRoot._next;
            thisRoot._next = Y;
            otherRoot._next = X;

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            // Iterate through the nodes of the set in the same order,
            // regardless of which node we're starting on.

            DisjointSet<T> root = this.FindRootNode();
            DisjointSet<T> cursor = root;

            do
            {
                yield return cursor._content;
                cursor = cursor._next;
            }
            while (cursor != root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (this == _representativeNode)
            {
                DisjointSet<T> other = obj as DisjointSet<T>;

                if (other == null)
                    return false;

                return base.Equals(other.FindRootNode());
            }

            return this.FindRootNode().Equals(obj);
        }

        public override int GetHashCode()
        {
            if (this == _representativeNode)
                return base.GetHashCode();

            return this.FindRootNode().GetHashCode();
        }

        public override string ToString()
        {
            return "{" + string.Join(", ", this) + "}";
        }
    }
}
