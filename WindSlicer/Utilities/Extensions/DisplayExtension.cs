using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace WindSlicer.Utilities.Extensions
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// Source: social.msdn.microsoft.com/Forums/en-US/88ede912-3f58-4b41-90be-8439c65f12b4/
    /// </remarks>
    public class DisplayExtension : MarkupExtension
    {
        private readonly string propertyName;
        private readonly Type type;

        public DisplayExtension(string propertyName, Type type)
        {
            this.propertyName = propertyName;
            this.type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget)
            {
                if (type.GetProperty(propertyName) is PropertyInfo propertyInfo)
                {
                    if (propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true)
                        .FirstOrDefault() is DisplayAttribute att)
                    {
                        if (att.ResourceType != null)
                        {

                            ResourceManager rm = new ResourceManager(att.ResourceType);
                            return rm.GetObject(att.Name).ToString();
                        }
                        else
                        {
                            return att.Name;
                        }
                    }
                }
            }

            return null;
        }
    }
}