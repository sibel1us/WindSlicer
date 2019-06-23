using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WindSlicer.Commands;
using WindSlicer.Services;

namespace WindSlicer.Utilities.Converters
{
    public class CommandToImageConverter : IValueConverter
    {
        private readonly IIconService iconService = new ShellIconService();

        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is BaseCommand cmd)
            {
                var icon = iconService.GetIcon(cmd);

                // TODO: cache the bitmaps in IconProvider or such?
                return Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }

            return null;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return null;
        }
    }
}
