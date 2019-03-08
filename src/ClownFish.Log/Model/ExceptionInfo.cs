using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using ClownFish.Base;
using ClownFish.Base.Xml;
using MongoDB.Bson.Serialization.Attributes;

namespace ClownFish.Log.Model
{
	/// <summary>
	/// 异常消息的持久化数据结构
	/// </summary>
#if _MongoDB_
	[BsonIgnoreExtraElements]
#endif	

	public class ExceptionInfo : BaseInfo
	{
		/// <summary>
		/// HTTP请求相关信息
		/// </summary>
		public HttpInfo HttpInfo { get; set; }


		/// <summary>
		/// 业务相关信息
		/// </summary>
		public BusinessInfo BusinessInfo { get; set; }


		/// <summary>
		/// 异常类型
		/// </summary>
		public string ExceptionType { get; set; }


		/// <summary>
		/// 异常信息
		/// </summary>
		public XmlCdata Exception { get; set; }


		// 说明：异常 Message 属性写到基类上去。


		/// <summary>
		/// SQL相关的执行信息
		/// </summary>
		public SqlInfo SqlInfo { get; set; }


		/// <summary>
		/// 附加信息
		/// </summary>
		public XmlCdata Addition { get; set; }



        /// <summary>
        /// 根据异常及运行中的相关信息构造完整的异常日志信息
        /// </summary>
        /// <param name="ex">Exception实例（必选）</param>
        /// <param name="context">HttpContext实例（可选）</param>
        /// <param name="dbCommand">DbCommand实例（可选）</param>
        /// <returns></returns>
        public static ExceptionInfo Create(Exception ex, HttpContext context, DbCommand dbCommand)
		{
			if( ex == null )
				throw new ArgumentNullException("ex");

			ExceptionInfo info = new ExceptionInfo();
			info.FillBaseInfo();
            info.Message = ex.GetErrorMessage();
            info.ExceptionType = ex.GetType().FullName;
			info.Exception = ex.ToString();

			if( context != null ) 
				info.HttpInfo = HttpInfo.Create(context);

			if( dbCommand != null )
				info.SqlInfo = SqlInfo.Create(dbCommand);

			return info;
		}


		/// <summary>
		/// 根据异常对象构造异常日志信息
		/// </summary>
		/// <param name="ex">Exception实例（必选）</param>
		/// <returns></returns>
		public static ExceptionInfo Create(Exception ex)
		{
			return Create(ex, null, null);
		}

	}
}
