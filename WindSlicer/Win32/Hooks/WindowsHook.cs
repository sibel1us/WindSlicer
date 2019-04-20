using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Win32.Hooks
{
    // TODO: check if IGCHandle needed
    public abstract class WindowsHook : IDisposable
    {
        protected abstract IntPtr HWinEventHook { get; set; }

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

        public abstract void Dispose();
    }
}
