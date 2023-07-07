using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Resolution;
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
    public class ImmediateLessThanCheckCompatibilityStrategyTests : PuzzleTestBase
    {
        [TestMethod]
        public void Run()
        {
            /*
             *  LessThan(Spaniard, Blue) & NextTo(Spaniard, Blue)
             *  
             *  Spaniard:   {     2nd 3rd 4th     }
             *  Blue:       {         3rd 4th 5th }
             *  Dog:        {         3rd 4th     }
             * 
             *  I must therefore conclude 
             *  Spaniard:   {     2nd     4th     }
             *  Blue:       {         3rd     5th }
             *  Dog:        {         3rd 4th     }
             * 
             */

            Setup(5, 5);

            CSet.Add(new LessThanConstraint(PSet["Spaniard"], PSet["Blue"], PSet.Category("Location")));
            CSet.Add(new NextToConstraint(PSet["Spaniard"], PSet["Blue"], PSet.Category("Location")));

            Grid.Disassociate(PSet["Spaniard"], PSet["Blue"]);
            Grid.Disassociate(PSet["Spaniard"], PSet["Dog"]);
            Grid.Disassociate(PSet["Blue"], PSet["Dog"]);

            Grid.Update(PSet["Spaniard"], PSet.ToSubset("2nd", "3rd", "4th"));
            Grid.Update(PSet["Blue"], PSet.ToSubset("3rd", "4th", "5th"));
            Grid.Update(PSet["Dog"], PSet.ToSubset("3rd", "4th"));

            PuzzleGrid expected = Grid.Clone();
            expected.Disassociate(PSet["Spaniard"], PSet["3rd"]);
            expected.Disassociate(PSet["Blue"], PSet["4th"]);

            Strategy strategy = new ImmediateLessThanCompatibilityCheckStrategy(false);
            strategy.Apply(Grid, CSet);

            Assert.IsTrue(GridMatch(expected));
        }
    }
}
