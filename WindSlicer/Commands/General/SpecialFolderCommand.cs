using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Utilities;
using static System.Environment;

namespace WindSlicer.Commands.General
{
    public class SpecialFolderCommand : FolderCommand
    {
        private static readonly IReadOnlyDictionary<SpecialFolder, string> valueCache =
            new LazyDictionary<SpecialFolder, string>(
                Util.ValuesOf<SpecialFolder>(),
                x => GetFolderPath(x));

        public SpecialFolder SpecialFolder { get; }

        public SpecialFolderCommand(SpecialFolder folder) : base(GetFolderPath(folder))
        {
            this.SpecialFolder = folder;
        }

        /// <summary>
        /// Overridden to prevent 
        /// </summary>
        protected override string GetPathFromParameter(string path)
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

        /// <summary>
        /// </summary>
        /// <remarks>
        /// At least on my environment MyComputer, LocalizedResources and CommonOemLinks returned
        /// empty paths. Only MyComputer of these is important, so ignore the rest.
        /// </remarks>
        public override bool CanExecute(object parameter)
        {
            return this.SpecialFolder == SpecialFolder.MyComputer ||
                (valueCache.TryGetValue(this.SpecialFolder, out string path) &&
                !string.IsNullOrEmpty(path));
        }

        public override string ToString()
        {
            return this.SpecialFolder.ToString();
        }
    }
}
