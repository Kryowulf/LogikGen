using LogikGenAPI.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Model
{
    public class PropertySet : IReadOnlyList<Property>
    {
        private Dictionary<string, Category> _categoriesByName;
        private Dictionary<string, Property> _propertiesByName;

        public IReadOnlyList<Category> Categories { get; private set; }
        public IReadOnlyList<Category> OrderedCategories { get; private set; }
        public IReadOnlyList<Property> Properties { get; private set; }
        public IReadOnlyList<IndexedPowerSet<Property>> Powersets { get; private set; }
        public IReadOnlyList<SubsetKey<Property>> Singletons { get; private set; }
        public int CategorySize { get; private set; }

        public int Count => this.Properties.Count;
        public Property this[int index] => this.Properties[index];
        public Property this[string name] => _propertiesByName[name];
        public Property Property(string name) => _propertiesByName[name];
        public Category Category(string name) => _categoriesByName[name];
        public SubsetKey<Property> Singleton(Property p) => this.Singletons[p.Index];
        public SubsetKey<Property> Singleton(string name) => this.Singletons[_propertiesByName[name].Index];
        public SubsetKey<Property> Empty(Category category) => this.Powersets[category.Index].Empty;
        public SubsetKey<Property> Full(Category category) => this.Powersets[category.Index].Full;

        public SubsetKey<Property> ToSubset(IEnumerable<Property> properties)
        {
            if (!properties.Any())
                throw new ArgumentException("Cannot resolve category for an empty properties list.");

            SubsetKey<Property> result = this.Empty(properties.First().Category);

            foreach (Property p in properties)
                result |= this.Singleton(p);

            return result;
        }

        public SubsetKey<Property> ToSubset(params Property[] properties) =>
            this.ToSubset((IEnumerable<Property>)properties);

        public SubsetKey<Property> ToSubset(IEnumerable<string> properties) =>
            this.ToSubset(properties.Select(pn => this[pn]));

        public SubsetKey<Property> ToSubset(params string[] properties) =>
            this.ToSubset(properties.Select(pn => this[pn]));

        public IEnumerator<Property> GetEnumerator() => this.Properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.Properties.GetEnumerator();

        public PropertySet(IEnumerable<CategoryDefinition> definitions)
        {
            List<Category> categories = new List<Category>();
            List<Property> properties = new List<Property>();
            List<IndexedPowerSet<Property>> powersets = new List<IndexedPowerSet<Property>>();
            List<SubsetKey<Property>> singletons = new List<SubsetKey<Property>>();

            int categoryIndex = 0;
            int propertyIndex = 0;

            foreach (CategoryDefinition cdef in definitions)
            {
                Category category = new Category(this, categoryIndex, propertyIndex, cdef);

                IndexedPowerSet<Property> powerset = new IndexedPowerSet<Property>(category);

                categories.Add(category);
                properties.AddRange(category);
                powersets.Add(powerset);
                singletons.AddRange(category.Select(p => powerset.Singleton(p)));

                categoryIndex++;
                propertyIndex += category.Count;
            }

            if (categories.Count == 0)
                throw new ArgumentException("No categories defined.");

            if (categories[0].Count == 0)
                throw new ArgumentException("Empty categories are not supported.");

            if (categories.Any(c => c.Count != categories[0].Count))
                throw new ArgumentException("Varying sized categories are not supported.");

            _categoriesByName = new Dictionary<string, Category>();
            categories.ForEach(c => _categoriesByName.Add(c.Name, c));

            _propertiesByName = new Dictionary<string, Property>();
            properties.ForEach(p => _propertiesByName.Add(p.Name, p));

            this.Categories = categories.AsReadOnly();
            this.OrderedCategories = categories.Where(c => c.IsOrdered).ToList().AsReadOnly();
            this.Properties = properties.AsReadOnly();
            this.Powersets = powersets.AsReadOnly();
            this.Singletons = singletons.AsReadOnly();
            this.CategorySize = categories[0].Count;
        }

        public PropertySet(params CategoryDefinition[] definitions)
            : this((IEnumerable<CategoryDefinition>)definitions)
        {
        }
    }
}
