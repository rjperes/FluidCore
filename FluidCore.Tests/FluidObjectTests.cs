using System;
using System.ComponentModel;
using Xunit;

namespace FluidCore.Tests
{
    public class FluidObjectTests
    {
        [Fact]
        static void CanGetRoot()
        {
            dynamic obj = new FluidObject();
            obj.A = "1";
            obj.A.B = 0;

            var root = obj.A.B[FluidObject.Root];

            Assert.Equal(obj, root);
        }

        [Fact]
        static void CanChangeType()
        {
            dynamic obj = new FluidObject();
            obj.A = "1";
            obj.A = 1;

            Assert.IsType<int>(obj.A[FluidObject.Value]);
        }

        [Fact]
        static void CanGetPath()
        {
            dynamic obj = new FluidObject();
            obj.A = "1";
            obj.A.B = 0;

            var path = obj.A.B[FluidObject.Path];

            Assert.Equal("A.B", path);
        }

        [Fact]
        static void CanCompare()
        {
            dynamic obj = new FluidObject(1);
            bool equal = obj == 1;

            Assert.True(equal);
        }

        [Fact]
        static void CanGetParent()
        {
            dynamic obj = new FluidObject();
            obj.A = "1";

            var parent = obj.A[FluidObject.Parent];
        }

        [Fact]
        static void CanGetValue()
        {
            dynamic obj = new FluidObject();
            obj.A = "1";

            var parent = obj.A[FluidObject.Value];
        }

        [Fact]
        static void CanGetType()
        {
            dynamic obj = new FluidObject();
            obj.A = "1";

            Type type = obj.A[FluidObject.Type];

            Assert.Equal(typeof(string), type);
        }

        [Fact]
        static void CanConvert()
        {
            dynamic obj = new FluidObject(1);
            int i = obj;
            float f = obj;
            string s = obj;
            DayOfWeek dow = obj;
            decimal d = obj;
            bool b = obj;

            Assert.Equal(1, i);
            Assert.Equal(1f, i);
            Assert.Equal("1", s);
            Assert.Equal(DayOfWeek.Monday, dow);
            Assert.Equal(1m, d);
            Assert.Equal(true, b);
        }

        [Fact]
        static void CanAccessImmediateMembers()
        {
            dynamic obj = new FluidObject();
            obj.A = "1";
        }

        [Fact]
        static void CanAccessDeepMembers()
        {
            dynamic obj = new FluidObject();
            obj.A = 1;
            obj.A.B = "1";

            Assert.Equal("1", obj.A.B[FluidObject.Value]);
        }

        [Fact]
        static void CanDynamicAccessMembers()
        {
            dynamic obj = new FluidObject();
            obj.A = "A";

            var a = obj["A"];

            Assert.Equal("A", a[FluidObject.Value]);
        }

        [Fact]
        static void CanUseAnonymous()
        {
            dynamic obj = new FluidObject(new { A = 1 });
            int i = obj.A;

            Assert.Equal(1, i);
        }

        [Fact]
        static void CanDoUnaryOperations()
        {
            dynamic obj = new FluidObject(1);
            int n = ~obj;
            int prei = ++obj;
            int pred = --obj;
            int posi = obj++;
            int posd = obj--;

            Assert.Equal(2, posd);
        }

        [Fact]
        static void CanDoBinaryOperations()
        {
            dynamic obj = new FluidObject(1);
            bool b1 = obj == 1;
            bool b2 = obj != null;

            Assert.True(b1);
            Assert.True(b2);
        }

        [Fact]
        static void CanUseTypeDescriptor()
        {
            dynamic obj = new FluidObject();
            obj.Name = "ricardo";
            var provider = TypeDescriptor.GetProvider(obj) as TypeDescriptionProvider;
            var descriptor = provider.GetTypeDescriptor(obj) as ICustomTypeDescriptor;
            var converter = descriptor.GetConverter() as TypeConverter;
            var attrs = TypeDescriptor.GetAttributes(obj);
            var props = TypeDescriptor.GetProperties(obj) as PropertyDescriptorCollection;
            var prop = props["Name"] as PropertyDescriptor;
            string value = (string)prop.GetValue(obj);

            Assert.Equal("ricardo", value);
        }

        [Fact]
        static void CanUseNotifyPropertyChanged()
        {
            dynamic obj = new FluidObject();
            var set = false;

            (obj as INotifyPropertyChanged).PropertyChanged += (s, e) =>
            {
                set = true;
            };

            obj.Name = "ricardo";

            Assert.True(set);
        }
    }
}
