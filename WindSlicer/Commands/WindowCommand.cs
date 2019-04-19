using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WindSlicer.Win32;

namespace WindSlicer.Commands
{
    /// <summary>
    /// Commands for window manipulation. They require a window handle to execute.
    /// </summary>
    public abstract class WindowCommand : BaseCommand
    {
        /// <summary>
        /// Static handle for the active window before the command window was opened.
        /// </summary>
        /// <remarks>
        /// Since all window commands have identical requirements for execution, this field is used
        /// to do the calculation once.
        /// </remarks>
        private static IntPtr activeHwnd = IntPtr.Zero;

        /// <summary>
        /// Whether <see cref="activeHwnd"/> is a valid handle for window commands.
        /// </summary>
        private static bool canExec = false;

        /// <summary>
        /// Execute the window command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract override void Execute(object parameter);

        public sealed override bool CanExecute(object parameter)
        {
            if (parameter is IntPtr hwnd)
            {
                if (activeHwnd != hwnd)
                {
                    UpdateCanExecute(hwnd);
                }

                return canExec;
            }

            return false;
        }

        private static void UpdateCanExecute(IntPtr hwnd)
        {
            activeHwnd = hwnd;
            canExec =
                hwnd != IntPtr.Zero &&
                hwnd != NativeMethods.GetDesktopWindow() &&
                NativeMethods.GetOpenWindowHandles().Contains(hwnd);
        }
    }
}
