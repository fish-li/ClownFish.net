using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Model;

namespace ClownFish.Log
{
    /// <summary>
    /// 异常的日志记录工具类
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 将一个异常对象通过 ClownFish.Log 记录下来，
        /// 如果还需要记录HTTP请求或者SQL执行信息，请先调用ExceptionInfo.Create()
        /// </summary>
        /// <param name="ex">Exception实例</param>
        /// <param name="syncWrite">是否采用同步方式写入日志</param>
        public static void SaveToLog(this Exception ex, bool syncWrite = false)
        {
            if( ex == null )
                throw new ArgumentException(nameof(ex));

            ExceptionInfo exceptionInfo = ExceptionInfo.Create(ex);
            SaveToLog(exceptionInfo);
        }

        /// <summary>
        /// 与ClownFish.Log.LogHelper.Write()等效的扩展方法
        /// </summary>
        /// <param name="ex">ExceptionInfo实例</param>
        /// <param name="syncWrite">是否采用同步方式写入日志</param>
        public static void SaveToLog(this ExceptionInfo ex, bool syncWrite = false)
        {
            if( ex == null )
                throw new ArgumentException(nameof(ex));

            if( syncWrite )
                LogHelper.SyncWrite(ex);
            else
                LogHelper.Write(ex);
        }
    }
}
