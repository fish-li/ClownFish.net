using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.WebClient;

namespace ClownFish.Base.UnitTest._Sample
{
    class Retry_DEMO
    {
        public void 发送HTTP请求_支持重试()
        {
            // 由于 HttpOption.GetResult 方法提供重试功能
            // 这里使用一个 null 的 Retry变量，指示使用HttpOption默认的重试功能
            Retry retry = null;

            string text = new HttpOption {
                Url = "http://www.fish-web-demo.com/abc.aspx",
                Data = new { id = 2, name = "abc" }
            }.GetResult(retry);
        }



        public void 发送HTTP请求_定制重试策略()
        {
            Retry retry =
                Retry.Create(100, 2000)     // 重试 100 次，间隔 2 秒
                .Filter<WebException>();    // 仅当出现 WebException 异常时才重试。

            string text = new HttpOption {
                Url = "http://www.fish-web-demo.com/abc.aspx",
                Data = new { id = 2, name = "abc" }
            }.GetResult(retry);
        }



        public void 读取文件_最大重试10次()
        {
            string text =
                Retry.Create(100, 0)        // 指定重试10，使用默认的重试间隔时间，且不分辨异常类型（有异常就重试）
                    .Run(() => {
                        return System.IO.File.ReadAllText(@"c:\abc.txt", Encoding.UTF8);
                    });
        }



        // 更多使用方法，可参考 RetryTest.cs
    }
}
