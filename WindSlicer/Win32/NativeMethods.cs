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
    /// Contains platform invoke methods. See <see cref="NativeApi"/> for helper functions wrapping
    /// the native methods.
    /// </summary>
    public static partial class NativeMethods
    {
        #region Delegates

        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);
        public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr LowLevelMouseProc(int code, WM wParam, [In] MSLLHOOKSTRUCT lParam);
        public delegate void WinEventDelegate(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime);


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

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        #endregion Structs

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

        [DllImport("user32", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hHook);

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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWinEventHook(
            uint eventMin,
            uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess,
            uint idThread,
            uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        internal static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

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
        internal static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumChildWindows(IntPtr hWnd, EnumWindowsProc callback, IntPtr i);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        internal static extern bool ClientToScreen(IntPtr hwnd, ref Point lpPoint);

        [DllImport("User32.dll")]
        public static extern bool MoveWindow(
            IntPtr handle,
            int x,
            int y,
            int width,
            int height,
            bool redraw);

        [DllImport("user32.dll")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll")]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetShellWindow();

        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(
            IntPtr hwnd, DwmWindowAttribute dwAttribute, out bool pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(
            IntPtr hwnd, DwmWindowAttribute dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out Point point);

        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(Point point);

        #endregion Windows

        #region Processes

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

        [DllImport("user32.dll")]
        internal static extern bool EnumThreadWindows(
            int dwThreadId,
            EnumThreadDelegate lpfn,
            IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool QueryFullProcessImageName(
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
