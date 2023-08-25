using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class ResourceLockTest
    {
        [TestMethod]
        public void Test()
        {
            string key1 = "11111111111";
            string key2 = key1.ToBase64().FromBase64();
            string key3 = "222222222222";

            Assert.AreEqual(key1, key2);

            // ResourceLock 主要是用于在并发场景下获取资源锁，
            // 但是这里就不再模拟并发了，因为我不想再去测试 ConcurrentDictionary
            // 这里只检查相同的KEY能拿到同一个锁对象就可以了

            ResourceLock resource = new ResourceLock();
            object lock1 = resource.GetLock(key1);
            object lock2 = resource.GetLock(key2);
            object lock3 = resource.GetLock(key3);

            Assert.IsTrue(object.ReferenceEquals(lock1, lock2));
            Assert.IsFalse(object.ReferenceEquals(lock1, lock3));
        }



    }
}
