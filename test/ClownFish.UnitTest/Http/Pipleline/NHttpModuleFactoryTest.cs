using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Http.Pipleline
{
    [TestClass]
    public class NHttpModuleFactoryTest
    {
        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<ArgumentNullException>(()=> {
                NHttpModuleFactory.RegisterModule(null);
            });

            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                NHttpModuleFactory.RegisterModule(typeof(string));
            });

            MyAssert.IsError<ArgumentException>(() => {
                NHttpModuleFactory.RegisterModule(typeof(XModule));
            });
        }


        public class XModule : NHttpModule
        {
            public XModule(string xx)
            {

            }
        }
    }
}
