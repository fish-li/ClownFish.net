using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class ByteBufferTest
    {
        [TestMethod]
        public void Test()
        {
            using( ByteBuffer buffer = new ByteBuffer(32) ) {
                Assert.IsTrue(buffer.Buffer.Length >= 32);
            }

            MyAssert.IsError<ArgumentOutOfRangeException>(()=> {
                _ = new ByteBuffer(0);
            });
        }
    }

}
