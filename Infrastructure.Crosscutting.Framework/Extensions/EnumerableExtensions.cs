using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Crosscutting.Framework.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> IntersectNonEmpty<T>(this IEnumerable<IEnumerable<T>> listOfLists)
        {
            var nonEmptyLists = listOfLists.Where(l => l.Any());

            var intersection = nonEmptyLists.Aggregate((l1, l2) => l1.Intersect(l2));

            return intersection;
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            IList<T> list = (source as IList<T>) ?? source.ToList();

            count = Math.Min(count, list.Count);

            for (int i = list.Count - count; i < list.Count; i++)
            {
                yield return list[i];
            }
        }

        public static bool In<T>(this T source, params T[] values)
        {
            return values.Contains(source);
        }

        public static List<T> ExtendedToList<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                return source.ToList();
            }
            else return null;
        }

        public static T[] ExtendedToArray<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                return source.ToArray();
            }
            else return null;
        }
    }
}
