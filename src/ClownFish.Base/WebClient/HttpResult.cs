using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;

namespace ClownFish.Base.WebClient
{
    /// <summary>
    /// 表示一个HTTP的调用结果，包含响应头和响应内容
    /// </summary>
    /// <typeparam name="T">响应内容的类型参数</typeparam>
    public sealed class HttpResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int StatusCode { get; internal set; }
        /// <summary>
        /// 从服务端返回响应头集合
        /// </summary>
        public HttpHeaderCollection Headers { get; internal set; }

        /// <summary>
        /// 响应体中的结果
        /// </summary>
        public T Result { get; internal set; }

    }
}
