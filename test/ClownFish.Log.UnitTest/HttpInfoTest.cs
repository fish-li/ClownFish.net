using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.AspnetMock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class HttpInfoTest : TestBase
	{
		internal static WebContext CreateWebContext()
		{
			string requestText = @"
POST http://www.bing.com/sfdjosfdj/slfjsfj/sdjfosf.aspx HTTP/1.1
Host: www.bing.com
Connection: keep-alive
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36 OPR/34.0.2036.25
Accept-Encoding: gzip, deflate, lzma, sdch
Accept-Language: zh-CN,zh;q=0.8
Cookie: SRCHUSR=AUTOREDIR=0&GEOVAR=&DOB=20141216; _EDGE_V=1; SRCHUID=V=2&GUID=7209F97031814D0BAE91BBD675B73E65; SRCHD=D=3659242&AF=NOFORM; MUIDB=2363ECC0008A6E49222DEBD2012B6F37; MUID=2363ECC0008A6E49222DEBD2012B6F37; SRCHHPGUSR=CW=1265&CH=924&DPR=1
";

			return WebContext.FromRawText(requestText);
		}


		[TestMethod]
		public void Test_ExceptionInfo_Addition()
		{
			// 测试普通情况

			using( WebContext context = CreateWebContext() ) {
				context.SetUserName("Fish Li");
				context.AddSession("session-1", "aaaaaaaaaaaa");
				context.AddSession("session-2", DateTime.Now);
				context.AddSession("session-3", null);

				context.Request.SetInputStream("a=1&b=2");


				Exception ex = LogHelperTest.CreateException("Test: HttpInfo.Create");
				ExceptionInfo info = ExceptionInfo.Create(ex, context.HttpContext, null);
				info.Addition = Guid.NewGuid().ToString();

				LogHelper.SyncWrite(info);

				LogHelperTest.AssertWriteOK(info.Addition);
			}
		}


		


		[TestMethod]
		public void Test2()
		{
			// 测试有上传文件时，获取表单数据的逻辑

			using( WebContext context = CreateWebContext() ) {
				context.SetUserName("Fish Li");


				Type httpFileCollectionType = typeof(HttpContext).Assembly.GetType("System.Web.HttpFileCollection");
				ConstructorInfo ctor = httpFileCollectionType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
				var httpFileCollection = ctor.Invoke(null);

				MethodInfo addFileMethod = httpFileCollectionType.GetMethod("AddFile", BindingFlags.Instance | BindingFlags.NonPublic);
				addFileMethod.Invoke(httpFileCollection, new object[] { "file1", null });

				FieldInfo field = typeof(HttpRequest).GetField("_files", BindingFlags.Instance | BindingFlags.NonPublic);
				field.SetValue(context.HttpContext.Request, httpFileCollection);


				context.Request.SetForm("a=1&b=2");


				Exception ex = LogHelperTest.CreateException("Test: HttpInfo.Create");
				ExceptionInfo info = ExceptionInfo.Create(ex, context.HttpContext, null);
				info.Addition = Guid.NewGuid().ToString();

				LogHelper.SyncWrite(info);

				LogHelperTest.AssertWriteOK(info.Addition);
			}

		}


		[TestMethod]
		public void Test_HttpInfo_Create_Argument_null()
		{
			var result = HttpInfo.Create(null);
			Assert.IsNull(result);


			MethodInfo method = typeof(HttpInfo).GetMethod("SetHttpInfo", BindingFlags.Instance | BindingFlags.NonPublic);
			
			HttpInfo info = new HttpInfo();
			method.Invoke(info, new object[] { null });		// 没异常就算是通过，不需要断言
		}

	}
}
