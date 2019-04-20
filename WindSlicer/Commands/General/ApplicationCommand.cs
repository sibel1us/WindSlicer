using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Win32;

namespace WindSlicer.Commands.General
{
    public class ApplicationCommand : BaseCommand
    {
        public enum FollowupModes
        {
            /// <summary>Never execute.</summary>
            Never = 0,

            /// <summary>When application was already open.</summary>
            AppOpen = 1,

            /// <summary>When application was not open and was started.</summary>
            AppStarted = 2,

            /// <summary>When application was brought to front or opened.</summary>
            OnSuccess = AppOpen | AppStarted,
        }

        public string ProcessName { get; private set; }
        public string FileLocation { get; private set; }
        public string Arguments { get; private set; }

        /// <summary>
        /// Prioritize window with this classname over <see cref="Process.MainWindowHandle"/> if defined.
        /// </summary>
        public string WindowClassName { get; set; }

        public ProcessWindowStyle? WindowStyle { get; set; }

        /// <summary>
        /// Action to be executed after the application is brought to front or started.
        /// </summary>
        public WindowCommand Followup { get; set; }

        /// <summary>
        /// Condition for <see cref="Followup"/> action. Default is <see cref="FollowupModes.OnSuccess"/>.
        /// </summary>
        public FollowupModes FollowupMode { get; set; } = FollowupModes.OnSuccess;

        /// <summary>
        /// Maximum time to wait for the process to accept input before canceling.
        /// </summary>
        public TimeSpan FollowupWaitTime { get; set; } = TimeSpan.FromSeconds(5);

        public ApplicationCommand(Process process) : this(process.MainModule.FileName) { }

        public ApplicationCommand(string filePath, string arguments = null)
        {
            if (!File.Exists(filePath))
            {
                //throw new FileNotFoundException("Couldn't find the specified file", filePath);
            }

            this.ProcessName = Path.GetFileName(filePath);
            this.FileLocation = Path.GetFullPath(filePath);
            this.Arguments = arguments;
        }

        public override void Execute(object parameter)
        {
            var processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(FileLocation));

            if (processes.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero) is Process process)
            {
                IntPtr hwnd = GetPreferredWindow(process) ?? process.MainWindowHandle;

                bool shown = NativeMethods.RestoreAndShowWindow(hwnd);
                bool foreground = NativeMethods.SetForegroundWindow(hwnd);

                if (shown && foreground)
                {
                    if (this.FollowupMode.HasFlag(FollowupModes.AppOpen)
                        && Followup != null)
                    {
                        if (Followup.CanExecute(hwnd))
                            Followup.Execute(hwnd);
                    }
                }
                else
                {
                    // Couldn't restore window
                }

            }
            else
            {
                // Start process
                var startInfo = new ProcessStartInfo(FileLocation);

                if (this.WindowStyle.HasValue)
                {
                    startInfo.WindowStyle = this.WindowStyle.Value;
                }

                startInfo.Arguments = this.Arguments;

                var p = new Process() { StartInfo = startInfo };

                if (this.FollowupMode.HasFlag(FollowupModes.AppStarted)
                    && Followup != null)
                {
                    if (!p.Start())
                    {

                    }
                    if (!p.WaitForInputIdle((int)this.FollowupWaitTime.TotalMilliseconds))
                    {
                    }

                    if (Followup.CanExecute(p.MainWindowHandle))
                        Followup.Execute(p.MainWindowHandle); ;
                }

                p.Start();
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

        private IntPtr? GetPreferredWindow(Process process)
        {
            if (string.IsNullOrEmpty(WindowClassName))
            {
                return null;
            }

            var handles = NativeMethods.EnumerateProcessWindowHandles(process);

            foreach (var hwnd in handles)
            {
                if (NativeMethods.GetClassName(hwnd) == this.WindowClassName)
                {
                    return hwnd;
                }
            }

            return null;
        }
    }
}
