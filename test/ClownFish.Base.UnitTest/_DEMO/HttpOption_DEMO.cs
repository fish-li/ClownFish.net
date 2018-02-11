using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.WebClient;

namespace ClownFish.Base.UnitTest._Sample
{
    class HttpOption_DEMO
    {
        public void 发送HTTP请求_GET()
        {
            string text = new HttpOption {
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" }
            }.GetResult();
        }
                
        public void 发送HTTP请求_POST_提交表单数据()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" }
            }.GetResult();
        }


        public void 发送HTTP请求_POST_以JSON方式提交数据()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },
                Format = Http.SerializeFormat.Json     // 注意这里
            }.GetResult();
        }


        public void 发送HTTP请求_POST_以XML方式提交数据()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },
                Format = Http.SerializeFormat.Xml     // 注意这里
            }.GetResult();
        }

        public void 根据请求文本发送请求()
        {
            // 下面这段文本，可以从 Fiddler 或者一些浏览器的开发工具中获取
            // 拿到这段文本，不需要做任何处理，直接调用 HttpOption.FromRawText 就可以了，就是这样简单！


            string request = @"
POST http://www.fish-web-demo.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1
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


        public void 发送HTTP请求_POST_提交表单数据_支持文件上传()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2,

                    // 如果 Data 属性包含 FileInfo 或者 HttpFile 类型的属性值，就认为是上传文件
                    file1 = new FileInfo(@"c:\abc.txt"),
                    file2 = new FileInfo(@"c:\qaz.dat")
                }
            }.GetResult();


            // 说明：
            // 1、上传文件时，不要为HttpOption指定 Format 属性，因为默认值就是支持上传文件。
            // 2、服务端如果使用ClownFish.Web 可以使用 HttpFile 来接收上传文件。
        }



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


        public void 发送HTTP请求_读取响应头()
        {
            string header1 = null;
            string header2 = null;

            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },
                ReadResponseAction = rep => {     // 注意这里
                    header1 = rep.Headers["x-header1"];
                    header2 = rep.Headers["x-header2"];
                }
            }.GetResult();
        }



        public void 发送HTTP请求_设置请求超时时间()
        {
            string text = new HttpOption {
                Method = "POST",
                Url = "http://www.fish-web-demo.com/test1.aspx",
                Data = new { id = 2, name = "abc" },
                Timeout = 3     // 注意这里
            }.GetResult();
        }



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
