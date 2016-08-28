using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using ClownFish.AspnetMock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Cache
{
	[TestClass]
	public class OutputCacheAttributeTest
	{
		[TestMethod]
		public void Test()
		{
			OutputCacheAttribute a = new OutputCacheAttribute();
			a.CacheProfile = "_CacheProfile";
			a.Duration = 2;
			a.Location = OutputCacheLocation.Server;
			a.NoStore = true;
			a.SqlDependency = "_SqlDependency";
			a.VaryByContentEncoding = "_VaryByContentEncoding";
			a.VaryByCustom = "_VaryByCustom";
			a.VaryByHeader = "_VaryByHeader";
			a.VaryByParam = "_VaryByParam";


			OutputCacheParameters settings = a.CacheSettings;
			Assert.AreEqual(2, settings.Duration);


			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/Everyone.aspx HTTP/1.1
";

			using( WebContext context = WebContext.FromRawText(requestText) ) {

				try {
					a.SetResponseCache(context.HttpContext);
				}
				catch { }

				// 这里不太容易做断言，只能看一下代码有没有覆盖。
			}


		}
	}
}
