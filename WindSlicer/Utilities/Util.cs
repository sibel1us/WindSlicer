using System;
using System.Collections.Generic;
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
        /// Get possible values of <typeparamref name="T"/> using <see cref="Enum.GetValues(Type)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ValuesOf<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
