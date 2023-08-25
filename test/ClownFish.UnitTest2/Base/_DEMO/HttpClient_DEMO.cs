using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;
using ClownFish.UnitTest.Base;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.UnitTest;

namespace ClownFish.Base.DEMO
{
    [TestClass]
    public class HttpClient_DEMO
    {
        public void 常规用法_获取各种类型的结果()
        {
            // 先构造一个HttpOption对象
            HttpOption httpOption = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/test1.aspx",
                Header = new { X_a = "a1", X_b = "b2" },
                Format = SerializeFormat.Form,
                Data = new { id = 2, name = "abc" }
            };


            // 下面演示一些常见的获取结果的方式

            // 1，以文本形式（含HTML）获取服务端的返回结果
            string text = httpOption.GetResult();

            // 2，以二进制形式获取服务端的返回结果
            byte[] bin = httpOption.GetResult<byte[]>();

            // 3，如果服务端返回 json / xml，可以直接通过反序列化得到强类型结果
            Product product = httpOption.GetResult<Product>();

            // 4，以文本形式获取服务端的返回结果，并需要访问响应头
            HttpResult<string> httpResult1 = httpOption.GetResult<HttpResult<string>>();
            string text2 = httpResult1.Result;
            string header1 = httpResult1.Headers.Get("Content-Type");  // 读取响应头

            // 5，以二进制形式获取服务端的返回结果，并需要访问响应头
            HttpResult<byte[]> httpResult2 = httpOption.GetResult<HttpResult<byte[]>>();
            byte[] bin2 = httpResult2.Result;

            // 6，服务端返回 json / xml，结果反序列化，并需要访问响应头
            HttpResult<Product> httpResult3 = httpOption.GetResult<HttpResult<Product>>();
            Product product2 = httpResult3.Result;

            // 7, 以Stream形式获取服务端的返回结果
            // 注意：拿到结果后，请使用 using 包起来使用
            Stream steram = httpOption.GetResult<Stream>();

            // 8, 以HttpWebResponse形式获取服务端的返回结果
            // 注意：拿到response结果后，请使用 using 包起来使用
            HttpWebResponse response = httpOption.GetResult<HttpWebResponse>();
        }

        private static readonly string s_small_file_txt = Path.Combine(AppDomain.CurrentDomain.GetTempPath(), "small_file_中文汉字.txt");
        private static readonly string s_small_file_bin = Path.Combine(AppDomain.CurrentDomain.GetTempPath(), "small_file_中文汉字.bin");

        static HttpClient_DEMO()
        {
            // 这里创建2个小文件
            string text = "文件内容，用于测试文件上传。";
            File.WriteAllText(s_small_file_txt, text, Encoding.UTF8);

            byte[] bb = Guid.NewGuid().ToByteArray();
            File.WriteAllBytes(s_small_file_bin, bb);
        }


        [TestMethod]
        public void 简单的GET请求()
        {
            HttpOption httpOption = new HttpOption {
                Url = "http://www.fish-test.com/show-request2.aspx",
                Data = new { id = 2, name = "abc" }  // 这里的数据将会合并到URL中
            };
            //Console.WriteLine(httpOption.ToRawText());

            string text = httpOption.GetResult();
            Console.WriteLine(text);

            Assert.IsTrue(text.Contains("Url: http://www.fish-test.com/show-request2.aspx?id=2&name=abc"));
        }

