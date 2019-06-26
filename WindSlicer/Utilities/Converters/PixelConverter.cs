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
    public class PixelConverter : DependencyObject, IValueConverter
    {
        private const string KEY_WIDTH = "Width";
        private const string KEY_HEIGHT = "Height";

        public int Width
        {
            get => (int)this.GetValue(WidthProperty);
            set => this.SetValue(WidthProperty, value);
        }

        public int Height
        {
            get => (int)this.GetValue(HeightProperty);
            set => this.SetValue(HeightProperty, value);
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(int),
                typeof(PixelConverter));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(int),
                typeof(PixelConverter));

        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is double multiplier)
            {
                switch (parameter)
                {
                    case KEY_WIDTH:
                        return (this.Width * multiplier).Round();
                    case KEY_HEIGHT:
                        return (this.Height * multiplier).Round();
                }
            }

            return 0;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (Util.ParseDouble(value) is double pixels)
            {
                switch (parameter)
                {
                    case KEY_WIDTH:
                        return pixels / (double)this.Width;
                    case KEY_HEIGHT:
                        return pixels / (double)this.Height;
                }
            }

            return 0.0;
        }
    }
}
