using ClownFish.Base.TypeExtend;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 数据访问过程中的事件管理类
	/// </summary>
	public sealed class EventManager : BaseEventObject
	{
		/// <summary>
		/// 连接打开事件
		/// </summary>
		public event EventHandler<ConnectionEventArgs> ConnectionOpened;

		/// <summary>
		/// 命令执行之前事件
		/// </summary>
		public event EventHandler<CommandEventArgs> BeforeExecute;

		/// <summary>
		/// 命令执行之后事件
		/// </summary>
		public event EventHandler<CommandEventArgs> AfterExecute;

		/// <summary>
		/// 异常事件
		/// </summary>
		public event EventHandler<ExceptionEventArgs> OnException;

		/// <summary>
		/// 提交事务
		/// </summary>
		public event EventHandler<ConnectionEventArgs> OnCommit;



		internal void FireConnectionOpened(DbConnection connection, bool async = false)
		{
			EventHandler<ConnectionEventArgs> handler = ConnectionOpened;
			if( handler != null ) {
				ConnectionEventArgs e = new ConnectionEventArgs();
				e.Connection = connection;
				e.IsAsync = async;
				handler(null, e);
			}
		}


		internal void FireOnCommit(DbConnection connection)
		{
			EventHandler<ConnectionEventArgs> handler = OnCommit;
			if( handler != null ) {
				ConnectionEventArgs e = new ConnectionEventArgs();
				e.Connection = connection;
				handler(null, e);
			}
		}

		internal void FireBeforeExecute(BaseCommand command, bool async = false)
		{
			EventHandler<CommandEventArgs> handler = BeforeExecute;
			if( handler != null ) {
				CommandEventArgs e = new CommandEventArgs();
				e.Command = command;
				e.IsAsync = async;
				handler(null, e);
			}
		}

		internal void FireAfterExecute(BaseCommand command)
		{
			EventHandler<CommandEventArgs> handler = AfterExecute;
			if( handler != null ) {
				CommandEventArgs e = new CommandEventArgs();
				e.Command = command;
				handler(null, e);
			}
		}

		internal void FireOnException(BaseCommand command, Exception ex)
		{
			EventHandler<ExceptionEventArgs> handler = OnException;
			if( handler != null ) {
				ExceptionEventArgs e = new ExceptionEventArgs();
				e.Command = command;
				e.Exception = ex;
				handler(null, e);
			}
		}

	}


	/// <summary>
	/// 数据库连接事件参数
	/// </summary>
	public sealed class ConnectionEventArgs : EventArgs
	{
		/// <summary>
		/// 当前打开的数据库连接
		/// </summary>
		public DbConnection Connection { get; internal set; }
		/// <summary>
		/// 是否为异步打开
		/// </summary>
		public bool IsAsync { get; internal set; }
	}

	/// <summary>
	/// 执行命令事件参数（执行前，执行后）
	/// </summary>
	public class CommandEventArgs : EventArgs
	{
		/// <summary>
		/// 当前正在执行的数据库命令（CPQuery/XmlCommand/StoreProcedure）
		/// </summary>
		public BaseCommand Command { get; internal set; }

		/// <summary>
		/// 当前正在执行的数据库命令（DbCommand实例）
		/// </summary>
		public DbCommand DbCommand { get { return Command.Command; } }
		/// <summary>
		/// 是否为异步执行
		/// </summary>
		public bool IsAsync { get; internal set; }
	}



	/// <summary>
	/// 异常事件参数
	/// </summary>
	public sealed class ExceptionEventArgs : CommandEventArgs
	{
		/// <summary>
		/// 异常事件中包含的内部异常
		/// </summary>
		public Exception Exception { get; internal set; }
	}


}
