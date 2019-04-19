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
        /// Execute the window command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract override void Execute(object parameter);

        public sealed override bool CanExecute(object parameter)
        {
            return
                parameter is IntPtr hwnd &&
                hwnd != IntPtr.Zero &&
                hwnd != NativeMethods.GetDesktopWindow() &&
                NativeMethods.GetOpenWindowHandles().Contains(hwnd);
        }
    }
}
