﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Win32;

namespace WindSlicer.Commands.General
{
    public class CalendarCommand : BaseCommand
    {
        private const uint WM_NCLBUTTONDOWN = 0x00A1;
        private const uint WM_NCLBUTTONUP = 0x00A2;
        private const uint HTCAPTION = 2;

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            IntPtr trayhwnd = NativeMethods.FindWindow("Shell_TrayWnd", "");
            IntPtr trayNotify = NativeMethods.FindWindowEx(trayhwnd, IntPtr.Zero, "TrayNotifyWnd", "");
            IntPtr clockHwnd = IntPtr.Zero;
            foreach (IntPtr childHwnd in NativeMethods.GetChildWindows(trayhwnd))
            {
                if (NativeMethods.GetClassName(childHwnd) == "TrayClockWClass")
                {
                    clockHwnd = childHwnd;
                    break;
                }
            }

            if (clockHwnd == IntPtr.Zero)
            {
                return;
            }

            // this doesn't work in win10 for one reason or another ?
            if (!(NativeMethods.GetWindowPosition(clockHwnd) is NativeMethods.RECT rect))
            {
                return;
            }

            IntPtr wParam = new IntPtr(HTCAPTION);
            IntPtr lParam = new IntPtr(rect.Top << 16 | rect.Left);

            NativeMethods.SendMessage(trayhwnd, WM_NCLBUTTONDOWN, wParam, lParam);
            System.Threading.Thread.Sleep(10);
            NativeMethods.SendMessage(trayhwnd, WM_NCLBUTTONUP, wParam, lParam);

            // TODO: figure out why the clock looks like mouse hovers over it
        }
    }
}
