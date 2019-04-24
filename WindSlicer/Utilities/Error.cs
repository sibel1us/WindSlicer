using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Utilities
{
    public static class Error
    {
        private static readonly string def = "Unspecified error";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Win32(string message = null)
        {
            throw new Exception(message ?? def, WindSlicer.Win32.NativeApi.LastError);
        }

        public static void InvalidOp(string message = null)
        {
            throw new InvalidOperationException(message ?? def);
        }

        public static void OutOfRange(string message = null)
        {
            throw new IndexOutOfRangeException(message ?? def);
        }
    }
}
