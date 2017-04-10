using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class Base64ExtensionsTest
	{
		[TestMethod]
		public void Test_ToBase64_FromBase64()
		{
			string s = "为进一步规范ERP发版工作，加强研发质量管控，在2017年初，我们对ERP产品发版管理制度V1.1进行了优化，详细调整点如下：";

			string s2 = s.ToBase64();
			string s3 = s2.FromBase64();

			Assert.AreEqual(s, s3);
		}
	}
}
