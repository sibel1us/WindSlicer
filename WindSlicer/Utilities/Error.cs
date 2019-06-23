using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace WindSlicer.Utilities
{
    public static class Error
    {
        public static bool ThrowOnError = false;

        private static readonly string def = "Unspecified error";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Win32(
            string message = null,
            [CallerMemberName]string caller = null)
        {
            Raise(new Exception($"{caller}: {message ?? def}", WindSlicer.Win32.NativeApi.LastError));
        }

        public static void InvalidOp(
            string message = null,
            [CallerMemberName]string caller = null)
        {
            Raise(new InvalidOperationException($"{caller}: {message ?? def}"));
        }

        public static void OutOfRange(
            string message = null,
            [CallerMemberName]string caller = null)
        {
            Raise(new IndexOutOfRangeException($"{caller}: {message ?? def}"));
        }

        private static void Raise(Exception e)
        {
            if (ThrowOnError)
            {
                throw e;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}
