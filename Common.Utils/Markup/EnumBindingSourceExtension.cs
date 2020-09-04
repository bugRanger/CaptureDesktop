namespace Common.Utils.Markup
{
    using System;
    using System.Windows.Markup;

    public class EnumBindingSourceExtension : MarkupExtension
    {
        #region Fields

        private Type _enumType;

        #endregion Fields

        #region Properties

        #endregion Properties

        public Type EnumType
        {
            get => _enumType;
            set
            {
                if (value == _enumType)
                    return;

                if (null != value)
                {
                    Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                    if (!enumType.IsEnum)
                        throw new ArgumentException("Type must be for an Enum.");
                }

                _enumType = value;
            }
        }

        #region Constructors
        
        public EnumBindingSourceExtension() { }

        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        #endregion Constructors

        #region Methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType == null)
                throw new InvalidOperationException("The EnumType must be specified.");

            Type actualEnumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == EnumType)
                return enumValues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }

        #endregion Methods 

    }
}
