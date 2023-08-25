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
            TempFilesUtils utils = new TempFilesUtils();
            utils.DeleteFiles(Guid.NewGuid().ToString(), TimeSpan.FromDays(3), true);
            Assert.AreEqual(0, utils.Count);
            Assert.AreEqual(0, utils.Exceptions.Count);


            utils.DeleteDirectories(Guid.NewGuid().ToString(), TimeSpan.FromDays(3));
            Assert.AreEqual(0, utils.Count);
            Assert.AreEqual(0, utils.Exceptions.Count);
        }


        [TestMethod]
        public void Test_Delete()
        {
            string tempPath = Path.Combine(RetryFileTest.TempRoot, Guid.NewGuid().ToString("N"));
            InitDirAndFiles(tempPath);


            TempFilesUtils utils = new TempFilesUtils();
            utils.DeleteFiles(tempPath, TimeSpan.FromDays(3), true);
            Assert.AreEqual(2, utils.Count);

            utils.DeleteFiles(tempPath, TimeSpan.FromDays(3), false);
            Assert.AreEqual(4, utils.Count);


            utils.DeleteFiles(tempPath, TimeSpan.FromDays(1), true);
            Assert.AreEqual(1, utils.Count);

            utils.DeleteFiles(tempPath, TimeSpan.FromDays(1), false);
            Assert.AreEqual(2, utils.Count);

            TempFilesUtils.DeleteOldFiles(tempPath, TimeSpan.Zero, false);
            




            utils.DeleteDirectories(tempPath, TimeSpan.Zero);
            Assert.AreEqual(2, utils.Count);

            TempFilesUtils.DeleteEmptyDirectories(tempPath, TimeSpan.Zero);

            RetryDirectory.Delete(tempPath);
        }
    }
}
