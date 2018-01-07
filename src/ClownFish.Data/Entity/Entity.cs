using ClownFish.Base.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.Linq;
using System.Web;
using System.Reflection;

namespace ClownFish.Data
{
	/// <summary>
	/// 数据实体的基类
	/// </summary>
	[Serializable]
	public abstract class Entity
	{
		#region 获取代理方法

		[NonSerialized]
		private DbContext _context;     // 仅在代理对象时才被赋值

		/// <summary>
		/// 获取与当前实体关联的DbContext实例
		/// </summary>
		protected DbContext DbContext
		{
			get {
				if( (this is IEntityProxy) == false )
					throw new InvalidOperationException("DbContext属性仅供代理类使用，实体对象不允许访问这个属性");

				if( _context == null )
					_context = ConnectionScope.GetDefaultDbConext();

				return _context;
			}
		}

		/// <summary>
		/// 绑定当前实体代理的DbContext关联，一般用于绑定从[EntityProxy]创建的实体代理。
		/// </summary>
		/// <param name="context"></param>
		public void BindDbContext(DbContext context)
		{
			if( context == null )
				throw new ArgumentNullException(nameof(context));

			IEntityProxy proxy = this as IEntityProxy;
			if( proxy == null )
				throw new InvalidOperationException("BindDbContext方法仅允许在实体代理对象上调用。");

			if( _context != null )
				throw new InvalidOperationException("当前实体代理对象已存在DbContext引用，不能重复绑定。");

			_context = context;
		}


		private IEntityProxy CreateProxy()
		{
			Type entityType = this.GetType();
			Type proxyType = EntityProxyFactory.GetProxy(entityType);

			if( proxyType == null )
				throw new NotImplementedException(
					string.Format("实体类型 {0} 并没有注册匹配的代理类型。", entityType.FullName)
					);
			else
				return proxyType.FastNew() as IEntityProxy;
		}

		/// <summary>
		/// 根据当前实体创建代理对象，然后可执行数据库更新操作，
		/// 代理对象将监视属性的赋值过程，当给代理对象的属性赋值后，对应的字段会标记为更新状态。
		/// </summary>
		/// <param name="context"></param>
		/// <returns>与实体相关的代理对象</returns>
		internal Entity GetProxy(DbContext context)
		{
			// context ，允许参数为 null

			IEntityProxy proxy = CreateProxy();
			(proxy as Entity)._context = context;		// 仅在代理对象时才被赋值
			proxy.Init(this);

			return proxy as Entity;
		}

		#endregion

		#region 扩展方法入口

		/// <summary>
		/// 创建与实体相关的EntityTable实例，开始数据库操作
		/// </summary>
		/// <typeparam name="T">实体的类型参数</typeparam>
		/// <returns>与实体相关的EntityTable实例</returns>
		public static EntityTable<T> From<T>() where T : Entity, new()
        {
            return new EntityTable<T>();
        }

		/// <summary>
		/// 开始LINQ查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="withNoLock">是否在表名后面添加 with(nolock) 提示</param>
		/// <returns></returns>
		public static EntityQuery<T> Query<T>(bool withNoLock = false) where T : Entity, new()
		{
			EntityLinqProvider provider = new EntityLinqProvider() { WithNoLock = withNoLock };
			return new EntityQuery<T>(provider);
		}



		/// <summary>
		/// 创建与实体相关的代理对象，并指示实体进入编辑状态，
		/// 请基于此方法的返回值来修改实体的属性，而不要直接修改原实体对象。
		/// 例如：var product = Entity.BeginEdit(product);
		/// 注意：Insert/Delete/Update操作必须基本此方法的返回值对象才能调用。
		/// 如果不指定entity参数，就创建一个新的实体对象（可用于新增）来封装代理对象。
		/// </summary>
		/// <typeparam name="T">实体的类型参数</typeparam>
		/// <param name="entity">需要封装成代理的实体对象</param>
		/// <returns>与实体相关的代理对象</returns>
		public static T BeginEdit<T>(T entity = null) where T : Entity, new()
		{
			if( entity == null )
				//throw new ArgumentNullException(nameof(entity));
				entity = new T();

			if (entity is IEntityProxy)
				throw new ArgumentException("BeginEdit方法只接收实体对象，不允许操作代理对象。");

            return (T)entity.GetProxy(null);
        }

