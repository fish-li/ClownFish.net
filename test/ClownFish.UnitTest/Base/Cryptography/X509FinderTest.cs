using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ClownFish.UnitTest.Base.Cryptography
{
    [TestClass]
    public class X509FinderTest
    {
        internal static readonly string CertSubject = "FishLi-TEST";
        internal static readonly string CertThumbprint = "4E076D5C0370422FE78ABEEB8B85E5CA8A9C5328";

        internal static readonly string PfxFilePath = @"..\..\cert\ClownFishTest.pfx";
        internal static readonly string CerFilePath = @"..\..\cert\ClownFishTest-pub.cer";

        internal static X509Certificate2 TestCert {
            get => X509Finder.FindByThumbprint(CertThumbprint, true);
        }

        internal static string PublicKeyText {
            get => File.ReadAllText(CerFilePath);
        }

        internal static byte[] PublicKeyBytes {
            get => File.ReadAllBytes(CerFilePath);
        }


        public static X509Certificate2 LoadPfxFile()
        {
            return X509Finder.LoadPfx(PfxFilePath, "pwdFishli");
        }

        public static X509Certificate2 LoadPublicKeyFile()
        {
            return X509Finder.LoadPublicKeyFile(CerFilePath);
        }

        [TestMethod]
        public void Test_LoadPfx()
        {
            X509Certificate2 cert1 = X509Finder.LoadPfx(PfxFilePath, "pwdFishli");
            Assert.IsTrue(cert1.HasPrivateKey);


            byte[] body = File.ReadAllBytes(PfxFilePath);
            X509Certificate2 cert2 = X509Finder.LoadPfx(body, "pwdFishli");
            Assert.IsTrue(cert2.HasPrivateKey);

            Assert.AreEqual(CertThumbprint, cert1.Thumbprint);
            Assert.AreEqual(CertThumbprint, cert2.Thumbprint);


            MyAssert.IsError<Exception>(() => {  // Internal.Cryptography.CryptoThrowHelper+WindowsCryptographicException
                _ = X509Finder.LoadPfx(body, "xxxxxxxx");
            });
        }

        [TestMethod]
        public void Test_LoadFromConfigFile()
        {
            string confBody = File.ReadAllText(@"..\..\cert\ClownFishTest.conf");
            X509Certificate2 cert3 = X509Finder.LoadFromConfigFile(confBody);
            Assert.AreEqual(CertThumbprint, cert3.Thumbprint);


            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.LoadFromConfigFile("");
            });

            MyAssert.IsError<ArgumentException>(() => {
                _ = X509Finder.LoadFromConfigFile("xxxxx");
            });

            MyAssert.IsError<Exception>(() => {
                _ = X509Finder.LoadFromConfigFile("aaaa \n bbbb");
            });
        }


        [TestMethod]
        public void Test_LoadPublicKey()
        {
            X509Certificate2 cert1 = X509Finder.LoadPublicKeyFile(CerFilePath);
            Assert.IsFalse(cert1.HasPrivateKey);


            X509Certificate2 cert2 = X509Finder.LoadFromPublicKey(PublicKeyText);
            Assert.IsFalse(cert2.HasPrivateKey);

            X509Certificate2 cert3 = X509Finder.LoadFromPublicKey(PublicKeyBytes);
            Assert.IsFalse(cert3.HasPrivateKey);


            Assert.AreEqual(CertThumbprint, cert1.Thumbprint);
            Assert.AreEqual(CertThumbprint, cert2.Thumbprint);
            Assert.AreEqual(CertThumbprint, cert3.Thumbprint);
        }

        [TestMethod]
        public void Test_Find()
        {
            X509Certificate2 cert1 = X509Finder.FindByThumbprint(CertThumbprint, true);
            Assert.IsNotNull(cert1);

            X509Certificate2 cert2 = X509Finder.FindBySubject(CertSubject, true);
            Assert.IsNotNull(cert2);

            Assert.AreEqual(CertThumbprint, cert1.Thumbprint);
            Assert.AreEqual(CertThumbprint, cert2.Thumbprint);
        }


        [TestMethod]
        public void Test_NotFound()
        {
            X509Certificate2 cert1 = X509Finder.FindByThumbprint("xxxxxxxxxxxxxxxx", false);
            Assert.IsNull(cert1);

            X509Certificate2 cert2 = X509Finder.FindBySubject("xxxxxxxxxxxxxxxx", false);
            Assert.IsNull(cert2);
        }

        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.FindBySubject(null, false);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.FindBySubject(null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.FindByThumbprint(null, false);
            });


            MyAssert.IsError<ArgumentException>(() => {
                _ = X509Finder.FindByThumbprint("xxxxxxxxxxxxxxxxx", true);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.FindByThumbprint(null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.LoadFromPublicKey((byte[])null);
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.LoadFromPublicKey(string.Empty);
            });
            MyAssert.IsError<Exception>(() => {  // Internal.Cryptography.CryptoThrowHelper+WindowsCryptographicException
                _ = X509Finder.LoadFromPublicKey(File.ReadAllBytes(PfxFilePath));
            });
            MyAssert.IsError<Exception>(() => {  // Internal.Cryptography.CryptoThrowHelper+WindowsCryptographicException
                _ = X509Finder.LoadFromPublicKey(PfxFilePath);
            });


            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.LoadPublicKeyFile(string.Empty);
            });
            MyAssert.IsError<Exception>(() => {  // Internal.Cryptography.CryptoThrowHelper+WindowsCryptographicException
                _ = X509Finder.LoadPublicKeyFile(PfxFilePath);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.LoadPfx((byte[])null, "xx");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = X509Finder.LoadPfx(string.Empty, "xx");
            });
        }
    }
}
