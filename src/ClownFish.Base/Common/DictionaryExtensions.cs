using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
	/// <summary>
	/// 包含一些字典相关的扩展方法
	/// </summary>
	public static class DictionaryExtensions
	{
		/// <summary>
		/// 往字典中插入数据项，如果有异常，则报告KEY是什么
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dict"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			try {
				dict.Add(key, value);

				// .NET 异常提示：已添加了具有相同键的项。
				// 这种提示很不利于排查问题，所以才封装了这个方法
			}
			catch( ArgumentException ex ) {
				throw new ArgumentException(string.Format("往集合中插入元素时发生了异常，当前Key={0}", key), ex);
			}
		}

		

		/// <summary>
		/// 往Hashtable中插入数据项，如果有异常，则报告KEY是什么
		/// </summary>
		/// <param name="table"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddValue(this Hashtable table, object key, object value)
		{
			try {
				table.Add(key, value);

				// 往HashTable中插入相同的KEY时，
				// .NET的异常提示：已添加项。字典中的关键字:“abc”所添加的关键字:“abc”

				// 虽然已在异常消息中指出了KEY值，
				// 但是为了和Dictionary有着一到的API，所以仍然封装了这个方法。
			}
			catch( ArgumentException ex ) {
				throw new ArgumentException(string.Format("往集合中插入元素时发生了异常，当前Key={0}", key), ex);
			}
		}

	}
}
