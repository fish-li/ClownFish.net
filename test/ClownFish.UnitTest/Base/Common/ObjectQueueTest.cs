using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class ObjectQueueTest
    {
        [TestMethod]
        public void Test()
        {
            ObjectQueue queue = new ObjectQueue(2);

            Assert.IsTrue(queue.Enqueue(new NameValue("key1", "abc")));
            Assert.IsTrue(queue.Enqueue(new NameValue("key2", "123")));
            Assert.IsFalse(queue.Enqueue(new NameValue("key3", "333")));

            NameValue item = (NameValue)queue.Dequeue();
            Assert.AreEqual("key1", item.Name);

            item = (NameValue)queue.Dequeue();
            Assert.AreEqual("key2", item.Name);

            item = (NameValue)queue.Dequeue();
            Assert.IsNull(item);

        }
    }
}
