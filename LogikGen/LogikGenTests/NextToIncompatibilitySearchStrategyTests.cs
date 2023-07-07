using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogikGenTests
{
    [TestClass]
    public class NextToIncompatibilitySearchStrategyTests : PuzzleTestBase
    {
        [TestMethod]
        public void Run()
        {
            Setup(4, 5);
            Strategy strategy = new NextToIncompatibilitySearchStrategy(false);

            /*
                            1st 2nd 3rd 4th 5th | Norwegian | Blue | 
                Norwegian    X               X  | --------- | ---- |
                Blue         X               X  | --------- | ---- |
                Dog                             |     X     |  X   |
                
                NextTo(Norwegian, Blue)

                Conclude: Dog does not live in the 3rd house.
            */

            Grid.Disassociate(PSet["Norwegian"], PSet["1st"]);
            Grid.Disassociate(PSet["Norwegian"], PSet["5th"]);
            Grid.Disassociate(PSet["Blue"], PSet["1st"]);
            Grid.Disassociate(PSet["Blue"], PSet["5th"]);
            Grid.Disassociate(PSet["Dog"], PSet["Norwegian"]);
            Grid.Disassociate(PSet["Dog"], PSet["Blue"]);

            CSet.Add(new NextToConstraint(PSet["Norwegian"], PSet["Blue"], PSet.Category("Location")));

            PuzzleGrid expected = Grid.Clone();
            expected.Disassociate(PSet["Dog"], PSet["3rd"]);

            strategy.Apply(Grid, CSet);

            Assert.IsTrue(GridMatch(expected));
        }
    }
}
