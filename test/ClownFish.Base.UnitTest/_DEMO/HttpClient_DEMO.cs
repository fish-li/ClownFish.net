using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest._Sample
{
    [TestClass]
    public class HttpClient_DEMO
    {
        public void 常规用法介绍()
        {
            // 先构造一个HttpOption对象
            HttpOption httpOption = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Header = new { X_a = "a1", X_b = "b2" },
                Format = SerializeFormat.Form,
                Data = new { id = 2, name = "abc" }
            };


            // 下面演示 6 种获取结果的方式

            // 1，以文本形式获取服务端的返回结果
            string text = httpOption.GetResult();

            // 2，以二进制形式获取服务端的返回结果
            byte[] bin = httpOption.GetResult<byte[]>();

            // 3，服务端返回 json / xml，可以直接反序列化结果
            Product product = httpOption.GetResult<Product>();

            // 4，以文本形式获取服务端的返回结果，并需要访问响应头
            HttpResult<string> httpResult1 = httpOption.GetResult<HttpResult<string>>();
            string text2 = httpResult1.Result;
            
            // 5，以二进制形式获取服务端的返回结果，并需要访问响应头
            HttpResult<byte[]> httpResult2 = httpOption.GetResult<HttpResult<byte[]>>();
            byte[] bin2 = httpResult2.Result;

            // 6，服务端返回 json / xml，结果反序列化，并需要访问响应头
            HttpResult<Product> httpResult3 = httpOption.GetResult<HttpResult<Product>>();
            Product product2 = httpResult3.Result;


            // 读取响应头
            string header = httpResult1.Headers["name"];
        }



        [TestMethod]
        public void 发送HTTP请求_GET()
        {
            string text = new HttpOption {
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" }
            }.GetResult();
        }

        [TestMethod]
        public void 发送HTTP请求_POST_提交表单数据()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" }
            }.GetResult();
        }

        [TestMethod]
        public void 发送HTTP请求_POST_以JSON方式提交数据()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },
                Format = Http.SerializeFormat.Json     // 注意这里
            }.GetResult();
        }

        [TestMethod]
        public void 发送HTTP请求_POST_以XML方式提交数据()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new NameValue { Name = "abc", Value = "123" },
                Format = Http.SerializeFormat.Xml     // 注意这里
            }.GetResult();
        }

        [TestMethod]
        public void 根据请求文本发送请求()
        {
            // 下面这段文本，可以从 Fiddler 或者一些浏览器的开发工具中获取
            // 拿到这段文本，不需要做任何处理，直接调用 HttpOption.FromRawText 就可以了，就是这样简单！


            string request = @"
POST http://www.fish-web-demo.com/test1.aspx HTTP/1.1
Host: www.fish-web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0
Accept: */*
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/x-www-form-urlencoded; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestAutoFindAction.htm
Content-Length: 72
Cookie: hasplmlang=_int_; LoginBy=productKey; PageStyle=Style2;
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache

input=Fish+Li&Base64=%E8%BD%AC%E6%8D%A2%E6%88%90Base64%E7%BC%96%E7%A0%81
";
            string text = 
                HttpOption.FromRawText(request)     // 构建 HttpOption 实例
                .GetResult();                       // 发送请求
        }

        [TestMethod]
        public void 结合FromRawText和Data发送复杂的HTTP请求()
        {
            // 可以从 Fiddler 抓到所需要请求头，去掉：数据部分
            string request = @"
POST http://www.fish-web-demo.com/test1.aspx HTTP/1.1
Host: www.fish-web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0
Accept: */*
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestAutoFindAction.htm
Cookie: hasplmlang=_int_; LoginBy=productKey; PageStyle=Style2;
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
";

            // 1，根据一段长文本 快速设置 URL, method, headers
            HttpOption httpOption = HttpOption.FromRawText(request);

            // 2，设置提交数据与格式
            httpOption.Format = SerializeFormat.Form;
            httpOption.Data = new { id = 2, name = "aaaa", time = DateTime.Now };

            // 3，发送请求，获取结果
            string result = httpOption.GetResult();

        }

        [TestMethod]
        public void 发送HTTP请求_POST_提交表单数据_支持文件上传()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2,

                    // 如果 Data 属性包含 FileInfo 或者 HttpFile 类型的属性值，就认为是上传文件
                    file1 = new FileInfo(@"ClownFish.Base.xml"),
                    file2 = new FileInfo(@"ClownFish.Base.dll")
                }
            }.GetResult();


            // 说明：
            // 1、上传文件时，不要为HttpOption指定 Format 属性，因为默认值就是支持上传文件。
            // 2、服务端如果使用ClownFish.Web 可以使用 HttpFile 来接收上传文件。
        }


        [TestMethod]
        public void 发送HTTP请求_指定请求头()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },
                Headers = new Dictionary<string, string>() {
                            { "X-Requested-With", "XMLHttpRequest" },
                            { "User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64)"} }
            }.GetResult();
        }


        [TestMethod]
        public void 请求头与响应头演示()
        {
            HttpResult<string> result = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },

                Header = new {      // 指定二个请求头，服务端会将它们合并
                    X_a = "a1",     // 对应请求头：X-a: a1
                    X_b = "b2"      // 对应请求头：X-b: b2
                }
            }.GetResult<HttpResult<string>>();

            // 注意调用上面方法时指定的泛型参数
            // 如果需要读取响应头，需要指定 HttpResult<T> 的类型参数

            // GetResult()  等同于 GetResult<string>() ，这种调用只能获取响应体，拿不到响应头
            // GetResult< HttpResult<string> >() ，此时 GetResult<T>() 的类型参数T是 HttpResult<string>


            // 读取响应结果
            string responseText = result.Result;    // this is html

            // 读取响应头
            string header = result.Headers["X-add-result"];
            Assert.AreEqual("a1b2", header);
        }

        [TestMethod]
        public void 发送HTTP请求_回调演示()
        {
            string header1 = null;
            string header2 = null;

            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },

                // 在发送请求时，允许最后设置 HttpWebRequest 对象
                SetRequestAction = req => {
                    req.UserAgent = "UserAgent/test";
                },

                // 在读取结果前，允许直接从 HttpWebResponse 获取任何内容
                ReadResponseAction = rep => {     // 注意这里
                    header1 = rep.Headers["x-header1"];
                    header2 = rep.Headers["x-header2"];
                }
            }.GetResult();
        }


        public void 发送HTTP请求_维护会话COOKIE()
        {
            CookieContainer cookie = new CookieContainer();

            // 登录
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/login.aspx",
                Data = new { name = "user1", pwd = "abc" },
                Cookie = cookie     // 注意这里
            }.GetResult();

            if( text != "OK" )
                return;     // 登录失败


            // 发送第二个请求
            text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },
                Cookie = cookie     // 注意这里
            }.GetResult();


            // 发送第三个请求
            text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test2.aspx",
                Data = new { id = 2, name = "abc" },
                Cookie = cookie     // 注意这里
            }.GetResult();
        }



        [TestMethod]
        public void 发送HTTP请求_设置请求超时时间()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },
                Timeout = 3     // 注意这里
            }.GetResult();
        }


        [TestMethod]
        public void 发送HTTP请求_发送当前进程的身份到服务端_服务端采用Windows身份认证()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },
                Credentials = CredentialCache.DefaultNetworkCredentials     // 注意这里
            }.GetResult();
        }


        

    }
}
