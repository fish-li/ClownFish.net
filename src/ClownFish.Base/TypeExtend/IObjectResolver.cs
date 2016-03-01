using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClownFish.Base.Reflection;

namespace ClownFish.Base.TypeExtend
{
	/// <summary>
	/// 对象的构造接口
	/// </summary>
	public interface IObjectResolver
	{
		/// <summary>
		/// 根据指定的类型获取对应的实例
		/// </summary>
		/// <param name="objectType"></param>
		/// <returns></returns>
		object CreateObject(Type objectType);
	}



	internal class DefaultObjectResolver : IObjectResolver
	{
		public object CreateObject(Type objectType)
		{
			if( objectType == null )
				throw new ArgumentNullException("objectType");


			// 封闭类型无法扩展，直接使用当前类型（不支持有参的构造函数）
			if( objectType.IsSealed )
				return objectType.FastNew();


			Type extType = ExtenderManager.GetExtendType(objectType);
	
			return (extType == null)
							? objectType.FastNew()	// 没有扩展类型，就直接使用原类型（不支持有参的构造函数）
							: extType.FastNew();	// 有扩展类型就创建扩展类型的实例
		}
	}
}
