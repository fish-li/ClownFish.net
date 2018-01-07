using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.Http;

namespace ClownFish.Data
{
	/// <summary>
	/// 用于指示ACTION参数是一个实体代理类型
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class EntityProxyAttribute : CustomDataAttribute
	{
		/// <summary>
		/// 根据HttpContext和ParameterInfo获取参数值
		/// </summary>
		/// <param name="context"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public override object GetHttpValue(HttpContext context, ParameterInfo p)
		{
			return EntityHttpLoader.LoadFromHttp(context, p);
		}


		// 注意：
		// 实体代理对象被框架创建后，本身是没有关联到任何DbContext实例的，
		// 如果希望控制关联，例如在异步代码中，需要调用 xx.BindDbContext(...) 方法
	
		
		// 可参考下面被注释的代码示例。


	}

/*

	public class TestController
	{
		/// <summary>
		/// EntityProxyAttribute 的用法演示（异步）
		/// </summary>
		/// <param name="p"></param>
		public async Task Action1Async([EntityProxy]Product p)
		{
			using( DbContext db = DbContext.Create() ) {

				// 绑定实体代理的DbContext引用
				p.BindDbContext(db);

				// 将实体插入到数据库
				await p.InsertAsync();
			}
		}


		/// <summary>
		/// EntityProxyAttribute 的用法演示（同步）
		/// </summary>
		/// <param name="p"></param>
		public int Action1([EntityProxy]Product p)
		{
			// 将实体插入到数据库，并返回新产生的自增列ID
			return p.InsertReturnNewId();


			// 说明：
			// 此时，实体代理在执行插入时，会自动创建一个DbContext实例，但前提是需要在程序初始化时执行下面的调用。
			// Initializer.Instance.AllowCreateOneOffDbContext();
		}


	}

*/

}
