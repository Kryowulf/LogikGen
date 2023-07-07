using System;
using System.Collections;
using System.Collections.Generic;

namespace LogikGenAPI.Utilities
{
    public struct SubsetKey<T> : IEquatable<SubsetKey<T>>, IComparable<SubsetKey<T>>, IReadOnlyList<T>
    {
        public int SubsetNumber { get; private set; }
        public IndexedPowerSet<T> Source { get; private set; }
        public int Count => this.Source.Lookup(this.SubsetNumber).Count;
        public bool IsEmpty => this.Source.Lookup(this.SubsetNumber).Count == 0;
        public T this[int index] => this.Source.Lookup(this.SubsetNumber)[index];

        public SubsetKey(IndexedPowerSet<T> source, int subsetNumber)
        {
            if (subsetNumber < 0 || source.Size <= subsetNumber)
                throw new ArgumentOutOfRangeException("subsetNumber");

            this.Source = source;
            this.SubsetNumber = subsetNumber;
        }

        public SubsetKey<T> Union(SubsetKey<T> other)
        {
            if (this.Source != other.Source)
                throw new ArgumentException("Argument does not share the same source collection.");

            return new SubsetKey<T>(this.Source, this.SubsetNumber | other.SubsetNumber);
        }

        public SubsetKey<T> Intersect(SubsetKey<T> other)
        {
            if (this.Source != other.Source)
                throw new ArgumentException("Argument does not share the same source collection.");

            return new SubsetKey<T>(this.Source, this.SubsetNumber & other.SubsetNumber);
        }

        public SubsetKey<T> Complement()
        {
            int maximumSubsetNumber = this.Source.Size - 1;
            return new SubsetKey<T>(this.Source, maximumSubsetNumber & ~this.SubsetNumber);
        }

        public SubsetKey<T> Subtract(SubsetKey<T> other)
        {
            return this.Intersect(other.Complement());
        }

        public SubsetKey<T> ShiftLeft(int n)
        {
            // The bits in the SubsetNumber are in reverse order from that of the items in the set.
            // It makes more sense to interpret the "shift left" operation as:

            // Domain:  { a b c d e } 
            //      S:  {     c     }
            // S << 1:  {   b       }

            int maximumSubsetNumber = this.Source.Size - 1;
            return new SubsetKey<T>(this.Source, maximumSubsetNumber & (this.SubsetNumber >> n));
        }

        public SubsetKey<T> ShiftRight(int n)
        {
            int maximumSubsetNumber = this.Source.Size - 1;
            return new SubsetKey<T>(this.Source, maximumSubsetNumber & (this.SubsetNumber << n));
        }

        public int CompareTo(SubsetKey<T> other)
        {
            if (this.Source != other.Source)
                throw new ArgumentException("Argument does not share the same source collection.");

            return this.SubsetNumber.CompareTo(other.SubsetNumber);
        }

        public bool ContainsSubset(SubsetKey<T> other)
        {
            return (this.SubsetNumber & other.SubsetNumber) == other.SubsetNumber;
        }

        public bool IsSubsetOf(SubsetKey<T> other)
        {
            return (other.SubsetNumber & this.SubsetNumber) == this.SubsetNumber;
        }

        public bool Equals(SubsetKey<T> other)
        {
            return (this.SubsetNumber == other.SubsetNumber && this.Source == other.Source);
        }

        public override bool Equals(object obj)
        {
            if (obj is SubsetKey<T>)
                return this.Equals((SubsetKey<T>)obj);
            else
                return false;
        }

        public override string ToString() => $"{{{string.Join(", ", this)}}}";
        public override int GetHashCode() => this.SubsetNumber.GetHashCode();
        public IEnumerator<T> GetEnumerator() => this.Source.Lookup(this.SubsetNumber).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Source.Lookup(this.SubsetNumber).GetEnumerator();
        public static bool operator ==(SubsetKey<T> left, SubsetKey<T> right) => left.Equals(right);
        public static bool operator !=(SubsetKey<T> left, SubsetKey<T> right) => !left.Equals(right);
        public static bool operator <(SubsetKey<T> left, SubsetKey<T> right) => left.CompareTo(right) < 0;
        public static bool operator >(SubsetKey<T> left, SubsetKey<T> right) => left.CompareTo(right) > 0;
        public static bool operator <=(SubsetKey<T> left, SubsetKey<T> right) => left.CompareTo(right) <= 0;
        public static bool operator >=(SubsetKey<T> left, SubsetKey<T> right) => left.CompareTo(right) >= 0;
        public static SubsetKey<T> operator |(SubsetKey<T> left, SubsetKey<T> right) => left.Union(right);
        public static SubsetKey<T> operator &(SubsetKey<T> left, SubsetKey<T> right) => left.Intersect(right);
        public static SubsetKey<T> operator -(SubsetKey<T> left, SubsetKey<T> right) => left.Subtract(right);
        public static SubsetKey<T> operator ~(SubsetKey<T> subset) => subset.Complement();
        public static SubsetKey<T> operator <<(SubsetKey<T> subset, int n) => subset.ShiftLeft(n);
        public static SubsetKey<T> operator >>(SubsetKey<T> subset, int n) => subset.ShiftRight(n);
    }
}
