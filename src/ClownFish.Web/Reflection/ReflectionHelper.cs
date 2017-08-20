using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web.Reflection
{
	internal static class ReflectionHelper
	{
		internal static readonly Type VoidType = typeof(void);


		/// <summary>
		/// 判断是否是一个可支持的参数类型。仅包括：基元类型，string ，decimal，DateTime，Guid, string[], 枚举
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsSupportableType(this Type type)
		{
			return type.IsPrimitive
				|| type == typeof(string)
				|| type == typeof(DateTime)
				|| type == typeof(decimal)
				|| type == typeof(Guid)
				|| type.IsEnum
				|| type == typeof(string[])
				|| type == typeof(byte[])
				;
		}



		public static string GetCodeString(this Type t)
		{
			if( t == null )
				throw new ArgumentNullException("t");

			if( t.IsGenericType ) {
				StringBuilder sb = new StringBuilder(128);

				string genericTypeDefinition = t.GetGenericTypeDefinition().FullName;
				int p = genericTypeDefinition.IndexOf("`");
				if( p > 0 )
					sb.Append(genericTypeDefinition.Substring(0, p));
				else
					throw new InvalidOperationException();

				sb.Append("<");

				int index = 0;
				foreach( Type gt in t.GetGenericArguments() ) {
					if( index++ > 0 )
						sb.Append(",");
					sb.Append(GetCodeString(gt));
				}

				sb.Append(">");

				return sb.ToString();
			}
			else {
				if( t.HasElementType )
					return t.GetElementType().FullName;
				else
					return t.FullName.Replace('+', '.');
			}
		}
		


	}
}
