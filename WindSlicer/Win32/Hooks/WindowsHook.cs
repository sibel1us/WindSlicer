using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using WindSlicer.Win32.Handles;

namespace WindSlicer.Win32.Hooks
{
    public abstract class WindowsHook : IDisposable
    {
        protected abstract SafeWinEventHookHandle HookHandle { get; set; }
        protected abstract NativeMethods.WinEventDelegate EventDelegate { get; }

        public abstract void Subscribe();

        protected abstract void WinEventProc(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime);

        public void Dispose()
        {
            this.HookHandle?.Dispose();
        }
    }
}
