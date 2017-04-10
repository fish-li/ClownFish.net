using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class CryptoHelperTest
	{
		private static readonly string s_key = "《ERP产品全生命周期管控策略V4.0》";

		private static readonly string s_input = "如对本管理制度有任何疑问，欢迎咨询各团队QA";


		[TestMethod]
		public void Test_TripleDESHelper()
		{
			string text = TripleDESHelper.Encrypt(s_input, s_key);
			string result = TripleDESHelper.Decrypt(text, s_key);
			Assert.AreEqual(s_input, result);


			byte[] b1 = s_input.GetBytes();
			byte[] b2 = TripleDESHelper.Encrypt(b1, s_key);
			byte[] b3 = TripleDESHelper.Decrypt(b2, s_key);
			Assert.IsTrue(ByteTestHelper.AreEqual(b1, b3));
		}


		[TestMethod]
		public void Test_AesHelper()
		{
			string text = AesHelper.Encrypt(s_input, s_key);
			string result = AesHelper.Decrypt(text, s_key);
			Assert.AreEqual(s_input, result);


			byte[] b1 = s_input.GetBytes();
			byte[] b2 = AesHelper.Encrypt(b1, s_key);
			byte[] b3 = AesHelper.Decrypt(b2, s_key);
			Assert.IsTrue(ByteTestHelper.AreEqual(b1, b3));
		}
	}
}
