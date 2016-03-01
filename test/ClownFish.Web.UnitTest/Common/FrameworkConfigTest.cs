using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Config
{
	[TestClass]
	public class FrameworkConfigTest
	{
		[TestMethod]
		public void Test()
		{
			// 简单测试
			// 预期值直接写在配置文件中。

			Assert.AreEqual("callback", FrameworkConfig.Instance.Action.JsonpCallback);

			Assert.AreEqual("/Page/404DiagnoseResult.aspx", FrameworkConfig.Instance.Pipeline.Http404PagePath);
		}
	}
}
