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
				targetType = Enum.GetUnderlyingType(targetType);
			}

			return System.Convert.ChangeType(value, targetType);
		}

	}
}
