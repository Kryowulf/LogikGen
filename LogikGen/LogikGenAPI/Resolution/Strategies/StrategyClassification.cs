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

        [Description("Double NextTo")]
        DoubleNextTo,

        [Description("Compatibility Check")]
        CompatibilityCheck,

        [Description("Binary Constraint Analysis")]
        BinaryConstraintAnalysis,

        [Description("Constraint Generation")]
        ConstraintGeneration,
    }
}
