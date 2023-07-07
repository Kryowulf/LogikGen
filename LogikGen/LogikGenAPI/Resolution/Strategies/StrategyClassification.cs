using System.ComponentModel;

namespace LogikGenAPI.Resolution.Strategies
{
    public enum StrategyClassification
    {
        [Description("Grid Only")]
        GridOnly,

        [Description("Basic Assertion")]
        BasicAssertion,

        [Description("Distinct Equivalent")]
        DistinctEquivalent,

        [Description("Simple Domain")]
        SimpleDomain,

        [Description("Constraint Generation")]
        ConstraintGeneration,

        [Description("Compatibility Check")]
        CompatibilityCheck,

        [Description("Double NextTo")]
        DoubleNextTo,

        [Description("Other")]
        Other
    }
}
