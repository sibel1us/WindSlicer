using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindSlicer.Utilities.Converters
{
    /// <summary>
    /// Used to bind to values from objects out visual/logical tree.
    /// </summary>
    /// <remarks>
    /// Source: https://thomaslevesque.com/2011/03/21/
    /// wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
    /// </remarks>
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data),
                typeof(object),
                typeof(BindingProxy),
                new UIPropertyMetadata(null));
    }
}
