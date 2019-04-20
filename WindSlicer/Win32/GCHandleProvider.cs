using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Win32
{
    /// <summary>
    /// Use to get IntPtr from a managed object.
    /// </summary>
    /// <remarks>
    /// Source: https://stackoverflow.com/a/52103996/
    /// </remarks>
    public class GCHandleProvider : IDisposable
    {
        public GCHandleProvider(object target)
        {
            this.Handle = GCHandle.Alloc(target);
            this.Pointer = GCHandle.ToIntPtr(this.Handle);
        }

        public IntPtr Pointer { get; }
        public GCHandle Handle { get; }

        private void ReleaseUnmanagedResources()
        {
            if (this.Handle.IsAllocated)
            {
                this.Handle.Free();
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~GCHandleProvider()
        {
            ReleaseUnmanagedResources();
        }
    }

    // example:
    /*
        using (var handleProvider = new GCHandleProvider(myList))
        {
            var b = EnumChildWindows(hwndParent, CallBack, handleProvider.Pointer);
        }
    */
}
