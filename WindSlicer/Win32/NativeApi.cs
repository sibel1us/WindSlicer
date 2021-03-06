﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static WindSlicer.Win32.NativeMethods;

namespace WindSlicer.Win32
{
    /// <summary>
    /// Contains wrappers and utilities for <see cref="NativeMethods"/>.
    /// </summary>
    public static class NativeApi
    {
        /// <summary>
        /// Returns an exception constructed using <see cref="Marshal.GetLastWin32Error"/>.
        /// </summary>
        public static Win32Exception LastError => new Win32Exception(Marshal.GetLastWin32Error());

        /// <summary>
        /// Restore window if it is iconic and show it. Equivalent to calling <see
        /// cref="ShowWindow(IntPtr, int)"/> first with <see cref="ShowWindowCommands.Restore"/> and
        /// then with <see cref="ShowWindowCommands.Show"/>.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static bool RestoreAndShowWindow(IntPtr hWnd)
        {
            if (IsIconic(hWnd))
            {
                ShowWindow(hWnd, (int)ShowWindowCommands.Restore);
            }

            return ShowWindow(hWnd, (int)ShowWindowCommands.Show);
        }

        /// <summary>
        /// Resize window.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <param name="alignment">Anchor point of the window when resizing.</param>
        /// <returns></returns>
        public static bool ClientResize(
            IntPtr hWnd,
            int newWidth,
            int newHeight,
            ContentAlignment _ = ContentAlignment.TopLeft)
        {
            if (!GetClientRect(hWnd, out RECT clientRect))
                return false;

            if (!GetWindowRect(hWnd, out RECT windowRect))
                return false;

            var diff = new Point(
                windowRect.Right - windowRect.Left - clientRect.Right,
                windowRect.Bottom - windowRect.Top - clientRect.Bottom);

            // TODO: calculate according to alignment property
            return MoveWindow(
                hWnd,
                windowRect.Left,
                windowRect.Top,
                newWidth + diff.X,
                newHeight + diff.Y,
                true);
        }

        public static string GetClassName(IntPtr hWnd)
        {
            var sb = new StringBuilder(256);
            NativeMethods.GetClassName(hWnd, sb, 256);
            return sb.ToString();
        }

        public static RECT? GetWindowPosition(IntPtr hWnd)
        {
            if (GetWindowRect(hWnd, out RECT rect))
                return rect;

            return null;
        }

        public static RECT? GetClientSize(IntPtr hWnd)
        {
            if (GetClientRect(hWnd, out RECT rect))
                return rect;

            return null;
        }

        public static RECT? GetClientPosition(IntPtr hWnd)
        {
            if (GetClientSize(hWnd) is RECT rect)
            {
                var point = Point.Empty;

                if (ClientToScreen(hWnd, ref point))
                {
                    return new RECT
                    {
                        Left = rect.Left + point.X,
                        Right = rect.Right + point.X,
                        Top = rect.Top + point.Y,
                        Bottom = rect.Bottom + point.Y
                    };
                }
            }

            return null;
        }

        public static RECT? GetWin10Bounds(IntPtr hWnd)
        {
            int retVal = DwmGetWindowAttribute(
                hWnd,
                DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS,
                out RECT outRect,
                Marshal.SizeOf<RECT>());

            if (retVal == 0)
                return outRect;

            return null;
        }

        /// <summary>
        /// Get windows that are child windows of <paramref name="parentHwnd"/>.
        /// </summary>
        /// <param name="parentHwnd">Handle for the window to get child windows from</param>
        /// <returns></returns>
        public static List<IntPtr> GetChildWindows(IntPtr parentHwnd)
        {
            var result = new List<IntPtr>();

            EnumChildWindows(parentHwnd, delegate (IntPtr childHwnd, int _)
            {
                result.Add(childHwnd);
                return true;
            },
            IntPtr.Zero);

            return result;
        }

        /// <summary>
        /// Get a dictionary of all open window handles and window titles.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<IntPtr, string> GetOpenWindowNames()
        {
            IntPtr shellhWnd = GetShellWindow();
            var windows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellhWnd || !IsWindowVisible(hWnd))
                    return true;

                int length = GetWindowTextLength(hWnd);

                if (length == 0)
                    return true;

                var builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;
            },
            0);

            return windows;
        }

        public static List<IntPtr> GetOpenWindowHandles()
        {
            IntPtr shellhWnd = GetShellWindow();
            var handles = new List<IntPtr>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd != shellhWnd &&
                    IsWindowVisible(hWnd) &&
                    GetWindowTextLength(hWnd) != 0)
                {
                    handles.Add(hWnd);
                }

                return true;
            },
            0);

            return handles;
        }

        public static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);

            if (length == 0)
                return null;

            var sb = new StringBuilder(length);

            if (GetWindowText(hWnd, sb, length + 1) != 0)
                return sb.ToString();

            return null;
        }

        /// <summary>
        /// Get the process id from a window handle.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public static int GetWindowProcessId(IntPtr hwnd)
        {
            GetWindowThreadProcessId(hwnd, out int processId);
            return processId;
        }

        /// <summary>
        /// Get full process name from a process handle.
        /// </summary>
        /// <param name="hproc"></param>
        /// <returns></returns>
        public static string GetFullProcessName(IntPtr hproc)
        {
            int capacity = 1024;
            var sb = new StringBuilder(capacity);

            if (QueryFullProcessImageName(hproc, 0, sb, ref capacity))
                return sb.ToString(0, capacity);

            return null;
        }

        /// <summary>
        /// Returns whether the specified key is pressed down, returning false on failure.
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        public static bool IsKeyPushedDown(System.Windows.Forms.Keys vKey)
        {
            return 0 != (GetAsyncKeyState(vKey) & 0x8000);
        }

        /// <summary>
        /// Returns a list of windows that run in threads of the process.
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static List<IntPtr> GetProcessWindows(Process process)
        {
            var handles = new List<IntPtr>();

            foreach (var procThread in process.Threads.OfType<ProcessThread>())
            {
                EnumThreadWindows(procThread.Id, (hwnd, lParam) =>
                {
                    handles.Add(hwnd);
                    return true;
                },
                IntPtr.Zero);
            }

            return handles;
        }
    }
}
