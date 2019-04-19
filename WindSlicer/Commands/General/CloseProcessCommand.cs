using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Commands.General
{
    public class CloseProcessCommand : BaseCommand
    {
        public string ProcessName { get; set; }
        public bool CloseAll { get; set; }

        public CloseProcessCommand(string processName, bool closeAllInstances)
        {
            this.ProcessName = ProcessName;
        }

        public CloseProcessCommand(Process p) : this(p.ProcessName, false) { }
        public CloseProcessCommand(string processName) : this(processName, false) { }

        public override void Execute(object parameter)
        {
            var processes = Process.GetProcessesByName(this.ProcessName);

            foreach (var p in processes)
            {
                // Try to close main window first
                if (!p.CloseMainWindow())
                {
                    p.Close();
                }

                if (CloseAll)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        public override bool CanExecute(object parameter)
        {
            return Process.GetProcessesByName(this.ProcessName).Any();
        }
    }
}
