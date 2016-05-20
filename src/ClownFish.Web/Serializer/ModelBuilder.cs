using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using ClownFish.Base.Reflection;
using ClownFish.Base.TypeExtend;
using ClownFish.Web.Reflection;

namespace ClownFish.Web.Serializer
{
	/// <summary>
	/// 从HTTP请求中创建数据对象的构造器
	/// </summary>
	public class ModelBuilder
	{
		private static Hashtable s_modelTable = Hashtable.Synchronized(
											new Hashtable(4096, StringComparer.OrdinalIgnoreCase));

		/// <summary>
		/// HttpContext的实例
		/// </summary>
		protected HttpContext _context;


		/// <summary>
		/// 从HTTP请求中构造复杂的参数对象（例如：自定义的数据类型）
		/// </summary>
		/// <param name="context"></param>
		/// <param name="parameterInfo"></param>
		/// <returns></returns>
		public virtual object CreateObjectFromHttp(HttpContext context, ParameterInfo parameterInfo)
		{
			if( context == null )
				throw new ArgumentNullException("context");
			if( parameterInfo == null )
				throw new ArgumentNullException("parameterInfo");

			_context = context;

			return CreateObject(parameterInfo);
		}

		/// <summary>
		/// 根据参数反射信息创建复杂的参数对象
		/// 如果需要控制对象的创建过程，可以重写这个方法
		/// </summary>
		/// <param name="parameterInfo"></param>
		/// <returns></returns>
		protected virtual object CreateObject(ParameterInfo parameterInfo)
		{
			object item = ObjectFactory.New(parameterInfo.ParameterType);
			FillModel(item, parameterInfo.Name);
			return item;
		}
		

		/// <summary>
		/// 根据HttpRequest填充一个数据实体。
		/// 这里不支持嵌套类型的数据实体，且要求各数据成员都是简单的数据类型。
		/// </summary>
		/// <param name="model"></param>
		/// <param name="paramName"></param>
		protected virtual void FillModel(object model, string paramName)
		{
			ModelDescription descripton = GetModelDescription(model.GetType());

			object val = null;
			foreach( PropertyInfo property in descripton.Properties ) {
				// 这里的实现方式不支持嵌套类型的数据实体。
				// 如果有这方面的需求，可以将这里改成递归的嵌套调用。

				string httpname = GetPropertyMapHttpName(property);
				Type ptype = property.PropertyType.GetRealType();
				val = GetValueFromHttpInternal(httpname, ptype, paramName);

				if( val == null ) {
					// 检查是否存在自定义的类型转换器
					IHttpDataConvert convert = HttpDataConvertFactory.GetConvert(ptype);
					if( convert != null )
						val = convert.Convert(_context, httpname);
				}

				if( val != null )
					property.SetValue(model, val);
			}
		}

		/// <summary>
		/// 获取数据对象属性对应的HTTP数据名称
		/// 重写这个方法可以解决服务端属性和前端名称不一致问题，例如：可以增加对别名的支持。
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		protected virtual string GetPropertyMapHttpName(PropertyInfo p)
		{
			return p.Name;
		}

		/// <summary>
		/// 返回一个实体类型的描述信息（全部属性及字段）。
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private ModelDescription GetModelDescription(Type type)
		{
			if( type == null )
				throw new ArgumentNullException("type");

			string key = type.FullName;
			ModelDescription mm = (ModelDescription)s_modelTable[key];

			if( mm == null ) {
				PropertyInfo[] array = (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
										where (p.GetMyAttribute<HttpValueIgnoreAttribute>(false) == null)
										select p
										).ToArray();

				mm = new ModelDescription { Properties = array };
				s_modelTable[key] = mm;
			}
			return mm;
		}


