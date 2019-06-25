using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WindSlicer.Utilities.Converters
{
    public class BooleanToDoubleConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (values.Length == 3 &&
                values[0] is bool b &&
                values[1] is double trueValue &&
                values[2] is double falseValue)
            {
                return b ? trueValue : falseValue;
            }

            return 0.0;
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
