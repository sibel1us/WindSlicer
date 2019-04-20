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
        private string displayName = null;

        /// <summary>
        /// 
        /// </summary>
        public string Directory { get; }

        public FolderCommand(string path)
        {
            this.Directory = this.GetPathFromParameter(path);
        }

        protected virtual string GetPathFromParameter(string path)
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

                        NativeMethods.RestoreAndShowWindow(hwnd);

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
            return displayName ?? (displayName = new DirectoryInfo(this.Directory).Name);
        }
    }
}
