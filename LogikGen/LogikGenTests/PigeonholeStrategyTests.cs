using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Model;
using LogikGenAPI.Resolution.Strategies;
using LogikGenAPI.Resolution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenTests
{
    [TestClass]
    public class PigeonholeStrategyTests : PuzzleTestBase
    {
        [TestMethod]
        public void Run()
        {
            Setup(4, 4);

            /*
                          | 1st 2nd 3rd 4th | Blue | Dog |
                Ukrainian |          X   X  |   X  |  X  |
                Blue      |          X   X  |------|     |
                Dog       |          X   X  |      |-----|

                
                The above implies that the Dog lives in the Blue house.
            */

            Grid.Disassociate(PSet["Ukrainian"], PSet["Blue"]);
            Grid.Disassociate(PSet["Ukrainian"], PSet["Dog"]);

            Grid.Disassociate(PSet["3rd"], PSet["Ukrainian"]);
            Grid.Disassociate(PSet["3rd"], PSet["Blue"]);
            Grid.Disassociate(PSet["3rd"], PSet["Dog"]);

            Grid.Disassociate(PSet["4th"], PSet["Ukrainian"]);
            Grid.Disassociate(PSet["4th"], PSet["Blue"]);
            Grid.Disassociate(PSet["4th"], PSet["Dog"]);

            PuzzleGrid expected = Grid.Clone();

            PigeonholeStrategy strategy = new PigeonholeStrategy();
            strategy.Apply(Grid, CSet);

            expected.Associate(PSet["Blue"], PSet["Dog"]);

            Assert.IsTrue(GridMatch(expected));
        }
    }
}
