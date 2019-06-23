using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using WindSlicer.Utilities;
using WindSlicer.Win32.Handles;

namespace WindSlicer.Win32.Hooks
{
    public class DragWindowHook : WindowsHook
    {
        public event EventHandler<WindowDraggedEventArgs> WindowDragged;

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        private const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

        protected override SafeWinEventHookHandle HookHandle { get; set; }
        protected override NativeMethods.WinEventDelegate EventDelegate { get; }

        public DragWindowHook()
        {
            this.EventDelegate = new NativeMethods.WinEventDelegate(this.WinEventProc);
        }

        public override void Subscribe()
        {
            if (this.HookHandle != null)
            {
                Error.InvalidOp("Already subscribed.");
                return;
            }

            IntPtr handle = NativeMethods.SetWinEventHook(
                EVENT_SYSTEM_MOVESIZESTART,
                EVENT_SYSTEM_MOVESIZEEND,
                IntPtr.Zero,
                this.EventDelegate,
                0,
                0,
                WINEVENT_OUTOFCONTEXT);

            this.HookHandle = new SafeWinEventHookHandle(handle);

            if (this.HookHandle.IsInvalid)
            {
                Error.Win32("SetWinEventHook failed");
            }
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

            switch (eventType)
            {
                case EVENT_SYSTEM_MOVESIZESTART:
                    WindowDragged?.Invoke(null, new WindowDraggedEventArgs(hwnd, true));
                    break;
                case EVENT_SYSTEM_MOVESIZEEND:
                    WindowDragged?.Invoke(null, new WindowDraggedEventArgs(hwnd, false));
                    break;
                default:
                    break;
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
