using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClownFish.Web.Config
{
	//说明：
	//这里设计一个配置文件用来配置MVC的路由规则，当然，你也可以选择使用代码方式注册路由。
	//设计配置文件的想法是：不希望反复修改初始化代码，而且路由规则在配置文件中对可读性或许会好一点（个人观点，不必纠结！）



	/// <summary>
	/// MVC路径表的配置文件
	/// </summary>
	[XmlRoot("RouteTable")]
	public sealed class RouteTableConfig
	{
		/// <summary>
		/// 所有路由规则列表
		/// </summary>
		[XmlArrayItem("Route")]
		public List<Route> Routes = new List<Route>();		
	}


	/// <summary>
	/// 表示一条路由配置规则
	/// </summary>
	public sealed class Route
	{
		/// <summary>
		/// 路由名称，可选项
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; set; }


		/// <summary>
		/// 路由URL，可包含参数占位符
		/// </summary>
		[XmlAttribute("url")]
		public string Url { get; set; }


		/// <summary>
		/// 与URL匹配的Controller的命名空间，可选项
		/// </summary>
		[XmlElement("namespace")]
		public string Namespace { get; set; }


		/// <summary>
		/// 与URL匹配的Controller的名称，可选项
		/// </summary>
		[XmlElement("controller")]
		public string Controller { get; set; }


		/// <summary>
		/// 与URL匹配的Action名称，可选项
		/// </summary>
		[XmlElement("action")]
		public string Action { get; set; }

	}



}
