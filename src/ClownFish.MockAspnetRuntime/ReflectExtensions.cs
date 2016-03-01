using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ClownFish.MockAspnetRuntime
{
    public static class ReflectExtensions
    {

		public static FieldInfo GetInstanceField(this Type t, string fieldName)
        {
			return GetField(t, fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

		public static FieldInfo GetStaticField(this Type t, string fieldName)
		{
			return GetField(t, fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}


		public static FieldInfo GetField(this Type t, string fieldName, BindingFlags flags)
		{
			if( t == null ) 
				throw new ArgumentNullException("t");
			
			if( string.IsNullOrEmpty(fieldName) ) 
				throw new ArgumentNullException("fieldName");
			

			return t.GetField(fieldName, flags);
		}


		public static void SetValue(this Type t, string fieldName, object instance, object value)
		{
			if( t == null )
				throw new ArgumentNullException("t");

			if( string.IsNullOrEmpty(fieldName) )
				throw new ArgumentNullException("fieldName");

			BindingFlags flags = instance == null
				? BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
				: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			PropertyInfo property = t.GetProperty(fieldName, flags);
			if( property != null ) {
				property.SetValue(instance, value);
			}
			else {
				FieldInfo field = t.GetField(fieldName, flags);

				field.SetValue(instance, value);
			}
		}


		public static void SetValue(this object instance, string fieldName, object value)
		{
			if( instance == null )
				throw new ArgumentNullException("instance");

			SetValue(instance.GetType(), fieldName, instance, value);
		}


		public static object GetValue(this Type t, string fieldName, object instance)
		{
			if( t == null )
				throw new ArgumentNullException("t");

			if( string.IsNullOrEmpty(fieldName) )
				throw new ArgumentNullException("fieldName");

			BindingFlags flags = instance == null
				? BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
				: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			PropertyInfo property = t.GetProperty(fieldName, flags);
			if( property != null ) {
				return property.GetValue(instance, null);
			}
			else {
				FieldInfo field = t.GetField(fieldName, flags);

				return field.GetValue(instance);
			}
		}


		public static object GetValue(this object instance, string fieldName)
		{
			if( instance == null )
				throw new ArgumentNullException("instance");

			return GetValue(instance.GetType(), fieldName, instance);
		}


		public static object InvokeMethod(this Type t, string methodName, object instance, params object[] parameters)
		{
			if( t == null )
				throw new ArgumentNullException("t");

			if( string.IsNullOrEmpty(methodName) )
				throw new ArgumentNullException("methodName");

			BindingFlags flags = instance == null
				? BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
				: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			MethodInfo method = t.GetMethod(methodName, flags);

			try {
				if( method.ReturnType == typeof(void) ) {
					method.Invoke(instance, parameters);
					return null;
				}
				else
					return method.Invoke(instance, parameters);
			}
			catch( TargetInvocationException ex ) {
				throw ex.InnerException;
			}
		}

		public static ConstructorInfo GetSpecificCtor(this Type t, params Type[] types)
		{
			if( t == null )
				throw new ArgumentNullException("t");


			return t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
				types, null);
		}
    }

}
