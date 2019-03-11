using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace FluidCore
{
    [Serializable]
    internal sealed class FluidObjectTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(FluidObject))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is FluidObject)
            {
                return value;
            }
            else
            {
                return new FluidObject(null, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(FluidObject))
            {
                return this.ConvertFrom(context, culture, value);
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var provider = TypeDescriptor.GetProvider(value);
            var descriptor = provider?.GetTypeDescriptor(value);
            return descriptor?.GetProperties(attributes) ?? PropertyDescriptorCollection.Empty;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            dynamic obj = new FluidObject();

            foreach (var key in propertyValues.Keys)
            {
                obj[key.ToString()] = propertyValues[key];
            }

            return obj;
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => false;
    }
}