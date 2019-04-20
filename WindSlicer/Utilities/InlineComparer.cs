using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Utilities
{
    public class InlineComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _compareFunc;
        private readonly Func<T, int> _hashcodeFunc;

        public InlineComparer(Func<T, T, bool> compareFunc)
            : this(compareFunc, obj => obj?.GetHashCode() ?? 0) { }

        public InlineComparer(Func<T, T, bool> compareFunc, Func<T,int> hashcodeFunc)
        {
            _compareFunc = compareFunc;
            _hashcodeFunc = hashcodeFunc;
        }

        public bool Equals(T x, T y) => _compareFunc(x, y);
        public int GetHashCode(T obj) => _hashcodeFunc(obj);
    }
}
