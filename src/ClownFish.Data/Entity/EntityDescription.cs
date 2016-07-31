using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;

namespace ClownFish.Data
{
	internal sealed class EntityDescription
	{
		public Dictionary<string, ColumnInfo> MemberDict { get; set; }

		/// <summary>
		/// 实体的属性数量，可能 大于 MemberDict.Count，因为有些属性可能被忽略！
		/// 这个属性目前用于控制【变量状态数组】的长度
		/// </summary>
		public int PropertyCount { get; set; }

		public DbEntityAttribute Attr { get; set; }
	}


	internal static class EntityDescriptionCache
	{

		private static Hashtable s_typeInfoDict = Hashtable.Synchronized(new Hashtable(2048));

		public static EntityDescription Get(Type entityType)
		{
			// 先尝试从缓存中获取
			EntityDescription description = s_typeInfoDict[entityType.FullName] as EntityDescription;
			if( description == null ) {

				// 创建类型的描述对象
				description = Create(entityType, true);

				// 添加到缓存字典
				s_typeInfoDict[entityType.FullName] = description;
			}

			return description;
		}

		private static bool IsSupportType(Type dataType)
		{
			return dataType.IsPrimitive
                || dataType == TypeList._string
                || dataType == TypeList._Guid
                || dataType == TypeList._DateTime
                || dataType == TypeList._decimal
                || dataType == TypeList._byteArray
                || dataType.IsEnum;
        }

		public static EntityDescription Create(Type entityType, bool checkIgnore)
		{
			// 获取所有类型的属性定义（注意：不处理Field）
			PropertyInfo[] properties = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			Dictionary<string, ColumnInfo> dict
				= new Dictionary<string, ColumnInfo>(properties.Length, StringComparer.OrdinalIgnoreCase);

			int index = -1;
			foreach( PropertyInfo prop in properties ) {
				// 为了方便排查问题，属性的序号以出现的次序为准，
				// 如果有属性被忽略了，那么序号也累加（对于变更状态数组来说，就是浪费对应的元素）
				index++;

                if (prop.CanWrite == false)     // 不能写的属性根本不能赋值，只能排除！
                    continue;

                Type dataType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                // 排除不支持的数据类型（有可能是嵌套实体）
                if ( IsSupportType(dataType) == false )
					continue;

                if (prop.IsIndexerProperty()) // 排除索引器属性
                    continue;

                // 获取属性的数据库定义信息（可能为 null ）
                DbColumnAttribute attr = prop.GetMyAttribute<DbColumnAttribute>();

				if( checkIgnore ) {
					if( attr != null && attr.Ignore )
						continue;
				}

				ColumnInfo info = new ColumnInfo(prop, attr) { DataType = dataType, Index = index };

				dict[info.PropertyInfo.Name] = info;
			}

			return new EntityDescription {
				MemberDict = dict,
				PropertyCount = properties.Length

				// 反射过程中，不需要实体到表结构的映射信息，所以就不读取DbEntityAttribute
				// CodeDom过程中只使用一次，不需要缓存，所以在获取后自行读取DbEntityAttribute
				//Attr = type.GetMyAttribute<DbEntityAttribute>()
			};
		}

	}
}
