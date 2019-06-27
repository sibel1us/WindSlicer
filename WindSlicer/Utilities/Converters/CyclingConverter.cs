using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WindSlicer.Utilities.Converters
{
    /// <summary>
    /// Converts item index in a collection into a value from another collection.
    /// </summary>
    public class CyclingConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (values.Length == 3 &&
                values[0] is IEnumerable<object> cycleList &&
                values[1] is object sourceItem &&
                values[2] is IEnumerable<object> sourceList)
            {

                var index = sourceList.ToList().IndexOf(sourceItem);

                if (index != -1)
                {
                    return cycleList.ElementAtOrDefault(index % cycleList.Count());
                }
            }

            return null;
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
