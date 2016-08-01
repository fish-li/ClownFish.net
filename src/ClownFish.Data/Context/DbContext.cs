using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Data
{
	/// <summary>
	/// 数据访问的上下文信息
	/// </summary>
	public sealed class DbContext :IDisposable
	{
		#region 数据成员定义

		/// <summary>
		/// 是否需要自动释放。
		/// 如果启用，将在执行完一次数据库操作后自动释放
		/// </summary>
		internal bool AutoDisposable { get; set; }

		private bool _useTransaction;
		private IsolationLevel? _isolationLevel;
		private string _changeDatabase;

		internal readonly EventManager EventManager = ObjectFactory.New<EventManager>();

		internal ConnectionInfo ConnectionInfo { get; private set; }
		
		private DbProviderFactory _factory;
		internal DbProviderFactory Factory
		{
			get
			{
				if( _factory == null )
					_factory = ProviderFactoryHelper.GetDbProviderFactory(ConnectionInfo.ProviderName);
				return _factory;
			}
		}

		private DbConnection _connection;
		internal DbConnection Connection
		{
			get
			{
				if( _connection == null ) {
					_connection = this.Factory.CreateConnection();
					_connection.ConnectionString = ConnectionInfo.ConnectionString;
				}
				return _connection;
			}
		}
		internal DbTransaction Transaction { get; private set; }

		#endregion

		#region 外挂方法的工厂对象引用

		private CPQueryFactory _factoryCPQuery;
		private XmlCommandFactory _factoryXmlCommand;
		private StoreProcedureFactory _factoryStoreProcedure;
		private EntityMethodFactory _entiryMethod;

		/// <summary>
		/// CPQuery工厂实例引用
		/// </summary>
		public CPQueryFactory CPQuery
		{
			get
			{
				if( _factoryCPQuery == null )
					_factoryCPQuery = new CPQueryFactory() { Context = this };
				return _factoryCPQuery;
			}
		}

		/// <summary>
		/// XmlCommand工厂实例引用
		/// </summary>
		public XmlCommandFactory XmlCommand
		{
			get
			{
				if( _factoryXmlCommand == null )
					_factoryXmlCommand = new XmlCommandFactory() { Context = this };
				return _factoryXmlCommand;
			}
		}

		/// <summary>
		/// StoreProcedure工厂实例引用
		/// </summary>
		public StoreProcedureFactory StoreProcedure
		{
			get
			{
				if( _factoryStoreProcedure == null )
					_factoryStoreProcedure = new StoreProcedureFactory() { Context = this };
				return _factoryStoreProcedure;
			}
		}


		/// <summary>
		/// EntityMethodFactory实例引用
		/// </summary>
		public EntityMethodFactory Entity {
			get {
				if( _entiryMethod == null )
					_entiryMethod = new EntityMethodFactory() { Context = this };
				return _entiryMethod;
			}
		}

		#endregion

		#region 构造函数

		internal DbContext(ConnectionInfo connectionInfo)
		{
			ConnectionInfo = connectionInfo;
		}

		/// <summary>
		/// 使用默认的连接字符串创建DbContext实例
		/// </summary>
		/// <returns></returns>
		public static DbContext Create()
		{
			ConnectionInfo connectionInfo = ConnectionManager.GetConnection(null);
			return new DbContext(connectionInfo);
		}

		/// <summary>
		/// 根据指定的数据库连接字符串创建DbContext实例
		/// </summary>
		/// <param name="connectionName"></param>
		/// <returns></returns>
		public static DbContext Create(string connectionName)
		{
			ConnectionInfo connectionInfo = ConnectionManager.GetConnection(connectionName);
			return new DbContext(connectionInfo);
		}

		/// <summary>
		/// 根据指定的数据库连接字符串和数据库类型创建DbContext实例
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="providerName"></param>
		/// <returns></returns>
		public static DbContext Create(string connectionString, string providerName)
		{
			ConnectionInfo connectionInfo = new ConnectionInfo(connectionString, providerName);
			return new DbContext(connectionInfo);
		}

		#endregion


		#region 基础操作
				

		/// <summary>
		/// 同步方式打开数据库连接
		/// </summary>
		internal void OpenConnection()
		{
			if( this.Connection.State == ConnectionState.Open )
				return;

			this.Connection.Open();
			EventManager.FireConnectionOpened(this.Connection);

			InitConnection();
		}

		/// <summary>
		/// 异步方式打开数据库连接
		/// </summary>
		/// <returns></returns>
		internal async Task OpenConnectionAsync()
		{
			if( this.Connection.State == ConnectionState.Open )
				return;

			await this.Connection.OpenAsync();
			EventManager.FireConnectionOpened(this.Connection);

			InitConnection();
		}

		/// <summary>
		/// 用于连接创建后的初始化
		/// </summary>
		private void InitConnection()
		{
			if( this._connection == null )      // 确信连接已创建
				throw new InvalidProgramException();

			// 开启事务
			if( _useTransaction ) {
				_useTransaction = false;    // 清除变量

				if( _isolationLevel.HasValue )
					this.Transaction = this._connection.BeginTransaction(_isolationLevel.Value);
				else
					this.Transaction = this._connection.BeginTransaction();
			}

			// 切换数据库
			if( string.IsNullOrEmpty(_changeDatabase) == false ) {
				this._connection.ChangeDatabase(_changeDatabase);
				_changeDatabase = null; // 清除变量
			}
		}

		/// <summary>
		/// 为打开的连接更改当前数据库。
		/// </summary>
		/// <param name="databaseName"></param>
		public void ChangeDatabase(string databaseName)
		{
			// 如果连接还没有创建，就用变量来保存要切换的数据库，等待连接创建后再切换
			if( this._connection == null )
				_changeDatabase = databaseName;
			else
				// 连接存在就直接切换
				this._connection.ChangeDatabase(databaseName);
		}

		/// <summary>
		/// 开启数据库事务
		/// </summary>
		public void BeginTransaction()
		{
			if( this._connection == null ) {
				_useTransaction = true;
				_isolationLevel = null;
			}
			else {
				if( this.Transaction != null )
					throw new InvalidOperationException("当前上下文中已存在打开的事务，请不要重复开启事务。");

				this.Transaction = this._connection.BeginTransaction();
			}
		}

		/// <summary>
		/// 开启数据库事务，并指定事务的隔离级别
		/// </summary>
		/// <param name="isolationLevel"></param>
		public void BeginTransaction(IsolationLevel isolationLevel)
		{
			if( this._connection == null ) {
				_useTransaction = true;
				_isolationLevel = isolationLevel;
			}
			else {
				if( this.Transaction != null )
					throw new InvalidOperationException("当前上下文中已存在打开的事务，请不要重复开启事务。");

				this.Transaction = this._connection.BeginTransaction(_isolationLevel.Value);
			}
		}



		/// <summary>
		/// 提交数据库事务
		/// </summary>
		public void Commit()
		{
			if( this.Transaction != null ) {
				this.Transaction.Commit();
				this.Transaction = null;

				EventManager.FireOnCommit(this.Connection);
			}
			else
				throw new InvalidOperationException("没有开启事务不能执行Commit操作。");
		}


		///// <summary>
		///// 回滚事务
		///// </summary>
		//public void Rollback()
		//{
		//	if( this.Transaction != null ) {
		//		this.Transaction.Rollback();
		//		this.Transaction = null;

		//		EventManager.FireOnRollback(this.Connection);
		//	}
		//	else
		//		throw new InvalidOperationException("没有开启事务不能执行Rollback操作。");
		//}

		/// <summary>
		/// 创建一个常用的命令参数
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="value"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public DbParameter CreateParameter(DbType dbType, object value, int? size = null)
		{
			DbParameter param = this.Factory.CreateParameter();
			param.DbType = dbType;

			if( size.HasValue )
				param.Size = size.Value;

			if( value != null )
				param.Value = value;
			else
				param.Value = DBNull.Value;
			return param;
		}

		/// <summary>
		/// 创建一个用于输出的命令参数
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="size"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public DbParameter CreateOutParameter(DbType dbType, int? size = null, object value = null)
		{
			DbParameter param = CreateParameter(dbType, value, size);
			param.Direction = ParameterDirection.Output;
			return param;
		}

		#endregion


		/// <summary>
		/// 实现IDisposable接口
		/// </summary>
		public void Dispose()
		{
			if( this._connection != null ) {
				this._connection.Dispose();
				this._connection = null;
			}
		}

	}
}
