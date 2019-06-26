using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Utilities
{
    /// <summary>
    /// General container for helpers that aren't specific enough to warrant their own utility
    /// classes.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Get all defined values of <typeparamref name="T"/> using <see
        /// cref="Enum.GetValues(Type)"/>.
        /// <para>
        /// Note that the underlying value of the enum can have multiple definitions, so use <see
        /// cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})"/> if needed.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ValuesOf<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static double? ParseDouble(object value)
        {
            var style = NumberStyles.Float;
            var provider = CultureInfo.InvariantCulture;

            switch (value)
            {
                case int i:
                    return i;
                case double d:
                    return d;
                case string s
                when double.TryParse(s, style, provider, out double dOut):
                    return dOut;
                default:
                    return null;
            }
        }
    }
}
