using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.SqlClient
{
	/// <summary>
	/// 实体针对SQLSERVER的扩展方法
	/// </summary>
	public static class EntityExtensions
	{
		private static CPQuery GetInsertReturnNewIdQuery(Entity entity)
		{
			if( entity == null )
				throw new ArgumentNullException("entity");

			IEntityProxy proxy = entity as IEntityProxy;
			if( proxy == null )
				throw new InvalidOperationException("当前方法只能在实体代理对象上调用。");

			CPQuery query = entity.GetInsertQuery();

			// 在 insert 语句后面加上一个调用。
			query = query + "; select SCOPE_IDENTITY();";


			proxy.ClearChangeFlags();  // 清除修改标记，防止多次调用

			return query;
		}
		/// <summary>
		/// 执行INSERT操作，并返回新的自增列ID
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static int InsertReturnNewId(this Entity entity)
		{
			CPQuery query = GetInsertReturnNewIdQuery(entity);

			// 执行INSERT操作，并查询最新的ID
			return query.ExecuteScalar<int>();
		}


		/// <summary>
		/// 执行INSERT操作，并返回新的自增列ID
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static async Task<int> InsertReturnNewIdAsync(this Entity entity)
		{
			CPQuery query = GetInsertReturnNewIdQuery(entity);

			// 执行INSERT操作，并查询最新的ID
			return await query.ExecuteScalarAsync<int>();
		}
	}
}
