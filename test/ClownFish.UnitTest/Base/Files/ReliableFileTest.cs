using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Files
{
    [TestClass]
    public class ReliableFileTest
    {
        // 复制这二个常量，防止单边修改
        internal static readonly string FileBodyEndFlag = "a725982063d746788cb5f599e4a4c52f";
        internal static readonly string TempFileExtensionName = ".ce200fd3b8ec4860b17ffe1dd5b5c7a2";


        [TestMethod]
        public void TestReadWriteText()
        {
            string filename = "ReliableFileTest.txt";

            string text = Guid.NewGuid().ToString();
            ReliableFile.Write(text, filename);
            Assert.AreEqual(text, ReliableFile.Read(filename));

            // 第二次写入时，逻辑不一样，中途会生产一个临时文件，由于速度太快，这里没法做断言
            string text2 = Guid.NewGuid().ToString();
            ReliableFile.Write(text2, filename);
            Assert.AreEqual(text2, ReliableFile.Read(filename));

            File.Delete(filename);
        }


        [TestMethod]
        public void Test_tempfile1()
        {
            string filename = "ReliableFileTest2.txt";

            string text = Guid.NewGuid().ToString();
            ReliableFile.Write(text, filename);


            // 构造一个“临时文件”
            string tempfile = filename + TempFileExtensionName;
            File.WriteAllText(tempfile, "123" + FileBodyEndFlag, Encoding.UTF8);

            // 这里读到的结果以临时文件为准，而且源文件也被反写了
            Assert.AreEqual("123", ReliableFile.Read(filename));
            Assert.AreEqual("123" + FileBodyEndFlag, File.ReadAllText(filename, Encoding.UTF8));

            File.Delete(filename);
        }


        [TestMethod]
        public void Test_tempfile2()
        {
            string filename = "ReliableFileTest3.txt";

            string text = Guid.NewGuid().ToString();
            ReliableFile.Write(text, filename);


            // 构造一个“临时文件”，结尾标识无效
            string tempfile = filename + TempFileExtensionName;
            File.WriteAllText(tempfile, "123a725982063d746788cb5f599e4a4c52fss", Encoding.UTF8);

            // 无效的临时文件将被删除
            Assert.AreEqual(text, ReliableFile.Read(filename));
            Assert.IsFalse(File.Exists(tempfile));

            File.Delete(filename);
        }


        [TestMethod]
        public void Test_flagerror()
        {
            string filename = "ReliableFileTest4.txt";

            string text = Guid.NewGuid().ToString();
            ReliableFile.Write(text, filename);
            Assert.AreEqual(text, ReliableFile.Read(filename));

            // 结尾标识无效
            File.AppendAllText(filename, "xxx", Encoding.UTF8);
            Assert.IsNull(ReliableFile.Read(filename));

            File.Delete(filename);
        }



        [TestMethod]
        public void TestReadWriteObject()
        {
            string filename = "ReliableFileTest.xml";

            List<NameValue> list = new List<NameValue> {
                new NameValue { Name = "key1", Value = "123" },
                new NameValue { Name = "key2", Value = "456" }
            };

            ReliableFile.WriteObject(list, filename);
            List<NameValue> list2 = ReliableFile.ReadObject<List<NameValue>>(filename);

            Assert.AreEqual("key1", list2[0].Name);
            Assert.AreEqual("123", list2[0].Value);

            Assert.AreEqual("key2", list2[1].Name);
            Assert.AreEqual("456", list2[1].Value);

            RetryFile.Delete(filename);
        }

        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<ArgumentNullException>(()=> {
                ReliableFile.Write(null, "filepath");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                ReliableFile.Write("text", null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _= ReliableFile.Read(null);
            });



            MyAssert.IsError<ArgumentNullException>(() => {
                ReliableFile.WriteObject(null, "filepath");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                ReliableFile.WriteObject("text", null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = ReliableFile.ReadObject<string>(null);
            });


            //string filename = $"temp_{Guid.NewGuid().ToString("N")}.txt";

            using( TempFile file = TempFile.CreateFile(Empty.Array<byte>()) ) {

                Product value = ReliableFile.ReadObject<Product>(file.FilePath);
                Assert.IsNull(value);
            }

        }
    }
}
