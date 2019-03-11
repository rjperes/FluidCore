using System;
using System.ComponentModel;

namespace FluidCore
{
    [Serializable]
    internal sealed class FluidObjectTypeDescriptionProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new FluidObjectTypeDescriptor(instance);
        }
    }
}