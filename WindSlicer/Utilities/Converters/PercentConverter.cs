using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WindSlicer.Utilities.Extensions;

namespace WindSlicer.Utilities.Converters
{
    public class PercentConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is double multiplier)
            {
                return (multiplier * 100.0).ToString("0.##", CultureInfo.InvariantCulture);
            }

            return 0;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (Util.ParseDouble(value) is double d)
            {
                return d / 100.0;
            }

            return 0.0;
        }
    }
}
