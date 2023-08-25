using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Files
{
    [TestClass]
    public class RetryFileTest
    {
        public static readonly string TempRoot = Path.Combine(Path.GetFullPath(Path.GetTempPath()), "_ClownFishUnitTest");

        [TestMethod]
        public void Test_error()
        {
            // 一个肯定不存在的文件
            string filePath = Path.Combine(TempRoot, "xxxx.xx");

            RetryFile.Delete(filePath);  // 不抛异常就好
            Assert.IsNull(RetryFile.TryReadAllText(filePath));

            MyAssert.IsError<FileNotFoundException>(() => {
                _ = RetryFile.ReadAllText(filePath);
            });

            MyAssert.IsError<FileNotFoundException>(() => {
                _ = RetryFile.ReadAllLines(filePath);
            });

            MyAssert.IsError<FileNotFoundException>(() => {
                _ = RetryFile.ReadAllBytes(filePath);
            });

            //MyAssert.IsError<FileNotFoundException>(() => {
                DateTime time = RetryFile.GetLastWriteTime(filePath);
                Console.WriteLine(time.ToTimeString());
            //});

            //MyAssert.IsError<FileNotFoundException>(() => {
                DateTime time2 = RetryFile.GetLastWriteTimeUtc(filePath);
                Console.WriteLine(time2.ToTimeString());
            //});

            MyAssert.IsError<FileNotFoundException>(() => {
                _ = RetryFile.GetAttributes(filePath);
            });

            MyAssert.IsError<FileNotFoundException>(() => {
                _ = RetryFile.IsHidden(filePath);
            });

            MyAssert.IsError<FileNotFoundException>(() => {
                RetryFile.ClearReadonly(filePath);
            });

            MyAssert.IsError<FileNotFoundException>(() => {
                RetryFile.Copy(filePath, "xx.xx", true);
            });

            MyAssert.IsError<FileNotFoundException>(() => {
                RetryFile.Move(filePath, "xx.xx");
            });

            MyAssert.IsError<FileNotFoundException>(() => {
                _ = RetryFile.OpenRead(filePath);
            });


        }
        [TestMethod]
        public void Test_a1()
        {
            string filePath = Path.Combine(TempRoot, Guid.NewGuid().ToString("N") + ".txt");
            Assert.IsFalse(RetryFile.Exists(filePath));

            string text = Guid.NewGuid().ToString();
            string textx = text + "\r\nxx";


            RetryFile.WriteAllText(filePath, text);
            Assert.IsTrue(RetryFile.Exists(filePath));

            string text2 = RetryFile.ReadAllText(filePath);
            Assert.AreEqual(text, text2);

            RetryFile.AppendAllText(filePath, "\r\nxx");
            string text3 = RetryFile.TryReadAllText(filePath);
            Assert.AreEqual(textx, text3);


            string[] lines = RetryFile.ReadAllLines(filePath);
            Assert.AreEqual(2, lines.Length);

            DateTime time1 = RetryFile.GetLastWriteTime(filePath);
            DateTime time2 = RetryFile.GetLastWriteTimeUtc(filePath);

            Assert.IsTrue((DateTime.Now - time1).TotalMinutes < 1);
            Assert.AreEqual(time2, time1.ToUniversalTime());

            FileAttributes attributes = RetryFile.GetAttributes(filePath);
            Assert.IsFalse(attributes.HasFlag(FileAttributes.Hidden));
            Assert.IsFalse(attributes.HasFlag(FileAttributes.ReadOnly));

            RetryFile.ClearReadonly(filePath);

            attributes |= FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System;
            File.SetAttributes(filePath, attributes);

            Assert.IsTrue(RetryFile.IsHidden(filePath));


            FileAttributes attributes2 = RetryFile.GetAttributes(filePath);
            Assert.IsTrue(attributes2.HasFlag(FileAttributes.ReadOnly));

            RetryFile.ClearReadonly(filePath);

            FileAttributes attributes3 = RetryFile.GetAttributes(filePath);
            Assert.IsFalse(attributes3.HasFlag(FileAttributes.ReadOnly));


            string bakPath = Path.Combine(TempRoot, "sub1", Guid.NewGuid().ToString("N") + ".bak");
            RetryFile.Copy(filePath, bakPath, true);
            Assert.IsTrue(RetryFile.Exists(bakPath));

            RetryFile.Delete(filePath);

            string newPath = Path.Combine(TempRoot, "sub2", Guid.NewGuid().ToString("N") + ".bak");
            RetryFile.Move(bakPath, newPath);
            Assert.IsTrue(RetryFile.Exists(newPath));
            RetryFile.Delete(newPath);

            RetryDirectory.Delete(Path.GetDirectoryName(bakPath));
            RetryDirectory.Delete(Path.GetDirectoryName(newPath));
        }


        [TestMethod]
        public void Test_a2()
        {
            string filePath = Path.Combine(TempRoot, Guid.NewGuid().ToString("N") + ".dat2");
            Assert.IsFalse(RetryFile.Exists(filePath));

            byte[] data = Guid.NewGuid().ToByteArray();

            RetryFile.WriteAllBytes(filePath, data);
            Assert.IsTrue(RetryFile.Exists(filePath));

            byte[] d2 = RetryFile.ReadAllBytes(filePath);
            Assert.IsTrue(data.IsEqual(d2));

            using( Stream stream = RetryFile.OpenRead(filePath) ) {
                byte[] d3 = stream.ToArray();
                Assert.IsTrue(data.IsEqual(d3));
            }

            RetryFile.Delete(filePath);
        }

        [TestMethod]
        public void Test_OpenWrite()
        {
            string filePath = Path.Combine(TempRoot, "Test_OpenWrite", Guid.NewGuid().ToString("N") + ".dat");
            Assert.IsFalse(RetryFile.Exists(filePath));

            byte[] data = Guid.NewGuid().ToByteArray();

            using( Stream stream = RetryFile.OpenWrite(filePath) ) {

                Assert.IsFalse(stream.CanRead);
                stream.Write(data, 0, data.Length);
            }

            byte[] d3 = RetryFile.ReadAllBytes(filePath);
            Assert.IsTrue(data.IsEqual(d3));

            RetryDirectory.Delete(Path.GetDirectoryName(filePath));
        }



        [TestMethod]
        public void Test_OpenAppend()
        {
            string filePath = Path.Combine(TempRoot, "Test_OpenAppend", Guid.NewGuid().ToString("N") + ".dat");
            Assert.IsFalse(RetryFile.Exists(filePath));

            byte[] data = Guid.NewGuid().ToByteArray();

            using( Stream stream = RetryFile.OpenAppend(filePath) ) {

                Assert.IsFalse(stream.CanRead);
                stream.Write(data, 0, data.Length);                
            }

            byte[] d3 = RetryFile.ReadAllBytes(filePath);
            Assert.IsTrue(data.IsEqual(d3));

            RetryDirectory.Delete(Path.GetDirectoryName(filePath));
        }


        [TestMethod]
        public void Test_Create()
        {
            string filePath = Path.Combine(TempRoot, "Test_Create", Guid.NewGuid().ToString("N") + ".dat");
            Assert.IsFalse(RetryFile.Exists(filePath));

            byte[] data = Guid.NewGuid().ToByteArray();

            using( Stream stream = RetryFile.Create(filePath) ) {

                Assert.IsTrue(stream.CanRead);
                Assert.IsTrue(stream.CanWrite);
                stream.Write(data, 0, data.Length);
            }

            byte[] d3 = RetryFile.ReadAllBytes(filePath);
            Assert.IsTrue(data.IsEqual(d3));

            RetryDirectory.Delete(Path.GetDirectoryName(filePath));
        }


        [TestMethod]
        public void Test_WriteAllBytes()
        {
            string filePath = Path.Combine(TempRoot, "Test_WriteAllBytes", Guid.NewGuid().ToString("N") + ".dat");
            Assert.IsFalse(RetryFile.Exists(filePath));

            byte[] data = Guid.NewGuid().ToByteArray();

            RetryFile.WriteAllBytes(filePath, data);

            byte[] d3 = RetryFile.ReadAllBytes(filePath);
            Assert.IsTrue(data.IsEqual(d3));

            RetryDirectory.Delete(Path.GetDirectoryName(filePath));
        }


        [TestMethod]
        public void Test_AppendAllText()
        {
            string filePath = Path.Combine(TempRoot, "Test_AppendAllText", Guid.NewGuid().ToString("N") + ".dat");
            Assert.IsFalse(RetryFile.Exists(filePath));

            string text = Guid.NewGuid().ToString();

            RetryFile.AppendAllText(filePath, text);

            string text2  = RetryFile.ReadAllText(filePath);
            Assert.AreEqual(text, text2);

            RetryDirectory.Delete(Path.GetDirectoryName(filePath));
        }


        [TestMethod]
        public void Test_WriteAllText()
        {
            string filePath = Path.Combine(TempRoot, "Test_WriteAllText", Guid.NewGuid().ToString("N") + ".dat");
            Assert.IsFalse(RetryFile.Exists(filePath));

            string text = Guid.NewGuid().ToString();

            RetryFile.WriteAllText(filePath, text);

            string text2 = RetryFile.ReadAllText(filePath);
            Assert.AreEqual(text, text2);

            RetryDirectory.Delete(Path.GetDirectoryName(filePath));
        }
    }
}
