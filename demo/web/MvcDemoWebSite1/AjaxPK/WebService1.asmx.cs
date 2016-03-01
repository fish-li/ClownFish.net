using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using ClownFish.Base.Xml;
using ClownFish.Web;
using DEMO.Model.AjaxPK;

namespace MvcDemoWebSite1.AjaxPK
{
	/// <summary>
	/// WebService1 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
	[System.Web.Script.Services.ScriptService]
	public class WebService1 : System.Web.Services.WebService
	{

		[WebMethod]
		public int Add(int a, int b)
		{
			return a + b;
		}



		[WebMethod]
		public string AddCustomer(Customer customer)
		{
			if( customer == null )
				return "customer is null.";

			// 简单地返回一个XML字符串。
			// 告诉客户端：服务端收到了什么样的数据。
			return XmlHelper.XmlSerialize(customer, Encoding.UTF8);
		}
	}
}
