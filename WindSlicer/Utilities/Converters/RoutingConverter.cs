using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WindSlicer.Utilities.Converters
{
    public class RoutingConverter : DependencyObject, IValueConverter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Condition
        {
            get => (bool)this.GetValue(ConditionProperty);
            set
            {
                this.SetValue(ConditionProperty, value);
                this.OnPropertyChanged();
            }
        }

        public static readonly DependencyProperty ConditionProperty =
            DependencyProperty.Register(
                nameof(Condition),
                typeof(bool),
                typeof(RoutingConverter));


        private IValueConverter ActiveConverter =>
            this.Condition ? this.TrueConverter : this.FalseConverter;

        public IValueConverter TrueConverter { get; set; }

        public IValueConverter FalseConverter { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ActiveConverter?.Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ActiveConverter?.ConvertBack(value, targetType, parameter, culture);
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
