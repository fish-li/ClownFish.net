using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;
using System.IO;

namespace ClownFish.UnitTest.Base.Files
{
    [TestClass]
    public class TempFileTest
    {
        [TestMethod]
        public void Test_bytes()
        {
            string filePath = null;

            byte[] bb = Guid.NewGuid().ToByteArray();
            using( TempFile file = TempFile.CreateFile(bb) ) {

                filePath = file.FilePath;

                Assert.IsNotNull(filePath);
                Assert.IsTrue(File.Exists(filePath));
            }

            System.Threading.Thread.Sleep(10);
            Assert.IsFalse(File.Exists(filePath));
        }


        [TestMethod]
        public void Test_stream()
        {
            string filePath = null;

            byte[] bb = Guid.NewGuid().ToByteArray();
            using MemoryStream ms = new MemoryStream(bb);

            using( TempFile file = TempFile.CreateFile(ms) ) {

                filePath = file.FilePath;

                Assert.IsNotNull(filePath);
                Assert.IsTrue(File.Exists(filePath));
            }

            System.Threading.Thread.Sleep(10);
            Assert.IsFalse(File.Exists(filePath));
        }

        [TestMethod]
        public void Test_error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TempFile.CreateFile((byte[])null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = TempFile.CreateFile((Stream)null);
            });
        }
    }
}
