using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Files
{
    [TestClass]
    public class FileHelperTest
    {
        [TestMethod]
        public void Test()
        {
            string filename1 = "FileHelperTest_1.dat";
            string text = Guid.NewGuid().ToString();
            byte[] bytes = text.GetBytes();

            using(MemoryStream ms = new MemoryStream(bytes) ) {
                ms.SaveToFile(filename1);                
            }


            string filename2 = "FileHelperTest_2.dat";
            string filename3 = "FileHelperTest_3.dat";

            FileHelper.EncryptFile(filename1, filename2, "xxxxxxx");
            FileHelper.DecryptFile(filename2, filename3, "xxxxxxx");

            string text1 = RetryFile.ReadAllText(filename1);
            string text3 = RetryFile.ReadAllText(filename3);

            Assert.AreEqual(text1, text3);

            RetryFile.Delete(filename1);
            RetryFile.Delete(filename2);
            RetryFile.Delete(filename3);

            Assert.IsFalse(RetryFile.Exists(filename1));
            Assert.IsFalse(RetryFile.Exists(filename2));
            Assert.IsFalse(RetryFile.Exists(filename3));
        }

        [TestMethod]
        public void Test_Error()
        {
            MemoryStream ms = new MemoryStream();

            MyAssert.IsError<ArgumentNullException>(()=> {
                FileHelper.SaveToFile((Stream)null, "savePath");
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                FileHelper.SaveToFile(ms, null);
            });



            MyAssert.IsError<FileNotFoundException>(() => {
                FileHelper.EncryptFile((string)null, "destFilePath", "password");
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                FileHelper.EncryptFile("ClownFish.App.config", (string)null, "password");
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                FileHelper.EncryptFile("ClownFish.App.config", "destFilePath", (string)null);
            });



            MyAssert.IsError<FileNotFoundException>(() => {
                FileHelper.DecryptFile((string)null, "destFilePath", "password");
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                FileHelper.DecryptFile("ClownFish.App.config", (string)null, "password");
            });
            MyAssert.IsError<ArgumentNullException>(() => {
                FileHelper.DecryptFile("ClownFish.App.config", "destFilePath", (string)null);
            });
        }


        [TestMethod]
        public void Test_AppendAllText()
        {
            int maxLen = 110;
            string filePath = "./temp/FileHelperTest_Test_AppendAllText.txt";
            RetryFile.Delete(filePath);

            string s1 = new string('a', 100);
            bool flag1 = FileHelper.AppendAllText(filePath, s1, false, maxLen);

            string s2 = RetryFile.ReadAllText(filePath);
            Assert.AreEqual(s2, s1);
            Assert.IsTrue(flag1);

            string s3 = new string('b', 20);
            bool flag2 = FileHelper.AppendAllText(filePath, s3, false, maxLen);
            string s4 = RetryFile.ReadAllText(filePath);

            Assert.IsFalse(flag2);
            Assert.AreEqual(s4, s1);
        }


    }
}
