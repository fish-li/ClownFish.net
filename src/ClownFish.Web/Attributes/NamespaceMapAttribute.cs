using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ClownFish.Web
{
	/// <summary>
	/// 命名空间映射标记。
	/// 注意：为了规范代码，在一个应用程序中的包含相同ShortName属性值的NamespaceMapAttribute实例不允许重复，
	/// 所以不要使用与程序集不匹配的命名空间。
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
	public sealed class NamespaceMapAttribute : System.Attribute
	{
		/// <summary>
		/// 命名空间的名字字符串
		/// </summary>
		public string Namespace { get; private set; }

		/// <summary>
		/// 映射之后的短名
		/// </summary>
		public string ShortName { get; private set; }



		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="namespaceString">命名空间的名字字符串</param>
		/// <param name="shortName">映射之后的短名</param>
		public NamespaceMapAttribute(string namespaceString, string shortName)
		{
			if( string.IsNullOrEmpty(namespaceString) )
				throw new ArgumentNullException("namespaceString");

			if( string.IsNullOrEmpty(shortName) )
				throw new ArgumentNullException("shortName");

			if( shortName.IndexOf('.') >= 0 )
				throw new ArgumentException("命名空间别名不允许使用 . 号，请考虑直接使用命名空间而不是使用别名。");

			this.Namespace = namespaceString;
			this.ShortName = shortName;
		}

	}
}