		/// <summary>
		/// 根据指定的名称及期望的数据类型，从HTTP上下文中获取对应的数据
		/// </summary>
		/// <param name="context"></param>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="parentName"></param>
		/// <returns></returns>
		public virtual object GetValueFromHttp(HttpContext context, string name, Type type, string parentName)
		{
			if( context == null )
				throw new ArgumentNullException("context");
			if( type == null )
				throw new ArgumentNullException("type");
			if( string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			_context = context;
			return GetValueFromHttpInternal(name, type, parentName);
		}


		/// <summary>
		/// 根据指定的名称及期望的数据类型，从HTTP上下文中获取对应的数据，
		/// GetValueFromHttp方法的内部实现（可重写）
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="parentName"></param>
		/// <returns></returns>
		protected virtual object GetValueFromHttpInternal(string name, Type type, string parentName)
		{
			string[] val = GetHttpValues(name, parentName);

			// 如果是字符串类型，就不用类型转换，直接返回。
			if( type == typeof(string[]) )
				return val;


			if( val == null || val.Length == 0 )
				return null;

			// 还原ASP.NET的默认数据格式
			string str = val.Length == 1 ? val[0] : string.Join(",", val);

			try {
				return StringToObject(str.Trim(), type);
			}
			catch( Exception ex ) {
				string message = string.Format("数据转换失败，当前参数名：{0} ，错误原因：{1}",
					(string.IsNullOrEmpty(parentName) ? name : parentName + "." + name),
					ex.Message
					);

				throw new InvalidCastException(message, ex);
			}
		}

		/// <summary>
		/// 根据名称读取相关的HTTP参数值
		/// 如果名称对应的参数没有数据，则根据父级名称再拼接一个名称重试
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parentName"></param>
		/// <returns></returns>
		protected virtual string[] GetHttpValues(string name, string parentName)
		{
			string[] val = GetHttpValues(name);
			if( val == null ) {
				// 再试一次。有可能是多个自定义类型，Form表单元素采用变量名做为前缀。
				if( string.IsNullOrEmpty(parentName) == false ) {
					val = GetHttpValues(parentName + "." + name);
				}
			}
			return val;
		}

		/// <summary>
		/// 根据名称读取相关的HTTP参数值
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected virtual string[] GetHttpValues(string name)
		{
			string[] val = _context.Request.QueryString.GetValues(name);

			if( val == null )
				val = _context.Request.Form.GetValues(name);

			if( val == null ) {
				// 尝试从 ActionHandler 读取更多的参数
				ActionHandler hanlder = _context.Handler as ActionHandler;
				if( hanlder != null ) {

					// 尝试从 PageRegexUrlAttribute 中读取正则表达式的匹配结果。
					if( hanlder.InvokeInfo.RegexMatch != null ) {
						string m = hanlder.InvokeInfo.RegexMatch.Groups[name].Value;
						if( m != null )
							val = new string[1] { m };
					}

					// 尝试从提取到的UrlActionInfo中获取。
					else if( hanlder.InvokeInfo.UrlActionInfo != null 
							&& hanlder.InvokeInfo.UrlActionInfo.Params != null ) {

						val = hanlder.InvokeInfo.UrlActionInfo.Params.GetValues(name);
					}
				}
			}

			return val;
		}



		private static readonly char[] s_stringSlitArray = new char[] { ',' };

		/// <summary>
		/// 将字符串转换成指定的数据类型
		/// </summary>
		/// <param name="value"></param>
		/// <param name="conversionType"></param>
		/// <returns></returns>
		public virtual object StringToObject(string value, Type conversionType)
		{
			// 注意：这个方法应该与ReflectionExtensions2.IsSupportableType保持一致性。
			// 如果 conversionType.IsSupportableType() 返回 true，这里应该能转换，除非字符串的格式不正确。

			if( conversionType == typeof(string) )
				return value;

			if( value == null || value.Length == 0 ) {
				// 空字符串根本不能做任何转换，所以直接返回null
				return null;
			}

			if( conversionType == typeof(string[]) )
				// 保持与NameValueCollection的行为一致。
				return value.Split(s_stringSlitArray, StringSplitOptions.RemoveEmptyEntries);


			if( conversionType == typeof(Guid) )
				return new Guid(value);

			if( conversionType.IsEnum )
				return Enum.Parse(conversionType, value);

			// 尝试使用类型的隐式转换（从字符串转换）
			if( conversionType.IsSupportableType() == false ) {
				MethodInfo stringImplicit = GetStringImplicit(conversionType);
				if( stringImplicit != null )
					return stringImplicit.FastInvoke(null, value);
			}

			if( conversionType == typeof(byte[]) )
				return Convert.FromBase64String(value);


			// 如果需要转换其它的数据类型，请重写下面的方法。
			return DefaultChangeType(value, conversionType);
		}


		/// <summary>
		/// 调用.NET的默认实现，将字符串转换成指定的数据类型。
		/// </summary>
		/// <param name="value"></param>
		/// <param name="conversionType"></param>
		/// <returns></returns>
		protected virtual object DefaultChangeType(string value, Type conversionType)
		{
			// 为了简单，直接调用 .net framework中的方法。
			// 如果转换失败，将会抛出异常。

			// 如果需要转换其它的数据类型，请重写这个方法。

			return Convert.ChangeType(value, conversionType);
		}


		/// <summary>
		/// 判断指定的类型是否能从String类型做隐式类型转换，如果可以，则返回相应的方法
		/// </summary>
		/// <param name="conversionType"></param>
		/// <returns></returns>
		private MethodInfo GetStringImplicit(Type conversionType)
		{
			MethodInfo m = conversionType.GetMethod("op_Implicit", 
													BindingFlags.Static | BindingFlags.Public, null,
													new Type[] { typeof(string) }, null);

			if( m != null && m.IsSpecialName && m.ReturnType == conversionType )
				return m;
			else
				return null;
		}

	}
}
