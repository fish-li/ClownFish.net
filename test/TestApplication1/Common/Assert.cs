using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.TestApplication1.Common
{
	public static class Assert
	{

		public static void IsTrue(bool condition)
		{
			if( condition == false )
				throw new AssertFailedException("IsTrue 断言判断失败。");
		}

		public static void IsFalse(bool condition)
		{
			if( condition )
				throw new AssertFailedException("IsFalse 断言判断失败。");
		}

		public static void IsNull(object value)
		{
			if( value != null )
				throw new AssertFailedException("IsNull 断言判断失败。");
		}

		public static void IsNotNull(object value)
		{
			if( value == null )
				throw new AssertFailedException("IsNotNull 断言判断失败。");
		}

		public static void AreSame(object expected, object actual)
		{
			if( object.ReferenceEquals(expected, actual) == false)
				throw new AssertFailedException("AreSame 断言判断失败。");
		}

		public static void AreNotSame(object notExpected, object actual)
		{
			if( object.ReferenceEquals(notExpected, actual) )
				throw new AssertFailedException("AreNotSame 断言判断失败。");
		}

		public static void AreEqual<T>(T expected, T actual)
		{
			if( object.Equals(expected, actual) == false ) {
				if ( IsSimpleType( typeof(T)) || (typeof(T) == typeof(string) && actual.ToString().Length < 256) )
					throw new AssertFailedException(string.Format("AreEqual 断言判断失败， 【{0}】, 【{1}】", expected, actual));
				else
					throw new AssertFailedException("AreEqual 断言判断失败。");
			}				
		}

		public static void AreNotEqual<T>(T notExpected, T actual)
		{
			if( object.Equals(notExpected, actual) ) {
				if( IsSimpleType(typeof(T)) || (typeof(T) == typeof(string) && actual.ToString().Length < 256) )
					throw new AssertFailedException(string.Format("AreNotEqual 断言判断失败， 【{0}】, 【{1}】", notExpected, actual));
				else
					throw new AssertFailedException("AreNotEqual 断言判断失败。");
			}				
		}


		private static bool IsSimpleType(Type t)
		{
			return t.IsPrimitive || t == typeof(DateTime) || t == typeof(Guid) || t == typeof(decimal);
		}
		
		public static void IsInstanceOfType(object value, Type expectedType)
		{
			if (expectedType.IsInstanceOfType(value) == false)
				throw new AssertFailedException("IsInstanceOfType 断言判断失败。");
		}

		public static void IsNotInstanceOfType(object value, Type wrongType)
		{
			if( wrongType.IsInstanceOfType(value) )
				throw new AssertFailedException("IsNotInstanceOfType 断言判断失败。");
		}


	}
}
