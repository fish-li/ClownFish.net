using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web.UnitTest.Controllers
{

	public sealed class ByteArrayItem
	{
		public string StrValue { get; set; }

		public byte[] ByteArray { get; set; }

		public override string ToString()
		{
			return string.Format("StrValue: {0}, Byte: {1}",
							StrValue, Convert.ToBase64String(ByteArray));
		}
	}

	public class AdvancedDataService : BaseController
	{
		public void TestContextDataAttribute(
			[ContextData("Request.HttpMethod")] string httpMethod,
			[ContextData("User.Identity.Name")] string username,
			[ContextData("HttpRuntime.AppDomainAppVirtualPath")] string appPath
			)
		{
			this.WriteHeader("Request.HttpMethod", httpMethod);
			this.WriteHeader("User.Identity.Name", username);
			this.WriteHeader("HttpRuntime.AppDomainAppVirtualPath", appPath);

		}


		public string InputByteArray(byte[] bb)
		{
			return Convert.ToBase64String(bb);
		}


		public string InputByteArrayItem(ByteArrayItem item)
		{
			return item.ToString();
		}
	}
}
