using System;
using WindSlicer.Win32;

namespace WindSlicer.Commands.Window
{
    /// <summary>
    /// Command to maximize a window.
    /// </summary>
    public class MaximizeWindowCommand : WindowCommand
    {
        /// <summary>
        /// Initialize a new command to maximize a window.
        /// </summary>
        public MaximizeWindowCommand() { }

        /// <summary>
        /// Maximize the window.
        /// </summary>
        /// <param name="hWnd">Target window</param>
        public override void Execute(object parameter)
        {
            var hWnd = (IntPtr)parameter;
            NativeMethods.ShowWindow(hWnd, (int)ShowWindowCommands.Maximize);
        }

        public override bool Equals(object obj) => obj is MaximizeWindowCommand;
        public override int GetHashCode() => 780023219;

        public override string ToString()
        {
            return $"Maximize window";
        }
    }
}
