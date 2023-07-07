using LogikGenAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Examples
{
    public class ZebraPuzzleBuilder
    {
        public const int MaximumTotalCategories = 8;
        public const int MaximumCategorySize = 8;
        public static IReadOnlyList<CategoryDefinition> AvailableCategories { get; private set; }

        static ZebraPuzzleBuilder()
        {
            AvailableCategories = new List<CategoryDefinition>() {
                new CategoryDefinition("Location", true, "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th"),
                new CategoryDefinition("Nationality", false, "Englishman", "Spaniard", "Ukrainian", "Norwegian", "Japanese", "Swede", "Dane", "German"),
                new CategoryDefinition("Color", false, "Red", "Green", "Blue", "Yellow", "White", "Orange", "Purple", "Brown"),
                new CategoryDefinition("Pet", false, "Dog", "Fox", "Zebra", "Snails", "Horse", "Bird", "Fish", "Cat"),
                new CategoryDefinition("Beverage", false, "Water", "Tea", "Coffee", "Milk", "Orange Juice", "Beer", "Wine", "Soda"),
                new CategoryDefinition("Cigar", false, "Kools", "Chesterfield", "Parliament", "Lucky Strike", "Old Gold", "Dunhill", "Prince", "Blue Master"),
                new CategoryDefinition("Age", true, "25", "30", "35", "40", "45", "50", "55", "60"),
                new CategoryDefinition("Game", false, "Checkers", "Chess", "Backgammon", "Go", "Reversi", "Poker", "Blackjack", "Dominoes")
            }.AsReadOnly();
        }

        public static PropertySet MakePropertySet(int totalCategories, int categorySize)
        {
            if (totalCategories < 0 || MaximumTotalCategories < totalCategories)
                throw new ArgumentOutOfRangeException(nameof(totalCategories));

            if (categorySize < 0 || MaximumCategorySize < categorySize)
                throw new ArgumentOutOfRangeException(nameof(categorySize));

            List<CategoryDefinition> definitions = new List<CategoryDefinition>();

            for (int i = 0; i < totalCategories; i++)
            {
                string categoryName = AvailableCategories[i].CategoryName;
                bool isOrdered = AvailableCategories[i].IsOrdered;
                IEnumerable<string> selectedProperties = AvailableCategories[i].PropertyNames.Take(categorySize);

                definitions.Add(new CategoryDefinition(categoryName, isOrdered, selectedProperties));
            }

            return new PropertySet(definitions);
        }
    }
}
