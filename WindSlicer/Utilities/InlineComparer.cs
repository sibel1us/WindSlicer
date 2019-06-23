using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Utilities
{
    public class InlineComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> compareFunc;
        private readonly Func<T, int> hashcodeFunc;

        public InlineComparer(Func<T, T, bool> compareFunc, Func<T,int> hashcodeFunc)
        {
            this.compareFunc = compareFunc
                ?? throw new ArgumentNullException(nameof(compareFunc));

            this.hashcodeFunc = hashcodeFunc
                ?? throw new ArgumentNullException(nameof(hashcodeFunc));
        }

        public InlineComparer(Func<T, T, bool> compareFunc)
            : this(compareFunc, obj => obj?.GetHashCode() ?? 0)
        { }

        public bool Equals(T x, T y) => this.compareFunc(x, y);
        public int GetHashCode(T obj) => this.hashcodeFunc(obj);
    }
}