		#endregion

		#region INSERT/DELETE/UPDATE
		
		internal string GetTableName()
		{
			// 这个方法必须运行在代理类型中
			Type entityType = this.GetType().BaseType;
			return entityType.GetDbTableName();
		}
		
		internal CPQuery GetInsertQuery()
		{
			IEntityProxy proxy = this as IEntityProxy;

			string[] names = proxy.GetChangeNames();
			object[] values = proxy.GetChangeValues();
			if( names.Length == 0 )
				return null;        // 没有设置任何属性，应该不会发生吧？


			CPQuery query = this.DbContext.CreateCPQuery();
			query = query
					+ "INSERT INTO " + GetTableName() + "("
					+ string.Join(",", names.ToArray())
					+ ") VALUES (";


			for( int i = 0; i < values.Length; i++ ) {
				object value = values[i];
				if( value == null )
					query = query + "NULL";
				else
					query = query + new QueryParameter(value);

				if( i < values.Length - 1 )
					query.AppendSql(",");
			}

			query.AppendSql(")");

			return query;
		}

		internal CPQuery GetWhereQuery()
		{
			IEntityProxy proxy = this as IEntityProxy;

			string[] names = proxy.GetChangeNames();
			object[] values = proxy.GetChangeValues();
			if( names.Length == 0 )
				return null;        // 没有设置任何属性，应该不会发生吧？

			CPQuery query = this.DbContext.CreateCPQuery() + " WHERE ";

			for( int i = 0; i < values.Length; i++ ) {
				string name = names[i];
				object value = values[i];

				if( i > 0 )
					query = query + " AND ";

				if( value == null )
					query = query + " " + name +  "=NULL";
				else
					query = query + " " + name + "=" + new QueryParameter(value);
			}

			return query;
		}

		internal CPQuery GetUpdateQuery(Tuple<string, object> rowKey)
		{
			IEntityProxy proxy = this as IEntityProxy;

			string[] names = proxy.GetChangeNames();
			object[] values = proxy.GetChangeValues();
			if( names.Length == 0 )
				return null;        // 没有设置任何属性，应该不会发生吧？

			int keyIndex = -1;		// 标记主键字段在数组的哪个位置
			if( rowKey != null ) 
				keyIndex = Array.IndexOf(names, rowKey.Item1);
			
			if( names.Length == 1 && keyIndex == 0)
				return null;        // 如果仅仅只设置了主键字段，这样的更新是无意义的

			int forcount = values.Length;

			if( keyIndex == forcount - 1 )		// 主键出现在最后面
				forcount--;

			CPQuery query = this.DbContext.CreateCPQuery()
							+ "UPDATE " + GetTableName() + " SET ";

			for( int i = 0; i < forcount; i++ ) {
				if( i == keyIndex )		// 忽略主键字段
					continue;

				string name = names[i];
				object value = values[i];

				if( value == null )
					query = query + " " + name + "=NULL";
				else
					query = query + " " + name + "=" + new QueryParameter(value);

				if( i < forcount - 1 )
					query.AppendSql(",");		// 注意，这个逗号的拼接，有可能主键出现在所有字段的最后。
			}

			return query;

		}

		internal CPQuery GetSelectQuery()
		{
			IEntityProxy proxy = this as IEntityProxy;

			string[] names = proxy.GetChangeNames();
			if( names.Length == 0 )
				return null;


			CPQuery query = this.DbContext.CreateCPQuery()
							+ " SELECT " + string.Join(",", names.ToArray());

			return query;
		}

		private CPQuery GetInsertQueryCommand()
		{
			IEntityProxy proxy = this as IEntityProxy;
			if( proxy == null )
				throw new InvalidOperationException("请在调用BeginEdit()的返回值对象上调用Insert方法。");


			CPQuery insert = GetInsertQuery();
			if( insert == null )
				return null;

			proxy.ClearChangeFlags();  // 清除修改标记，防止多次调用

			return insert;
		}

