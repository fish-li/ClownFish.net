using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebApi.Result
{
    /// <summary>
    /// 表示Action的执行结果为XML
    /// </summary>
    public sealed class XmlResult : IActionResult
    {
        /// <summary>
		/// 需要以JSON形式输出的数据对象
		/// </summary>
		public object Model { get; private set; }

        /// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="model">将要序列化的对象</param>
		public XmlResult(object model)
        {
            if( model == null )
                throw new ArgumentNullException("model");

            this.Model = model;
        }


        void IActionResult.Ouput(NHttpContext context)
        {
            context.Response.ContentType = ResponseContentType.XmlUtf8;
            string xml = this.Model.ToXml();
            context.Response.WriteAll(xml.GetBytes());
        }
    }
}
