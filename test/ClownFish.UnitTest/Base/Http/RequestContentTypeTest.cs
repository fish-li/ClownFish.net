using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using ClownFish.Base.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Http
{
    [TestClass]
    public class RequestContentTypeTest
    {
        [TestMethod]
        public void Test_GetFormat()
        {
            Assert.AreEqual(SerializeFormat.None, RequestContentType.GetFormat(null));
            Assert.AreEqual(SerializeFormat.None, RequestContentType.GetFormat(string.Empty));

            Assert.AreEqual(SerializeFormat.Json, RequestContentType.GetFormat("application/json"));
            Assert.AreEqual(SerializeFormat.Json, RequestContentType.GetFormat("application/json; charset=utf-8"));

            Assert.AreEqual(SerializeFormat.Xml, RequestContentType.GetFormat("application/xml"));
            Assert.AreEqual(SerializeFormat.Xml, RequestContentType.GetFormat("application/xml; charset=utf-8"));

            Assert.AreEqual(SerializeFormat.Form, RequestContentType.GetFormat("application/x-www-form-urlencoded"));
            Assert.AreEqual(SerializeFormat.Form, RequestContentType.GetFormat("application/x-www-form-urlencoded; charset=utf-8"));

            Assert.AreEqual(SerializeFormat.Multipart, RequestContentType.GetFormat("multipart/form-data; boundary=xxxxx"));

            Assert.AreEqual(SerializeFormat.Binary, RequestContentType.GetFormat("application/octet-stream"));

            Assert.AreEqual(SerializeFormat.Text, RequestContentType.GetFormat("text/plain"));
            Assert.AreEqual(SerializeFormat.Text, RequestContentType.GetFormat("text/plain; charset=utf-8"));

            Assert.AreEqual(SerializeFormat.Unknown, RequestContentType.GetFormat("application/xx"));
            Assert.AreEqual(SerializeFormat.Unknown, RequestContentType.GetFormat("multipart/xx"));
            Assert.AreEqual(SerializeFormat.Unknown, RequestContentType.GetFormat("text/xx"));
            Assert.AreEqual(SerializeFormat.Unknown, RequestContentType.GetFormat("xx/xx"));
        }

        [TestMethod]
        public void Test_GetByFormat()
        {
            Assert.AreEqual(RequestContentType.Text, RequestContentType.GetByFormat(SerializeFormat.Text));
            Assert.AreEqual(RequestContentType.Json, RequestContentType.GetByFormat(SerializeFormat.Json));
            Assert.AreEqual(RequestContentType.Json, RequestContentType.GetByFormat(SerializeFormat.Json2));
            Assert.AreEqual(RequestContentType.Xml, RequestContentType.GetByFormat(SerializeFormat.Xml));
            Assert.AreEqual(RequestContentType.Form, RequestContentType.GetByFormat(SerializeFormat.Form));
            Assert.AreEqual(RequestContentType.Multipart, RequestContentType.GetByFormat(SerializeFormat.Multipart));
            Assert.AreEqual(RequestContentType.Binary, RequestContentType.GetByFormat(SerializeFormat.Binary));
            Assert.AreEqual(string.Empty, RequestContentType.GetByFormat(SerializeFormat.None));
            Assert.AreEqual(string.Empty, RequestContentType.GetByFormat(SerializeFormat.Auto));
            Assert.AreEqual(string.Empty, RequestContentType.GetByFormat(SerializeFormat.Unknown));
        }
    }
}
