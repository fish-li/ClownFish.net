using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Log.Serializer
{
	/// <summary>
	/// 不做任何写入操作的Writer
	/// </summary>
	public class NullWriter : ILogWriter
	{
		#region ILogWriter 成员

		/// <summary>
		/// 
		/// </summary>
		public void Init()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="info"></param>
		public void Write<T>(T info) where T : Model.BaseInfo
		{
			// 不执行写入操作
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public void Write<T>(List<T> list) where T : Model.BaseInfo
		{
			// 不执行写入操作
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="guid"></param>
		/// <returns></returns>
		public T Get<T>(Guid guid) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public List<T> GetList<T>(DateTime t1, DateTime t2) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
