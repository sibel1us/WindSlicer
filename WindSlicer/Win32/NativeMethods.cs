using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace WindSlicer.Win32
{
    /// <summary>
    /// Contains platform invoke methods and wrappers utilizing them.
    /// </summary>
    public static partial class NativeMethods
    {
        #region Delegates

        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);
        public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr LowLevelMouseProc(int code, WM wParam, [In] MSLLHOOKSTRUCT lParam);

        #endregion Delegates

        #region Structs

        [DebuggerDisplay("RECT {Left},{Top},{Right},{Bottom}")]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public Point pt;
            public int mouseData;
            public int flags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        #endregion Structs

        #region Public Helpers

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

        public static bool ClientResize(IntPtr hWnd, int newWidth, int newHeight)
        {
            bool gotClientRect = GetClientRect(hWnd, out RECT clientRect);
            bool gotWindowRect = GetWindowRect(hWnd, out RECT windowRect);

            if (!gotClientRect || !gotWindowRect)
                return false;

            var diff = new Point(
                windowRect.Right - windowRect.Left - clientRect.Right,
                windowRect.Bottom - windowRect.Top - clientRect.Bottom);

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
            GetClassName(hWnd, sb, 256);
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
            // TODO: check error code
            DwmGetWindowAttribute(
                hWnd,
                DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS,
                out RECT outVal,
                Marshal.SizeOf(typeof(RECT)));

            if (outVal is RECT rect)
            {
                return rect;
            }

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

            using (var gchProvider = new GCHandleProvider(result))
            {
                EnumChildWindows(parentHwnd, delegate (IntPtr childHwnd, int pointer)
                {
                    var gcHandle = GCHandle.FromIntPtr(new IntPtr(pointer));

                    if (gcHandle.Target is List<IntPtr> list)
                    {
                        list.Add(childHwnd);
                        return true;
                    }
                    else
                    {
                        throw new InvalidCastException(
                            "GCHandle Target could not be cast as List<IntPtr>");
                    }

                },
                gchProvider.Pointer);
            }

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
                if (hWnd == shellhWnd)
                    return true;
                if (!IsWindowVisible(hWnd))
                    return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                    return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;
            }, 0);

            return windows;
        }

        public static List<IntPtr> GetOpenWindowHandles()
        {
            IntPtr shellhWnd = GetShellWindow();
            var handles = new List<IntPtr>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellhWnd)
                    return true;
                if (!IsWindowVisible(hWnd))
                    return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                    return true;

                handles.Add(hWnd);
                return true;
            }, 0);

            return handles;
        }

        public static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);

            if (length == 0)
                return null;

            var sb = new StringBuilder(length);

            return (GetWindowText(hWnd, sb, length + 1) != 0)
                ? sb.ToString()
                : null;
        }

        /// <summary>
        /// Get the process id from a window handle.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public static int GetWindowProcessId(IntPtr hwnd)
        {
            return (GetWindowThreadProcessId(hwnd, out int id) == 0) ? id : -1;
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
            return QueryFullProcessImageName(hproc, 0, sb, ref capacity)
                ? sb.ToString(0, capacity)
                : null;
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

        #endregion Public Helpers

        #region Hooks and Keyboard

        /// <summary>
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="id">Window hotkey ID to register</param>
        /// <param name="fsModifiers">
        /// Modifier keys, cast to <see cref="uint"/> from <see
        /// cref="System.Windows.Input.ModifierKeys"/>
        /// </param>
        /// <param name="vk">
        /// Keys, cast to <see cref="uint"/> from <see cref="System.Windows.Forms.Keys"/>
        /// </param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="id">Window hotkey ID to unregister</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(
            HookType hookType,
            HookProc lpfn,
            IntPtr hMod,
            uint dwThreadId);

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain. A hook
        /// procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hhk">This parameter is ignored.</param>
        /// <param name="nCode">
        /// The hook code passed to the current hook procedure. The next hook procedure uses this
        /// code to determine how to process the hook information.
        /// </param>
        /// <param name="wParam">
        /// The meaning of this parameter depends on the type of hook associated with the current
        /// hook chain.
        /// </param>
        /// <param name="lParam">
        /// The meaning of this parameter depends on the type of hook associated with the current
        /// hook chain.
        /// </param>
        /// <returns>
        /// This value is returned by the next hook procedure in the chain. The current hook
        /// procedure must also return this value. The meaning of the return value depends on the
        /// hook type.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        #endregion

        #region Windows

        [DllImport("User32.dll")]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("User32.dll")]
        public static extern IntPtr FindWindowEx(
            IntPtr hwndParent,
            IntPtr hwndChildAfter,
            string className,
            string windowName);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(
            IntPtr hWnd,
            int hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int wFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr hWnd, EnumWindowsProc callback, IntPtr i);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hwnd, ref Point lpPoint);

        [DllImport("User32.dll")]
        public static extern bool MoveWindow(
            IntPtr handle,
            int x,
            int y,
            int width,
            int height,
            bool redraw);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(
            IntPtr hwnd, DwmWindowAttribute dwAttribute, out bool pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(
            IntPtr hwnd, DwmWindowAttribute dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point point);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);

        #endregion Windows

        #region Processes

        public static List<IntPtr> EnumerateProcessWindowHandles(Process process)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread procThread in process.Threads.OfType<ProcessThread>())
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

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(
            int dwThreadId,
            EnumThreadDelegate lpfn,
            IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool QueryFullProcessImageName(
            [In]IntPtr hProc,
            [In]int dwFlags,
            [Out]StringBuilder lpExeName,
            ref int lpdwSize);

        #endregion Processes

        #region Drawing

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hWnd, IntPtr dc);

        #endregion Drawing
    }
}
