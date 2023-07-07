using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogikGenTests
{
    [TestClass]
    public class NextToCheckCompatibilityStrategyTests : PuzzleTestBase
    {
        private Strategy Strategy { get; set; }

        [TestInitialize]
        public void Init()
        {
            Setup(5, 5);

            Grid.Disassociate(PSet["Spaniard"], PSet["Dog"]);
            Grid.Disassociate(PSet["Spaniard"], PSet["Fox"]);
            Grid.Disassociate(PSet["Yellow"], PSet["Dog"]);
            Grid.Disassociate(PSet["Yellow"], PSet["Fox"]);

            CSet.Add(new NextToConstraint(PSet["Spaniard"], PSet["Yellow"], PSet.Category("Location")));

            this.Strategy = new NextToCompatibilityCheckStrategy(false);
        }

        [TestMethod]
        public void TestEmptyNeighbors()
        {
            /*
             *  Spaniard : { 1st 2nd 3rd 4th 5th }
             *  Yellow   : { 1st     3rd     5th } 
             *  
             *  Should conclude Spaniard: {2nd 4th}
             */

            Grid.Disassociate(PSet["Yellow"], PSet["2nd"]);
            Grid.Disassociate(PSet["Yellow"], PSet["4th"]);

            PuzzleGrid expected = Grid.Clone();

            Strategy.Apply(Grid, CSet);

            expected.Disassociate(PSet["Spaniard"], PSet["1st"]);
            expected.Disassociate(PSet["Spaniard"], PSet["3rd"]);
            expected.Disassociate(PSet["Spaniard"], PSet["5th"]);

            Assert.IsTrue(GridMatch(expected));
        }

        [TestMethod]
        public void TestIncompatibleAssociations()
        {
            /*  
             *  Spaniard : { 1st 2nd 3rd 4th 5th }
             *  Yellow   : { 1st 2nd 3rd 4th 5th }
             *  Dog      : {     2nd 3rd         }
             *  Fox      : {         3rd 4th     }
             *  
             *  Should conclude Spaniard {1st 2nd 4th 5th }
             */

            Grid.Disassociate(PSet["Dog"], PSet["1st"]);
            Grid.Disassociate(PSet["Dog"], PSet["4th"]);
            Grid.Disassociate(PSet["Dog"], PSet["5th"]);

            Grid.Disassociate(PSet["Fox"], PSet["1st"]);
            Grid.Disassociate(PSet["Fox"], PSet["2nd"]);
            Grid.Disassociate(PSet["Fox"], PSet["5th"]);

            PuzzleGrid expected = Grid.Clone();

            Strategy.Apply(Grid, CSet);

            expected.Disassociate(PSet["Spaniard"], PSet["3rd"]);

            Assert.IsTrue(GridMatch(expected));
        }
    }
}
