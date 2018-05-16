using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Log.Model
{
    /// <summary>
    /// 表示一条警告消息
    /// </summary>
    public sealed class WarningInfo : BaseInfo
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message"></param>
        public WarningInfo(string message)
        {
            if( string.IsNullOrEmpty(message) )
                throw new ArgumentNullException(nameof(message));

            this.FillBaseInfo();
            this.Message = message;
        }
    }
}
