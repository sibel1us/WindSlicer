using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using WindSlicer.Models;

namespace WindSlicer.Utilities.Converters
{
    public class ScreenNameConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is IScreen screen)
            {
                var display = $"{screen.DeviceName} {screen.Bounds.Width}x{screen.Bounds.Height}";

                if (screen.Primary)
                    display += " (Primary)";

                return display;
            }

            return "";
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
