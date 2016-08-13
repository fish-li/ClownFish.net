using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.Reflection
{
	internal static class AttributeFactory
	{
		public static readonly Hashtable s_table = Hashtable.Synchronized(new Hashtable(1024));


		public static T GetMyAttribute<T>(object obj, bool inherit = false) where T : Attribute
		{
			var key = new AttributeKey { Target = obj, AttributeType = typeof(T) };

			object result = s_table[key];

			if( DBNull.Value.Equals(result) )
				return null;

			if( result == null ) {
				if( obj is MemberInfo )
					result = (obj as MemberInfo).GetCustomAttribute<T>(inherit);

				else if( obj is ParameterInfo )
					result = (obj as ParameterInfo).GetCustomAttribute<T>(inherit);

				else
					throw new NotSupportedException();

				if( result == null )
					s_table[key] = DBNull.Value;
				else
					s_table[key] = result;
			}

			return result as T;
		}


		public static T[] GetMyAttributes<T>(object obj, bool inherit = false) where T : Attribute
		{
			var key = new AttributeKey { Target = obj, AttributeType = typeof(T) };

			object result = s_table[key];
			
			if( result == null ) {
				if( obj is MemberInfo )
					result = (obj as MemberInfo).GetCustomAttributes<T>(inherit);

				else if( obj is ParameterInfo )
					result = (obj as ParameterInfo).GetCustomAttributes<T>(inherit);

				else if( obj is Type )
					result = (obj as Type).GetCustomAttributes<T>(inherit);

				else
					throw new NotSupportedException();

				// 就算是找不到指定的Attribute，GetCustomAttributes()会返回一个空数组，所以不需要引用空值判断
				s_table[key] = result;
			}

			return result as T[];
		}

		private class AttributeKey
		{
			public object Target { get; set; }
			public Type AttributeType { get; set; }

			public override bool Equals(object obj)
			{
				if( obj == null )
					return false;

				AttributeKey key = (AttributeKey)obj;
				return key.Target == this.Target && key.AttributeType == this.AttributeType;
			}

			public override int GetHashCode()
			{
				int hash = 13;
				hash = (hash * 7) + Target.GetHashCode();
				hash = (hash * 7) + AttributeType.GetHashCode();

				return hash;
			}
		}
	}
}
