using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ClownFish.Base
{
    /// <summary>
    /// 封装了Exception的一些扩展方法
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 获取异常有价值的错误描述
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetErrorMessage(this Exception ex)
        {
            if( ex == null )
                throw new ArgumentNullException(nameof(ex));


            Exception baseException = ex.GetBaseException();

            SqlException sqlException = baseException as SqlException;
            if( sqlException != null ) {
                return $"错误编号 {sqlException.Number}，级别 {sqlException.Class}，状态 {sqlException.State}，第 {sqlException.LineNumber} 行。" + sqlException.Message;
            }

            // TODO: 可能还需要完善

            if( ex == baseException )
                return ex.Message;
            else
                return ex.Message + " => " + baseException.Message;
        }
    }
}
