using System;
using Microsoft.Win32.SafeHandles;

namespace WindSlicer.Win32.Handles
{
    /// <summary>
    /// Provides a finalizer that performs <see cref="NativeMethods.UnhookWinEvent(IntPtr)"/> to the
    /// handle.
    /// </summary>
    public class SafeWinEventHookHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeWinEventHookHandle(IntPtr hookHandle) : base(true)
        {
            this.SetHandle(hookHandle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.UnhookWinEvent(this.handle);
        }
    }
}
