using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Cryptography
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


        [TestMethod]
        public void Test_FromBase64()
        {
            string s1 = "请指定要查看的日期!!#@%$@#%$";

            string s2 = s1.ToBase64();
            string s3 = s2.FromBase64(true);
            Assert.AreEqual(s1, s3);
            
            string s4 = s2 + "xxx";
            string s5 = s4.FromBase64();  // 默认行为：忽略异常输入
            Assert.IsNull(s5);

            MyAssert.IsError<FormatException>(()=> {
                _ = s4.FromBase64(true);
            });            
        }

        [TestMethod]
		public void Test_Input_Empty()
        {
			Assert.AreEqual(string.Empty, Base64Extensions.ToBase64(string.Empty));
			Assert.AreEqual(null, Base64Extensions.ToBase64(null));

			Assert.AreEqual(string.Empty, Base64Extensions.FromBase64(string.Empty));
			Assert.AreEqual(null, Base64Extensions.FromBase64(null));
		}

		[TestMethod]
		public void Test_Input_Error()
        {
			Assert.AreEqual(null, Base64Extensions.FromBase64("xxxx!#$$$@#$%$xxxxxxxxxxxxx"));
		}
	}
}
