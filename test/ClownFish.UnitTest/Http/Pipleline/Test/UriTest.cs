using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Http.Pipleline.Test
{
    [TestClass]
    public class UriTest
    {
        [TestMethod]
        public void Test1()
        {
            // 下面这些代码主要是为了看一下 Uri 的各属性值是什么

            Uri uri = new Uri("http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3");
            Console.WriteLine("OriginalString: " + uri.OriginalString);
            Console.WriteLine("AbsolutePath: " + uri.AbsolutePath);
            Console.WriteLine("AbsoluteUri: " + uri.AbsoluteUri);
            Console.WriteLine("PathAndQuery: " + uri.PathAndQuery);
            Console.WriteLine("Port: " + uri.Port);
            Console.WriteLine("Scheme: " + uri.Scheme);
            Console.WriteLine("Host: " + uri.Host);
            Console.WriteLine("LocalPath: " + uri.LocalPath);
            Console.WriteLine("Query: " + uri.Query);
            Console.WriteLine("Authority: " + uri.Authority);
            Console.WriteLine("DnsSafeHost: " + uri.DnsSafeHost);
            Console.WriteLine("Fragment: " + uri.Fragment);
            Console.WriteLine("IdnHost: " + uri.IdnHost);            
            Console.WriteLine("Segments: " + uri.Segments.Merge("   "));

            Assert.IsTrue(uri.OriginalString.StartsWith("http"));
        }
    }
}
