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
	/// StoreProcedure工厂，供框架内部使用
	/// </summary>
	public sealed class StoreProcedureFactory
	{
		internal StoreProcedureFactory() { }

		internal DbContext Context { get; set; }

		/// <summary>
		/// 根据存储过程名称创建StoreProcedure实例
		/// </summary>
		/// <param name="spName"></param>
		/// <returns></returns>
		public StoreProcedure Create(string spName)
		{
			StoreProcedure sp = new StoreProcedure(this.Context);
			sp.Init(spName, null);
			return sp;
		}

		/// <summary>
		/// 根据存储过程名称和参数对象创建StoreProcedure实例
		/// </summary>
		/// <param name="spName"></param>
		/// <param name="parameterObject"></param>
		/// <returns></returns>
		public StoreProcedure Create(string spName, object parameterObject)
		{
			StoreProcedure sp = new StoreProcedure(this.Context);
			sp.Init(spName, parameterObject);
			return sp;
		}

		/// <summary>
		/// 根据存储过程名称和参数对象创建StoreProcedure实例
		/// </summary>
		/// <param name="spName"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public StoreProcedure Create(string spName, params DbParameter[] parameters)
		{
			StoreProcedure sp = new StoreProcedure(this.Context);
			sp.Init(spName, parameters);
			return sp;
		}
	}
}
