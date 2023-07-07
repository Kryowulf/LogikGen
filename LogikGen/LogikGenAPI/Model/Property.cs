using LogikGenAPI.Utilities;

namespace LogikGenAPI.Model
{
    public class Property
    {
        public PropertySet Source { get; private set; }
        public int Index { get; private set; }
        public Category Category { get; private set; }
        public string Name { get; private set; }

        public SubsetKey<Property> Singleton => this.Source.Singleton(this);
        public SubsetKey<Property> LessThan => new SubsetKey<Property>(this.Singleton.Source, this.Singleton.SubsetNumber - 1);
        public SubsetKey<Property> LessThanOrEqual => this.LessThan.Union(this.Singleton);
        public SubsetKey<Property> GreaterThanOrEqual => this.LessThan.Complement();
        public SubsetKey<Property> GreaterThan => this.LessThan.Union(this.Singleton).Complement();
        

        public Property(PropertySet source, int index, Category category, string name)
        {
            this.Source = source;
            this.Index = index;
            this.Category = category;
            this.Name = name;
        }

        public override string ToString() => this.Name;
    }
}
