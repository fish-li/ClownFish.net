using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	internal static class ConvertExtensions
	{
		internal static object Convert(this object value, Type targetType)
		{
			if( value == null )
				return null;

			if( targetType == TypeList._string)
				return value.ToString();

			if( value.GetType() == targetType) {
				return value;
			}

			if(targetType == TypeList._Guid && value is string ) {
				return new Guid(value.ToString());
			}

			if(targetType.IsEnum ) {
				// 返回指定枚举的基础类型，通常应该是 int
				// 也就说，枚举类型都是按 int 来返回结果
				targetType = Enum.GetUnderlyingType(targetType);
			}

			return System.Convert.ChangeType(value, targetType);
		}

	}
}
