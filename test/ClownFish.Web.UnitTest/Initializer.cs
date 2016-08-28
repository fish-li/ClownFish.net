using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.TypeExtend;
using ClownFish.MockAspnetRuntime;
using ClownFish.Web.UnitTest.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest
{
	[TestClass]
	public class Initializer
	{
		private static readonly HttpRuntime HttpRuntimeInstance = new HttpRuntime();


		[AssemblyInitialize]
		public static void InitRuntime(TestContext context)
		{
			ExtenderManager.RegisterExtendType(typeof(WebRuntimeExt));

			MockHttpRuntime.AppDomainAppPath = AppDomain.CurrentDomain.BaseDirectory;
			MockHttpRuntime.AppDomainAppVirtualPath = "/";
		}

	}
}
