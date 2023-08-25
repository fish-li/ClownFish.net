using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Cryptography
{
    [TestClass]
    public class AesHelperTest
    {
		private static readonly string s_key = "《ERP产品全生命周期管控策略V4.0》";

		private static readonly string s_input = "如对本管理制度有任何疑问，欢迎咨询各团队QA";


		[TestMethod]
		public void Test()
		{
			string text = AesHelper.Encrypt(s_input, s_key);
			string result = AesHelper.Decrypt(text, s_key);
			Assert.AreEqual(s_input, result);


			byte[] b1 = s_input.GetBytes();
			byte[] b2 = AesHelper.Encrypt(b1, s_key);
			byte[] b3 = AesHelper.Decrypt(b2, s_key);
			Assert.IsTrue(b1.IsEqual(b3));
		}

	}
}
