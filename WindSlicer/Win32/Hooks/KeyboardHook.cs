using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WindSlicer.Utilities;
using WindSlicer.Win32.Handles;

namespace WindSlicer.Win32.Hooks
{
    public sealed class KeyboardHook : IDisposable
    {
        private enum KeyEvents
        {
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            SKeyDown = 0x0104,
            SKeyUp = 0x0105
        }

        public event EventHandler<KeyChangedEventArgs> KeyChanged;

        public Keys Key { get; private set; }
        public bool Repeat { get; }

        private bool keyState = false;

        private NativeMethods.HookProc HookProc { get; }
        private SafeWindowsHookExHandle HookHandle { get; set; }

        public KeyboardHook(bool repeat)
        {
            this.Repeat = repeat;
            this.HookProc = new NativeMethods.HookProc(this.KeyboardHookProc);
        }

        public void Subscribe(Keys key)
        {
            if (this.HookHandle?.IsClosed == true)
            {
                Error.InvalidOp("Already closed");
                return;
            }

            if (this.Key != default || this.HookHandle != null)
            {
                Error.InvalidOp($"Already hooked to {this.Key}");
                return;
            }

            if (key == Keys.None)
            {
                Error.InvalidOp($"Cannot hook key {key}");
                return;
            }

            IntPtr handle = NativeMethods.SetWindowsHookEx(
                HookType.WH_KEYBOARD_LL,
                this.HookProc,
                IntPtr.Zero,
                0);

            this.HookHandle = new SafeWindowsHookExHandle(handle);

            if (this.HookHandle.IsInvalid)
            {
                Error.Win32("SetWindowsHookEx failed");
                return;
            }

            this.Key = key;
        }

        /// <summary>
        /// Handle key presses. First read
        /// </summary>
        /// <param name="code"></param>
        /// <param name="wParam">Contains the key event</param>
        /// <param name="lParam">Contains the key pressed</param>
        /// <returns></returns>
        private IntPtr KeyboardHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && (Keys)Marshal.ReadInt32(lParam) == this.Key)
            {
                var keyEvent = (KeyEvents)wParam.ToInt32();

                if (keyEvent == KeyEvents.KeyDown || keyEvent == KeyEvents.SKeyDown)
                {
                    if (this.keyState == false || this.Repeat)
                    {
                        this.keyState = true;
                        KeyChanged?.Invoke(null, new KeyChangedEventArgs(this.Key, true));
                    }
                }
                else if (keyEvent == KeyEvents.KeyUp || keyEvent == KeyEvents.SKeyUp)
                {
                    this.keyState = false;
                    KeyChanged?.Invoke(null, new KeyChangedEventArgs(this.Key, false));
                }
            }

            return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        public void Dispose()
        {
            this.HookHandle?.Dispose();
        }
    }

    public sealed class KeyChangedEventArgs : EventArgs
    {
        public Keys Key { get; }
        public bool KeyDown { get; }

        public KeyChangedEventArgs(Keys key, bool keyDown)
        {
            this.Key = key;
            this.KeyDown = keyDown;
        }
    }
}
