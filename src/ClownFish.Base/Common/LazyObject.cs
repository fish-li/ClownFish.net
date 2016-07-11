using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Base
{
	// 设计思路：
	// 用一个轻量级的对象（当前类型的实例），去包装一个需要延迟创建的对象（通过Getter属性来延迟创建）


	/// <summary>
	/// 表示需要延迟创建的对象包装类
	/// </summary>
	/// <typeparam name="T">需要延迟创建的对象类型</typeparam>
	public class LazyObject<T> where T : class, new()
	{
		/// <summary>
		/// 是否需要线程安全
		/// </summary>
		private bool _threadSafe;
		/// <summary>
		/// 实例引用
		/// </summary>
		private T _instance;

		private static readonly object s_lock = new object();

		/// <summary>
		/// 构造函数
		/// </summary>
		public LazyObject() : this(false) { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="threadSafe">是否需要线程安全</param>
		public LazyObject(bool threadSafe)
		{
			_threadSafe = threadSafe;
		}

		

		/// <summary>
		/// 获取延迟创建的对象引用
		/// </summary>
		public T Instance
		{
			get
			{
				if( _threadSafe )
					return GetInstanceThreadSafe();
				else
					return GetInstance();
			}
		}

		private T GetInstance()
		{
			if( _instance == null )
				_instance = ObjectFactory.New<T>();
			return _instance;
		}

		private T GetInstanceThreadSafe()
		{
			if( _instance == null ) {
				lock( s_lock ) {
					if( _instance == null )
						_instance = ObjectFactory.New<T>();
				}
			}

			return _instance;
		}
	}
}
