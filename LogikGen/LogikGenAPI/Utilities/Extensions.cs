using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace LogikGenAPI.Utilities
{
    public static class Extensions
    {
        public static T Select<T>(this Random random, IReadOnlyList<T> list)
        {
            int index = random.Next(list.Count);
            return list[index];
        }

        public static void Shuffle<T>(this Random random, IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);

                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public static string PadCenter(this string value, int totalWidth)
        {
            return value.PadCenter(totalWidth, ' ');
        }
       
        public static string PadCenter(this string value, int totalWidth, char paddingChar)
        {
            return value.PadLeft((totalWidth - value.Length) / 2 + value.Length, paddingChar)
                        .PadRight(totalWidth, paddingChar);
        }

        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> where)
        {
            int index = 0;

            foreach (T item in list)
            {
                if (where(item))
                    return index;

                index++;
            }

            return -1;
        }

        public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (T item in items)
                collection.Add(item);
        }

        public static void AddAll<T>(this ICollection<T> collection, params T[] items)
        {
            AddAll(collection, (IEnumerable<T>)items);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, params T[] items)
        {
            return collection.Except((IEnumerable<T>)items);
        }

        public static string GetDescriptionAttribute(this Enum value)
        {
            FieldInfo info = value.GetType().GetField(value.ToString());
            IEnumerable<DescriptionAttribute> attributes = info.GetCustomAttributes<DescriptionAttribute>();
            return attributes.Any() ? attributes.First().Description : string.Empty;
        }
    }
}
