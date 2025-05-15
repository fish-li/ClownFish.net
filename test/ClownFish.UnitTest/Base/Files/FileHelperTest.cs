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
            string filePath = "./temp/FileHelperTest_AppendAllText.txt";
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


        [TestMethod]
        public void Test_ReadFileTails()
        {
            string filePath = "./temp/FileHelperTest_ReadFileTails.txt";
            
            if( File.Exists(filePath) == false ) {
                for( int i = 0; i < 4; i++ ) {
                    string s1 = new string((char)(i+49), 20) + "大明王朝\r\n";
                    File.AppendAllText(filePath, s1, Encoding.UTF8);
                }

                File.AppendAllText(filePath, "xxx", Encoding.UTF8);
            }

            string text1 = FileHelper.ReadFileTails(filePath, 1);
            Console.WriteLine(text1);
            Console.WriteLine(Console2.SeparatedLine);

            string[] lines1 = text1.Split(new char[] { '\n' });
            Assert.AreEqual(1, lines1.Length);
            Assert.AreEqual("xxx", text1);


            string text2 = FileHelper.ReadFileTails(filePath, 2);
            Console.WriteLine(text2);
            Console.WriteLine(Console2.SeparatedLine);
            
            string[] lines2 = text2.Split(new char[] {'\n' });
            Assert.AreEqual(2, lines2.Length);
            Assert.AreEqual(lines2[0], new string('4', 20) + "大明王朝\r");
            Assert.AreEqual(lines2[1], "xxx");


            string text3 = FileHelper.ReadFileTails(filePath, 3);
            Console.WriteLine(text3);
            Console.WriteLine(Console2.SeparatedLine);
            
            string[] lines3 = text3.Split(new char[] { '\n' });
            Assert.AreEqual(3, lines3.Length);
            Assert.AreEqual(lines3[0], new string('3', 20) + "大明王朝\r");
            Assert.AreEqual(lines3[1], new string('4', 20) + "大明王朝\r");
            Assert.AreEqual(lines3[2], "xxx");



            string text4 = FileHelper.ReadFileTails(filePath, 4);
            Console.WriteLine(text4);
            Console.WriteLine(Console2.SeparatedLine);
            
            string[] lines4 = text4.Split(new char[] { '\n' });
            Assert.AreEqual(4, lines4.Length);
            Assert.AreEqual(lines4[0], new string('2', 20) + "大明王朝\r");
            Assert.AreEqual(lines4[1], new string('3', 20) + "大明王朝\r");
            Assert.AreEqual(lines4[2], new string('4', 20) + "大明王朝\r");
            Assert.AreEqual(lines4[3], "xxx");



            string text5 = FileHelper.ReadFileTails(filePath, 5);
            Console.WriteLine(text5);
            Console.WriteLine(Console2.SeparatedLine);
            string[] lines5 = text5.Split(new char[] {  '\n' });
            Assert.AreEqual(5, lines5.Length);
            Assert.AreEqual(lines5[0], new string('1', 20) + "大明王朝\r");
            Assert.AreEqual(lines5[1], new string('2', 20) + "大明王朝\r");
            Assert.AreEqual(lines5[2], new string('3', 20) + "大明王朝\r");
            Assert.AreEqual(lines5[3], new string('4', 20) + "大明王朝\r");
            Assert.AreEqual(lines5[4], "xxx");

            string text6 = FileHelper.ReadFileTails(filePath, 6);
            Assert.AreEqual(text5, text6);

            string text7 = FileHelper.ReadFileTails(filePath, 7);
            Assert.AreEqual(text5, text7);

            string text0 = FileHelper.ReadFileTails(filePath, 0);
            Assert.AreEqual(text5, text0);
        }

        [TestMethod]
        public void Test_ReadFileTails2()
        {
            string filePath = "./temp/FileHelperTest_ReadFileTails2.txt";

            if( File.Exists(filePath) == false ) {
                for( int i = 0; i < 4; i++ ) {
                    string s1 = new string((char)(i + 49), 20) + "大明王朝\r\n";
                    File.AppendAllText(filePath, s1, Encoding.UTF8);
                }
                // 注意：这个文件最后一行是个 “空行”
            }

            string text1 = FileHelper.ReadFileTails(filePath, 1);
            Assert.AreEqual("", text1);


            string text2 = FileHelper.ReadFileTails(filePath, 2);
            Console.WriteLine(text2);
            Console.WriteLine(Console2.SeparatedLine);

            string[] lines2 = text2.Split(new char[] { '\n' });
            Assert.AreEqual(2, lines2.Length);
            Assert.AreEqual(lines2[0], new string('4', 20) + "大明王朝\r");
            Assert.AreEqual(lines2[1], "");


            string text3 = FileHelper.ReadFileTails(filePath, 3);
            Console.WriteLine(text3);
            Console.WriteLine(Console2.SeparatedLine);

            string[] lines3 = text3.Split(new char[] { '\n' });
            Assert.AreEqual(3, lines3.Length);
            Assert.AreEqual(lines3[0], new string('3', 20) + "大明王朝\r");
            Assert.AreEqual(lines3[1], new string('4', 20) + "大明王朝\r");
            Assert.AreEqual(lines3[2], "");



            string text4 = FileHelper.ReadFileTails(filePath, 4);
            Console.WriteLine(text4);
            Console.WriteLine(Console2.SeparatedLine);

            string[] lines4 = text4.Split(new char[] { '\n' });
            Assert.AreEqual(4, lines4.Length);
            Assert.AreEqual(lines4[0], new string('2', 20) + "大明王朝\r");
            Assert.AreEqual(lines4[1], new string('3', 20) + "大明王朝\r");
            Assert.AreEqual(lines4[2], new string('4', 20) + "大明王朝\r");
            Assert.AreEqual(lines4[3], "");



            string text5 = FileHelper.ReadFileTails(filePath, 5);
            Console.WriteLine(text5);
            Console.WriteLine(Console2.SeparatedLine);
            string[] lines5 = text5.Split(new char[] { '\n' });
            Assert.AreEqual(5, lines5.Length);
            Assert.AreEqual(lines5[0], new string('1', 20) + "大明王朝\r");
            Assert.AreEqual(lines5[1], new string('2', 20) + "大明王朝\r");
            Assert.AreEqual(lines5[2], new string('3', 20) + "大明王朝\r");
            Assert.AreEqual(lines5[3], new string('4', 20) + "大明王朝\r");
            Assert.AreEqual(lines5[4], "");

            string text6 = FileHelper.ReadFileTails(filePath, 6);
            Assert.AreEqual(text5, text6);

            string text7 = FileHelper.ReadFileTails(filePath, 7);
            Assert.AreEqual(text5, text7);

            string text0 = FileHelper.ReadFileTails(filePath, 0);
            Assert.AreEqual(text5, text0);
        }


        [TestMethod]
        public void Test_GetFileVersion()
        {
            string ver1 = FileHelper.GetFileVersion(Path.Combine(AppContext.BaseDirectory, "ClownFish.net.dll"));
            string ver2 = FileHelper.GetFileVersion(Path.Combine(AppContext.BaseDirectory, "ClownFish.net.pdb"));
            string ver3 = FileHelper.GetFileVersion(Path.Combine(AppContext.BaseDirectory, "xxxxxxxxxx.dll"));

            Assert.AreEqual("9.25.515.1", ver1);
            Assert.IsNull(ver2);
            Assert.AreEqual("0.0.0.0", ver3);
        }

    }
}
