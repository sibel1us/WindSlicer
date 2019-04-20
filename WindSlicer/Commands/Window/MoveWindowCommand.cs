using System;
using WindSlicer.Win32;

namespace WindSlicer.Commands.Window
{
    /// <summary>
    /// Command to move a window by an offset without resizing it.
    /// </summary>
    public class MoveWindowCommand : WindowCommand
    {
        /// <summary>
        /// Pixels the window moves horizontally.
        /// </summary>
        public int DeltaX { get; }

        /// <summary>
        /// Pixels the window moves vertically.
        /// </summary>
        public int DeltaY { get; }

        /// <summary>
        /// Initialize a new window moving command.
        /// </summary>
        /// <param name="deltaX">Amount of pixels to move the window horizontally</param>
        /// <param name="deltaY">Amount of pixels to move the window vertically</param>
        public MoveWindowCommand(int deltaX, int deltaY)
        {
            if (deltaX == 0 && deltaY == 0)
            {
                throw new ArgumentException(
                    "Cannot initialize a window move command with zero delta values.");
            }

            this.DeltaX = deltaX;
            this.DeltaY = DeltaY;
        }

        /// <summary>
        /// Move the window's position by amount specified in <see cref="DeltaX"/> and <see cref="DeltaY"/>.
        /// </summary>
        /// <param name="hWnd">Target window</param>
        public override void Execute(object parameter)
        {
            IntPtr hWnd = (IntPtr)parameter;

            // Un-maximize first, or SetWindowPos won't work
            NativeMethods.ShowWindow(hWnd, (int)ShowWindowCommands.Normal);

            if (NativeApi.GetWindowPosition(hWnd) is NativeMethods.RECT rect)
            {
                int top = (int)SpecialWindowHandles.HWND_TOP;
                int noSize = (int)SetWindowPosFlags.SWP_NOSIZE;
                NativeMethods.SetWindowPos(hWnd, top, rect.Left, rect.Top, 0, 0, noSize);
            }

            // Couldn't get position
        }

        public override bool Equals(object obj) =>
            obj is MoveWindowCommand other &&
            this.DeltaX == other.DeltaX &&
            this.DeltaY == other.DeltaY;

        public override int GetHashCode()
        {
            var hashCode = -1069744805;
            hashCode = hashCode * -1521134295 + DeltaX.GetHashCode();
            hashCode = hashCode * -1521134295 + DeltaY.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return "TODO: move window";
        }
    }
}
