using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using ClownFish.Web.Reflection;
using ClownFish.Web.Client;

namespace ClownFish.Web.Serializer
{
	internal class FormDataProvider : BaseDataProvider, IActionParametersProvider2
	{
		#region IActionParametersProvider 成员

		public object[] GetParameters(HttpContext context, MethodInfo method)
		{
			// 不需要实现这个接口，使用另一个重载版本更高效。
			throw new NotImplementedException();
		}

		#endregion

		


		public object[] GetParameters(HttpContext context, ActionDescription action)
		{
			if( context == null )
				throw new ArgumentNullException("context");
			if( action == null )
				throw new ArgumentNullException("action");

			ParameterInfo p = null;
			object value = null;
			object[] parameters = new object[action.Parameters.Length];

			for( int i = 0; i < action.Parameters.Length; i++ ) {
				p = action.Parameters[i];
				value = null;

				if( TryGetSpecialParameter(context, p, out value) )
					parameters[i] = value;
				else
					parameters[i] = GetObjectFromHttp(context, p);
			}

			return parameters;
		}


		

		



	}
}
