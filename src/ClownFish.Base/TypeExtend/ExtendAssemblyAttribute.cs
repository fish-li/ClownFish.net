using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.TypeExtend
{
	/// <summary>
	/// 指示这个是一个包含扩展类型的程序集。
	/// 注意：如果代码直接调用 ExtenderManager.RegisterExtendType 或者 RegisterSubscriber 将不参考这个标记。
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class ExtendAssemblyAttribute : Attribute
	{
	}


	/// <summary>
	/// 指示这是一个扩展类型（继承类型，或者事件订阅类型），用于自动加载扩展类型时识别。
	/// 注意：如果代码直接调用 ExtenderManager.RegisterExtendType 或者 RegisterSubscriber 将不参考这个标记。
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExtendTypeAttribute : Attribute
	{
	}
}
