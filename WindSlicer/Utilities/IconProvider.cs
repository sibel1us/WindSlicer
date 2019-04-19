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

namespace WindSlicer.Utilities
{
    using PathIconMap = ConcurrentDictionary<string, Icon>;

    public static class IconProvider
    {
        private static readonly Lazy<Icon> _default = new Lazy<Icon>(() => SystemIcons.Application);
        private static readonly Lazy<Icon> _error = new Lazy<Icon>(() => SystemIcons.Error);
        private static readonly Lazy<Icon> _window = new Lazy<Icon>(() => SystemIcons.Application);
        private static readonly Lazy<Icon> _folder = new Lazy<Icon>(DefaultIcons.FetchIcon, true);

        /// <summary>
        /// Cache of application icons.
        /// </summary>
        private static readonly PathIconMap _appIcons = new PathIconMap();

        public static Icon Default => _default.Value;

        public static Icon GetIcon(BaseCommand command)
        {
            switch (command)
            {
                case WindowCommand _:
                    return _window.Value;
                case DirectoryCommand _:
                    return _folder.Value;
                case ApplicationCommand cmd:
                    return GetApplicationIcon(cmd);
                default:
                    return _default.Value;
            }
        }

        private static Icon GetApplicationIcon(ApplicationCommand cmd)
        {
            string path = cmd.FileLocation;

            if (!File.Exists(path))
            {
                return _error.Value;
            }

            if (_appIcons.ContainsKey(path))
            {
                return _appIcons.TryGetValue(path, out Icon icon) ? icon : Default;
            }
            else
            {
                var icon = Icon.ExtractAssociatedIcon(path);
                _appIcons.TryAdd(path, icon);
                return icon;
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
