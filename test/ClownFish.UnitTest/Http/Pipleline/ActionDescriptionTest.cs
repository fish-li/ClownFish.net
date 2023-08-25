using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;
using ClownFish.UnitTest.Http.Pipleline.Test;

namespace ClownFish.UnitTest.Http.Pipleline
{
    [TestClass]
    public class ActionDescriptionTest
    {
        [TestMethod]
        public void Test1()
        {
            MethodInfo method = typeof(TestController).GetInstanceMethod("Action1");
            ActionDescription action = new ActionDescription(typeof(TestController), method);

            Assert.AreEqual(typeof(TestController), action.ControllerType);
            Assert.AreEqual("Action1", action.MethodInfo.Name);
            Assert.AreEqual("xx1", action.GetActionAttribute<TestMvcAttribute>().X1);
        }

        [TestMethod]
        public void Test2()
        {
            MethodInfo method = typeof(TestController).GetInstanceMethod("Action2");
            ActionDescription action = new ActionDescription(typeof(TestController), method);

            Assert.AreEqual(typeof(TestController), action.ControllerType);
            Assert.AreEqual("Action2", action.MethodInfo.Name);
            Assert.AreEqual("root", action.GetActionAttribute<TestMvcAttribute>().X1);
        }

        [TestMethod]
        public void Test3()
        {
            MethodInfo method = typeof(TestController2).GetInstanceMethod("Action1");
            ActionDescription action = new ActionDescription(typeof(TestController2), method);

            Assert.AreEqual("Action1", action.MethodInfo.Name);
            Assert.IsNull(action.GetActionAttribute<TestMvcAttribute>());
        }


        [TestMethod]
        public void Test_Error1()
        {
            Type type = typeof(TestController);
            MethodInfo method = typeof(TestController).GetInstanceMethod("Action1");

            MyAssert.IsError<ArgumentNullException>(()=> {
                var x = new ActionDescription((Type)null, method);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                var x = new ActionDescription(type, (MethodInfo)null);
            });
        }


        [TestMethod]
        public void Test_Error2()
        {
            Type type = typeof(TestController);
            MethodInfo method = typeof(TestController).GetInstanceMethod("Action1");
            TestController controller = new TestController();

            MyAssert.IsError<ArgumentNullException>(() => {
                var x = new ActionDescription((object)null, method, type);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                var x = new ActionDescription(controller, (MethodInfo)null, type);
            });


            ActionDescription action1 = new ActionDescription(controller, method);
            ActionDescription action2 = new ActionDescription(controller, method, type);
            Assert.AreEqual(action1.ControllerType, action2.ControllerType);
        }
    }
}
