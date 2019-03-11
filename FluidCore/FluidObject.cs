using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FluidCore
{
    [Serializable]
    [TypeConverter(typeof(FluidObjectTypeConverter))]
    [TypeDescriptionProvider(typeof(FluidObjectTypeDescriptionProvider))]
    public sealed class FluidObject : DynamicObject, IDictionary<string, object>, ICloneable, INotifyPropertyChanged
    {
        public const string Root = "$root";
        public const string Path = "$path";
        public const string Type = "$type";
        public const string Parent = "$parent";
        public const string Value = "$value";

        private static readonly string[] SpecialKeys = new string[] { Path, Parent, Root, Value, Type };

        private readonly IDictionary<string, object> _values = new Dictionary<string, object>();
        private object _value;
        private FluidObject _parent;

        public FluidObject() : this(null, null)
        {
        }

        internal FluidObject(FluidObject parent, object value)
        {
            this._parent = parent;
            this._value = (value is FluidObject) ? ((FluidObject)value)._value : value;
        }

        public FluidObject(object value) : this(null, value)
        {
        }

        public override string ToString()
        {
            if (this._value != null)
            {
                return this._value.ToString();
            }
            else
            {
                var dict = this as IDictionary<string, object>;
                return string.Format("{{{0}}}", string.Join(", ", dict.Keys.Zip(dict.Values, (k, v) => string.Format("{0}={1}", k, v))));
            }
        }

        public override int GetHashCode()
        {
            if (this._value != null)
            {
                return this._value.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as FluidObject;

            if (other == null)
            {
                return false;
            }

            if (!object.Equals(other._value, this._value))
            {
                return false;
            }

            return this._values.SequenceEqual(other._values);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this._values.Keys.Concat((this._value != null) ? TypeDescriptor.GetProperties(this._value).OfType<PropertyDescriptor>().Select(x => x.Name) : Enumerable.Empty<string>());
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            if (binder.Operation == ExpressionType.Equal)
            {
                result = object.Equals(this._value, arg);
                return true;
            }
            else if (binder.Operation == ExpressionType.NotEqual)
            {
                result = !object.Equals(this._value, arg);
                return true;
            }

            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            if (binder.Operation == ExpressionType.Increment)
            {
                if (this._value is short)
                {
                    result = (short)_value + 1;
                    return true;
                }
                else if (this._value is int)
                {
                    result = (int)_value + 1;
                    return true;
                }
                else if (this._value is long)
                {
                    result = (long)_value + 1;
                    return true;
                }
                else if (this._value is ushort)
                {
                    result = (ushort)_value + 1;
                    return true;
                }
                else if (this._value is uint)
                {
                    result = (uint)_value + 1;
                    return true;
                }
                else if (this._value is ulong)
                {
                    result = (ulong)_value + 1;
                    return true;
                }
                else if (this._value is decimal)
                {
                    result = (decimal)_value + 1;
                    return true;
                }
                else if (this._value is float)
                {
                    result = (float)_value + 1;
                    return true;
                }
                else if (this._value is double)
                {
                    result = (double)_value + 1;
                    return true;
                }
            }
            else if (binder.Operation == ExpressionType.Decrement)
            {
                if (this._value is short)
                {
                    result = (short)_value - 1;
                    return true;
                }
                else if (this._value is int)
                {
                    result = (int)_value - 1;
                    return true;
                }
                else if (this._value is long)
                {
                    result = (long)_value - 1;
                    return true;
                }
                else if (this._value is ushort)
                {
                    result = (ushort)_value - 1;
                    return true;
                }
                else if (this._value is uint)
                {
                    result = (uint)_value - 1;
                    return true;
                }
                else if (this._value is ulong)
                {
                    result = (ulong)_value - 1;
                    return true;
                }
                else if (this._value is decimal)
                {
                    result = (decimal)_value - 1;
                    return true;
                }
                else if (this._value is float)
                {
                    result = (float)_value - 1;
                    return true;
                }
                else if (this._value is double)
                {
                    result = (double)_value - 1;
                    return true;
                }
            }
            else if (binder.Operation == ExpressionType.Not)
            {
                if (this._value is bool)
                {
                    result = !(bool)_value;
                    return true;
                }
            }
            else if (binder.Operation == ExpressionType.OnesComplement)
            {
                if (this._value is short)
                {
                    result = ~(short)_value;
                    return true;
                }
                else if (this._value is int)
                {
                    result = ~(int)_value;
                    return true;
                }
                else if (this._value is long)
                {
                    result = ~(long)_value;
                    return true;
                }
                else if (this._value is ushort)
                {
                    result = ~(ushort)_value;
                    return true;
                }
                else if (this._value is uint)
                {
                    result = ~(uint)_value;
                    return true;
                }
                else if (this._value is ulong)
                {
                    result = ~(ulong)_value;
                    return true;
                }
            }

            return base.TryUnaryOperation(binder, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var method = this.GetType().GetMethod(binder.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                foreach (var type in this.GetType().GetInterfaces())
                {
                    method = type.GetMethod(binder.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (method != null)
                    {
                        break;
                    }
                }
            }

            if (method != null)
            {
                result = method.Invoke(this, args);

                return true;
            }
            else
            {
                return base.TryInvokeMember(binder, args, out result);
            }
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (this._value != null)
            {
                if (binder.Type.IsInstanceOfType(this._value))
                {
                    result = this._value;
                    return true;
                }
                else if (binder.Type.IsEnum)
                {
                    result = Enum.Parse(binder.Type, this._value.ToString());
                    return true;
                }
                else if ((typeof(IConvertible).IsAssignableFrom(binder.Type)) && (typeof(IConvertible).IsAssignableFrom(this._value.GetType())))
                {
                    result = Convert.ChangeType(this._value, binder.Type);
                    return true;
                }
                else if (binder.Type == typeof(string))
                {
                    result = this._value.ToString();
                    return true;
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(binder.Type);

                    if (converter.CanConvertFrom(this._value.GetType()))
                    {
                        result = converter.ConvertFrom(this._value);
                        return true;
                    }
                }
            }
            else if (binder.Type.IsClass)
            {
                result = null;
                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            (this as IDictionary<string, object>)[binder.Name] = value;

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (this._value != null)
            {
                var prop = TypeDescriptor.GetProperties(this._value)[binder.Name];

                if (prop != null)
                {
                    result = prop.GetValue(this._value);
                    return true;
                }
            }

            return this._values.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if ((indexes.Length != 1) || (indexes[0] == null))
            {
                return false;
            }

            var key = indexes[0] as string;

            if (indexes[0] is int)
            {
                var index = (int)indexes[0];

                if (this._values.Count < index)
                {
                    key = this._values.ElementAt(index).Key;
                }
            }
            else if (key == null)
            {
                return false;
            }

            (this as IDictionary<string, object>)[key] = value;

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if ((indexes.Length != 1) || (indexes[0] == null))
            {
                result = null;
                return false;
            }

            var key = indexes[0] as string;

            if (key != null)
            {
                if (this._value != null)
                {
                    var prop = TypeDescriptor.GetProperties(this._value)[key];

                    if (prop != null)
                    {
                        result = prop.GetValue(this._value);
                        return true;
                    }
                }

                if (string.Equals(Parent, key, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = this._parent;
                    return true;
                }
                else if (string.Equals(Value, key, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = this._value;
                    return true;
                }
                else if (string.Equals(Type, key, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = ((this._value != null) ? this._value.GetType() : null);
                    return true;
                }
                else if (string.Equals(Root, key, StringComparison.InvariantCultureIgnoreCase))
                {
                    var root = this;

                    while (root != null)
                    {
                        if (root._parent == null)
                        {
                            break;
                        }

                        root = root._parent;
                    }

                    result = root;
                    return true;
                }
                else if (string.Equals(Path, key, StringComparison.InvariantCultureIgnoreCase))
                {
                    var list = new LinkedList<string>();

                    var p = this._parent;
                    var previous = (object)this;

                    while (p != null)
                    {
                        var kv = p._values.SingleOrDefault(x => (object)x.Value == (object)previous);

                        list.AddFirst(kv.Key);

                        previous = ((FluidObject)kv.Value)._parent;
                        p = p._parent;
                    }

                    result = string.Join(".", list);
                    return true;
                }
                else
                {
                    return this._values.TryGetValue(key, out result);
                }
            }
            else if (indexes[0] is int)
            {
                var index = (int)indexes[0];

                if (this._values.Count < index)
                {
                    result = this._values.ElementAt(index).Value;
                    return true;
                }
            }

            result = null;
            return false;
        }

        void IDictionary<string, object>.Add(string key, object value)
        {
            (this as IDictionary<string, object>)[key] = value;
        }

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return this.GetDynamicMemberNames().Contains(key);
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get
            {
                return this.GetDynamicMemberNames().ToList();
            }
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return this._values.Remove(key);
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            if (this._value != null)
            {
                var prop = TypeDescriptor.GetProperties(this._value)[key];

                if (prop != null)
                {
                    value = prop.GetValue(this._value);
                    return true;
                }
            }

            return this._values.TryGetValue(key, out value);
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get
            {
                return this._values.Values.Concat((this._value != null) ? TypeDescriptor.GetProperties(this._value).OfType<PropertyDescriptor>().Select(x => x.GetValue(this._value)) : Enumerable.Empty<object>()).ToList();
            }
        }

        object IDictionary<string, object>.this[string key]
        {
            get
            {
                if (this._value != null)
                {
                    var prop = TypeDescriptor.GetProperties(this._value)[key];

                    if (prop != null)
                    {
                        return prop.GetValue(this._value);
                    }
                }

                return this._values[key];
            }
            set
            {
                if (value is FluidObject)
                {
                    this._values[key] = value;
                    ((FluidObject)value)._parent = this;
                }
                else if (value == null)
                {
                    this._values[key] = null;
                }
                else
                {
                    this._values[key] = new FluidObject(this, value);
                }

                this.OnPropertyChanged(new PropertyChangedEventArgs(key));
            }
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            (this as IDictionary<string, object>)[item.Key] = item.Value;
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            this._values.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return this._values.Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            this._values.CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<string, object>>.Count
        {
            get
            {
                return this._values.Count;
            }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get
            {
                return this._values.IsReadOnly;
            }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return this._values.Remove(item);
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return this._values.GetEnumerator();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        object ICloneable.Clone()
        {
            var clone = new FluidObject(null, this._value) as IDictionary<string, object>;

            foreach (var key in this._values.Keys)
            {
                clone[key] = (this._values[key] is ICloneable) ? (this._values[key] as ICloneable).Clone() : this._values[key];
            }

            return clone;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IDictionary<string, object>).GetEnumerator();
        }

        public static bool operator == (FluidObject fluid, object obj)
        {
            if (!(obj is FluidObject))
            {
                return fluid._value?.Equals(obj) == true;
            }

            return fluid.Equals(obj);
        }

        public static bool operator != (FluidObject fluid, object obj)
        {
            return !(fluid == obj);
        }
    }
}
