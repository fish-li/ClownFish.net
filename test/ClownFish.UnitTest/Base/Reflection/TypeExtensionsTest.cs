using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Reflection
{
    [TestClass]
    public class TypeExtensionsTest
    {
        [TestMethod]
        public void Test_GetArgumentType_Error()
        {
            Type t = typeof(TypeExtensionsTest);

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TypeExtensionsCF.GetArgumentType(null, t);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TypeExtensionsCF.GetArgumentType(t, null);
            });

            MyAssert.IsError<ArgumentException>(() => {
                _ = TypeExtensionsCF.GetArgumentType(t, t);
            });

            Assert.AreEqual(typeof(int), TypeExtensionsCF.GetArgumentType(typeof(List<int>), typeof(List<>)));

            Assert.IsNull(TypeExtensionsCF.GetArgumentType(t, typeof(List<>)));
        }

        [TestMethod]
        public void TestCanNew()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TypeExtensionsCF.CanNew(null);
            });

            Assert.IsTrue(TypeExtensionsCF.CanNew(typeof(HttpOption)));

            Assert.IsFalse(TypeExtensionsCF.CanNew(typeof(HttpOptionExtensions)));
            Assert.IsFalse(TypeExtensionsCF.CanNew(typeof(BaseHttpClient)));

            Assert.IsFalse(TypeExtensionsCF.CanNew(typeof(DataSpliter<string>)));
        }

        [TestMethod]
        public void Test_GetInstanceMethod()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TypeExtensionsCF.GetInstanceMethod(null, "xx");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TypeExtensionsCF.GetInstanceMethod(typeof(HttpOption), "");
            });
        }

        [TestMethod]
        public void Test_GetStaticMethod()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TypeExtensionsCF.GetStaticMethod(null, "xx");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TypeExtensionsCF.GetStaticMethod(typeof(HttpOption), "");
            });


            Assert.IsNotNull(TypeExtensionsCF.GetStaticMethod(typeof(AppConfig), "InitConfig"));

        }
    }
}
