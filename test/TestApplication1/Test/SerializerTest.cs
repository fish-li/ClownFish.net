using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.WebClient;
using ClownFish.TestApplication1.Common;

namespace ClownFish.TestApplication1.Test
{
	public class SerializerTest  : TestBase
	{

		[TestMethod("XML, JSON序列化测试（18组测试）")]
		public void Test1()
		{
			HttpOption option = HttpOption.FromRawText(@"
GET http://www.fish-web-demo.com/api/ns/TestSerializer/GetXml.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: null; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 1
");

			string xmlResult = option.Send<string>();
			string expected = "</TestSerializerModel>";
			Assert.IsTrue(xmlResult.EndsWith(expected));




			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test1_AutoCheck.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/xml; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 2

" + xmlResult);

			string actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);




			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test2_AutoCheck.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/xml; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Content-Length: 387
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 3

" + xmlResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);





			option = HttpOption.FromRawText(@"
GET http://www.fish-web-demo.com/api/ns/TestSerializer/GetJson.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: null; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Cookie: PageStyle=Style2
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 4
");

			string jsonResult = option.Send<string>();
			Assert.IsTrue(jsonResult.StartsWith("{") && jsonResult.EndsWith("}"));





			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test1_AutoCheck.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/json; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 5

" + jsonResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);





			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test2_AutoCheck.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/json; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 6

" + jsonResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);






			option = HttpOption.FromRawText(@"
GET http://www.fish-web-demo.com/api/ns/TestSerializer/GetXml2.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: null; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 7

");

			xmlResult = option.Send<string>();
			expected = "</TestSerializerModel2>";
			Assert.IsTrue(xmlResult.EndsWith(expected));





			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test3_AutoCheck.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/xml; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 8

" + xmlResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);




			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test4_AutoCheck.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/xml; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 9

" + xmlResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);





			option = HttpOption.FromRawText(@"
GET http://www.fish-web-demo.com/api/ns/TestSerializer/GetJson2.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: null; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 10
");

			jsonResult = option.Send<string>();
			Assert.IsTrue(jsonResult.StartsWith("{") && jsonResult.EndsWith("}"));





			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test3_AutoCheck.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/json; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 11

" + jsonResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);






			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test4_AutoCheck.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/json; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 12

" + jsonResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);




			option = HttpOption.FromRawText(@"
GET http://www.fish-web-demo.com/api/ns/TestSerializer/GetXml5.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: null; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 13

");

			xmlResult = option.Send<string>();
			expected = "</TestSerializerModel3>";
			Assert.IsTrue(xmlResult.EndsWith(expected));




			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test5.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/xml; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 14

" + xmlResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);




			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test6.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/xml; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 15

" + xmlResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);





			option = HttpOption.FromRawText(@"
GET http://www.fish-web-demo.com/api/ns/TestSerializer/GetJson5.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: null; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 16
");

			jsonResult = option.Send<string>();
			Assert.IsTrue(jsonResult.StartsWith("{") && jsonResult.EndsWith("}"));






			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test5.aspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/json; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 17

" + jsonResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);






			option = HttpOption.FromRawText(@"
POST http://www.fish-web-demo.com/api/ns/TestSerializer/Test6.caspx HTTP/1.1
Host: www.fish-ClownFish.Web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0
Accept: text/plain, */*; q=0.01
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/json; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestSerializer.htm
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
test: 18

" + jsonResult);

			actual = option.Send<string>();
			expected = "1";
			Assert.AreEqual(expected, actual);





		}

	}
}
