using LogikGenAPI.Utilities;

namespace LogikGenAPI.Model
{
    public interface IGrid
    {
        PropertySet PropertySet { get; }
        SubsetKey<Property> this[Property row, Category column] { get; }
        int TotalUnresolvedAssociations { get; }
        bool Contradiction { get; }
        bool Solved { get; }
        bool Complete { get; }
    }
}
