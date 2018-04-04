using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Log.Model
{
    /// <summary>
    /// 记录某次HTTP调用的报文信息
    /// </summary>
    public class HttpRequestData : BaseInfo
    {
        /// <summary>
        /// HttpInfo实例
        /// </summary>
        public HttpInfo HttpInfo { get; set; }
    }
}
