using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class ByteExtensionsTest
    {
        [TestMethod]
        public void Test_IsEqual()
        {
            byte[] b1 = new byte[] { 1, 2 };
            byte[] b2 = new byte[] { 1, 2 };
            Assert.AreEqual(true, b1.IsEqual(b2));

            byte[] b3 = new byte[] { 1, 2 };
            byte[] b4 = new byte[] { 1, 3 };
            Assert.AreEqual(false, b3.IsEqual(b4));

            byte[] b5 = null;
            byte[] b6 = null;
            Assert.AreEqual(true, b5.IsEqual(b6));

            byte[] b7 = new byte[] { 1, 2 };
            byte[] b8 = null;
            Assert.AreEqual(false, b7.IsEqual(b8));

        }


        [TestMethod]
        public void Test_ToBase64()
        {
            string text = "将byte[]做BASE64编码";
            byte[] bb = Encoding.UTF8.GetBytes(text);

            string base64 = bb.ToBase64();
            Assert.AreEqual("5bCGYnl0ZVtd5YGaQkFTRTY057yW56CB", base64);
        }


        [TestMethod]
        public void Test_ToHexString()
        {
            string text = "将byte[]做BASE64编码";
            byte[] bb1 = Encoding.UTF8.GetBytes(text);
            string hex1 = bb1.ToHexString();
            Assert.AreEqual("E5B086627974655B5DE5819A424153453634E7BC96E7A081", hex1);

            byte[] bb2 = null;
            string hex2 = bb2.ToHexString();
            Assert.AreEqual(string.Empty, hex2);
        }


        [TestMethod]
        public void Test_ToUtf8Bytes()
        {
            string text = "将byte[]做BASE64编码";
            byte[] bb1 = text.ToUtf8Bytes();
            string text2 = bb1.ToUtf8String();
            Assert.AreEqual(text, text2);

            byte[] bb2 = ((string)null).ToUtf8Bytes();
            Assert.IsNotNull(bb2);
            Assert.AreEqual(0, bb2.Length);

            byte[] bb3 = "".ToUtf8Bytes();
            Assert.IsNotNull(bb3);
            Assert.AreEqual(0, bb3.Length);


            string text3 = ((byte[])null).ToUtf8String();
            Assert.AreEqual("", text3);

            string text4 = Empty.Array<byte>().ToUtf8String();
            Assert.AreEqual("", text4);
        }
    }
}
