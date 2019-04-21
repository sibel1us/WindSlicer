using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Utilities;

namespace WindSlicer.Win32.Hooks
{
    public sealed class WindowLocationHook : WindowsHook
    {
        public event EventHandler<WindowLocationChangedEventArgs> WindowLocationChanged;

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

        public IntPtr HWnd { get; }
        protected override IntPtr HWinEventHook { get; set; }
        protected override NativeMethods.WinEventDelegate EventDelegate { get; }

        public WindowLocationHook(IntPtr hwnd)
        {
            this.HWnd = hwnd;
            this.EventDelegate = new NativeMethods.WinEventDelegate(this.WinEventProc);
        }

        public override void Subscribe()
        {
            if (this.HWinEventHook != IntPtr.Zero)
                Error.InvalidOp("Already subscribed");

            int threadId = NativeMethods.GetWindowThreadProcessId(this.HWnd, out int processId);

            if (processId == default)
                Error.Win32("Could not retrieve process Id");

            if (threadId == default)
                Error.Win32("Could not retrieve thread Id");

            this.HWinEventHook = NativeMethods.SetWinEventHook(
                EVENT_OBJECT_LOCATIONCHANGE,
                EVENT_OBJECT_LOCATIONCHANGE,
                IntPtr.Zero,
                this.EventDelegate,
                (uint)processId,
                (uint)threadId,
                WINEVENT_OUTOFCONTEXT);

            if (IntPtr.Zero == this.HWinEventHook)
                Error.Win32($"SetWinEventHook failed");
        }

        protected override void WinEventProc(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime)
        {
            if (hwnd == IntPtr.Zero)
                return;

            WindowLocationChanged?.Invoke(null, new WindowLocationChangedEventArgs(hwnd));
        }

        public override void Dispose()
        {
            if (this.HWinEventHook != IntPtr.Zero)
            {
                NativeMethods.UnhookWinEvent(this.HWinEventHook);
            }
        }
    }

    public class WindowLocationChangedEventArgs
    {
        public IntPtr HWnd { get; }

        public WindowLocationChangedEventArgs(IntPtr hwnd)
        {
            this.HWnd = hwnd;
        }
    }
}
