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
    public class FunctionTest
    {
        [TestMethod]
        public void Test_FastInvoke_Static_Func()
        {
            MethodInfo fun0 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun0), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(1252574, (int)fun0.FastInvoke(null, null));
            Assert.AreEqual(1252574, (int)fun0.FastInvoke2(null, null));

            MethodInfo fun1 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun1), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(11, (int)fun1.FastInvoke(null, new object[] { 1 }));
            Assert.AreEqual(11, (int)fun1.FastInvoke2(null, new object[] { 1 }));

            MethodInfo fun2 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun2), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(13, (int)fun2.FastInvoke(null, new object[] { 1, 2 }));
            Assert.AreEqual(13, (int)fun2.FastInvoke2(null, new object[] { 1, 2 }));

            MethodInfo fun3 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun3), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(16, (int)fun3.FastInvoke(null, new object[] { 1, 2, 3 }));
            Assert.AreEqual(16, (int)fun3.FastInvoke2(null, new object[] { 1, 2, 3 }));

            MethodInfo fun4 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun4), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(20, (int)fun4.FastInvoke(null, new object[] { 1, 2, 3, 4 }));
            Assert.AreEqual(20, (int)fun4.FastInvoke2(null, new object[] { 1, 2, 3, 4 }));

            MethodInfo fun5 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun5), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(25, (int)fun5.FastInvoke(null, new object[] { 1, 2, 3, 4, 5 }));
            Assert.AreEqual(25, (int)fun5.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5 }));

            MethodInfo fun6 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun6), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(31, (int)fun6.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6 }));
            Assert.AreEqual(31, (int)fun6.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6 }));

            MethodInfo fun7 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun7), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(38, (int)fun7.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7 }));
            Assert.AreEqual(38, (int)fun7.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6, 7 }));

            MethodInfo fun8 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun8), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(46, (int)fun8.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 }));
            Assert.AreEqual(46, (int)fun8.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 }));

            MethodInfo fun9 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun9), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(55, (int)fun9.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
            Assert.AreEqual(55, (int)fun9.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));

            MethodInfo fun10 = typeof(StaticObject).GetMethod(nameof(StaticObject.Fun10), BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(65, (int)fun10.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));
            Assert.AreEqual(65, (int)fun10.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));
        }


        [TestMethod]
        public void Test_FastInvoke_Instace_Func()
        {
            InstanceObject test = new InstanceObject(123);

            MethodInfo fun0 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun0), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(1252574, (int)fun0.FastInvoke(test, null));
            Assert.AreEqual(1252574, (int)fun0.FastInvoke2(test, null));
            MyAssert.IsError<ArgumentNullException>(() => fun0.FastInvoke(null, null));

            MethodInfo fun1 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun1), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(11, (int)fun1.FastInvoke(test, new object[] { 1 }));
            Assert.AreEqual(11, (int)fun1.FastInvoke2(test, new object[] { 1 }));
            MyAssert.IsError<ArgumentNullException>(() => fun1.FastInvoke(null, new object[] { 1 }));

            MethodInfo fun2 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun2), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(13, (int)fun2.FastInvoke(test, new object[] { 1, 2 }));
            Assert.AreEqual(13, (int)fun2.FastInvoke2(test, new object[] { 1, 2 }));
            MyAssert.IsError<ArgumentNullException>(() => fun2.FastInvoke(null, new object[] { 1, 2 }));

            MethodInfo fun3 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun3), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(16, (int)fun3.FastInvoke(test, new object[] { 1, 2, 3 }));
            Assert.AreEqual(16, (int)fun3.FastInvoke2(test, new object[] { 1, 2, 3 }));
            MyAssert.IsError<ArgumentNullException>(() => fun3.FastInvoke(null, new object[] { 1, 2, 3 }));

            MethodInfo fun4 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun4), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(20, (int)fun4.FastInvoke(test, new object[] { 1, 2, 3, 4 }));
            Assert.AreEqual(20, (int)fun4.FastInvoke2(test, new object[] { 1, 2, 3, 4 }));
            MyAssert.IsError<ArgumentNullException>(() => fun4.FastInvoke(null, new object[] { 1, 2, 3, 4 }));

            MethodInfo fun5 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun5), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(25, (int)fun5.FastInvoke(test, new object[] { 1, 2, 3, 4, 5 }));
            Assert.AreEqual(25, (int)fun5.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5 }));
            MyAssert.IsError<ArgumentNullException>(() => fun5.FastInvoke(null, new object[] { 1, 2, 3, 4, 5 }));

            MethodInfo fun6 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun6), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(31, (int)fun6.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6 }));
            Assert.AreEqual(31, (int)fun6.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6 }));
            MyAssert.IsError<ArgumentNullException>(() => fun6.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6 }));

            MethodInfo fun7 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun7), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(38, (int)fun7.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6, 7 }));
            Assert.AreEqual(38, (int)fun7.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6, 7 }));
            MyAssert.IsError<ArgumentNullException>(() => fun7.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7 }));

            MethodInfo fun8 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun8), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(46, (int)fun8.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 }));
            Assert.AreEqual(46, (int)fun8.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 }));
            MyAssert.IsError<ArgumentNullException>(() => fun8.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 }));

            MethodInfo fun9 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun9), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(55, (int)fun9.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
            Assert.AreEqual(55, (int)fun9.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
            MyAssert.IsError<ArgumentNullException>(() => fun9.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));

            MethodInfo fun10 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun10), BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(65, (int)fun10.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));
            Assert.AreEqual(65, (int)fun10.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));
            MyAssert.IsError<ArgumentNullException>(() => fun10.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));

        }


        [TestMethod]
        public void Test_StronglyTyped_Function()
        {
            InstanceObject test = new InstanceObject(123);

            MethodInfo fun0 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun0), BindingFlags.Instance | BindingFlags.Public);
            FunctionWrapper<InstanceObject, int> wrapper0 = new FunctionWrapper<InstanceObject, int>();
            wrapper0.BindMethod(fun0);
            Assert.AreEqual(1252574, wrapper0.Call(test));
            MyAssert.IsError<ArgumentNullException>(() => wrapper0.Call(null));
            MyAssert.IsError<ArgumentNullException>(() => wrapper0.BindMethod(null));


            MethodInfo fun1 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun1), BindingFlags.Instance | BindingFlags.Public);
            FunctionWrapper<InstanceObject, int, int> wrapper1 = new FunctionWrapper<InstanceObject, int, int>();
            wrapper1.BindMethod(fun1);
            Assert.AreEqual(11, wrapper1.Call(test, 1));
            MyAssert.IsError<ArgumentNullException>(() => wrapper1.Call(null, 1));

            MethodInfo fun2 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun2), BindingFlags.Instance | BindingFlags.Public);
            FunctionWrapper<InstanceObject, int, int, int> wrapper2 = new FunctionWrapper<InstanceObject, int, int, int>();
            wrapper2.BindMethod(fun2);
            Assert.AreEqual(13, wrapper2.Call(test, 1, 2));
            MyAssert.IsError<ArgumentNullException>(() => wrapper2.Call(null, 1, 2));


            MethodInfo fun3 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Fun3), BindingFlags.Instance | BindingFlags.Public);
            FunctionWrapper<InstanceObject, int, int, int, int> wrapper3 = new FunctionWrapper<InstanceObject, int, int, int, int>();
            wrapper3.BindMethod(fun3);
            Assert.AreEqual(16, wrapper3.Call(test, 1, 2, 3));
            MyAssert.IsError<ArgumentNullException>(() => wrapper3.Call(null, 1, 2, 3));

        }
    }
}
