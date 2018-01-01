using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// CPQuery工厂类，供框架内部使用
	/// </summary>
	public sealed class CPQueryFactory
	{
		internal CPQueryFactory() { }

		internal DbContext Context { get; set; }

		/// <summary>
		/// 根据指定的参数化SQL创建CPQuery实例
		/// </summary>
		/// <param name="parameterizedSQL"></param>
		/// <returns></returns>
		public CPQuery Create(string parameterizedSQL)
		{
			CPQuery query = new CPQuery(this.Context);
			query.Init(parameterizedSQL);
			return query;
		}

		/// <summary>
		/// 根据指定的参数化SQL和参数对象创建CPQuery实例
		/// </summary>
		/// <param name="parameterizedSQL"></param>
		/// <param name="argsObject"></param>
		/// <returns></returns>
		public CPQuery Create(string parameterizedSQL, object argsObject)
		{
			CPQuery query = new CPQuery(this.Context);
			query.Init(parameterizedSQL, argsObject);
			return query;
		}

		/// <summary>
		/// 根据指定的参数化SQL和参数对象创建CPQuery实例
		/// </summary>
		/// <param name="parameterizedSQL"></param>
		/// <param name="dictionary"></param>
		/// <returns></returns>
		public CPQuery Create(string parameterizedSQL, IDictionary dictionary)
		{
			CPQuery query = new CPQuery(this.Context);
			query.Init(parameterizedSQL, dictionary);
			return query;
		}

		/// <summary>
		/// 根据指定的参数化SQL和参数对象创建CPQuery实例
		/// </summary>
		/// <param name="parameterizedSQL"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public CPQuery Create(string parameterizedSQL, params DbParameter[] parameters)
		{
			CPQuery query = new CPQuery(this.Context);
			query.Init(parameterizedSQL, parameters);
			return query;
		}

	}
}
