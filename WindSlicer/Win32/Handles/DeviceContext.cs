using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Win32.Handles
{
    /// <summary>
    /// Wrapper class for <see cref="NativeMethods.GetDC(IntPtr)"/> and <see
    /// cref="NativeMethods.ReleaseDC(IntPtr, IntPtr)"/>.
    /// </summary>
    public sealed class DeviceContext : IDisposable
    {
        /// <summary>
        /// Window handle used to get the device context.
        /// </summary>
        public IntPtr HWnd { get; }

        /// <summary>
        /// Device context handle.
        /// </summary>
        public IntPtr Hdc { get; }

        /// <summary>
        /// Initializes a new device context using the specified window handle using <see
        /// cref="NativeMethods.GetDC(IntPtr)"/>.
        /// </summary>
        /// <param name="hwnd"></param>
        public DeviceContext(IntPtr hwnd)
        {
            this.HWnd = hwnd;
            this.Hdc = NativeMethods.GetDC(hwnd);
        }

        /// <summary>
        /// Returns a new <see cref="Graphics"/> from the device context handle. The graphics object
        /// must be disposed.
        /// </summary>
        /// <returns></returns>
        public Graphics GetGraphics()
        {
            return Graphics.FromHdc(this.Hdc);
        }

        /// <summary>
        /// Releases the device context handle.
        /// </summary>
        public void Dispose()
        {
            if (this.Hdc != IntPtr.Zero)
                NativeMethods.ReleaseDC(this.HWnd, this.Hdc);
        }
    }
}
