using LogikGenAPI.Model;
using LogikGenAPI.Utilities;
using System;
using System.Linq;
using System.Text;

namespace LogikGenAPI.Resolution
{
    public class GridPrinter
    {
        public static void PrintGrid(IGrid grid)
        {
            Console.WriteLine(BuildGridString(grid));
        }

        public static void PrintSolutionTable(IGrid grid)
        {
            Console.WriteLine(BuildSolutionTableString(grid));
        }

        public static string BuildGridString(IGrid grid)
        {
            PropertySet pset = grid.PropertySet;
            StringBuilder sb = new StringBuilder();

            int propertyNameLength = pset.Max(p => p.Name.Length);

            sb.Append(' ', propertyNameLength);
            sb.Append('|');

            foreach (Category category in pset.Categories)
            {
                foreach (Property p in category)
                    sb.Append(p.Name[0]);

                sb.Append('|');
            }

            int printedTableWidth = sb.Length;

            sb.AppendLine();
            sb.AppendLine(new string('-', printedTableWidth));

            foreach (Category rowCategory in pset.Categories)
            {
                foreach (Property rowProperty in rowCategory)
                {
                    sb.Append(rowProperty.Name.PadRight(propertyNameLength));
                    sb.Append('|');

                    foreach (Category columnCategory in pset.Categories)
                    {
                        SubsetKey<Property> domain = grid[rowProperty, columnCategory];

                        foreach (Property columnProperty in columnCategory)
                        {
                            if (domain == columnProperty.Singleton)
                                sb.Append('O');
                            else if (domain.ContainsSubset(columnProperty.Singleton))
                                sb.Append(' ');
                            else
                                sb.Append('.');
                        }

                        sb.Append('|');
                    }

                    sb.AppendLine();
                }

                sb.AppendLine(new string('-', printedTableWidth));
            }

            return sb.ToString();
        }

        public static string BuildSolutionTableString(IGrid grid)
        {
            PropertySet pset = grid.PropertySet;
            StringBuilder sb = new StringBuilder();

            int fieldWidth = pset.Max(e => e.Name.Length) + 2;
            int tableWidth = 1 + (fieldWidth + 1) * pset.CategorySize;

            sb.AppendLine(new string('-', tableWidth));

            sb.Append("|");

            foreach (Property headingProperty in pset.Categories[0])
                sb.Append(headingProperty.Name.PadCenter(fieldWidth) + "|");

            sb.AppendLine();

            for (int i = 1; i < pset.Categories.Count; i++)
            {
                sb.Append("|");

                foreach (Property headingProperty in pset.Categories[0])
                {
                    SubsetKey<Property> tableValue = grid[headingProperty, pset.Categories[i]];

                    if (tableValue.Count == 1)
                        sb.Append(tableValue[0].Name.PadCenter(fieldWidth));
                    else
                        sb.Append(new string(' ', fieldWidth));

                    sb.Append("|");
                }

                sb.AppendLine();
            }

            sb.AppendLine(new string('-', tableWidth));

            return sb.ToString();
        }
    }
}
