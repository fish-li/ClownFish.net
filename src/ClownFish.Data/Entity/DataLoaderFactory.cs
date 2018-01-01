using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;
using ClownFish.Data;

namespace ClownFish.Data
{
	/// <summary>
	/// DataLoader工厂
	/// </summary>
	public static class DataLoaderFactory
	{
		private static Hashtable s_table = Hashtable.Synchronized(new Hashtable(2048));

		/// <summary>
		/// 获取某种数据类型的IDataLoader实现对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IDataLoader<T> GetLoader<T>() where T : class, new()
		{
			//ClownFish.Data.CodeDom.ProxyBuilder.Init();

			object value = s_table[typeof(T)];
			if( value == null )
				// 如果没有注册实体对应的IDataLoader实现类型，就使用默认的反射实现
				return new DefaultDataLoader<T>();

			else {
				Type dataloaderType = value as Type;
				if( dataloaderType != null )
					return (IDataLoader<T>)dataloaderType.FastNew();
				else
					return (IDataLoader<T>)value;
			}
		}

		private static Type GetEntityType(Type dataloaderType)
		{
			if( dataloaderType == null )
				throw new ArgumentNullException(nameof(dataloaderType));

			Type entityType = null;

			Type[] types = dataloaderType.GetInterfaces();
			foreach( Type t in types ) {
				if( t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDataLoader<>) ) {
					entityType = t.GetGenericArguments()[0];
					break;
				}
			}

			if( entityType == null )
				throw new ArgumentException(string.Format("类型 {0} 没有实现 IDataLoader<TEntity> 接口",
					dataloaderType.FullName));


			if( dataloaderType.GetConstructor(Type.EmptyTypes) == null )
				throw new ArgumentException(string.Format("类型 {0} 没有公开的无参构造函数",
					dataloaderType.FullName));

			return entityType;
		}

		/// <summary>
		/// 注册数据实体类型对应的IDataLoader实现类型
		/// </summary>
		/// <param name="dataloaderType"></param>
		public static void RegisterType(Type dataloaderType)
		{
            Type entityType = GetEntityType(dataloaderType);

			lock ( ((ICollection)s_table).SyncRoot ) {
				s_table[entityType] = dataloaderType;
			}
		}

		/// <summary>
		/// 注册数据实体类型对应的IDataLoader实现对象
		/// </summary>
		/// <param name="instance"></param>
		public static void RegisterInstance(object instance)
		{
			if( instance == null )
				throw new ArgumentNullException(nameof(instance));

			// 防止错误的调用
			if( instance is Type )
				throw new ArgumentException("参数不能是类型对象。");


			// 检验对象的类型是否有效
			Type entityType = GetEntityType(instance.GetType());

			lock( ((ICollection)s_table).SyncRoot ) {
				s_table[entityType] = instance;
			}
		}


		/// <summary>
		/// （内部使用的）批量注册DataLoader
		/// </summary>
		/// <param name="dataloaders"></param>
		internal static void BatchRegister(IEnumerable<object> dataloaders)
		{
			lock ( ((ICollection)s_table).SyncRoot ) {
				foreach(object instance in dataloaders ) {
                    Type entityType = GetEntityType(instance.GetType());
                    s_table[entityType] = instance;
                }
			}
		}
	}
}
