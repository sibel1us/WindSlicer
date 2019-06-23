using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WindSlicer.Utilities;

namespace WindSlicer.Win32.Hooks
{
    public sealed class HotkeyHook : IDisposable
    {
        /// <remarks>
        /// This is here instead of NativeMethods because it shouldn't be called anywhere else.
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        /// <remarks>
        /// This is here instead of NativeMethods because it shouldn't be called anywhere else.
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private class HookedWindow : NativeWindow, IDisposable
        {
            private static readonly int WM_HOTKEY = (int)WM.HOTKEY;

            public HookedWindow()
            {
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    KeyPressed?.Invoke(this, new HotkeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<HotkeyPressedEventArgs> KeyPressed;

            public void Dispose()
            {
                this.DestroyHandle();
            }
        }

        private readonly HookedWindow hookedWindow;
        private int currentId;

        public HotkeyHook()
        {
            this.hookedWindow = new HookedWindow();

            this.hookedWindow.KeyPressed += delegate (object _, HotkeyPressedEventArgs args)
            {
                KeyPressed?.Invoke(this, args);
            };
        }

        /// <summary>
        /// Registers a hotkey in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            this.currentId += 1;

            if (!RegisterHotKey(this.hookedWindow.Handle, this.currentId, (uint)modifier, (uint)key))
                Error.Win32("RegisterHotKey failed");
        }

        public event EventHandler<HotkeyPressedEventArgs> KeyPressed;

        public void Dispose()
        {
            for (int i = this.currentId; i > 0; i--)
            {
                UnregisterHotKey(this.hookedWindow.Handle, i);
            }

            this.hookedWindow.Dispose();
        }
    }

    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class HotkeyPressedEventArgs : EventArgs
    {
        public ModifierKeys Modifier { get; }
        public Keys Key { get; }

        internal HotkeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            this.Modifier = modifier;
            this.Key = key;
        }
    }
}
