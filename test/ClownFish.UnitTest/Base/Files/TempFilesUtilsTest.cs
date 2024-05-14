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
    public class TempFilesUtilsTest
    {
        private void InitDirAndFiles(string tempPath)
        {
            InitDir(tempPath, "");

            InitDir(tempPath, "dir1");
            InitDir(tempPath, "dir2");

            System.Threading.Thread.Sleep(10);
        }


        private void InitDir(string tempPath, string name)
        {
            string dir1 = Path.Combine(tempPath, name);

            Directory.CreateDirectory(dir1);

            CreateFile(Path.Combine(dir1, "000.txt"), DateTime.Now.AddDays(-5.1));
            CreateFile(Path.Combine(dir1, "111.txt"), DateTime.Now.AddDays(-3.1));
            CreateFile(Path.Combine(dir1, "222.txt"), DateTime.Now.AddDays(-1.1));
        }


        private void CreateFile(string filePath, DateTime time, string text = null)
        {
            if( text == null )
                text = Guid.NewGuid().ToString();

            RetryFile.WriteAllText(filePath, text);

            File.SetCreationTime(filePath, time);
            File.SetLastWriteTime(filePath, time);
        }


        [TestMethod]
        public void Test_Notfound()
        {
            var list1 = TempFilesUtils.DeleteOldFiles(Guid.NewGuid().ToString(), TimeSpan.FromDays(3), true);
            Assert.AreEqual(0, list1.Count);

            var list2 = TempFilesUtils.DeleteEmptyDirectories(Guid.NewGuid().ToString(), TimeSpan.FromDays(3));
            Assert.AreEqual(0, list2.Count);
        }


        [TestMethod]
        public void Test_Delete()
        {
            string tempPath = Path.Combine(RetryFileTest.TempRoot, Guid.NewGuid().ToString("N"));
            InitDirAndFiles(tempPath);

            var list1 = TempFilesUtils.DeleteOldFiles(tempPath, TimeSpan.FromDays(3), true);
            Assert.AreEqual(2, list1.Count);

            var list2 = TempFilesUtils.DeleteOldFiles(tempPath, TimeSpan.FromDays(3), false);
            Assert.AreEqual(4, list2.Count);


            var list3 = TempFilesUtils.DeleteOldFiles(tempPath, TimeSpan.FromDays(1), true);
            Assert.AreEqual(1, list3.Count);

            var list4 = TempFilesUtils.DeleteOldFiles(tempPath, TimeSpan.FromDays(1), false);
            Assert.AreEqual(2, list4.Count);

            TempFilesUtils.DeleteOldFiles(tempPath, TimeSpan.Zero, false);



            var list5 = TempFilesUtils.DeleteEmptyDirectories(tempPath, TimeSpan.Zero);
            Assert.AreEqual(2, list5.Count);

            TempFilesUtils.DeleteEmptyDirectories(tempPath, TimeSpan.Zero);

            RetryDirectory.Delete(tempPath);


        }

    }
}
