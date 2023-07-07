using LogikGenAPI.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Model
{
    public class Category : IReadOnlyList<Property>
    {
        public PropertySet Source { get; private set; }
        public int Index { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyList<Property> Properties { get; private set; }
        public bool IsOrdered { get; private set; }
        public int Count => this.Properties.Count;
        public Property this[int index] => this.Properties[index];

        public SubsetKey<Property> Full => this.Source.Full(this);
        public SubsetKey<Property> Empty => this.Source.Empty(this);

        public Category(PropertySet source, int categoryIndex, int initialPropertyIndex, CategoryDefinition cdef)
        {
            this.Source = source;
            this.Index = categoryIndex;
            this.Name = cdef.CategoryName;
            this.Properties = cdef.PropertyNames.Select(pn =>
                                new Property(source, initialPropertyIndex++, this, pn))
                                .ToList().AsReadOnly();
            this.IsOrdered = cdef.IsOrdered;
        }

        public override string ToString() => this.Name;
        public IEnumerator<Property> GetEnumerator() => this.Properties.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Properties.GetEnumerator();
    }
}
