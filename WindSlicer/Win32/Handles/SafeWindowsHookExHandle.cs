using System;
using Microsoft.Win32.SafeHandles;

namespace WindSlicer.Win32.Handles
{
    /// <summary>
    /// Provides a finalizer that performs <see
    /// cref="NativeMethods.UnhookWindowsHookEx(IntPtr)(IntPtr)"/> to the handle.
    /// </summary>
    public class SafeWindowsHookExHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeWindowsHookExHandle(IntPtr eventHandle) : base(true)
        {
            this.SetHandle(eventHandle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.UnhookWindowsHookEx(this.handle);
        }
    }
}
