﻿using System;
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
            return !@this.Any();
        }

        public static bool None<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        {
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

        public static void AddRange<K,V>(
            this IDictionary<K,V> @this,
            IEnumerable<KeyValuePair<K,V>> keyValuePairs)
        {
            foreach (var item in keyValuePairs)
            {
                @this.Add(item);
            }
        }

        /// <summary>
        /// Attempts to retrieve the first item in the collection based on a predicate.
        /// </summary>
        /// <remarks>
        /// Use with inline parameters to reduce visual clutter and null checks that result from
        /// using FirstOfDefault.
        /// </remarks>
        public static bool TryGetFirst<T>(
            this IEnumerable<T> @this,
            Func<T, bool> predicate,
            out T value)
        {
            foreach (var item in @this.Where(predicate))
            {
                value = item;
                return true;
            }

            value = default;
            return false;
        }
    }
}
