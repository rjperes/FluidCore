using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FluidCore
{
    [Serializable]
    internal sealed class FluidObjectPropertyDescriptor : PropertyDescriptor
    {
        public FluidObjectPropertyDescriptor(string name) : base(name, null)
        {
        }

        public override bool CanResetValue(object component) => false;

        public override Type ComponentType => typeof(FluidObject);

        public override object GetValue(object component)
        {
            return (component as IDictionary<string, object>)[this.Name];
        }

        public override bool IsReadOnly => false;

        public override Type PropertyType => typeof(FluidObject);

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            (component as IDictionary<string, object>)[this.Name] = value;
        }

        public override Boolean ShouldSerializeValue(object component) => true;
    }
}