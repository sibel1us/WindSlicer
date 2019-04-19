using System;
using System.Drawing;
using WindSlicer.Win32;

namespace WindSlicer.Commands.Window
{
    /// <summary>
    /// Command to snap a window to the specified coordinates.
    /// Snapping involves changing window size.
    /// </summary>
    public class SnapWindowCommand : WindowCommand
    {
        /// <summary>
        /// Pixel offset of the window's left edge after snapping.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Pixel offset of the window's top edge after snapping.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Width of the window after snapping.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height of the window after snapping.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Initialize a new window snapping command to the specified coordinates.
        /// </summary>
        /// <param name="x">Absolute X position (window's top left corner) after snapping</param>
        /// <param name="y">Absolute Y position (window's top left corner) after snapping</param>
        /// <param name="width">Width of the window after snapping</param>
        /// <param name="height">Height of the window after snapping</param>
        public SnapWindowCommand(int x, int y, int width, int height)
        {
            Validate(x, y, width, height);

            // TODO: size validation
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Initialize a new window snapping command to the specified position.
        /// </summary>
        /// <param name="rectangle">Position and size of the window after snapping</param>
        public SnapWindowCommand(Rectangle rectangle) : this(
            rectangle.X,
            rectangle.Y,
            rectangle.Width,
            rectangle.Height)
        { }

        /// <summary>
        /// Snap the window to the specified coordinates.
        /// </summary>
        /// <param name="hWnd">Target window</param>
        /// <remarks>
        /// Win10 Borders are stupid: stackoverflow.com/questions/42473554
        /// </remarks>
        public override void Execute(object parameter)
        {
            IntPtr hWnd = (IntPtr)parameter;
            NativeMethods.ShowWindow(hWnd, (int)ShowWindowCommands.Normal);

            if (NativeMethods.GetWindowPosition(hWnd) is NativeMethods.RECT rectWindow &&
                NativeMethods.GetWin10Bounds(hWnd) is NativeMethods.RECT rectFixed)
            {
                // Get difference of normal and real bounds to apply to SetWindowPos
                // which uses normal coordinates
                int Diff(Func<NativeMethods.RECT, int> f) => f(rectWindow) - f(rectFixed);

                NativeMethods.SetWindowPos(
                    hWnd,
                    (int)SpecialWindowHandles.HWND_TOP,
                    X + Diff(r => r.Left),
                    Y + Diff(r => r.Top),
                    Width + Diff(r => r.Right) - Diff(r => r.Left),
                    Height + Diff(r => r.Bottom),
                    0);
            }
        }

        public override bool Equals(object obj) =>
            obj is SnapWindowCommand other &&
            this.X == other.X &&
            this.Y == other.Y &&
            this.Width == other.Width &&
            this.Height == other.Height;

        public override int GetHashCode()
        {
            var hashCode = 466501756;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }

        private void Validate(int x, int y, int width, int height)
        {
        }

        public override string ToString()
        {
            return $"Snap: {X},{Y}/{Width},{Height}";
        }
    }
}
