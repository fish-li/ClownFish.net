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
	/// 用于数据加载相关的扩展工具类，供框架内部使用！
	/// </summary>
	public static class DataLoaderHelper
	{
		/// <summary>
		/// 供实体加载器使用的内部方法，请不要在代码中调用！
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="len"></param>
		/// <param name="kvs"></param>
		/// <returns></returns>
		public static int[] CreateNameMapIndex(object dataSource, int len, params KeyValuePair<int, string>[] kvs)
		{
			string[] names = null;
			DbDataReader reader = dataSource as DbDataReader;
			if( reader != null )
				names = DataExtensions.GetColumnNames(reader);
			else
				names = DataExtensions.GetColumnNames((DataTable)dataSource);


			// 优化这段代码的性能 （性能提升 60%）
			//int[] colIndex = new int[19];
			//colIndex[0] = DataExtensions.FindIndex(names, "rid");
			//colIndex[1] = DataExtensions.FindIndex(names, "intA");
			//colIndex[2] = DataExtensions.FindIndex(names, "timeA");
			//colIndex[3] = DataExtensions.FindIndex(names, "moneyA");
			// ............................

			int[] colIndex = new int[len];

			foreach( var kv in kvs ) {
				int index = -1;

				for( int i = 0; i < names.Length; i++ ) {
					if( names[i] != null
						&& string.Compare(names[i], kv.Value, StringComparison.OrdinalIgnoreCase) == 0 ) {

						index = i;
						names[i] = null;    // 节省后续查找时间
						break;
					}
				}
				colIndex[kv.Key] = index;
			}
			return colIndex;
		}


	}
}
