using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Model
{
    public class CategoryDefinition
    {
        public string CategoryName { get; private set; }
        public bool IsOrdered { get; private set; }
        public IReadOnlyList<string> PropertyNames { get; private set; }

        public CategoryDefinition(string categoryName, bool isOrdered, IEnumerable<string> propertyNames)
        {
            this.CategoryName = categoryName;
            this.IsOrdered = isOrdered;
            this.PropertyNames = propertyNames.ToList().AsReadOnly();
        }

        public CategoryDefinition(string categoryName, bool isOrdered, params string[] propertyNames)
            : this(categoryName, isOrdered, (IEnumerable<string>)propertyNames)
        {
        }
    }
}
