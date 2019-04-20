using System;
using WindSlicer.Win32;

namespace WindSlicer.Commands.Window
{
    /// <summary>
    /// Command to minimize a window.
    /// </summary>
    public class MinimizeWindowCommand : WindowCommand
    {
        private static readonly MinimizeWindowCommand _slave = new MinimizeWindowCommand();

        public static void ExecuteOn(IntPtr hwnd)
        {
            if (_slave.CanExecute(hwnd))
                _slave.Execute(hwnd);
        }

        /// <summary>
        /// Initialize a new command to minimize a window.
        /// </summary>
        public MinimizeWindowCommand() { }

        /// <summary>
        /// Minimize the window.
        /// </summary>
        /// <param name="hWnd">Target window</param>
        public override void Execute(object parameter)
        {
            var hWnd = (IntPtr)parameter;
            NativeMethods.ShowWindow(hWnd, (int)ShowWindowCommands.Minimize);
        }

        public override bool Equals(object obj) => obj is MinimizeWindowCommand;
        public override int GetHashCode() => 1265810827;

        public override string ToString()
        {
            return $"Minimize window";
        }
    }
}
