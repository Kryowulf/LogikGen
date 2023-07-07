using LogikGenAPI.Model;

namespace LogikGenAPI.Resolution.Terms
{
    public class Variable
    {
        public Term Owner { get; set; }
        public Property Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public void Clear()
        {
            this.Owner = null;
            this.Value = null;
        }
    }
}
