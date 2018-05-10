using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Base
{
    /// <summary>
    /// 封装一些与异常有关的扩展方法
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 获取异常的错误原因，相当于 ex.GetBaseException().Message，
        /// 如果是AggregateException，会将所有内部异常的原因全部拼接在一起。
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetMessage(this Exception ex)
        {
            if( ex == null )
                throw new ArgumentNullException(nameof(ex));

            AggregateException ex1 = ex as AggregateException;
            if( ex1 != null ) {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ex1.Message);

                foreach( var x in ex1.InnerExceptions )
                    sb.AppendLine("-" + x.GetBaseException().Message);

                return sb.ToString();
            }


            return ex.GetBaseException().Message;
        }
    }
}
