using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ClownFish.Base;
using ClownFish.UnitTest.Base.Init;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: PreApplicationStartMethod(typeof(AppStartX), "Init2")]

namespace ClownFish.UnitTest.Base.Init
{
    [TestClass]
    public class ApplicationInitializerTest
    {
        [TestMethod]
        public void Test_Start()
        {
            ApplicationInitializer.Start();
            Assert.AreEqual(2, AppStartX.Flag);
        }

        [ExpectedException(typeof(InvalidProgramException))]
        [TestMethod]
        public void Test_Error()
        {
            PreApplicationStartMethodAttribute attr = new PreApplicationStartMethodAttribute(typeof(AppStartX), "xxx");
            ApplicationInitializer.Invoke(attr, typeof(AppStartX).Assembly);
        }

        [ExpectedException(typeof(InvalidProgramException))]
        [TestMethod]
        public void Test_Error2()
        {
            PreApplicationStartMethodAttribute attr = new PreApplicationStartMethodAttribute(typeof(AppStartX), "Init3");
            ApplicationInitializer.Invoke(attr, typeof(AppStartX).Assembly);
        }

        [TestMethod]
        public void Test_Error3()
        {
            MyAssert.IsError<ArgumentNullException>(()=> {
                _ = new PreApplicationStartMethodAttribute(null, "xx");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = new PreApplicationStartMethodAttribute(typeof(ApplicationInitializerTest), "");
            });
        }
    }



    public static class AppStartX
    {
        public static int Flag = 1;

        public static void Init2()
        {
            Flag = 2;
        }


        public static void Init3()
        {
            int a = 1;
            int b = DateTime.Now.Hour > 0 ? 0 : 1;
            int c = a / b;
            Console.WriteLine(c);
        }
    }
    
}
