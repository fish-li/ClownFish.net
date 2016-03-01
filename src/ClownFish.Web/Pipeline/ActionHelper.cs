using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web
{
	/// <summary>
	/// Action的一些默认操作，可重写用于个性化调整
	/// </summary>
	public class ActionHelper
	{
		
		/// <summary>
		/// 将一个对象包装成IActionResult的实例
		/// 默认以TEXT形式输出，如果需要修改默认格式，可以重写这个方法。
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public virtual IActionResult ObjectToResult(object result)
		{
			if( result == null )
				return null;

			return new TextResult(result);
		}

		/// <summary>
		/// 当Action方法没有指定[Action]标记时，创建一个默认的ActionAttribute的实例，
		/// 如果希望修改ActionAttribute的一些默认属性，可以重写这个方法。
		/// </summary>
		/// <returns></returns>
		public virtual ActionAttribute CreateDefaultActionAttribute()
		{
			return new ActionAttribute();
		}
	}
}
