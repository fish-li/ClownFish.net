using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Cryptography
{
    [TestClass]
    public class TripleDESHelperTest
    {
        private static readonly string s_key = "《ERP产品全生命周期管控策略V4.0》";

        private static readonly string s_input = "如对本管理制度有任何疑问，欢迎咨询各团队QA";


		[TestMethod]
		public void Test()
		{
			string text = TripleDESHelper.Encrypt(s_input, s_key);
			string result = TripleDESHelper.Decrypt(text, s_key);
			Assert.AreEqual(s_input, result);


			byte[] b1 = s_input.GetBytes();
			byte[] b2 = TripleDESHelper.Encrypt(b1, s_key);
			byte[] b3 = TripleDESHelper.Decrypt(b2, s_key);
			Assert.IsTrue(b1.IsEqual(b3));
		}

	}
}
