using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Resolution.Strategies
{
    public class DefaultStrategyList : IReadOnlyList<Strategy>
    {
        private List<Strategy> _strategies;

        public Strategy this[int index] => _strategies[index];
        public int Count => _strategies.Count;
        public IEnumerator<Strategy> GetEnumerator() => _strategies.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _strategies.GetEnumerator();

        public DefaultStrategyList()
        {
            _strategies = new List<Strategy>() {

                new BinaryConstraintAnalysisStrategy(IndirectionLevel.Direct),
                new BinaryConstraintAnalysisStrategy(IndirectionLevel.IndirectBoth),
                new BinaryConstraintAnalysisStrategy(IndirectionLevel.IndirectDistinctOnly),
                new BinaryConstraintAnalysisStrategy(IndirectionLevel.IndirectEqualOnly),
                new BlockCrossoutStrategy(),
                new DistinctConstraintStrategy(),
                new DoubleNextToImpliesBetweenStrategy(IndirectionLevel.Direct),
                new DoubleNextToImpliesBetweenStrategy(IndirectionLevel.IndirectBoth),
                new DoubleNextToImpliesBetweenStrategy(IndirectionLevel.IndirectDistinctOnly),
                new DoubleNextToImpliesBetweenStrategy(IndirectionLevel.IndirectEqualOnly),
                new DoubleNextToImpliesEqualStrategy(false),
                new DoubleNextToImpliesEqualStrategy(true),
                new EitherOrArgumentUnionStrategy(),
                new EitherOrDomainStrategy(),
                new EitherOrImpliesDistinctStrategy(),
                new EitherOrTransitiveConstraintGenerationStrategy(IndirectionLevel.Direct),
                new EitherOrTransitiveConstraintGenerationStrategy(IndirectionLevel.IndirectBoth),
                new EitherOrTransitiveConstraintGenerationStrategy(IndirectionLevel.IndirectDistinctOnly),
                new EitherOrTransitiveConstraintGenerationStrategy(IndirectionLevel.IndirectEqualOnly),
                new EqualConstraintStrategy(),
                new IdentityConstraintStrategy(),
                new ImmediateLessThanCompatibilityCheckStrategy(false),
                new ImmediateLessThanCompatibilityCheckStrategy(true),
                new LessThanDomainStrategy(),
                new LessThanImpliesDistinctStrategy(),
                new LessThanManyCompatibilityCheckStrategy(false),
                new LessThanManyCompatibilityCheckStrategy(true),
                new LessThanManyDomainStrategy(IndirectionLevel.Direct),
                new LessThanManyDomainStrategy(IndirectionLevel.IndirectBoth),
                new LessThanManyDomainStrategy(IndirectionLevel.IndirectDistinctOnly),
                new LessThanManyDomainStrategy(IndirectionLevel.IndirectEqualOnly),
                new LessThanNextToTransitiveConstraintGenerationStrategy(false),
                new LessThanNextToTransitiveConstraintGenerationStrategy(true),
                new LessThanTransitiveConstraintGenerationStrategy(false),
                new LessThanTransitiveConstraintGenerationStrategy(true),
                new NextToCompatibilityCheckStrategy(false),
                new NextToCompatibilityCheckStrategy(true),
                new NextToDomainStrategy(),
                new NextToImpliesDistinctStrategy(),
                new NextToIncompatibilitySearchStrategy(false),
                new NextToIncompatibilitySearchStrategy(true),
                new PigeonholeStrategy(),
                new PropertyPairAnalysisStrategy(),
                new SynchronizeStrategy(),
            };
        }
    }
}
