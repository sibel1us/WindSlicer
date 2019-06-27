using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WindSlicer.Utilities.Converters
{
    public class ColorLightnessConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is SolidColorBrush brush && Util.ParseDouble(parameter) is double factor)
            {
                return new SolidColorBrush(Scale(brush.Color, factor));
            }

            return null;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is SolidColorBrush brush && Util.ParseDouble(parameter) is double factor)
            {
                return new SolidColorBrush(Scale(brush.Color, 1.0 / factor));
            }

            return null;
        }

        private static Color Scale(Color color, double factor)
        {
            factor = Math.Max(0.0, factor);

            return Color.FromRgb(
                Math.Min((byte)255, (byte)(color.R * factor)),
                Math.Min((byte)255, (byte)(color.G * factor)),
                Math.Min((byte)255, (byte)(color.B * factor)));
        }
    }
}
