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

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj) == true)
            {
                return true;
            }

            if (!(obj is FluidObject other))
            {
                return false;
            }

            if (object.Equals(other._value, this._value) == false)
            {
                return false;
            }

            return this._values.SequenceEqual(other._values);
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
                    result = (short)this._value + 1;
                    return true;
                }
                else if (this._value is int)
                {
                    result = (int)this._value + 1;
                    return true;
                }
                else if (this._value is long)
                {
                    result = (long)this._value + 1;
                    return true;
                }
                else if (this._value is ushort)
                {
                    result = (ushort)this._value + 1;
                    return true;
                }
                else if (this._value is uint)
                {
                    result = (uint)this._value + 1;
                    return true;
                }
                else if (this._value is ulong)
                {
                    result = (ulong)this._value + 1;
                    return true;
                }
                else if (this._value is decimal)
                {
                    result = (decimal)this._value + 1;
                    return true;
                }
                else if (this._value is float)
                {
                    result = (float)this._value + 1;
                    return true;
                }
                else if (this._value is double)
                {
                    result = (double)this._value + 1;
                    return true;
                }
            }
            else if (binder.Operation == ExpressionType.Decrement)
            {
                if (this._value is short)
                {
                    result = (short)this._value - 1;
                    return true;
                }
                else if (this._value is int)
                {
                    result = (int)this._value - 1;
                    return true;
                }
                else if (this._value is long)
                {
                    result = (long)this._value - 1;
                    return true;
                }
                else if (this._value is ushort)
                {
                    result = (ushort)this._value - 1;
                    return true;
                }
                else if (this._value is uint)
                {
                    result = (uint)this._value - 1;
                    return true;
                }
                else if (this._value is ulong)
                {
                    result = (ulong)this._value - 1;
                    return true;
                }
                else if (this._value is decimal)
                {
                    result = (decimal)this._value - 1;
                    return true;
                }
                else if (this._value is float)
                {
                    result = (float)this._value - 1;
                    return true;
                }
                else if (this._value is double)
                {
                    result = (double)this._value - 1;
                    return true;
                }
            }
            else if (binder.Operation == ExpressionType.Not)
            {
                if (this._value is bool)
                {
                    result = !(bool)this._value;
                    return true;
                }
                //TODO: support numeric types?
            }
            else if (binder.Operation == ExpressionType.OnesComplement)
            {
                if (this._value is short)
                {
                    result = ~(short)this._value;
                    return true;
                }
                else if (this._value is int)
                {
                    result = ~(int)this._value;
                    return true;
                }
                else if (this._value is long)
                {
                    result = ~(long)this._value;
                    return true;
                }
                else if (this._value is ushort)
                {
                    result = ~(ushort)this._value;
                    return true;
                }
                else if (this._value is uint)
                {
                    result = ~(uint)this._value;
                    return true;
                }
                else if (this._value is ulong)
                {
                    result = ~(ulong)this._value;
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
                if (binder.Type.IsInstanceOfType(this._value) == true)
                {
                    result = this._value;
                    return true;
                }
                else if (binder.Type.IsEnum == true)
                {
                    result = Enum.Parse(binder.Type, this._value.ToString());
                    return true;
                }
                else if ((typeof(IConvertible).IsAssignableFrom(binder.Type) == true) && (typeof(IConvertible).IsAssignableFrom(this._value.GetType()) == true))
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

                    if (converter.CanConvertFrom(this._value.GetType()) == true)
                    {
                        result = converter.ConvertFrom(this._value);
                        return true;
                    }
                }
            }
            else if (binder.Type.IsClass == true)
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

            if (indexes[0] is int index)
            {
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

            if (indexes[0] is string key)
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

                if (string.Equals(Parent, key, StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    result = this._parent;
                    return true;
                }
                else if (string.Equals(Value, key, StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    result = this._value;
                    return true;
                }
                else if (string.Equals(Type, key, StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    result = this._value?.GetType();
                    return true;
                }
                else if (string.Equals(Root, key, StringComparison.InvariantCultureIgnoreCase) == true)
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
                else if (string.Equals(Path, key, StringComparison.InvariantCultureIgnoreCase) == true)
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
            else if (indexes[0] is int index)
            {
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

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            (this as IDictionary<string, object>)[item.Key] = item.Value;
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            this._values.Clear();
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            this._values.CopyTo(array, arrayIndex);
        }

        object ICloneable.Clone()
        {
            var clone = new FluidObject(null, this._value) as IDictionary<string, object>;

            foreach (var key in this._values.Keys)
            {
                clone[key] = (this._values[key] is ICloneable) ? (this._values[key] as ICloneable).Clone() : this._values[key];
            }

            return clone;
        }

        public override int GetHashCode() => this._value?.GetHashCode() ?? 0;

        public override IEnumerable<string> GetDynamicMemberNames() => this._values.Keys.Concat((this._value != null) ? TypeDescriptor.GetProperties(this._value).OfType<PropertyDescriptor>().Select(x => x.Name) : Enumerable.Empty<string>());

        bool IDictionary<string, object>.ContainsKey(string key) => this.GetDynamicMemberNames().Contains(key);

        ICollection<string> IDictionary<string, object>.Keys => this.GetDynamicMemberNames().ToList();

        bool IDictionary<string, object>.Remove(string key) => this._values.Remove(key);

        ICollection<object> IDictionary<string, object>.Values => this._values.Values.Concat((this._value != null) ? TypeDescriptor.GetProperties(this._value).OfType<PropertyDescriptor>().Select(x => x.GetValue(this._value)) : Enumerable.Empty<object>()).ToList();

        private void OnPropertyChanged(PropertyChangedEventArgs e) => this.PropertyChanged?.Invoke(this, e);

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) => this._values.Contains(item);

        int ICollection<KeyValuePair<string, object>>.Count => this._values.Count;

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => this._values.IsReadOnly;

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) => this._values.Remove(item);

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => this._values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => (this as IDictionary<string, object>).GetEnumerator();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}