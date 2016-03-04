using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Base.Common
{

	/// <summary>
	/// 辅助测试的工具类
	/// </summary>
	public static class TestHelper
	{
		
		/// <summary>
		/// 为测试强制设置一个异常，然后在调用TryThrowException()时将会抛出，
		/// 由于这个属性仅仅用于测试环境，因此不考虑线程安全问题
		/// </summary>
		public static Exception ExceptionForTest { get; set; }

		/// <summary>
		/// 调用这个方法可以模拟意外的异常发生，用于检验catch的代码是否能正确工作。
		/// 抛出 ExceptionForTest 指定的异常，并将ExceptionForTest设置为NULL，
		/// 如果 ExceptionForTest 为NULL，将忽略本次调用
		/// </summary>
		public static void TryThrowException()
		{
			if( ExceptionForTest != null ) {
				Exception ex = ExceptionForTest;

				// 确保只触发一次
				ExceptionForTest = null;

				throw ex;
			}
		}


	}

}
