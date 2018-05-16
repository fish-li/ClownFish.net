using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Log.UnitTest
{
    [TestClass]
    public class Initializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {

        }


        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            // 等待 HttpWriter的操作
            System.Threading.Thread.Sleep(2000);
        }
    }
}
