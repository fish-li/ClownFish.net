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
    /// 用于数据加载相关的扩展工具类
    /// </summary>
    public static class DataExtensions
    {
		/// <summary>
		/// 返回一个DbDataReader对象获取到的结果集的全部列名
		/// </summary>
		/// <param name="reader">DbDataReader实例</param>
		/// <returns>结果集的全部列名</returns>
		public static string[] GetColumnNames(this DbDataReader reader)
		{
			int count = reader.FieldCount;
			string[] array = new string[count];

			for( int i = 0; i < count; i++ )
				array[i] = reader.GetName(i);

			return array;
		}

		/// <summary>
		/// 返回一个DataTable包含的全部列名
		/// </summary>
		/// <param name="table">DataTable实例</param>
		/// <returns>数据表的全部列名</returns>
		public static string[] GetColumnNames(this DataTable table)
		{
			string[] array = new string[table.Columns.Count];

			for( int i = 0; i < table.Columns.Count; i++ )
				array[i] = table.Columns[i].ColumnName;

			return array;
		}


		/// <summary>
		/// 在一个字符串数组中查找指定的字符串所在的序号。
		/// </summary>
		/// <param name="array">一个字符串数组</param>
		/// <param name="value">要搜索的字符串</param>
		/// <returns>如果找到，则返回从零开始的索引；否则为 -1。</returns>
		public static int FindIndex(this string[] array, string value)
		{
			return Array.FindIndex<string>(array, x => string.Compare(x, value, StringComparison.OrdinalIgnoreCase) == 0);
		}


		/// <summary>
		/// 在一个字符串列表中查找指定的字符串所在的序号。
		/// </summary>
		/// <param name="list">一个字符串列表</param>
		/// <param name="value">要搜索的字符串</param>
		/// <returns>如果找到，则返回从零开始的索引；否则为 -1。</returns>
		public static int FindIndex(this List<string> list, string value)
		{
			return list.FindIndex(x => string.Compare(x, value, StringComparison.OrdinalIgnoreCase) == 0);
		}


	}
}
