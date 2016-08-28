using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ClownFish.AspnetMock;
using ClownFish.Web;
using ClownFish.Web.UnitTest.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest
{
	[TestClass]
	public class FrameworkTest : BaseTest
	{
		

		[Description("整体测试--ClownFish.AspnetMock")]
		[TestMethod]
		public void TestMethod1()
		{
			TestPage page = new TestPage();

			string requestText = @"
POST http://www.fish-mvc-demo.com/pages/abc.aspx?id=2&name=aa HTTP/1.1
Host: www.fish-mvc-demo.com
User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)
Accept: */*
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/x-www-form-urlencoded; charset=UTF-8
Referer: http://www.fish-mvc-demo.com/a2.aspx?cc=5
Cookie: PageStyle=Style2; cookie1=cccccccccc
h1: 111111111
h2: 22222222

a=1&b=2&c=3
";

			using(WebContext context = WebContext.FromRawText(requestText)){
				context.SetUserName("fish-li");
				context.BindPage(page);		// 如果需要访问 HttpRequest, HttpResponse 对象，可以参照这个方法来操作（在页面中访问）

				Encoding contentEncoding = Encoding.UTF8;
				string formData = "a=1&b=2&c=3";	// 请求体数据，用于后续比较，需要确保与 requestText 的最后一行数据保持一致。


				Console.WriteLine("PathInfo: " + HttpContext.Current.Request.PathInfo);
				Console.WriteLine("PhysicalPath: " + HttpContext.Current.Request.PhysicalPath);
				Console.WriteLine("UserAgent: " + HttpContext.Current.Request.UserAgent);

				string currentPath = AppDomain.CurrentDomain.BaseDirectory;


				// 开始测试
				Assert.AreEqual(currentPath, page.GetAppDomainPath());
				Assert.AreEqual(currentPath + @"\test.dat", page.GetServerMappingPath("/test.dat"));
				Assert.AreEqual(currentPath + @"\test.dat", page.GetServerMappingPath("~/test.dat"));
				Assert.AreEqual(currentPath + @"\pages\test.dat", page.GetServerMappingPath("test.dat"));
				Assert.AreEqual(currentPath + @"\pages\dd\test.dat", page.GetServerMappingPath("dd/test.dat"));

				Assert.AreEqual(currentPath + @"\test.dat", page.GetRequestMappingPath("/test.dat"));
				Assert.AreEqual(currentPath + @"\test.dat", page.GetRequestMappingPath("~/test.dat"));
				Assert.AreEqual(currentPath + @"\pages\test.dat", page.GetRequestMappingPath("test.dat"));
				Assert.AreEqual(currentPath + @"\pages\dd\test.dat", page.GetRequestMappingPath("dd/test.dat"));

				Assert.AreEqual("fish-li", HttpContext.Current.User.Identity.Name);
				Assert.AreEqual(true, HttpContext.Current.Request.IsAuthenticated);
				Assert.AreEqual(true, HttpContext.Current.User.Identity.IsAuthenticated);


				Assert.AreEqual("/a2.aspx?cc=5", HttpContext.Current.Request.UrlReferrer.PathAndQuery);
				Assert.AreEqual("/pages/abc.aspx", HttpContext.Current.Request.FilePath);
				Assert.AreEqual("/pages/abc.aspx", HttpContext.Current.Request.Path);
				Assert.AreEqual("/", HttpContext.Current.Request.ApplicationPath);
				Assert.AreEqual("~/pages/abc.aspx", HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath);
				Assert.AreEqual(contentEncoding, HttpContext.Current.Request.ContentEncoding);
				Assert.AreEqual("application/x-www-form-urlencoded", HttpContext.Current.Request.ContentType);
				Assert.AreEqual("/pages/abc.aspx?id=2&name=aa", HttpContext.Current.Request.RawUrl);
				Assert.AreEqual("POST", HttpContext.Current.Request.RequestType);
				Assert.AreEqual("http://www.fish-mvc-demo.com/pages/abc.aspx", HttpContext.Current.Request.Url.AbsoluteUri);

				Assert.AreEqual(contentEncoding.GetByteCount(formData), HttpContext.Current.Request.ContentLength);

				Assert.AreEqual("IE", HttpContext.Current.Request.Browser.Browser);
				Assert.AreEqual(7, HttpContext.Current.Request.Browser.MajorVersion);
				Assert.AreEqual("cccccccccc", HttpContext.Current.Request.Cookies["cookie1"].Value);

				Assert.AreEqual("2", page.ReadQuerySting("id"));
				Assert.AreEqual("aa", page.ReadQuerySting("name"));
				Assert.AreEqual("aa", page.ReadParams("name"));
				Assert.AreEqual("aa", page.ReadItem("name"));
				Assert.AreEqual("aa", HttpContext.Current.Request.QueryString["name"]);

				Assert.AreEqual("1", page.ReadForm("a"));
				Assert.AreEqual("2", page.ReadForm("b"));
				Assert.AreEqual("3", page.ReadForm("c"));

				Assert.AreEqual("111111111", page.ReadHeader("h1"));
				Assert.AreEqual("22222222", page.ReadHeader("h2"));

				

				Guid sessionData = Guid.NewGuid();
				page.WriteSession("s1", sessionData);
				Assert.AreEqual(sessionData, (Guid)page.ReadSession("s1"));
				Assert.AreEqual(sessionData, (Guid)HttpContext.Current.Session["s1"]);

				int num1 = 123;
				page.WriteApplication("key1", num1);
				Assert.AreEqual(num1, (int)page.ReadApplication("key1"));


				page.SetResponseContentType("application/octet-stream");
				Assert.AreEqual("application/octet-stream", HttpContext.Current.Response.ContentType);


				string writeText = "SQL语法分析和SQL解释实现. SQL语法分析/解释。为设计/实现SQL语法分析器提供参考";
				page.WriteToResponse(writeText);
				Assert.AreEqual(writeText, context.Response.GetText());
				
				context.Request.SetInputStream(writeText);
				Assert.AreEqual(writeText, page.GetInputStreamString());


				string cookieData = Guid.NewGuid().ToString();
				page.WriteCookie("test-write-cookie1", cookieData);
				Assert.AreEqual(cookieData, page.ReadCookie("test-write-cookie1"));


				string headerData = Guid.NewGuid().ToString();
				page.WriteHeader("test-write-header", headerData);

				ArrayList customHeaders = context.Response.Response.GetValue("_customHeaders") as ArrayList;
				object header = customHeaders[0];
				Assert.AreEqual("test-write-header", (string)header.GetValue("Name"));
				Assert.AreEqual(headerData, (string)header.GetValue("Value"));

			}
		}

		



	





	}
}
