using LogikGenAPI.Examples;
using LogikGenAPI.Model;
using LogikGenAPI.Resolution;
using LogikGenAPI.Resolution.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogikGenTests
{
    [TestClass]
    public class SynchronizeStrategyTests : PuzzleTestBase
    {
        [TestMethod]
        public void Run()
        {
            Setup(5, 5);

            /*
                Initial Relationships:

                    Spaniard lives in the 2nd house       
                    Dog does not live in the 2nd house.     

                    Green house owner drinks tea.
                    Green house owner does not own a fox.

                    The Spaniard drinks tea.
            

                Input:                                                  Expected:

                         | 2nd | spaniard | green | tea | fox | dog |            | 2nd | spaniard | green | tea | fox | dog |
                2nd      |-----|     O    |       |     |     |  X  |   2nd      |-----|     O    |   O   |  O  |  X  |  X  |
                spaniard |  O  |----------|       |  O  |     |     |   spaniard |  O  |----------|   O   |  O  |  X  |  X  |
                green    |     |          |-------|  O  |  X  |     |   green    |  O  |     O    |-------|  O  |  X  |  X  |
                tea      |     |     O    |   O   |-----|     |     |   tea      |  O  |     O    |   O   |-----|  X  |  X  |
                fox      |     |          |   X   |     |-----|  X  |   fox      |  X  |     X    |   X   |  X  |-----|  X  |
                dog      |  X  |          |       |     |  X  |-----|   dog      |  X  |     X    |   X   |  X  |  X  |-----|
                    
            */

            Grid.Disassociate(PSet["2nd"], PSet["Dog"]);
            Grid.Associate(PSet["2nd"], PSet["Spaniard"]);

            Grid.Disassociate(PSet["Green"], PSet["Fox"]);
            Grid.Associate(PSet["Green"], PSet["Tea"]);

            Grid.Associate(PSet["Spaniard"], PSet["Tea"]);
            
            PuzzleGrid expected = Grid.Clone();

            Strategy strategy = new SynchronizeStrategy();
            strategy.Apply(Grid, CSet);

            expected.Associate(PSet["2nd"], PSet["Green"]);
            expected.Associate(PSet["2nd"], PSet["Tea"]);
            expected.Disassociate(PSet["2nd"], PSet["Fox"]);

            expected.Associate(PSet["Spaniard"], PSet["Green"]);
            expected.Disassociate(PSet["Spaniard"], PSet["Fox"]);
            expected.Disassociate(PSet["Spaniard"], PSet["Dog"]);

            expected.Disassociate(PSet["Green"], PSet["Dog"]);

            expected.Disassociate(PSet["Tea"], PSet["Fox"]);
            expected.Disassociate(PSet["Tea"], PSet["Dog"]);

            Assert.IsTrue(GridMatch(expected));
        }
    }
}