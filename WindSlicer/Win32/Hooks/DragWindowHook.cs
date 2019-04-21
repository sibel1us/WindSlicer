using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Utilities;

namespace WindSlicer.Win32.Hooks
{
    public class DragWindowHook : WindowsHook
    {
        public event EventHandler<WindowDraggedEventArgs> WindowDragged;

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        private const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

        protected override IntPtr HWinEventHook { get; set; }
        protected override NativeMethods.WinEventDelegate EventDelegate { get; }

        public DragWindowHook()
        {
            this.EventDelegate = new NativeMethods.WinEventDelegate(this.WinEventProc);
        }

        public override void Subscribe()
        {
            if (this.HWinEventHook != IntPtr.Zero)
                Error.InvalidOp("Already subscribed.");

            this.HWinEventHook = NativeMethods.SetWinEventHook(
                EVENT_SYSTEM_MOVESIZESTART,
                EVENT_SYSTEM_MOVESIZEEND,
                IntPtr.Zero,
                this.EventDelegate,
                0,
                0,
                WINEVENT_OUTOFCONTEXT);

            if (this.HWinEventHook == IntPtr.Zero)
                Error.Win32("SetWinEventHook failed");
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

            if (eventType == EVENT_SYSTEM_MOVESIZESTART)
            {
                WindowDragged?.Invoke(null, new WindowDraggedEventArgs(hwnd, true));
            }
            else if (eventType == EVENT_SYSTEM_MOVESIZEEND)
            {
                WindowDragged?.Invoke(null, new WindowDraggedEventArgs(hwnd, false));
            }
        }

        public override void Dispose()
        {
            if (this.HWinEventHook != IntPtr.Zero)
            {
                NativeMethods.UnhookWinEvent(this.HWinEventHook);
            }
        }
    }

    public class WindowDraggedEventArgs : EventArgs
    {
        /// <summary>
        /// Handle of the window being dragged.
        /// </summary>
        public IntPtr HWnd { get; }

        /// <summary>
        /// Whether the drag ended or started.
        /// </summary>
        public bool DragStarted { get; }

        public WindowDraggedEventArgs(IntPtr hwnd, bool dragStarted)
        {
            this.HWnd = hwnd;
            this.DragStarted = dragStarted;
        }
    }
}
