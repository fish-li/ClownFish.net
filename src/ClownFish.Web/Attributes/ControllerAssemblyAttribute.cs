using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.Web
{
	/// <summary>
	/// 指示包含Controller的程序集
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class ControllerAssemblyAttribute : Attribute
	{
	}
}
