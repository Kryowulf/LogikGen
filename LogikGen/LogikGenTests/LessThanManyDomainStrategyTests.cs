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
    public class LessThanManyDomainStrategyTests : PuzzleTestBase
    {
        private Strategy Strategy { get; set; }

        [TestInitialize]
        public void Init()
        {
            Setup(3, 5);
            this.Strategy = new LessThanManyDomainStrategy(IndirectionLevel.IndirectBoth);
        }

        [TestMethod]
        public void TestLessThanMany()
        {
            Category loc = PSet.Category("Location");

            CSet.Add(new LessThanConstraint(PSet["Englishman"], PSet["Red"], loc));
            CSet.Add(new LessThanConstraint(PSet["Englishman"], PSet["Green"], loc));
            CSet.Add(new LessThanConstraint(PSet["Englishman"], PSet["Blue"], loc));

            PuzzleGrid expected = Grid.Clone();
            expected.Update(PSet["Englishman"], PSet.ToSubset("1st", "2nd"));
            expected.Disassociate(PSet["Red"], PSet["1st"]);
            expected.Disassociate(PSet["Green"], PSet["1st"]);
            expected.Disassociate(PSet["Blue"], PSet["1st"]);

            Strategy.Apply(Grid, CSet);

            Assert.IsTrue(GridMatch(expected));
        }

        [TestMethod]
        public void TestGreaterThanMany()
        {
            Category loc = PSet.Category("Location");

            CSet.Add(new LessThanConstraint(PSet["Red"], PSet["Englishman"], loc));
            CSet.Add(new LessThanConstraint(PSet["Green"], PSet["Englishman"], loc));
            CSet.Add(new LessThanConstraint(PSet["Blue"], PSet["Englishman"], loc));

            PuzzleGrid expected = Grid.Clone();
            expected.Update(PSet["Englishman"], PSet.ToSubset("4th", "5th"));
            expected.Disassociate(PSet["Red"], PSet["5th"]);
            expected.Disassociate(PSet["Green"], PSet["5th"]);
            expected.Disassociate(PSet["Blue"], PSet["5th"]);

            Strategy.Apply(Grid, CSet);

            Assert.IsTrue(GridMatch(expected));
        }
    }
}
