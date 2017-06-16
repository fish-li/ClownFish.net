using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Caching;


/*  使用方法
		private static FileDependencyManager<List<User>>
					s_cacheItem = new FileDependencyManager<List<User>>(
							files => XmlHelper.XmlDeserializeFromFile<List<User>>(files[0]),
							Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data\Users.config"));

		public static List<User> Users
		{
			get { return s_cacheItem.Result; }
		}
*/

namespace ClownFish.Base.Files
{

	/// <summary>
	/// 文件缓存依赖的管理类。
	/// 注意：请将些类型的实例用【静态字段】来引用，否则会产生内存泄露。
	/// </summary>
	/// <typeparam name="T">缓存的数据类型</typeparam>
	public sealed class FileDependencyManager<T>
	{
		private string[] _files;
		private Func<string[], T> _loadFileCallback;
        private Action<string[], CacheItemRemovedReason> _afterRemoveCallback;

        private readonly string CacheKey = Guid.NewGuid().ToString();

		private CacheResult<T> _cacheResult;
		/// <summary>
		/// 缓存结果
		/// </summary>
		public T Result
		{
			get { return _cacheResult.Result; }
		}

        /// <summary>
        /// 构造方法。
        /// 注意：请将些类型的实例用【静态字段】来引用，否则会产生内存泄露。
        /// </summary>
        /// <param name="loadFileCallback">当需要加载文件时的回调委托</param>
        /// <param name="files">要读取的文件，读取后会做修改监控</param>
        public FileDependencyManager(Func<string[], T> loadFileCallback,  params string[] files)
            : this(loadFileCallback,  null, files)
        {
        }

        /// <summary>
        /// 构造方法。
        /// 注意：请将些类型的实例用【静态字段】来引用，否则会产生内存泄露。
        /// </summary>
        /// <param name="loadFileCallback">当需要加载文件时的回调委托</param>
        /// <param name="afterRemoveCallback">缓存移除后触发的回调委托</param>
        /// <param name="files">要读取的文件，读取后会做修改监控</param>
        public FileDependencyManager(Func<string[], T> loadFileCallback, 
                                    Action<string[], CacheItemRemovedReason> afterRemoveCallback, 
                                    params string[] files)
		{
			if( loadFileCallback == null )
				throw new ArgumentNullException("loadFileCallback");

			if( files == null || files.Length == 0 )
				throw new ArgumentNullException("files");


			_loadFileCallback = loadFileCallback;
            _afterRemoveCallback = afterRemoveCallback;
			_files = files;

			this.GetObject();
		}

		private void GetObject()
		{
			Exception ex = null;
			T result = default(T);

			try {
				result = _loadFileCallback(_files);
			}
			catch( Exception e ) {
				ex = e;
			}

			//if( ex == null ) {

			// 让Cache帮我们盯住这个配置文件。
			CacheDependency dep = new CacheDependency(_files);
			HttpRuntime.Cache.Insert(CacheKey, "Fish Li", dep,
							System.Web.Caching.Cache.NoAbsoluteExpiration,
							System.Web.Caching.Cache.NoSlidingExpiration,
							CacheItemPriority.NotRemovable, RemovedCallback);

			//}

			_cacheResult = new CacheResult<T>(result, ex);
		}


		/// <summary>
		/// 等待文件句柄关闭的时间，单位：毫秒，默认值：3000（3秒）。
		/// 默认值是一个比较保守的时间，为了快速运行单元测试用例，可以修改这个时间
		/// </summary>
		private static int s_WaitFileCloseTimeout = 3000;

		private void RemovedCallback(string key, object value, CacheItemRemovedReason reason)
		{
            // 参数中的 key, value 在这里是没有意义的，所以忽略这二个参数。

			System.Threading.Thread.Sleep(s_WaitFileCloseTimeout);		// 等待文件关闭

            // 只要是缓存项被移除，就重新加载，不管是什么原因
			this.GetObject();

            // 缓存移除后调用回调委托，可以实现一些诸如：文件同步之类的操作
            _afterRemoveCallback?.Invoke(_files, reason);
		}
	}
}