        [TestMethod]
        public void POST_提交表单数据()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-request2.aspx",
                Data = new { id = 2, name = "abc" }
            }.GetResult();
            Console.WriteLine(text);

            Assert.IsTrue(text.Contains("Url: http://www.fish-test.com/show-request2.aspx"));
            Assert.IsTrue(text.Contains("Content-Type: application/x-www-form-urlencoded"));
        }

        [TestMethod]
        public void POST_JSON方式提交数据()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-body.aspx",
                Data = new { id = 2, name = "abc" },
                Format = Http.SerializeFormat.Json     // 注意这里
            }.GetResult();
            Console.WriteLine(text);

            Assert.IsTrue(text.Contains("Content-Type: application/json"));
        }

        [TestMethod]
        public void POST_以XML方式提交数据()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-body.aspx",
                Data = new NameValue { Name = "abc", Value = "123" },
                Format = Http.SerializeFormat.Xml     // 注意这里
            }.GetResult();
            Console.WriteLine(text);

            Assert.IsTrue(text.Contains("Content-Type: application/xml"));
        }


        [TestMethod]
        public void POST_bytes()
        {
            var data = new NameValue { Name = "abc", Value = "123" };
            string json = data.ToJson();
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-body.aspx",
                Data = bytes,
                Format = Http.SerializeFormat.Binary     // 注意这里
            }.GetResult();
            Console.WriteLine(text);

            Assert.IsTrue(text.Contains("Content-Type: application/octet-stream"));
        }


        [TestMethod]
        public void POST_GzipJson()
        {
            var data = new NameValue { Name = "abc", Value = "123" };
            string json = data.ToJson();
            byte[] jsonGzip = json.ToGzip();

            HttpOption httpOption = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-request2.aspx",
                Data = jsonGzip,
                Format = SerializeFormat.None
            };
            httpOption.Headers.Add("Content-Type", "application/json");
            httpOption.Headers.Add("Content-Encoding", "gzip");

            string text = httpOption.GetResult();
            Console.WriteLine(text);

            Assert.IsTrue(text.Contains("Content-Type = application/json"));
            Assert.IsTrue(text.Contains("Content-Encoding = gzip"));
            Assert.IsTrue(text.Contains("Content-Length = 45"));
        }

        [TestMethod]
        public void POST_Stream()
        {
            var data = new NameValue { Name = "abc", Value = "123" };
            string json = data.ToJson();
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            using( MemoryStream ms = new MemoryStream(bytes, false) ) {

                string text = new HttpOption {
                    Method = "POST",
                    Url = "http://www.fish-test.com/show-body.aspx",
                    UserAgent = "ClownFish.UnitTest",
                    Data = ms,
                    Format = Http.SerializeFormat.Binary     // 注意这里
                }.GetResult();
                Console.WriteLine(text);

                Assert.IsTrue(text.Contains("Content-Type: application/octet-stream"));
            }
        }

        [TestMethod]
        public void POST_表单数据_含文件上传()
        {
            string filePath1 = s_small_file_txt;
            string filePath2 = s_small_file_bin;
            string filePath3 = "ClownFish.Log.config";

            string cnText = "文件上传模式@##$R$#&$^*^%@#$@$~@!$!";

            string hash1 = HashHelper.FileMD5(filePath1);
            string hash2 = HashHelper.FileMD5(filePath2);
            string hash3 = HashHelper.FileMD5(filePath3);

            HttpOption httpOption = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-request2.aspx",
                Format = SerializeFormat.Multipart,  // 文件上传模式
                Data = new {
                    id = 123456789,
                    name = "Fish Li",
                    data = cnText,

                    // 如果 Data 属性包含 FileInfo 或者 HttpFile 类型的属性值，就认为是上传文件
                    file1 = new FileInfo(filePath1),
                    file2 = new FileInfo(filePath2),
                    file3 = new FileInfo(filePath3),
                }
            };

            var result = httpOption.GetResult<HttpResult<string>>();
            Console.WriteLine(result.Result);

            Assert.IsTrue(result.Result.Contains("id = 123456789"));
            Assert.IsTrue(result.Result.Contains("name = Fish Li"));
            Assert.IsTrue(result.Result.Contains("data = " + cnText));

            Assert.AreEqual(hash1, result.Headers["x-FileMd5-file1"]);
            Assert.AreEqual(hash2, result.Headers["x-FileMd5-file2"]);
            Assert.AreEqual(hash3, result.Headers["x-FileMd5-file3"]);

            Assert.IsTrue(result.Result.Contains("Content-Type: multipart/form-data"));
        }



        [TestMethod]
        public void 根据Raw文本发送请求()
        {
            // 下面这段文本，可以从 Fiddler 或者一些浏览器的开发工具中获取
            // 拿到这段文本，不需要做任何处理，直接调用 HttpOption.FromRawText 就可以了，就是这样简单！


            string request = @"
POST http://www.fish-test.com/show-request2.aspx HTTP/1.1
Host: www.fish-test.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0
Accept: */*
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/x-www-form-urlencoded; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-test.com/Pages/Demo/TestAutoFindAction.htm
Cookie: hasplmlang=_int_; LoginBy=productKey; PageStyle=Style2;
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache

input=Fish+Li&Base64=%E8%BD%AC%E6%8D%A2%E6%88%90Base64%E7%BC%96%E7%A0%81
";
            string text = 
                HttpOption.FromRawText(request)     // 构建 HttpOption 实例
                .GetResult();                       // 发送请求
            
            Console.WriteLine(text);

            Assert.IsTrue(text.Contains("Content-Type: application/x-www-form-urlencoded"));
        }

        [TestMethod]
        public void 结合RawText和Data发送请求()
        {
            // 可以从 Fiddler 抓到所需要请求头，去掉：数据部分
            string request = @"
POST http://www.fish-test.com/show-request2.aspx HTTP/1.1
Host: www.fish-test.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0
Accept: */*
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-test.com/Pages/Demo/TestAutoFindAction.htm
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
            Console.WriteLine(result);

            Assert.IsTrue(result.Contains("Content-Type: application/x-www-form-urlencoded"));
        }

        [TestMethod]
        public void 设置请求超时时间()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-request.aspx",
                Data = new { id = 2, name = "abc" },
                Timeout = 3000     // 3 秒超时
            }.GetResult();
            Console.WriteLine(text);



            string text2 = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-request.aspx",
                Data = new { id = 2, name = "abc" },
                Timeout = 0     // 无限等待
            }.GetResult();
        }


        [TestMethod]
        public void 发送时指定请求头()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-request2.aspx",
                Data = new { id = 2, name = "abc" },
                Headers = new Dictionary<string, string>() {
                            { "X-Requested-With", "XMLHttpRequest" },
                            { "User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64)"} }
            }.GetResult();
            Console.WriteLine(text);
        }


        [TestMethod]
        public void 处理请求头与响应头()
        {
            HttpResult<string> result = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/test-header.aspx",
                Data = new { id = 2, name = "abc" },

                Header = new {      // 指定二个请求头，服务端会将它们合并
                    X_a = "a1",     // 对应请求头：X-a: a1
                    X_b = "b2"      // 对应请求头：X-b: b2
                }
            }.GetResult<HttpResult<string>>();

            // 注意调用上面方法时指定的泛型参数
            // 如果需要读取响应头，需要指定 HttpResult<T> 的类型参数

            // 读取响应结果
            string responseText = result.Result;    // this is html

            // 读取响应头
            string header = result.Headers["X-add-result"];
            Assert.AreEqual("a1b2", header);

            Console.WriteLine(responseText);
        }

        [TestMethod]
        public void 发送请求时指定身份信息()
        {
            string text = new HttpOption {
                Url = "http://RabbitHost:15672/api/queues",
                Credentials = new NetworkCredential("fishli", "1qazxsw2_Sedrvsg234_ef234_ZZ"),
                Timeout = 6 * 1000,
            }.GetResult();
            Console.WriteLine(text);
        }


        [TestMethod]
        public void 发送当前进程的身份到服务端_服务端采用Windows身份认证()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/show-request.aspx",
                Data = new { id = 2, name = "abc" },
                Credentials = CredentialCache.DefaultNetworkCredentials     // 注意这里
            }.GetResult();
            Console.WriteLine(text);
        }

        [TestMethod]
        public void 检验Gzip解压缩()
        {
            HttpResult<string> result = new HttpOption {
                Url = "http://www.fish-test.com/gzip-page.aspx",
            }.GetResult<HttpResult<string>>();

            string responseText = result.Result;
            Console.WriteLine(responseText);

            // 服务端返回的结果是GZIP内容，所以如果不自动解压缩，结果会是“乱码”
            Assert.IsTrue(responseText.IndexOf("HttpClient 和生存期管理") >= 0);

            
        }


        [TestMethod]
        public void 发送HTTP请求_维护会话COOKIE()
        {
            // 准备一个Cookie会话容器
            CookieContainer cookieContainer = new CookieContainer();

            HttpOption page1 = new HttpOption {
                Url = "http://www.fish-test.com/user/page1.aspx",
            };

            // 没有登录，返回 403
            int status = GetStatusCode(page1);
            Assert.AreEqual(403, status);



            // 登录
            HttpOption login = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/user/login.aspx",
                Data = new { name = "fish li", pwd = "abc" },
                Cookie = cookieContainer // 接收cookie
            };
            string text = login.GetResult();
            Console.WriteLine(text);
            Assert.AreEqual("Login OK.", text);


            // 再次访问，带上Cookie，就正常了
            page1 = new HttpOption {
                Url = "http://www.fish-test.com/user/page1.aspx",
                Cookie = cookieContainer
            };

            int status2 = GetStatusCode(page1);
            Assert.AreEqual(200, status2);






            string name = Guid.NewGuid().ToString("N").Substring(0, 8);
            string value = Guid.NewGuid().ToString();

            // 写入Cookie
            HttpOption writecookie = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-test.com/user/write-cookie.aspx",
                Data = new { name, value },
                Cookie = cookieContainer
            };
            string text2 = writecookie.GetResult();
            Console.WriteLine(text2);

            string expected = string.Format("Write Cookie OK: {0} = {1}", name, value);
            Assert.AreEqual(expected, text2);


            string current = new HttpOption {
                Url = "http://www.fish-test.com/show-request2.aspx",
                Cookie = cookieContainer
            }.GetResult();
            Console.WriteLine(current);


            // 读取Cookie
            HttpOption showcookie = new HttpOption {
                Method = "GET",
                Url = "http://www.fish-test.com/user/show-cookie.aspx",
                Data = new { name },
                Cookie = cookieContainer
            };
            string text3 = showcookie.GetResult();
            Console.WriteLine(text3);

            expected = string.Format("Cookie: {0} = {1}", name, value);
            Assert.AreEqual(expected, text3);
        }


        [TestMethod]
        public void TestCookieContainer()
        {
            CookieContainer container = new CookieContainer();

            container.SetCookies(
                new Uri("http://www.fish-test.com/user/login.aspx"),
                "us123xx=74A7AA7D047A8C7272BD139C91FA5D43F1291F7; expires=Fri, 14-Feb-2020 03:32:24 GMT; path=/; HttpOnly; SameSite=Lax"
                );

            container.SetCookies(
                new Uri("http://www.fish-test.com/user/write-cookie.aspx"),
                "38a1f922=b4e2c60b-1e06-4443-89cd-8bf552e727c5; expires=Wed, 19-Feb-2020 03:02:24 GMT; path=/"
                );

            string header = container.GetCookieHeader(new Uri("http://www.fish-test.com/show-request2.aspx"));
            Console.WriteLine(header);
        }


        private static int GetStatusCode(HttpOption option)
        {
            try {
                var xx = option.GetResult();
                return 200;
            }
            catch( RemoteWebException remoteWebException ) {
                return remoteWebException.StatusCode;
            }
            catch( WebException ex ) {
                return (int)(ex.Response as HttpWebResponse).StatusCode;
            }
            catch( Exception ) {
                return 500;
            }
        }



        [TestMethod]
        public void 仅仅发送请求_不需要读取结果()
        {
            HttpOption httpOption = new HttpOption {
                Url = "http://www.fish-test.com/show-request2.aspx",
            };

            httpOption.GetResult<ClownFish.Base.Void>();  // 注意这里的类型参数
        }



        [TestMethod]
        public void 发送请求_启用重试功能()
        {
            HttpOption httpOption = new HttpOption {
                Url = "http://www.fish-test.com/show-request2.aspx",
            };

            // 创建默认的重试策略
            // 当发生 “网络不通” 或者 “HTTP 502,503” 时，最大重试 7 次，每次间隔 1000 毫秒
            Retry retry = HttpRetry.Create();
            string html = httpOption.GetResult<string>(retry);
        }


    }
}
