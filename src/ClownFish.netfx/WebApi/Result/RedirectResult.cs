using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebApi.Result
{
    /// <summary>
    /// 表示一个重定向的结果
    /// </summary>
    public sealed class RedirectResult : IActionResult
    {
        /// <summary>
		/// 需要重定向的目标地址
		/// </summary>
		public string Url { get; private set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="url">需要重定向的目标地址</param>
        public RedirectResult(string url)
        {
            if( string.IsNullOrEmpty(url) )
                throw new ArgumentNullException("url");

            Url = url;
        }


        void IActionResult.Ouput(NHttpContext context)
        {
            //context.Response.Redirect(Url);
        }


    }
}
