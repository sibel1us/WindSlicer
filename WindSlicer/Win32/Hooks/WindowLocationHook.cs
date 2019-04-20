using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Win32.Hooks
{
    public class WindowLocationHook : WindowsHook
    {
        public event EventHandler<WindowLocationChangedEventArgs> WindowLocationChanged;

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

        protected override IntPtr HWinEventHook { get; set; }
        protected override NativeMethods.WinEventDelegate EventDelegate { get; }

        public WindowLocationHook()
        {
            this.EventDelegate = new NativeMethods.WinEventDelegate(this.WinEventProc);
        }

        public override void Subscribe()
        {
            if (this.HWinEventHook != IntPtr.Zero)
                throw new InvalidOperationException("Already subscribed");

            this.HWinEventHook = NativeMethods.SetWinEventHook(
                EVENT_OBJECT_LOCATIONCHANGE,
                EVENT_OBJECT_LOCATIONCHANGE,
                IntPtr.Zero,
                this.EventDelegate,
                0,
                0,
                WINEVENT_OUTOFCONTEXT);

            if (IntPtr.Zero == this.HWinEventHook)
                throw NativeApi.LastError;
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
            if (hwnd == IntPtr.Zero || eventType != EVENT_OBJECT_LOCATIONCHANGE)
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
