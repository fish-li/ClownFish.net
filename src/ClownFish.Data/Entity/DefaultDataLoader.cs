using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;
using ClownFish.Data.CodeDom;

namespace ClownFish.Data
{
	/// <summary>
	/// 默认的实体加载IDataLoader实现
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal sealed class DefaultDataLoader<T> : IDataLoader<T> where T : class, new()
	{

		#region 实现 IDataLoader 接口

		public T ToSingle(DataRow row)
		{
			EntityDescription description = EntityDescriptionCache.Get(typeof(T));
			DataTable table = row.Table;

			return GetSingle(row, description, table);
		}

		public T ToSingle(DbDataReader reader)
		{
			if( reader.Read() ) {
				EntityDescription description = EntityDescriptionCache.Get(typeof(T));
				string[] names = DataExtensions.GetColumnNames(reader);

				return GetSingle(reader, description, names);
			}
			else
				return default(T);
		}

		public List<T> ToList(DataTable table)
		{
			EntityDescription description = EntityDescriptionCache.Get(typeof(T));

			List<T> list = new List<T>(Initializer.Instance.DefaultEntityListLen);

			foreach( DataRow row in table.Rows ) {
				T instance = GetSingle(row, description, table);
				list.Add(instance);
			}
			return list;
		}

		public List<T> ToList(DbDataReader reader)
		{
			EntityDescription description = EntityDescriptionCache.Get(typeof(T));

			return ToList(reader, description);
		}

		#endregion


		#region 内部辅助方法

		private T GetSingle(DbDataReader reader, EntityDescription description, string[] names)
		{
			T instance = (T)typeof(T).FastNew();

			for( int i = 0; i < names.Length; i++ ) {
				string name = names[i];

				ColumnInfo info;
				if( description.MemberDict.TryGetValue(name, out info) ) {
					object val = reader.GetValue(i);
					SetPropertyValue(info, instance, val, name);
				}
			}
			return instance;
		}

		private void SetPropertyValue(ColumnInfo info, object instance, object val, string name)
		{
			try {
				if( val != null && DBNull.Value.Equals(val) == false ) {
					info.PropertyInfo.FastSetValue(instance, val.Convert(info.DataType));
				}
			}
			catch( Exception ex ) {
				throw new DataException("数据转换失败，当前列名：" + name, ex);
			}
		}


		private List<T> ToList(DbDataReader reader, EntityDescription description) 
		{
			List<T> list = new List<T>(Initializer.Instance.DefaultEntityListLen);
			string[] names = DataExtensions.GetColumnNames(reader);

			while( reader.Read() ) {
				T instance = GetSingle(reader, description, names);
				list.Add(instance);
			}
			return list;
		}

        private T GetSingle(DataRow row, EntityDescription description, DataTable table)
        {
            T instance = (T)typeof(T).FastNew();

            for (int i = 0; i < table.Columns.Count; i++) {
                string name = table.Columns[i].ColumnName;

                ColumnInfo info;
                if (description.MemberDict.TryGetValue(name, out info)) {
                    object val = row[i];
                    SetPropertyValue(info, instance, val, name);
                }
            }
            return instance;
        }

		#endregion

	}
}
