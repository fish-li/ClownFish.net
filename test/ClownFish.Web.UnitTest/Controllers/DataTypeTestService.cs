using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web;

namespace ClownFish.Web.UnitTest.Controllers
{
	public class DataTypeTestService :  BaseController
	{
		public string Input_string_ToUpper(string input)
		{
			if( input == null )
				input = "input is empty.";

			return input.ToUpper();
		}

		public string Input_string_array(string[] array)
		{
			if( array == null )
				return "NULL";

			return string.Join("-", array);
		}

		public int Input_int_Add(int a, int b)
		{
			return a + b;
		}

		public int Input_int_nullable_Add(int? a, int? b)
		{
			return a.GetValueOrDefault() + b.GetValueOrDefault();
		}

		public decimal Input_decimal_Add(decimal a, decimal b)
		{
			return a + b;
		}

		public decimal Input_decimal_nullable_Add(decimal? a, decimal? b)
		{
			return a.GetValueOrDefault() + b.GetValueOrDefault();
		}

		public double Input_number_Add(int a, long b, float c, double d)
		{
			return a + b + c + d;
		}

		public double Input_number_nullable_Add(int? a, long? b, float? c, double? d)
		{
			return a.GetValueOrDefault() + b.GetValueOrDefault() 
					+ c.GetValueOrDefault() + d.GetValueOrDefault();
		}

		public string Input_DateTime(DateTime dt)
		{
			return dt.ToString("yyyyMMdd");
		}

		public string Input_DateTime_nullable(DateTime? dt)
		{
			return dt.GetValueOrDefault().ToString("yyyyMMdd");
		}

		public string Input_Guid(Guid guid)
		{
			return guid.ToString("N");
		}

		public string Input_Guid_nullable(Guid? guid)
		{
			return guid.GetValueOrDefault().ToString("N");
		}

		public string Input_Enum(DayOfWeek day)
		{
			// 测试时可输入【枚举名】或者【数字】，验证框架是否可正确转换
			return day.ToString();
		}


		public string Input_bool(bool b)
		{
			return b.ToString().ToLower();
		}

	}
}
