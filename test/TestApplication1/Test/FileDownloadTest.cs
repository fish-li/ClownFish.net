using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web.Client;
using ClownFish.TestApplication1.Common;

namespace ClownFish.TestApplication1.Test
{
	public class FileDownloadTest : TestBase
	{
		[TestMethod("文件下载测试 -- StreamResult 指定文件名")]
		public async Task Test1()
		{
			HttpOption option = HttpOption.FromRawText(@"
POST http://www.fish-mvc-demo.com/ajax/ns/TestFile/Download1.aspx HTTP/1.1
Host: www.fish-mvc-demo.com
Connection: keep-alive
Content-Length: 171
Cache-Control: max-age=0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
Origin: http://www.fish-mvc-demo.com
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.155 Safari/537.36 OPR/31.0.1889.174
Content-Type: application/x-www-form-urlencoded
Referer: http://www.fish-mvc-demo.com/Pages/Demo/TestFileDownload.aspx
Accept-Encoding: gzip, deflate, lzma
Accept-Language: zh-CN,zh;q=0.8

filename=%E4%B8%AD%E6%96%87%E6%B1%89%E5%AD%97%E6%97%A0%E4%B9%B1%E7%A0%81%7E%21%40%23%24%25%5E%26*%28%29_%2B-%3D%3C%3E%3F%7C.txt&submit=%E4%B8%8B%E8%BD%BD%E6%96%87%E4%BB%B6");

			string headerValue = null;

			option.ReadResponseAction = (response) =>headerValue = response.Headers["Content-Disposition"];

			string returnText = await option.SendAsync<string>();
			string expected = Guid.Empty.ToString();

			Assert.AreEqual(expected, returnText);
			Assert.IsTrue(System.Web.HttpUtility.UrlDecode(headerValue).IndexOf("中文汉字无乱码") > 0);

		}

		[TestMethod("文件下载测试 -- URL 指定文件名")]
		public async Task Test2()
		{
			HttpOption option = HttpOption.FromRawText(@"
GET http://www.fish-mvc-demo.com/file-download/demo1/%E4%B8%AD%E6%96%87%E6%B1%89%E5%AD%97%E6%97%A0%E4%B9%B1%E7%A0%81~!%40%23%24%25%5E%26*()_%2B-%3D%3C%3E%3F%7C.txt HTTP/1.1
Host: www.fish-mvc-demo.com
Connection: keep-alive
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.155 Safari/537.36 OPR/31.0.1889.174
Referer: http://www.fish-mvc-demo.com/Pages/Demo/TestFileDownload.aspx
Accept-Encoding: gzip, deflate, lzma, sdch
Accept-Language: zh-CN,zh;q=0.8

");

			string actual = await option.SendAsync<string>();
			string expected = Guid.Empty.ToString();
			Assert.AreEqual(expected, actual);
			
		}
	}
}
