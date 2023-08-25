using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ClownFish.UnitTest.Base.Reflection
{
    [TestClass]
    public class FieldTest
    {
        [TestMethod]
        public void Test_Instace_FastGetSet()
        {
            DataObject data = (DataObject)typeof(DataObject).FastNew();
            data.Value1 = 10L;

            FieldInfo field = typeof(DataObject).GetField(nameof(data.Value1), BindingFlags.Instance | BindingFlags.Public);
            object value = field.FastGetValue(data);
            Assert.IsTrue(value.GetType() == typeof(long));
            Assert.AreEqual(10L, (long)value);

            field.FastSetValue(data, 20L);
            value = field.FastGetValue(data);
            Assert.AreEqual(20L, (long)value);
        }

        [TestMethod]
        public void Test_Static_FastGetSet()
        {
            DataObject.Value2 = 10L;

            FieldInfo field = typeof(DataObject).GetField(nameof(DataObject.Value2), BindingFlags.Static | BindingFlags.Public);
            object value = field.FastGetValue(null);
            Assert.IsTrue(value.GetType() == typeof(long));
            Assert.AreEqual(10L, (long)value);

            field.FastSetValue(null, 20L);
            value = field.FastGetValue(null);
            Assert.AreEqual(20L, (long)value);
        }


        [TestMethod]
        public void Test_DynamicMethodFactory_Error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = DynamicMethodFactory.CreateFieldGetter(null);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = DynamicMethodFactory.CreateFieldSetter(null);
            });            
        }



        [TestMethod]
        public void Test_Extensions_ArgumentNullException()
        {
            FieldInfo fieldInfo = null;
            object x = new object();
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = fieldInfo.FastGetValue(x);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                fieldInfo.FastSetValue(x, (object)2);
            });
        }
    }
}
