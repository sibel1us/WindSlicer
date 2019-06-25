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
    public class DisplayExtension : MarkupExtension
    {
        private string propertyName;
        private Type type;

        public DisplayExtension(string propertyName, Type type)
        {
            this.propertyName = propertyName;
            this.type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget pvt)
            {
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                if (propertyInfo != null)
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