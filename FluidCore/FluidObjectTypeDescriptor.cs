using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FluidCore
{
    [Serializable]
    internal sealed class FluidObjectTypeDescriptor : CustomTypeDescriptor
    {
        private readonly FluidObject _instance;

        public FluidObjectTypeDescriptor(object instance)
        {
            this._instance = instance as FluidObject;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            if (this._instance != null)
            {
                return new PropertyDescriptorCollection((this._instance as IDictionary<string, object>).Keys.Select(x => new FluidObjectPropertyDescriptor(x)).ToArray());
            }
            else
            {
                return base.GetProperties();
            }
        }

        public override TypeConverter GetConverter()
        {
            return new FluidObjectTypeConverter();
        }

        public override AttributeCollection GetAttributes()
        {
            return new AttributeCollection(new SerializableAttribute());
        }
    }
}