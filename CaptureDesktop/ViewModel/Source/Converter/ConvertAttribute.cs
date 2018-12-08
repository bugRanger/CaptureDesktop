using System;
using System.Windows;
using System.Windows.Data;

using System.Reflection;
using System.Globalization;
using System.ComponentModel;

namespace Sima.Common.WPF.Tools.PropertyAttribute
{
    /// <summary>
    /// Извлечение данных о свойстве из отслеживаемых полей.
    /// </summary>
    /// <typeparam name="T">Настраиваемый атрибут.</typeparam>
    public class PropertyAttributeConverter<T> : IValueConverter where T : System.Attribute
    {
        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            string propertyName = parameter as string;
            if (String.IsNullOrEmpty(propertyName))
                return new ArgumentNullException("parameter").ToString();

            Type type = value.GetType();

            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                return new ArgumentOutOfRangeException("parameter", parameter,
                    "Property \"" + propertyName + "\" not found in type \"" + type.Name + "\".").ToString();

            if (!property.IsDefined(typeof(T), true))
                return new ArgumentOutOfRangeException("parameter", parameter,
                    "Property \"" + propertyName + "\" of type \"" + type.Name + "\"" +
                    " has no associated Description attribute.").ToString();

            return ((T)property.GetCustomAttributes(typeof(T), true)[0]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    /// <summary>
    /// Извлекаем данные из аттрибута DisplayNameAttribute.
    /// </summary>
    public sealed class PropertyDisplayNameConvert : PropertyAttributeConverter<DisplayNameAttribute>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = base.Convert(value, targetType, parameter, culture);            
            return result is DisplayNameAttribute ? ((DisplayNameAttribute)result).DisplayName : null;
        }
    }

    /// <summary>
    /// Извлекаем данные из аттрибута DescriptionAttribute.
    /// </summary>
    public sealed class PropertyDescriptionConvert : PropertyAttributeConverter<DescriptionAttribute>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = base.Convert(value, targetType, parameter, culture);
            return result is DescriptionAttribute ? ((DescriptionAttribute)result).Description : null;
        }
    }
}
