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
    public class RetryDirectoryTest
    {
        [TestMethod]
        public void Test_error()
        {
            string path = Path.Combine(RetryFileTest.TempRoot, new string('x', 1024));

#if NETFRAMEWORK
            MyAssert.IsError<PathTooLongException>(() => {
                _ = RetryDirectory.CreateDirectory(path);
            });
#else
            MyAssert.IsError<IOException>(() => {
                _ = RetryDirectory.CreateDirectory(path);
            });
#endif

            MyAssert.IsError<IOException>(() => {
                _ = RetryDirectory.GetDirectories(path, "*.*", SearchOption.AllDirectories);
            });
        }

        // 在单元测试项目中，即使创建了 app.manifest 也没有作用，所以没法设置 <ws2:longPathAware>true</ws2:longPathAware>
        // 因此下面的测试就不做了~~

        //#if NETFRAMEWORK

        //        [TestMethod]
        //        public void Test_LongPath()
        //        {
        //            string path = Path.Combine(RetryFileTest.TempRoot, new string('x', 1024)).ToLongPath();

        //            DirectoryInfo dir = RetryDirectory.CreateDirectory(path);

        //            string text = Guid.NewGuid().ToString();
        //            string file = Path.Combine(path, "test1.txt");
        //            RetryFile.WriteAllText(file, text);

        //            Assert.AreEqual(text, RetryFile.ReadAllText(file));
        //            RetryFile.Delete(file);

        //            RetryDirectory.Delete(path);
        //        }
        //#endif

    }
}
