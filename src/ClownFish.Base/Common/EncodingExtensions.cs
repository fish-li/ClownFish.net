using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
    internal static class EncodingExtensions
    {
        /// <summary>
        /// 如果参数为NULL，则返回默认值，否则返回本身
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Encoding TryGet(this Encoding encoding)
        {
            return encoding ?? Encoding.UTF8;
        }
    }
}
