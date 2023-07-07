using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using System.Collections.Generic;

namespace LogikGenAPI.Examples
{
    public class Zebra
    {
        public PropertySet PropertySet { get; private set; }
        public IReadOnlyList<Constraint> Constraints { get; private set; }

        public Zebra()
        {
            PropertySet pset = new PropertySet(
                new CategoryDefinition("Location", true, "First", "Second", "Third", "Fourth", "Fifth"),
                new CategoryDefinition("Nationality", false, "Englishman", "Spaniard", "Ukrainian", "Norwegian", "Japanese"),
                new CategoryDefinition("Color", false, "Red", "Green", "Ivory", "Yellow", "Blue"),
                new CategoryDefinition("Pet", false, "Dog", "Snails", "Fox", "Horse", "Zebra"),
                new CategoryDefinition("Cigar", false, "Old Gold", "Kools", "Chesterfields", "Lucky Strike", "Parliaments"),
                new CategoryDefinition("Beverage", false, "Coffee", "Tea", "Milk", "Orange Juice", "Water")
            );

            this.PropertySet = pset;

            this.Constraints = new List<Constraint>()
            {
                new EqualConstraint(pset["Englishman"], pset["Red"]),
                new EqualConstraint(pset["Spaniard"], pset["Dog"]),
                new EqualConstraint(pset["Coffee"], pset["Green"]),
                new EqualConstraint(pset["Ukrainian"], pset["Tea"]),
                new LessThanConstraint(pset["Ivory"], pset["Green"], pset.Category("Location")),
                new NextToConstraint(pset["Ivory"], pset["Green"], pset.Category("Location")),
                new EqualConstraint(pset["Old Gold"], pset["Snails"]),
                new EqualConstraint(pset["Kools"], pset["Yellow"]),
                new EqualConstraint(pset["Milk"], pset["Third"]),
                new EqualConstraint(pset["Norwegian"], pset["First"]),
                new NextToConstraint(pset["Chesterfields"], pset["Fox"], pset.Category("Location")),
                new NextToConstraint(pset["Kools"], pset["Horse"], pset.Category("Location")),
                new EqualConstraint(pset["Lucky Strike"], pset["Orange Juice"]),
                new EqualConstraint(pset["Japanese"], pset["Parliaments"]),
                new NextToConstraint(pset["Norwegian"], pset["Blue"], pset.Category("Location"))
            };
        }
    }
}
