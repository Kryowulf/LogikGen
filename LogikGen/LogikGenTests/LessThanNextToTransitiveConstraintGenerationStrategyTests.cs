using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenTests
{
    [TestClass]
    public class LessThanNextToTransitiveConstraintGenerationStrategyTests : PuzzleTestBase
    {
        [TestMethod]
        public void TestLeftToRight()
        {
            // LessThan(x, a) & LessThan(a, b) & NextTo(y, b) -> LessThan(x, y)

            Setup(5, 5);

            Category loc = PSet.Category("Location");

            CSet.Add(new LessThanConstraint(PSet["Spaniard"], PSet["Blue"], loc));
            CSet.Add(new LessThanConstraint(PSet["Blue"], PSet["Snails"], loc));
            CSet.Add(new NextToConstraint(PSet["Snails"], PSet["Coffee"], loc));

            ConstraintSet expected = CSet.Clone();
            expected.Add(new LessThanConstraint(PSet["Spaniard"], PSet["Coffee"], loc));

            Strategy strategy = new LessThanNextToTransitiveConstraintGenerationStrategy(true);
            strategy.Apply(Grid, CSet);

            Assert.IsTrue(CSetMatch(expected));
        }

        [TestMethod]
        public void TestRightToLeft()
        {
            // NextTo(x, a) & LessThan(a, b) & LessThan(b, y) -> LessThan(x, y)

            Setup(5, 5);

            Category loc = PSet.Category("Location");

            CSet.Add(new NextToConstraint(PSet["Spaniard"], PSet["Blue"], loc));
            CSet.Add(new LessThanConstraint(PSet["Blue"], PSet["Snails"], loc));
            CSet.Add(new LessThanConstraint(PSet["Snails"], PSet["Coffee"], loc));

            ConstraintSet expected = CSet.Clone();
            expected.Add(new LessThanConstraint(PSet["Spaniard"], PSet["Coffee"], loc));

            Strategy strategy = new LessThanNextToTransitiveConstraintGenerationStrategy(true);
            strategy.Apply(Grid, CSet);

            Assert.IsTrue(CSetMatch(expected));
        }
    }
}
