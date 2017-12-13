using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.Base.Files
{
	/// <summary>
	/// 缓存结果项的包装类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class CacheResult<T>
	{
		internal CacheResult(T result, Exception ex)
		{
			_exception = ex;
			_result = result;
		}

		private Exception _exception;

		private T _result;

		/// <summary>
		/// 缓存结果项
		/// </summary>
		public T Result
		{
			get
			{
				if( _exception != null )
					throw _exception;

				return _result;
			}
		}

        /// <summary>
        /// 直接获取结果，不判断有没有异常，如果有异常则返回类型的默认值。
        /// </summary>
        /// <returns></returns>
        public T TryGetResult()
        {
            return _result;
        }

	}
}
