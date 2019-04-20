using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Win32;
using WindSlicer.Utilities.Extensions;

namespace WindSlicer.Commands.General
{
    public class ApplicationCommand : BaseCommand
    {
        public string ProcessName { get; private set; }
        public string FileLocation { get; private set; }
        public string Arguments { get; private set; }

        /// <summary>
        /// Prioritize window with this classname over <see cref="Process.MainWindowHandle"/> if
        /// defined.
        /// </summary>
        public string WindowClassName { get; set; }

        public ProcessWindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Initializes a new application command.
        /// </summary>
        /// <param name="filePath"></param>
        public ApplicationCommand(string filePath)
            : this(filePath, null)
        { }

        /// <summary>
        /// Initializes a new application command with the specified arguments.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="arguments">
        /// Arguments used to start the executable if it is not already runnin.
        /// </param>
        public ApplicationCommand(string filePath, string arguments)
        {
            this.ProcessName = Path.GetFileName(filePath);
            this.FileLocation = Path.GetFullPath(filePath);
            this.Arguments = arguments;
        }

        public override void Execute(object parameter)
        {
            var processes = Process.GetProcessesByName(
                Path.GetFileNameWithoutExtension(this.FileLocation));

            if (processes.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero) is Process proc)
            {
                var hwnd = this.GetPreferredWindowHandle(proc);
                NativeApi.RestoreAndShowWindow(hwnd);
            }
            else
            {
                // Start process
                var startInfo = new ProcessStartInfo(FileLocation)
                {
                    Arguments = this.Arguments,
                    WindowStyle = this.WindowStyle
                };

                // TODO: create ChainCommand/MultiCommand

                var newProc = new Process() { StartInfo = startInfo };

                newProc.Start();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return File.Exists(FileLocation);
        }

        public override string ToString()
        {
            return Path.GetFileNameWithoutExtension(FileLocation);
        }

        private IntPtr GetPreferredWindowHandle(Process process)
        {
            if (!string.IsNullOrEmpty(WindowClassName))
            {
                foreach (var hwnd in NativeApi.EnumerateProcessWindowHandles(process))
                {
                    if (NativeApi.GetClassName(hwnd) == this.WindowClassName)
                    {
                        return hwnd;
                    }
                }
            }

            return process.MainWindowHandle;
        }
    }
}
