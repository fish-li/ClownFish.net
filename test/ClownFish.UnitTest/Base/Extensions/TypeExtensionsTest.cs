using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;
using System.Reflection;
using ClownFish.Data;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class TypeExtensionsTest
    {
        [TestMethod]
        public void Test_IsSimpleValueType()
        {
            Assert.IsTrue(typeof(DateTime).IsSimpleValueType());
            Assert.IsTrue(typeof(TimeSpan).IsSimpleValueType());
            Assert.IsTrue(typeof(Guid).IsSimpleValueType());
            Assert.IsTrue(typeof(decimal).IsSimpleValueType());
            Assert.IsTrue(typeof(DayOfWeek).IsSimpleValueType());
        }


        [TestMethod]
        public void Test_GetRealType()
        {
            Assert.AreEqual(typeof(int), typeof(int?).GetRealType());
            Assert.AreEqual(typeof(decimal), typeof(decimal?).GetRealType());
            Assert.AreEqual(typeof(DateTime), typeof(DateTime?).GetRealType());

            Assert.AreEqual(typeof(int), typeof(int).GetRealType());
            Assert.AreEqual(typeof(decimal), typeof(decimal).GetRealType());
            Assert.AreEqual(typeof(DateTime), typeof(DateTime).GetRealType());

            Assert.AreEqual(typeof(List<int>), typeof(List<int>).GetRealType());
            Assert.AreEqual(typeof(List<string>), typeof(List<string>).GetRealType());
        }


        [TestMethod]
        public void Test_IsNullableType()
        {
            Assert.IsTrue(typeof(int?).IsNullableType());
            Assert.IsTrue(typeof(TimeSpan?).IsNullableType());
            Assert.IsTrue(typeof(Guid?).IsNullableType());

            Assert.IsFalse(typeof(int).IsNullableType());
            Assert.IsFalse(typeof(TimeSpan).IsNullableType());
            Assert.IsFalse(typeof(Guid).IsNullableType());

            Assert.IsFalse(typeof(List<int>).IsNullableType());
            Assert.IsFalse(typeof(int[]).IsNullableType());
        }

        [TestMethod]
        public void Test_IsNullableEnum()
        {
            Assert.IsTrue(typeof(DayOfWeek?).IsNullableEnum());
            Assert.IsTrue(typeof(BindingFlags?).IsNullableEnum());


            Assert.IsFalse(typeof(DayOfWeek).IsNullableEnum());
            Assert.IsFalse(typeof(BindingFlags).IsNullableEnum());

            Assert.IsFalse(typeof(int).IsNullableEnum());
            Assert.IsFalse(typeof(TimeSpan).IsNullableEnum());
            Assert.IsFalse(typeof(Guid).IsNullableEnum());

            Assert.IsFalse(typeof(List<int>).IsNullableEnum());
            Assert.IsFalse(typeof(int[]).IsNullableEnum());
        }


        private abstract class Base1 { }
        private class Child1 : Base1 { }

#pragma warning disable IDE1006 // 命名样式
        private interface F1 { }
#pragma warning restore IDE1006 // 命名样式
        private class ClassX : F1 { }

        private class ClassX2 { }


        [TestMethod]
        public void Test_IsCompatible()
        {
            Assert.IsTrue(typeof(Child1).IsCompatible(typeof(Base1)));
            Assert.IsTrue(typeof(Child1).IsCompatible(typeof(Child1)));
            Assert.IsFalse(typeof(ClassX2).IsCompatible(typeof(Base1)));
            Assert.IsFalse(typeof(Base1).IsCompatible(typeof(Child1)));

            Assert.IsTrue(typeof(ClassX).IsCompatible(typeof(F1)));
            Assert.IsFalse(typeof(ClassX2).IsCompatible(typeof(F1)));
            Assert.IsFalse(typeof(F1).IsCompatible(typeof(ClassX)));

#if NETFRAMEWORK
            Assert.IsTrue(typeof(ClownFish.Base.WebClient.V1.HttpClient).IsCompatible(typeof(ClownFish.Base.WebClient.BaseHttpClient)));
#else
            Assert.IsTrue(typeof(ClownFish.Base.WebClient.V2.HttpClient2).IsCompatible(typeof(ClownFish.Base.WebClient.BaseHttpClient)));
#endif
        }

        [TestMethod]
        public void Test_GetFullName()
        {
            Assert.AreEqual("ClownFish.Base.RetryFile, ClownFish.net", typeof(RetryFile).GetFullName());
            Assert.AreEqual("ClownFish.Data.CPQuery, ClownFish.net", typeof(CPQuery).GetFullName());


            MyAssert.IsError<ArgumentNullException>(() => {
                _= TypeExtensionsCF.GetFullName(null);
            });
        }


        [TestMethod]
        public void Test_GetTypeCodeText()
        {
            Assert.AreEqual("ClownFish.Base.RetryFile", typeof(RetryFile).GetTypeCodeText());
            Assert.AreEqual("ClownFish.Data.CPQuery[]", typeof(CPQuery[]).GetTypeCodeText());
            Assert.AreEqual("System.Collections.Generic.List<ClownFish.Data.CPQuery>", typeof(List<CPQuery>).GetTypeCodeText());
            Assert.AreEqual("System.Collections.Generic.List<ClownFish.Data.CPQuery[]>", typeof(List<CPQuery[]>).GetTypeCodeText());
            Assert.AreEqual("System.Collections.Generic.Dictionary<int, ClownFish.Data.CPQuery[]>", typeof(Dictionary<int, CPQuery[]>).GetTypeCodeText());
            Assert.AreEqual("ClownFish.Base.CacheItem<ClownFish.Data.CPQuery[]>", typeof(CacheItem<CPQuery[]>).GetTypeCodeText());

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TypeExtensionsCF.GetTypeCodeText(null);
            });
        }
    }
}
