using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Cryptography
{
    [TestClass]
    public class CryptoHelperTest
    {
        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<ArgumentNullException>(()=>{
                _ = CryptoHelper.Encrypt("abc", null);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = CryptoHelper.Encrypt(new byte[] { 23 }, null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = CryptoHelper.Decrypt("abc", null);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = CryptoHelper.Decrypt(new byte[] { 23 }, null);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                CryptoHelper.SetKeyIV(null, "xxx");
            });


            TripleDES sa = TripleDES.Create();

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = CryptoHelper.Encrypt((string)null, sa);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = CryptoHelper.Encrypt((byte[])null, sa);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = CryptoHelper.Decrypt((string)null, sa);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = CryptoHelper.Decrypt((byte[])null, sa);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                CryptoHelper.SetKeyIV(sa, null);
            });
        }
    }
}
