using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Utilities.Extensions
{
    public static class CollectionExtensions
    {
        public static bool None<T>(this IEnumerable<T> @this)
        {
            if (@this is null)
                throw new ArgumentNullException(nameof(@this));

            return !@this.Any();
        }

        public static bool None<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        {
            if (@this is null)
                throw new ArgumentNullException(nameof(@this));

            return !@this.Any(predicate);
        }

        public static int RemoveAll<K, V>(this IDictionary<K, V> @this)
        {
            int removed = 0;

            for (int i = @this.Keys.Count - 1; i >= 0; i--)
            {
                if (@this.Remove(@this.Keys.ElementAt(i)))
                {
                    removed++;
                }
            }

            return removed;
        }

        public static void AddRange<K, V>(this IDictionary<K, V> @this, IDictionary<K, V> other)
        {
            foreach (var kvp in other)
            {
                @this.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
