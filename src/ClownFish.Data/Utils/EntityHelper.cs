using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;

namespace ClownFish.Data
{
	internal static class EntityHelper
	{
		/// <summary>
		/// 获取实体属性对应的数据库字段名
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public static string GetDbFieldName(this MemberInfo member)
		{
			DbColumnAttribute a = member.GetMyAttribute<DbColumnAttribute>();
			return (a != null && string.IsNullOrEmpty(a.Alias) == false)
					? a.Alias
					: member.Name;
		}

		/// <summary>
		/// 获取实体对应的数据库表名
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		public static string GetDbTableName(this Type entityType)
		{
			DbEntityAttribute a = entityType.GetMyAttribute<DbEntityAttribute>();
			return (a != null && string.IsNullOrEmpty(a.Alias) == false)
					? a.Alias
					: entityType.Name;
		}

	}
}
