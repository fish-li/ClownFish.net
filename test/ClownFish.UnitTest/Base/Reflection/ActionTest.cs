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
    public class ActionTest
    {

        [TestMethod]
        public void Test_FastInvoke_Static_Action()
        {
            MethodInfo action0 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action0), BindingFlags.Static | BindingFlags.Public);
            action0.FastInvoke(null, null);
            Assert.AreEqual(1252574, StaticObject.Result);

            StaticObject.Result = 0;
            action0.FastInvoke2(null, null);
            Assert.AreEqual(1252574, StaticObject.Result);

            MethodInfo action1 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action1), BindingFlags.Static | BindingFlags.Public);
            action1.FastInvoke(null, new object[] { 1 });
            Assert.AreEqual(11, StaticObject.Result);

            StaticObject.Result = 0;
            action1.FastInvoke2(null, new object[] { 1 });
            Assert.AreEqual(11, StaticObject.Result);

            MethodInfo action2 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action2), BindingFlags.Static | BindingFlags.Public);
            action2.FastInvoke(null, new object[] { 1, 2 });
            Assert.AreEqual(13, StaticObject.Result);

            StaticObject.Result = 0;
            action2.FastInvoke2(null, new object[] { 1, 2 });
            Assert.AreEqual(13, StaticObject.Result);

            MethodInfo action3 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action3), BindingFlags.Static | BindingFlags.Public);
            action3.FastInvoke(null, new object[] { 1, 2, 3 });
            Assert.AreEqual(16, StaticObject.Result);

            StaticObject.Result = 0;
            action3.FastInvoke2(null, new object[] { 1, 2, 3 });
            Assert.AreEqual(16, StaticObject.Result);

            MethodInfo action4 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action4), BindingFlags.Static | BindingFlags.Public);
            action4.FastInvoke(null, new object[] { 1, 2, 3, 4 });
            Assert.AreEqual(20, StaticObject.Result);

            StaticObject.Result = 0;
            action4.FastInvoke2(null, new object[] { 1, 2, 3, 4 });
            Assert.AreEqual(20, StaticObject.Result);

            MethodInfo action5 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action5), BindingFlags.Static | BindingFlags.Public);
            action5.FastInvoke(null, new object[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(25, StaticObject.Result);

            StaticObject.Result = 0;
            action5.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(25, StaticObject.Result);

            MethodInfo action6 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action6), BindingFlags.Static | BindingFlags.Public);
            action6.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6 });
            Assert.AreEqual(31, StaticObject.Result);

            StaticObject.Result = 0;
            action6.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6 });
            Assert.AreEqual(31, StaticObject.Result);

            MethodInfo action7 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action7), BindingFlags.Static | BindingFlags.Public);
            action7.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7 });
            Assert.AreEqual(38, StaticObject.Result);

            StaticObject.Result = 0;
            action7.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6, 7 });
            Assert.AreEqual(38, StaticObject.Result);

            MethodInfo action8 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action8), BindingFlags.Static | BindingFlags.Public);
            action8.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            Assert.AreEqual(46, StaticObject.Result);

            StaticObject.Result = 0;
            action8.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            Assert.AreEqual(46, StaticObject.Result);

            MethodInfo action9 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action9), BindingFlags.Static | BindingFlags.Public);
            action9.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.AreEqual(55, StaticObject.Result);

            StaticObject.Result = 0;
            action9.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.AreEqual(55, StaticObject.Result);

            MethodInfo action10 = typeof(StaticObject).GetMethod(nameof(StaticObject.Action10), BindingFlags.Static | BindingFlags.Public);
            action10.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.AreEqual(65, StaticObject.Result);

            StaticObject.Result = 0;
            action10.FastInvoke2(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.AreEqual(65, StaticObject.Result);
        }

        [TestMethod]
        public void Test_FastInvoke_Instance_Action()
        {
            InstanceObject test = new InstanceObject(123);

            MethodInfo action0 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action0), BindingFlags.Instance | BindingFlags.Public);
            action0.FastInvoke(test, null);
            Assert.AreEqual(1252574, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action0.FastInvoke(null, null); });

            test.Result = 0;
            action0.FastInvoke2(test, null);
            Assert.AreEqual(1252574, test.Result);

            MethodInfo action1 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action1), BindingFlags.Instance | BindingFlags.Public);
            action1.FastInvoke(test, new object[] { 1 });
            Assert.AreEqual(11, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action1.FastInvoke(null, new object[] { 1 }); });

            test.Result = 0;
            action1.FastInvoke2(test, new object[] { 1 });
            Assert.AreEqual(11, test.Result);

            MethodInfo action2 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action2), BindingFlags.Instance | BindingFlags.Public);
            action2.FastInvoke(test, new object[] { 1, 2 });
            Assert.AreEqual(13, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action2.FastInvoke(null, new object[] { 1, 2 }); });

            test.Result = 0;
            action2.FastInvoke2(test, new object[] { 1, 2 });
            Assert.AreEqual(13, test.Result);

            MethodInfo action3 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action3), BindingFlags.Instance | BindingFlags.Public);
            action3.FastInvoke(test, new object[] { 1, 2, 3 });
            Assert.AreEqual(16, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action3.FastInvoke(null, new object[] { 1, 2, 3 }); });

            test.Result = 0;
            action3.FastInvoke2(test, new object[] { 1, 2, 3 });
            Assert.AreEqual(16, test.Result);

            MethodInfo action4 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action4), BindingFlags.Instance | BindingFlags.Public);
            action4.FastInvoke(test, new object[] { 1, 2, 3, 4 });
            Assert.AreEqual(20, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action4.FastInvoke(null, new object[] { 1, 2, 3, 4 }); });

            test.Result = 0;
            action4.FastInvoke2(test, new object[] { 1, 2, 3, 4 });
            Assert.AreEqual(20, test.Result);

            MethodInfo action5 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action5), BindingFlags.Instance | BindingFlags.Public);
            action5.FastInvoke(test, new object[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(25, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action5.FastInvoke(null, new object[] { 1, 2, 3, 4, 5 }); });

            test.Result = 0;
            action5.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(25, test.Result);

            MethodInfo action6 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action6), BindingFlags.Instance | BindingFlags.Public);
            action6.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6 });
            Assert.AreEqual(31, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action6.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6 }); });

            test.Result = 0;
            action6.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6 });
            Assert.AreEqual(31, test.Result);

            MethodInfo action7 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action7), BindingFlags.Instance | BindingFlags.Public);
            action7.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6, 7 });
            Assert.AreEqual(38, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action7.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7 }); });


            test.Result = 0;
            action7.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6, 7 });
            Assert.AreEqual(38, test.Result);

            MethodInfo action8 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action8), BindingFlags.Instance | BindingFlags.Public);
            action8.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            Assert.AreEqual(46, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action8.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 }); });


            test.Result = 0;
            action8.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            Assert.AreEqual(46, test.Result);

            MethodInfo action9 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action9), BindingFlags.Instance | BindingFlags.Public);
            action9.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.AreEqual(55, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action9.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }); });


            test.Result = 0;
            action9.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.AreEqual(55, test.Result);

            MethodInfo action10 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action10), BindingFlags.Instance | BindingFlags.Public);
            action10.FastInvoke(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.AreEqual(65, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => { action10.FastInvoke(null, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }); });


            test.Result = 0;
            action10.FastInvoke2(test, new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.AreEqual(65, test.Result);
        }


        [TestMethod]
        public void Test_StronglyTyped_Action()
        {
            InstanceObject test = new InstanceObject(123);

            MethodInfo action0 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action0), BindingFlags.Instance | BindingFlags.Public);
            ActionWrapper<InstanceObject> wrapper0 = new ActionWrapper<InstanceObject>();
            wrapper0.BindMethod(action0);

            test.Result = 0;
            wrapper0.Call(test);
            Assert.AreEqual(1252574, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => wrapper0.Call(null));


            MethodInfo action1 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action1), BindingFlags.Instance | BindingFlags.Public);
            ActionWrapper<InstanceObject, int> wrapper1 = new ActionWrapper<InstanceObject, int>();
            wrapper1.BindMethod(action1);

            test.Result = 0;
            wrapper1.Call(test, 1);
            Assert.AreEqual(11, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => wrapper1.Call(null, 1));


            MethodInfo action2 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action2), BindingFlags.Instance | BindingFlags.Public);
            ActionWrapper<InstanceObject, int, int> wrapper2 = new ActionWrapper<InstanceObject, int, int>();
            wrapper2.BindMethod(action2);

            test.Result = 0;
            wrapper2.Call(test, 1, 2);
            Assert.AreEqual(13, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => wrapper2.Call(null, 1, 2));


            MethodInfo action3 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action3), BindingFlags.Instance | BindingFlags.Public);
            ActionWrapper<InstanceObject, int, int, int> wrapper3 = new ActionWrapper<InstanceObject, int, int, int>();
            wrapper3.BindMethod(action3);

            test.Result = 0;
            wrapper3.Call(test, 1, 2, 3);
            Assert.AreEqual(16, test.Result);
            MyAssert.IsError<ArgumentNullException>(() => wrapper3.Call(null, 1, 2, 3));
        }
    }
}
