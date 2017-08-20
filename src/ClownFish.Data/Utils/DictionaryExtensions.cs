using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	internal static class DictionaryExtensions
    {

		public static void UpdateValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			try {
				TValue tvalue;
				if( dict.TryGetValue(key, out tvalue) ) {
					dict[key] = value;
				}
				else {
					dict.Add(key, value);
				}
			}
			catch( ArgumentException ex ) {
				throw new ArgumentException(string.Format("更新集合中元素时发生了异常，当前Key={0}", key), ex);
			}
		}
	}
}
