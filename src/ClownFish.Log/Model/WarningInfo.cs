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
        /// 根据消息构造WarningInfo实例
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static WarningInfo Create(string message)
        {
            if( string.IsNullOrEmpty(message) )
                throw new ArgumentNullException(nameof(message));

            WarningInfo info = new WarningInfo();
            info.FillBaseInfo();
            info.Message = message;
            return info;
        }



    }
}
