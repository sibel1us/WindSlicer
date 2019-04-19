using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WindSlicer.Win32;
using static System.Environment;

namespace WindSlicer.Commands.General
{
    public class SpecialFolderCommand : FolderCommand
    {
        private static HashSet<SpecialFolder> validValues =
            new HashSet<SpecialFolder>(
                Enum.GetValues(typeof(SpecialFolder)).Cast<SpecialFolder>());

        public SpecialFolder SpecialFolder { get; }

        public SpecialFolderCommand(SpecialFolder folder)
            : base(Environment.GetFolderPath(folder))
        {
            this.SpecialFolder = folder;
        }

        protected override string GetFullPath(string path)
        {
            return path;
        }

        protected override void OpenWindow()
        {
            if (this.SpecialFolder == SpecialFolder.MyComputer)
            {
                Process.Start("::{20d04fe0-3aea-1069-a2d8-08002b30309d}");
            }
            else
            {
                base.OpenWindow();
            }
        }

        protected override bool IsCorrectWindow(SHDocVw.InternetExplorer window)
        {
            if (this.SpecialFolder == SpecialFolder.MyComputer)
                return window.LocationURL == "";

            return base.IsCorrectWindow(window);
        }

        public override bool CanExecute(object parameter)
        {
            return validValues.Contains(this.SpecialFolder);
        }

        public override string ToString()
        {
            return this.SpecialFolder.ToString();
        }
    }

    /// <summary>
    /// Open a directory, or bring it to foreground if it's already open.
    /// </summary>
    public class FolderCommand : BaseCommand
    {
        private string name = null;

        /// <summary>
        /// 
        /// </summary>
        public string Directory { get; }

        public FolderCommand(string path)
        {
            this.Directory = this.GetFullPath(path);
        }

        protected virtual string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
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

                        // Found correct
                        return;
                    }
                }
            }

            this.OpenWindow();
        }

        protected virtual void OpenWindow()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo(this.Directory)
            };

            proc.Start();
        }

        protected virtual bool IsCorrectWindow(SHDocVw.InternetExplorer window)
        {
            if (string.IsNullOrEmpty(window.LocationURL))
                return false;

            var uri = new Uri(window.LocationURL);
            return this.Directory == Path.GetFullPath(uri.LocalPath);
        }

        public override bool CanExecute(object parameter)
        {
            return System.IO.Directory.Exists(this.Directory);
        }

        public override string ToString()
        {
            return name ?? (name = new DirectoryInfo(this.Directory).Name);
        }
    }
}
