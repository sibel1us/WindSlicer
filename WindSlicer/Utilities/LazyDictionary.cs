using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindSlicer.Utilities
{
    /// <summary>
    /// Provides a dictionary that loads dictionary values from predetermined keys using lazy
    /// loading.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <remarks>
    /// This class is a little unnecessary, it is mainly written as a mental excercise.
    /// </remarks>
    public class LazyDictionary<K, V> : IReadOnlyDictionary<K, V>
    {
        /// <summary>
        /// Inner dictionary containing the keys and lazy constructors.
        /// </summary>
        private readonly IReadOnlyDictionary<K, Lazy<V>> inner;

        public V this[K key] => this.inner[key].Value;

        public IEnumerable<K> Keys => this.inner.Keys;

        /// <summary>
        /// Gets an enumerable collection that contains the values of dictionary. Values are
        /// initialized when accessed.
        /// </summary>
        public IEnumerable<V> Values => this.inner.Values.Select(x => x.Value);

        public int Count => this.inner.Count;

        /// <summary>
        /// Initialize a new lazy dictionary with thread safe value initialization.
        /// </summary>
        /// <param name="keys">Keys to seed the dictionary with</param>
        /// <param name="valueFactory">
        /// Value factory method passed into the inner <see cref="Lazy{T}"/> values.
        /// </param>
        public LazyDictionary(IEnumerable<K> keys, Func<K, V> valueFactory)
            : this(keys, valueFactory, LazyThreadSafetyMode.ExecutionAndPublication)
        { }

        /// <summary>
        /// Initialize a new lazy loaded diictionary using the specified thread safety option for
        /// value initialization.
        /// </summary>
        /// <param name="keys">Keys to seed the dictionary with</param>
        /// <param name="valueFactory">
        /// Value factory method passed into the inner <see cref="Lazy{T}"/> values.
        /// </param>
        /// <param name="isThreadSafe">
        /// Thread safety option passed into the inner <see cref="Lazy{T}"/> values.
        /// </param>
        public LazyDictionary(IEnumerable<K> keys, Func<K, V> valueFactory, bool isThreadSafe)
            : this(keys, valueFactory, isThreadSafe
                 ? LazyThreadSafetyMode.ExecutionAndPublication
                 : LazyThreadSafetyMode.None)
        { }

        /// <summary>
        /// Initialize a new lazy loaded diictionary using the specified thread safety mode for value
        /// initialization.
        /// </summary>
        /// <param name="keys">Keys to seed the dictionary with</param>
        /// <param name="valueFactory">
        /// Value factory method passed into the inner <see cref="Lazy{T}"/> values.
        /// </param>
        /// <param name="mode">
        /// Thread safety mode passed into the inner <see cref="Lazy{T}"/> values.
        /// </param>
        public LazyDictionary(
            IEnumerable<K> keys,
            Func<K, V> valueFactory,
            LazyThreadSafetyMode mode)
        {
            this.inner = new ReadOnlyDictionary<K, Lazy<V>>(keys
                .ToDictionary(x => x, x => new Lazy<V>(() => valueFactory(x), mode)));
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(K key)
        {
            return this.inner.ContainsKey(key);
        }

        /// <summary>
        /// Gets an enumerator that iterates through the dictionary and initializes the values as
        /// needed.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            foreach (var kvp in this.inner)
            {
                yield return new KeyValuePair<K, V>(kvp.Key, kvp.Value.Value);
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key, initializing it if needed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(K key, out V value)
        {
            if (this.inner.TryGetValue(key, out Lazy<V> lazy))
            {
                value = lazy.Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
