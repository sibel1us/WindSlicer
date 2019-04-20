using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Win32;

namespace WindSlicer.Commands.Window
{
    /// <summary>
    /// Command to bring a window to the foreground.
    /// </summary>
    [Obsolete("Window showing as a WindowCommand makes no sense semantically.", error: true)]
    public class ShowWindowCommand : WindowCommand
    {
        /// <summary>
        /// Initialize a new command to bring a window to foreground.
        /// </summary>
        public ShowWindowCommand() { }

        /// <summary>
        /// Show the window.
        /// </summary>
        /// <param name="hwnd">Target window</param>
        public override void Execute(object parameter)
        {
            IntPtr hWnd = (IntPtr)parameter;
            NativeApi.RestoreAndShowWindow(hWnd);
        }

        public override bool Equals(object obj) => obj is ShowWindowCommand;
        public override int GetHashCode() => 173061137;
    }
}
