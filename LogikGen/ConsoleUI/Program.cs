using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using System.Collections.Generic;
using System.Linq;
using System;
using LogikGenAPI.Resolution.Strategies;
using LogikGenAPI.Resolution;
using System.Diagnostics;
using LogikGenAPI.Utilities;
using LogikGenAPI.Generation.Patterns;
using LogikGenAPI.Generation;
using LogikGenAPI.Examples;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            PropertySet pset = ZebraPuzzleBuilder.MakePropertySet(4, 4);
            PuzzleSolver solver = new PuzzleSolver(pset);
            solver.AddConstraints(
                new LessThanConstraint(pset["Blue"], pset["Norwegian"], pset.Category("Location")),
                new NextToConstraint(pset["Spaniard"], pset["Red"], pset.Category("Location")),
                new IdentityConstraint(pset["Red"], pset["2nd"], pset["Zebra"], pset["Norwegian"]),
                new EitherOrConstraint(pset["Englishman"], pset["Fox"], pset["Red"]),
                new NextToConstraint(pset["Norwegian"], pset["Blue"], pset.Category("Location")),
                new LessThanConstraint(pset["Green"], pset["Snails"], pset.Category("Location")));

            Console.WriteLine(String.Join('\n', solver.Explain(true)));
                
            Console.WriteLine("Press enter to quit.");
            Console.ReadKey();
        }
    }
}
