using System.Collections.Generic;

namespace LogikGenAPI.Model
{
    public class OrderedGroup
    {
        public Property Key { get; set; }
        public Category OrderingCategory { get; set; }
        public IList<Property> Values { get; set; }

        public OrderedGroup(Property key, Category orderingCategory)
        {
            this.Key = key;
            this.OrderingCategory = orderingCategory;
            this.Values = new List<Property>();
        }
    }
}
