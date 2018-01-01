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
	/// 执行数据库操作的基类
	/// </summary>
	public abstract class BaseCommand
	{
		/// <summary>
		/// DbContext实例
		/// </summary>
		protected DbContext _context;
		internal DbContext Context { get { return _context; } }

		/// <summary>
		/// 用于SQL语句的参数占位符，命令名称的计算
		/// </summary>
		internal ParaNameBuilder ParaNameBuilder { get; private set; }

		/// <summary>
		/// DbCommand实例
		/// </summary>
		protected DbCommand _command;
		/// <summary>
		/// 执行存储过程使用的DbCommand对象
		/// </summary>
		public virtual DbCommand Command { get { return _command; } }

		internal BaseCommand(DbContext context)
		{
			if( context == null )
				throw new ArgumentNullException("context");

			_context = context;
			_command = context.Connection.CreateCommand();
			ParaNameBuilder = ParaNameBuilderFactory.Factory.Instance.GetBuilder(_command);
		}


		internal DbParameter[] CloneParameters()
		{
			DbParameter[] parameters2 = new DbParameter[_command.Parameters.Count];

			int i = 0;
			foreach( DbParameter src in _command.Parameters ) {
                DbParameter newParam = null;
                ICloneable x = src as ICloneable;
                if( x != null ) {
                    newParam = (DbParameter)x.Clone();
                }
                else {
                    newParam = _command.CreateParameter();
                    newParam.ParameterName = src.ParameterName;
                    newParam.DbType = src.DbType;
                    newParam.Size = src.Size;
                    newParam.Value = src.Value;
                    newParam.Direction = src.Direction;
                }
				parameters2[i++] = newParam;
			}
			return parameters2;
		}


		#region Execute 方法


		/// <summary>
		/// 开始执行数据库操作前要处理的额外操作
		/// </summary>
		protected virtual void BeforeExecute()
		{
		}

		private T Execute<T>(Func<DbCommand, T> func)
		{
			// 打开数据库连接
			_context.OpenConnection();


			// 设置命令的连接以及事务对象
			DbCommand command = this.Command;
			command.Connection = _context.Connection;

			if( _context.Transaction != null )
				command.Transaction = _context.Transaction;

			// 触发执行 前 事件
			_context.EventManager.FireBeforeExecute(this);

			this.BeforeExecute();

			try {
				// 执行数据库操作
				T result = func(command);

				// 触发执行 后 事件
				_context.EventManager.FireAfterExecute(this);

				return result;
			}
			catch( System.Exception ex ) {
				// 触发 异常 事件
				_context.EventManager.FireOnException(this, ex);

				// 重新抛出一个特定的异常，方便异常日志中记录command信息。
				throw new DbExceuteException(ex, command);
			}
			finally {
				// 让命令与连接，事务断开，避免这些资源外泄。
				command.Connection = null;
				command.Transaction = null;

				if( _context.AutoDisposable )
					_context.Dispose();
			}
		}


		private async Task<T> ExecuteAsync<T>(Func<DbCommand, Task<T>> func)
		{
			// 打开数据库连接
			await _context.OpenConnectionAsync();


			// 设置命令的连接以及事务对象
			DbCommand command = this.Command;
			command.Connection = _context.Connection;

			if( _context.Transaction != null )
				command.Transaction = _context.Transaction;

			// 触发执行 前 事件
			_context.EventManager.FireBeforeExecute(this);

			this.BeforeExecute();

			try {
				// 执行数据库操作
				T result = await func(command);

				// 触发执行 后 事件
				_context.EventManager.FireAfterExecute(this);

				return result;
			}
			catch( System.Exception ex ) {
				// 触发 异常 事件
				_context.EventManager.FireOnException(this, ex);

				// 重新抛出一个特定的异常，方便异常日志中记录command信息。
				throw new DbExceuteException(ex, command);
			}
			finally {
				// 让命令与连接，事务断开，避免这些资源外泄。
				command.Connection = null;
				command.Transaction = null;

				if( _context.AutoDisposable )
					_context.Dispose();
			}
		}



		/// <summary>
		/// 执行命令，并返回影响函数
		/// </summary>
		/// <returns>影响行数</returns>
		public int ExecuteNonQuery()
		{
			return Execute<int>(
					cmd => cmd.ExecuteNonQuery()
					);
		}


		/// <summary>
		/// 执行命令，并返回影响函数
		/// </summary>
		/// <returns>影响行数</returns>
		public async Task<int> ExecuteNonQueryAsync()
		{
			return await ExecuteAsync<int>(
					async cmd => await cmd.ExecuteNonQueryAsync()
					);
		}



		/// <summary>
		/// 执行查询，以DataTable形式返回结果
		/// </summary>
		/// <returns>数据集</returns>
		public DataTable ToDataTable()
		{
			return Execute<DataTable>(
				cmd => {
					DataTable table = new DataTable("_tb");
					DbDataAdapter da = _context.Factory.CreateDataAdapter();
					da.SelectCommand = cmd;
					da.Fill(table);

					return table;

				});
		}


		/// <summary>
		/// 执行查询，以DataTable形式返回结果
		/// </summary>
		/// <returns>数据集</returns>
		public async Task<DataTable> ToDataTableAsync()
		{
			return await ExecuteAsync<DataTable>(
				async cmd => {
					DataSet ds = new DataSet();
					ds.EnforceConstraints = false;  // 禁用约束检查

					DataTable table = new DataTable("_tb");
					ds.Tables.Add(table);

					using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {
						table.Load(reader);
					}

					return table;

				});
		}



		/// <summary>
		/// 执行查询，以DataSet形式返回结果
		/// </summary>
		/// <returns>数据集</returns>
		public DataSet ToDataSet()
		{
			return Execute<DataSet>(
				cmd => {
					DataSet ds = new DataSet();

					DbDataAdapter adapter = _context.Factory.CreateDataAdapter();
					adapter.SelectCommand = cmd;

					adapter.Fill(ds);
					for( int i = 0; i < ds.Tables.Count; i++ ) {
						ds.Tables[i].TableName = "_tb" + i.ToString();
					}
					return ds;
				}
				);
		}


		/// <summary>
		/// 执行查询，以DataSet形式返回结果
		/// </summary>
		/// <returns>数据集</returns>
		public async Task<DataSet> ToDataSetAsync()
		{
			return await ExecuteAsync<DataSet>(
				async cmd => {
					DataSet ds = new DataSet();
					ds.EnforceConstraints = false;  // 禁用约束检查

					using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {

						int index = 0;
						while( true ) {
							DataTable table = new DataTable("_tb" + (index++).ToString());
							ds.Tables.Add(table);
							table.Load(reader);

							// 上面代码中隐含着一个调用: reader.NextResult()，遗憾的是，它是个同步调用！
							// 所以，就不需要像下面这样再调用了，否则还会出现异常

							//if( await reader.NextResultAsync() == false )
							//	break;

							if( reader.IsClosed )
								break;
						}
					}

					return ds;
				}
				);
		}


		/// <summary>
		/// 执行命令，返回DbDataReader对象实例
		/// </summary>
		/// <returns>DbDataReader实例</returns>
		public DbDataReader ExecuteReader()
		{
			return Execute<DbDataReader>(
				cmd => cmd.ExecuteReader()
				);
		}


		/// <summary>
		/// 执行命令，返回DbDataReader对象实例
		/// </summary>
		/// <returns>DbDataReader实例</returns>
		public async Task<DbDataReader> ExecuteReaderAsync()
		{
			return await ExecuteAsync<DbDataReader>(
				async cmd => await cmd.ExecuteReaderAsync()
				);
		}


		private T ToScalar<T>(object obj)
		{
			if( obj == null || DBNull.Value.Equals(obj) )
				return default(T);

			if( obj is T )
				return (T)obj;

			Type targetType = typeof(T);


			// 有时候获取结果时，虽然字段的数据类型不是 string，但是就是希望以 string 形式返回
			// 例如以下使用场景，
			// sql = "select RowGuid from table1 where aa=2"
			// List<string> list = CPQuery.Create(sql).ToScalarList<string>();

			if( targetType == TypeList._string )
				return (T)(object)obj.ToString();

			//单测走不到
			//if( targetType == typeof(object) ) 
			//	return (T)obj;

			return (T)Convert.ChangeType(obj, targetType);
		}



		/// <summary>
		/// 执行命令，返回第一行第一列的值
		/// </summary>
		/// <typeparam name="T">返回值类型</typeparam>
		/// <returns>结果集的第一行,第一列</returns>
		public T ExecuteScalar<T>()
		{
			return Execute<T>(
				cmd => ToScalar<T>(cmd.ExecuteScalar())
				);
		}


		/// <summary>
		/// 执行命令，返回第一行第一列的值
		/// </summary>
		/// <typeparam name="T">返回值类型</typeparam>
		/// <returns>结果集的第一行,第一列</returns>
		public async Task<T> ExecuteScalarAsync<T>()
		{
			return await ExecuteAsync<T>(
				async cmd => ToScalar<T>(await cmd.ExecuteScalarAsync())
				);
		}


		/// <summary>
		/// 执行命令，并返回第一列的值列表
		/// </summary>
		/// <typeparam name="T">返回值类型</typeparam>
		/// <returns>结果集的第一列集合</returns>
		public List<T> ToScalarList<T>()
		{
			return Execute<List<T>>(
				cmd => {
					List<T> list = new List<T>();
					using( DbDataReader reader = cmd.ExecuteReader() ) {
						while( reader.Read() ) {
							list.Add(ToScalar<T>(reader[0]));
						}
						return list;
					}
				}
				);
		}


		/// <summary>
		/// 执行命令，并返回第一列的值列表
		/// </summary>
		/// <typeparam name="T">返回值类型</typeparam>
		/// <returns>结果集的第一列集合</returns>
		public async Task<List<T>> ToScalarListAsync<T>()
		{
			return await ExecuteAsync<List<T>>(
				async cmd => {
					List<T> list = new List<T>();
					using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {
						while( reader.Read() ) {
							list.Add(ToScalar<T>(reader[0]));
						}
						return list;
					}
				}
				);
		}



		/// <summary>
		/// 执行命令，将结果集转换为实体列表
		/// </summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <returns>实体集合</returns>
		public List<T> ToList<T>() where T : class, new()
		{
			return Execute<List<T>>(
				cmd => {
					using( DbDataReader reader = cmd.ExecuteReader() ) {
						IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
						return loader.ToList(reader);
					}
			});
		}


		/// <summary>
		/// 执行命令，将结果集转换为实体列表
		/// </summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <returns>实体集合</returns>
		public async Task<List<T>> ToListAsync<T>() where T : class, new()
		{
			return await ExecuteAsync<List<T>>(
				async cmd => {
					using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {
						IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
						return loader.ToList(reader);
					}
				});
		}


		/// <summary>
		/// 执行命令，将结果集的第一行转换为实体
		/// </summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <returns>实体</returns>
		public T ToSingle<T>() where T : class, new()
		{
			return Execute<T>(
				cmd => {
					using( DbDataReader reader = cmd.ExecuteReader() ) {
						IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
						return loader.ToSingle(reader);
					}
			});
		}

		/// <summary>
		/// 执行命令，将结果集的第一行转换为实体
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public async Task<T> ToSingleAsync<T>() where T : class, new()
		{
			return await ExecuteAsync<T>(
				async cmd => {
					using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {
						IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
						return loader.ToSingle(reader);
					}
				});
		}

		#endregion

	}
}
