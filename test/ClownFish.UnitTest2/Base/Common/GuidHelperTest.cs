using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class GuidHelperTest
    {
        [TestMethod]
        public void Test()
        {
            // 有序GUID如何判断，还没有想到比较好的方法，这里先只是覆盖下代码
            Guid guid = GuidHelper.NewSeqGuid();
            Assert.IsFalse(guid == Guid.Empty);
        }
    }
}
