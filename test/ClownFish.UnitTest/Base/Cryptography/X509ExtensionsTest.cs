using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Cryptography
{
    [TestClass]
    public class X509ExtensionsTest
    {
        [TestMethod]
        public void Test()
        {
            X509Certificate2 cert1 = X509Finder.FindByThumbprint(X509FinderTest.CertThumbprint, true);
            string text = "1111111111111111111";
            byte[] bb = Encoding.ASCII.GetBytes(text);

            string signature = cert1.Sign(bb);
            bool flag = cert1.Verify(bb, signature);
            Assert.IsTrue(flag);
            
            bool flag2 = cert1.GetRSAPublicKey().VerifyData(bb, Convert.FromBase64String(signature), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            Assert.IsTrue(flag2);

            string signature2 = cert1.GetRSAPrivateKey().SignData(bb, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1).ToBase64();
            bool flag3 = cert1.Verify(bb, signature2);
            Assert.IsTrue(flag3);

            byte[] enc = cert1.Encrypt(bb);
            byte[] bb2 = cert1.Decrypt(enc);

            Assert.IsTrue(bb.IsEqual(bb2));

        }


        [TestMethod]
        public void Test_Error()
        {
            X509Certificate2 x509 = X509Finder.FindByThumbprint(X509FinderTest.CertThumbprint, true);

            X509Certificate2 cert2 = X509Finder.LoadPublicKeyFile(X509FinderTest.CerFilePath);

            byte[] bb = Guid.NewGuid().ToByteArray();

            MyAssert.IsError<ArgumentNullException>(()=> {
                _ = X509Extensions.Sign(null, bb);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Extensions.Sign(x509, null);
            });
            MyAssert.IsError<ArgumentException>(() => {
                _ = X509Extensions.Sign(cert2, bb);
            });



            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Extensions.Verify(null, bb, "xx");
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Extensions.Verify(x509, null, "xx");
            });
            MyAssert.IsError<ArgumentException>(() => {
                _ = X509Extensions.Verify(x509, bb, string.Empty);
            });



            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Extensions.Encrypt(null, bb);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Extensions.Encrypt(x509, null);
            });


            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Extensions.Decrypt(null, bb);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Extensions.Decrypt(x509, null);
            });
            MyAssert.IsError<ArgumentException>(() => {
                _ = X509Extensions.Decrypt(cert2, bb);
            });
        }
    }
}
