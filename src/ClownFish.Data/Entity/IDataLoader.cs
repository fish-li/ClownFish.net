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
	/// 定义每个实体的数据加载接口
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDataLoader<T>  where T : class, new()
	{
		/// <summary>
		/// 从DataRow中加载单个实体对象
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		T ToSingle(DataRow row);

		/// <summary>
		/// 从DbDataReader中加载单个实体对象
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		T ToSingle(DbDataReader reader);

		/// <summary>
		/// 从DataTable中加载实体列表
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		List<T> ToList(DataTable table);

		/// <summary>
		/// 从DbDataReader中加载实体列表
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		List<T> ToList(DbDataReader reader);
	}
}
