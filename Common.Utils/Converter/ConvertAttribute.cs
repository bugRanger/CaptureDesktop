namespace Common.Utils.Converter
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Data;

    /// <summary>
    /// Извлечение данных о свойстве из отслеживаемых полей.
    /// </summary>
    /// <typeparam name="T">Настраиваемый атрибут.</typeparam>
    public class PropertyAttributeConverter<T> : IValueConverter where T : Attribute
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            var propertyName = parameter as string;
            if (string.IsNullOrEmpty(propertyName))
                return new ArgumentNullException(nameof(parameter)).ToString();

            Type type = value.GetType();

            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                return new ArgumentOutOfRangeException(nameof(parameter), parameter,
                    "Property \"" + propertyName + "\" not found in type \"" + type.Name + "\".").ToString();

            if (!property.IsDefined(typeof(T), true))
                return new ArgumentOutOfRangeException(nameof(parameter), parameter,
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
    /// Извлекаем данные из атрибута <c>DisplayNameAttribute</c>.
    /// </summary>
    public sealed class PropertyDisplayNameConvert : PropertyAttributeConverter<DisplayNameAttribute>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = base.Convert(value, targetType, parameter, culture);            
            return result is DisplayNameAttribute attribute ? attribute.DisplayName : null;
        }
    }

    /// <summary>
    /// Извлекаем данные из атрибута <c>DescriptionAttribute</c>.
    /// </summary>
    public sealed class PropertyDescriptionConvert : PropertyAttributeConverter<DescriptionAttribute>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = base.Convert(value, targetType, parameter, culture);
            return result is DescriptionAttribute attribute ? attribute.Description : null;
        }
    }
}
