﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Utilities.Extensions;
using WindSlicer.Win32;

namespace WindSlicer.Commands.General
{
    /// <summary>
    /// Sends a click event to the calendar to show it. Does not work on Windows 10.
    /// </summary>
    /// <remarks>
    /// Windows 10 limits messages sent to system processes, so this command does not work without
    /// admin privileges.
    /// </remarks>
    [Obsolete("Does not work on Windows 10.", error: false)]
    public class CalendarCommand : BaseCommand
    {
        private const uint WM_NCLBUTTONDOWN = 0x00A1;
        private const uint WM_NCLBUTTONUP = 0x00A2;
        private const uint HTCAPTION = 2;

        private const string TRAY_SHELL = "Shell_TrayWnd";
        private const string TRAY_NOTIFYWND = "TrayNotifyWnd";
        private const string TRAY_CLOCK = "TrayClockWClass";

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            IntPtr trayhwnd = NativeMethods.FindWindow(TRAY_SHELL, "");
            //IntPtr trayNotify = NativeMethods.FindWindowEx(trayhwnd, IntPtr.Zero, TRAY_NOTIFYWND, "");
            IntPtr clockHwnd = IntPtr.Zero;
            foreach (IntPtr childHwnd in NativeApi.GetChildWindows(trayhwnd))
            {
                if (NativeApi.GetClassName(childHwnd) == TRAY_CLOCK)
                {
                    clockHwnd = childHwnd;
                    break;
                }
            }

            if (clockHwnd == IntPtr.Zero)
                return;

            if (!(NativeApi.GetWindowPosition(clockHwnd) is NativeMethods.RECT rect))
                return;

            IntPtr wParam = new IntPtr(HTCAPTION);
            IntPtr lParam = new IntPtr(rect.Top << 16 | rect.Left);

            NativeMethods.SendMessage(trayhwnd, WM_NCLBUTTONDOWN, wParam, lParam);
            System.Threading.Thread.Sleep(10);
            NativeMethods.SendMessage(trayhwnd, WM_NCLBUTTONUP, wParam, lParam);

            // TODO: figure out why the clock looks like mouse hovers over it
        }
    }
}
