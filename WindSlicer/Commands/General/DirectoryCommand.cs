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
    public class DirectoryCommand : BaseCommand
    {
        private readonly Environment.SpecialFolder? _specialFolder;

        /// <summary>
        /// 
        /// </summary>
        public string Location { get; }

        public DirectoryCommand(string path) : this(null, path) { }

        public DirectoryCommand(Environment.SpecialFolder folder) : this(folder, null) { }

        private DirectoryCommand(Environment.SpecialFolder? folder, string path)
        {
            if (folder.HasValue && folder.Value != Environment.SpecialFolder.Desktop)
            {
                _specialFolder = folder;
                Location = Environment.GetFolderPath(folder.Value);
            }
            else
            {
                Location = Path.GetFullPath(path);
            }

            if (!Directory.Exists(Location))
            {
                //throw new DirectoryNotFoundException($"Directory not found ({Location})");
            }
        }

        public override void Execute(object parameter = null)
        {
            var shellWindows = new SHDocVw.ShellWindows();

            // Get open explorer windows
            foreach (SHDocVw.InternetExplorer window in shellWindows)
            {
                if (Path.GetFileNameWithoutExtension(window.FullName)
                    .Equals("explorer", StringComparison.OrdinalIgnoreCase))
                {
                    // "My PC" -folder has an empty url
                    if (string.IsNullOrEmpty(window.LocationURL))
                    {
                        continue;
                    }

                    var uri = new Uri(window.LocationURL);

                    // Found open window
                    if (Location == Path.GetFullPath(uri.LocalPath))
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

            var process = new Process { StartInfo = new ProcessStartInfo(Location) };

            bool started = process.Start();
        }

        public override bool CanExecute(object parameter) => Directory.Exists(Location);

        public override string ToString()
        {
            return _specialFolder.HasValue
                ? _specialFolder.Value.ToString()
                : new DirectoryInfo(Location).Name;
        }
    }
}