		/// <summary>
		/// 根据已修改的实体属性，生成INSERT语句，并执行数据库插入操作，
		/// 注意：此方法只能在实体的代理对象上调用。
		/// </summary>
		/// <returns>数据库操作过程中影响的行数</returns>
		public int Insert()
        {
			CPQuery query = GetInsertQueryCommand();
			if( query == null )
				return -1;

			return query.ExecuteNonQuery();
		}


		/// <summary>
		/// 根据已修改的实体属性，生成INSERT语句，并执行数据库插入操作，
		/// 注意：此方法只能在实体的代理对象上调用。
		/// </summary>
		/// <returns>数据库操作过程中影响的行数</returns>
		public async Task<int> InsertAsync()
		{
			CPQuery query = GetInsertQueryCommand();
			if( query == null )
				return -1;

			return await query.ExecuteNonQueryAsync();
		}



		private CPQuery GetDeleteQueryCommand()
		{
			IEntityProxy proxy = this as IEntityProxy;
			if( proxy == null )
				throw new InvalidOperationException("请在调用BeginEdit()的返回值对象上调用Delete方法。");


			CPQuery where = GetWhereQuery();
			if( where == null )
				return null;      // 不允许没有WHERE条件的删除，如果确实需要，请手工写SQL

			CPQuery query = this.DbContext.CreateCPQuery()
							+ "DELETE FROM " + GetTableName() + where;

			proxy.ClearChangeFlags();  // 清除修改标记，防止多次调用

			return query;
		}

		/// <summary>
		/// 根据已修改的实体属性，生成DELETE查询条件，并执行数据库插入操作，
		/// 注意：此方法只能在实体的代理对象上调用。
		/// </summary>
		/// <returns>数据库操作过程中影响的行数</returns>
		public int Delete()
        {
			CPQuery query = GetDeleteQueryCommand();
			if( query == null )
				return -1;

			return query.ExecuteNonQuery();
		}

		/// <summary>
		/// 根据已修改的实体属性，生成DELETE查询条件，并执行数据库插入操作，
		/// 注意：此方法只能在实体的代理对象上调用。
		/// </summary>
		/// <returns>数据库操作过程中影响的行数</returns>
		public async Task<int> DeleteAsync()
		{
			CPQuery query = GetDeleteQueryCommand();
			if( query == null )
				return -1;

			return await query.ExecuteNonQueryAsync();
		}


		private CPQuery GetUpdateQueryCommand()
		{
			IEntityProxy proxy = this as IEntityProxy;
			if( proxy == null )
				throw new InvalidOperationException("请在调用BeginEdit()的返回值对象上调用Update方法。");

			// 获取数据实体对象的主键值，如果数据实体没有指定主键，将会抛出一个异常
			Tuple<string, object> rowKey = proxy.GetRowKey();

			CPQuery update = GetUpdateQuery(rowKey);
			if( update == null )
				return null;

			CPQuery query = update
							+ " WHERE " + rowKey.Item1 + " = " + new QueryParameter(rowKey.Item2);

			proxy.ClearChangeFlags();  // 清除修改标记，防止多次调用

			return query;
		}




		/// <summary>
		/// 根据已修改的实体属性，生成UPDATE操作语句（WHERE条件由主键生成[DbColumn(PrimaryKey=true)]），并执行数据库插入操作，
		/// 注意：此方法只能在实体的代理对象上调用。
		/// </summary>
		/// <returns>数据库操作过程中影响的行数</returns>
		public int Update()
        {
			CPQuery query = GetUpdateQueryCommand();
			if( query == null )
				return -1;

			return query.ExecuteNonQuery();
		}


		/// <summary>
		/// 根据已修改的实体属性，生成UPDATE操作语句（WHERE条件由主键生成[DbColumn(PrimaryKey=true)]），并执行数据库插入操作，
		/// 注意：此方法只能在实体的代理对象上调用。
		/// </summary>
		/// <returns>数据库操作过程中影响的行数</returns>
		public async Task<int> UpdateAsync()
		{
			CPQuery query = GetUpdateQueryCommand();
			if( query == null )
				return -1;

			return await query.ExecuteNonQueryAsync();
		}

		#endregion


	}



}
