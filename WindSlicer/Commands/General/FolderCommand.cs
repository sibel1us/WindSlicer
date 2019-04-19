using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WindSlicer.Win32;

namespace WindSlicer.Commands.General
{
    /// <summary>
    /// Open a directory, or bring it to foreground if it's already open.
    /// </summary>
    public class FolderCommand : BaseCommand
    {
        private readonly Environment.SpecialFolder? special;

        private bool MyComputer => this.special == Environment.SpecialFolder.MyComputer;

        /// <summary>
        /// 
        /// </summary>
        public string Directory { get; }

        public FolderCommand(string path)
        {
            this.Directory = Path.GetFullPath(path);
        }

        public FolderCommand(Environment.SpecialFolder folder)
        {
            this.special = folder;
            this.Directory = Environment.GetFolderPath(folder);
        }

        public override void Execute(object parameter = null)
        {
            var shellWindows = new SHDocVw.ShellWindows();

            // Get open explorer windows
            foreach (SHDocVw.InternetExplorer window in shellWindows)
            {
                var windowFileName = Path.GetFileNameWithoutExtension(window.FullName);

                if (windowFileName.Equals("explorer", StringComparison.OrdinalIgnoreCase))
                {
                    if (this.IsCorrectWindow(window))
                    {
                        IntPtr hwnd = (IntPtr)window.HWND;

                        if (NativeMethods.IsIconic(hwnd))
                        {
                            NativeMethods.ShowWindow(hwnd, (int)ShowWindowCommands.Restore);
                        }

                        NativeMethods.ShowWindow(hwnd, (int)ShowWindowCommands.Show);
                        bool shown = NativeMethods.SetForegroundWindow(hwnd);
                    }
                }
            }

            if (this.MyComputer)
            {
                Process.Start("::{20d04fe0-3aea-1069-a2d8-08002b30309d}");
            }
            else
            {

                new Process { StartInfo = new ProcessStartInfo(this.Directory) }.Start();
            }
        }

        private bool IsCorrectWindow(SHDocVw.InternetExplorer window)
        {
            if (string.IsNullOrEmpty(window.LocationName) && !this.MyComputer)
                return false;

            string windowPath;

            if (this.MyComputer)
            {
                windowPath = "";
            }
            else
            {
                var uri = new Uri(window.LocationURL);
                windowPath = Path.GetFullPath(uri.LocalPath);
            }

            return this.Directory == windowPath;
        }

        public override bool CanExecute(object parameter)
        {
            return System.IO.Directory.Exists(this.Directory) || this.MyComputer;
        }

        public override string ToString()
        {
            return this.special.HasValue
                ? this.special.Value.ToString()
                : new DirectoryInfo(this.Directory).Name;
        }
    }
}
