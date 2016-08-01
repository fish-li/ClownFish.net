using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 表示连接与事务的作用域
	/// </summary>
	public sealed class ConnectionScope : IDisposable
	{
		private int _refCount;

		#region 构造函数
		internal ConnectionScope(DbContext context)
		{
			if( context == null )
				throw new ArgumentNullException("context");

			_refCount++;
			Context = context;
			SetCurrent();
		}

		/// <summary>
		/// 使用默认的连接字符串创建ConnectionScope实例
		/// </summary>
		/// <returns></returns>
		public static ConnectionScope Create()
		{
			DbContext context = DbContext.Create();
			return new ConnectionScope(context);
		}


		/// <summary>
		/// 尝试从当前上下文中获取已存在的ConnectionScope，
		/// 如果没有已存在的实例，就用默认的连接字符串创建一个ConnectionScope实例
		/// </summary>
		/// <returns></returns>
		public static ConnectionScope GetExistOrCreate()
		{
			ConnectionScope current = s_current;
			if( current != null ) {
				current._refCount++;        // 增加引用计数
				return current;
			}

			return Create();
		}

		/// <summary>
		/// 根据指定的数据库连接字符串创建ConnectionScope实例
		/// </summary>
		/// <param name="connectionName"></param>
		/// <returns></returns>
		public static ConnectionScope Create(string connectionName)
		{
			DbContext context = DbContext.Create(connectionName);
			return new ConnectionScope(context);
		}

		/// <summary>
		/// 根据指定的数据库连接字符串和数据库类型创建ConnectionScope实例
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="providerName"></param>
		/// <returns></returns>
		public static ConnectionScope Create(string connectionString, string providerName)
		{
			DbContext context = DbContext.Create(connectionString, providerName);
			return new ConnectionScope(context);
		}

		#endregion


		#region static Current 封装

		/// <summary>
		/// 用一个静态变量来保存【当前实例】的引用
		/// </summary>
		[ThreadStatic]
		private static ConnectionScope s_current;

		/// <summary>
		/// 当前using范围块之前的ConnectionScope实例
		/// </summary>
		private ConnectionScope _lastInstance;

		//internal static ConnectionScope Current
		//{
		//	get {
		//		ConnectionScope current = s_current;

		//		if( current == null )
		//			throw new InvalidProgramException("当前代码执行环境中没有创建ConnectionScope的实例");

		//		return current;
		//	}
		//}

		private void SetCurrent()
		{
			// 保存前一个实例引用（有可能是 null）
			_lastInstance = s_current;

			// 将当前实例设置为【当前】实例
			s_current = this;
		}

		/// <summary>
		/// 实现IDisposable接口
		/// </summary>
		public void Dispose()
		{
			_refCount--;

			if( _refCount == 0 ) {
				// 恢复之前的【当前】实例
				s_current = _lastInstance;

				if( this.Context != null ) {
					this.Context.Dispose();
					this.Context = null;
				}
			}
		}

		#endregion


		#region DbContext 封装

		/// <summary>
		/// DbContext实例引用
		/// </summary>
		public DbContext Context { get; private set; }

		internal static DbContext GetDefaultDbConext()
		{
			//return ConnectionScope.Current.Context;

			ConnectionScope current = s_current;

			if( current == null ) {
				if( Initializer.Instance.IsAutoCreateOneOffDbContext ) {
					DbContext context = DbContext.Create();
					context.AutoDisposable = true;
					return context;
				}
				else {
					throw new InvalidProgramException("当前代码执行环境中没有创建ConnectionScope的实例");
				}
			}
			else {
				return current.Context;
			}
		}

		/// <summary>
		/// 开启数据库事务
		/// </summary>
		public void BeginTransaction()
		{
			Context.BeginTransaction();
		}

		/// <summary>
		/// 提交数据库事务
		/// </summary>
		public void Commit()
		{
			Context.Commit();
		}


		#endregion

	}
}
