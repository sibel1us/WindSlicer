using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Commands;
using WindSlicer.Commands.General;

namespace WindSlicer.Services
{
    //using PathIconMap = ConcurrentDictionary<string, Icon>;

    public class ShellIconService : IIconService
    {
        private readonly Icon _folder = DefaultIcons.FetchIcon();

        /// <summary>
        /// Cache of application icons.
        /// </summary>
        private readonly IDictionary<string, Icon> _appIcons;

        public Icon Default { get; } = SystemIcons.Application;

        public ShellIconService()
        {
            _appIcons = new Dictionary<string, Icon>();
        }

        public Icon GetIcon(BaseCommand command)
        {
            switch (command)
            {
                case WindowCommand _:
                    return SystemIcons.Application; // TODO: icon for snaps
                case FolderCommand _:
                    return _folder;
                case ApplicationCommand cmd:
                    return this.GetApplicationIcon(cmd);
                default:
                    return Default;
            }
        }

        private Icon GetApplicationIcon(ApplicationCommand cmd)
        {
            string path = cmd?.FileLocation ?? "";

            if (!File.Exists(path))
            {
                return SystemIcons.Error;
            }

            if (_appIcons.TryGetValue(path, out Icon existingIcon))
            {
                return existingIcon;
            }
            else
            {
                if (Icon.ExtractAssociatedIcon(path) is Icon newIcon)
                {
                    _appIcons.Add(path, newIcon);
                    return newIcon;
                }
                else
                {
                    return this.Default;
                }
            }
        }

        public void Dispose()
        {
            _folder.Dispose();

            foreach (var icon in this._appIcons.Values)
            {
                icon?.Dispose();
            }
        }

        /// <summary>
        /// Provides default windows folder icons.
        /// </summary>
        /// <remarks>
        /// Source: https://stackoverflow.com/a/42933011
        /// </remarks>
        protected static class DefaultIcons
        {
            public static Icon FetchIcon()
            {
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var icon = ExtractFromPath(new DirectoryInfo(appdata).FullName);
                return icon;
            }

            private static Icon ExtractFromPath(string path)
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                SHGetFileInfo(
                    path,
                    0, ref shinfo, (uint)Marshal.SizeOf(shinfo),
                    SHGFI_ICON | SHGFI_LARGEICON);
                return Icon.FromHandle(shinfo.hIcon);
            }

            //Struct used by SHGetFileInfo function
            [StructLayout(LayoutKind.Sequential)]
            private struct SHFILEINFO
            {
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            };

            [DllImport("shell32.dll")]
            private static extern IntPtr SHGetFileInfo(
                string pszPath,
                uint dwFileAttributes,
                ref SHFILEINFO psfi,
                uint cbSizeFileInfo,
                uint uFlags);

            private const uint SHGFI_ICON = 0x100;
            private const uint SHGFI_LARGEICON = 0x0;
            private const uint SHGFI_SMALLICON = 0x000000001;
        }
    }
}
