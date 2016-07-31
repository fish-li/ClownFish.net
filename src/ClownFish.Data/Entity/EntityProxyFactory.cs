using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 实体代理的工厂类型
	/// </summary>
	public static class EntityProxyFactory
	{
		private static Hashtable s_table = Hashtable.Synchronized(new Hashtable(2048));

		///// <summary>
		///// 获取某个实体类型的代理类型
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <returns></returns>
		//public static Type GetProxy<T>()
		//{
		//	return s_table[typeof(T)] as Type;
		//}

		/// <summary>
		/// 获取某个实体类型的代理类型
		/// </summary>
		/// <returns></returns>
		public static Type GetProxy(Type entityType)
		{
			//ClownFish.Data.CodeDom.ProxyBuilder.Init();

			return s_table[entityType] as Type;
		}


		/// <summary>
		/// 注册实体的代理类型
		/// </summary>
		/// <param name="proxyType"></param>
		public static void Register(Type proxyType)
		{
			if( proxyType == null )
				throw new ArgumentNullException(nameof(proxyType));

            Type entityType = proxyType.BaseType;
            if( entityType.BaseType != TypeList._Entity)
                throw new ArgumentException("无效的实体代理类型：" + proxyType.FullName);

            if(TypeList._IEntityProxy.IsAssignableFrom(proxyType) == false )
                throw new ArgumentException("无效的实体代理类型：" + proxyType.FullName);

            lock ( ((ICollection)s_table).SyncRoot ) {
				s_table[entityType] = proxyType;
			}
		}


        /// <summary>
        /// （内部使用的）批量注册EntityProxy
        /// </summary>
        /// <param name="proxyTypes"></param>
        internal static void BatchRegister(IEnumerable<Type> proxyTypes)
		{
			// 内部使用版本，不做参数检查

			lock ( ((ICollection)s_table).SyncRoot ) {

				foreach( Type proxyType in proxyTypes) {

                    Type entityType = proxyType.BaseType;
                    s_table[entityType] = proxyType;
                }
			}
		}
	}
}
