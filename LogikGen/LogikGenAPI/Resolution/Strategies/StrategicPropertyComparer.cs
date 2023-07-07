using LogikGenAPI.Model;

namespace LogikGenAPI.Resolution.Strategies
{
    public class StrategicPropertyComparer : IPropertyComparer
    {
        private IndirectionLevel _level;
        private IGrid _grid;

        public StrategicPropertyComparer(IndirectionLevel level, IGrid grid)
        {
            _level = level;
            _grid = grid;
        }

        public bool ProvenDistinct(Property x, Property y)
        {
            if (_level == IndirectionLevel.IndirectDistinctOnly || _level == IndirectionLevel.IndirectBoth)
                return (_grid[x, y.Category] & y.Singleton) == y.Category.Empty;
            else
                return x.Category == y.Category && x != y;
        }

        public bool ProvenEqual(Property x, Property y)
        {
            if (_level == IndirectionLevel.IndirectEqualOnly || _level == IndirectionLevel.IndirectBoth)
                return _grid[x, y.Category] == y.Singleton;
            else
                return x == y;
        }
    }
}
