using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 表示在数据访问执行过程中发生的异常。
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237")]
	public sealed class DbExceuteException : Exception, ClownFish.Base.Internals.IIncludeDbCommand
    {
		/// <summary>
		/// SQL执行时关联的命令对象。
		/// </summary>
		public DbCommand Command { get; private set; }
		// 这个对象不可序列化


        /// <summary>
        /// 当前连接的连接字符串
        /// </summary>
        public string ConnectionString { get; private set; }


		/// <summary>
		/// 初始化 <see cref="DbExceuteException"/>对象。
		/// </summary>
		/// <param name="innerException">当前异常对象。</param>
		/// <param name="command"><see cref="DbCommand"/>的实例。</param>
		public DbExceuteException(Exception innerException, DbCommand command) :
			base(innerException.Message, innerException)
		{
			if( innerException == null )
				throw new ArgumentNullException("innerException");

			if( command == null )
				throw new ArgumentNullException("command");

			Command = command;

            if( command.Connection != null )
                this.ConnectionString = command.Connection.ConnectionString;
		}
		
	}
}
