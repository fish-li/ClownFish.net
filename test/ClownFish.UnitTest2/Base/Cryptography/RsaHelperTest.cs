using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ClownFish.UnitTest.Base.Cryptography
{
	[TestClass]
	public class RsaHelperTest
	{
		//注意：
		// 以下测试用例需要使用X509证书，请在测试前导入
		// 测试证书存放路径  [当前项目]\cert


		internal static readonly string TestString = @"
大家好：
为进一步规范ERP发版工作，加强研发质量管控，在2017年初，我们对ERP产品发版管理制度V1.1进行了优化，详细调整点如下：
1、结合公司《ERP产品全生命周期管控策略V4.0》，将孵化期产品和成长&成熟期产品的发版活动进行区分细化
2、新增定义《ERP研发质量标准v1.0》
3、结合公司《产品PMO管理机制v1.0》，修订《产品上市评估表》形成2017年版本
4、针对主项目和子项目协作问题，梳理形成《ERP发版-主项目工作清单v1.0》

经ERP353和云ERP1.0SP3发版前工作的试运行、各事业部核心角色的评审，最终经产品PMO的审核，现正式发布ERP产品发版管理制度V2.0，即日生效！
   
如对本管理制度有任何疑问，欢迎咨询各团队QA";

		[TestMethod]
		public void Test_Sign_Verify()
		{
			byte[] data = TestString.GetBytes();
			string signature = RsaHelper.Sign(data, X509FinderTest.CertSubject);

			bool flag = RsaHelper.Verify(data, signature, X509FinderTest.PublicKeyBytes);
			Assert.IsTrue(flag);

			bool flag2 = RsaHelper.Verify(data, signature, X509FinderTest.PublicKeyText);
			Assert.IsTrue(flag2);

			bool flag3 = RsaHelper.Verify(data, signature, X509FinderTest.TestCert);
			Assert.IsTrue(flag3);


			// 随便改一下签名
			string signX = "abc" + signature.Substring(3);
			flag = RsaHelper.Verify(data, signX, X509FinderTest.PublicKeyBytes);
			Assert.IsFalse(flag);


			// 随便修改下原始输入数据
			string s2 = "abc" + TestString;
			byte[] data2 = s2.GetBytes();
			string sign2 = RsaHelper.Sign(data2, X509FinderTest.CertSubject);

			// 数据修改了，签名就肯定不一样
			Assert.AreNotEqual(signature, sign2);

		}


		[TestMethod]
		public void Test_Sign()
		{
			byte[] data = TestString.GetBytes();
			string signature1 = RsaHelper.Sign(data, X509FinderTest.CertSubject);
			string signature2 = RsaHelper.Sign(data, X509FinderTest.TestCert);
			Assert.AreEqual(signature1, signature2);
		}



		[TestMethod]
		public void Test_Encrypt_Decrypt()
		{
			string s = @"已评估过类似方案，上海和融25版本做过类似改动，但需求有差异。";
			byte[] bb = Encoding.UTF8.GetBytes(s);

			byte[] b1 = RsaHelper.Encrypt(bb, X509FinderTest.CertSubject);
			byte[] b2 = RsaHelper.Decrypt(b1, X509FinderTest.CertSubject);

			Assert.IsTrue(bb.IsEqual(b2));
		}


		[TestMethod]
		public void Test_EncryptText()
		{
			// 加密只允许用公钥，私钥虽然也能加密，但这样的话就没必要用RSA算法了！
			X509Certificate2 x509 = X509Finder.LoadFromPublicKey(X509FinderTest.PublicKeyText);
			string encString = RsaHelper.EncryptText(x509, TestString);

			string text = RsaHelper.DecryptText(X509FinderTest.TestCert, encString);

			Assert.AreEqual(TestString, text);
		}


		[TestMethod]
		public void Test_Encrypt_CertNotFound()
		{
			byte[] bb = Guid.NewGuid().ToByteArray();

			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				RsaHelper.Encrypt(bb, "abc");
			});

			Assert.AreEqual($"不能根据指定的证书主题 abc 找到匹配的证书。", error);
		}

		[TestMethod]
		public void Test_Decrypt_CertNotFound()
		{
			byte[] bb = Guid.NewGuid().ToByteArray();

			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				RsaHelper.Decrypt(bb, "abc");
			});

			Assert.AreEqual($"不能根据指定的证书主题 abc 找到匹配的证书。", error);
		}


		[TestMethod]
		public void Test_Sign_CertNotFound()
		{
			byte[] bb = Guid.NewGuid().ToByteArray();

			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				RsaHelper.Sign(bb, "abc");
			});

			Assert.AreEqual($"不能根据指定的证书主题 abc 找到匹配的证书。", error);
		}


		[TestMethod]
		public void Test_Error()
        {
			byte[] bb = new byte[] { 1, 2, 3 };

			X509Certificate2 cert2 = X509Finder.FindBySubject(X509FinderTest.CertSubject, true);

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Sign((byte[])null, "xx");
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Sign(bb, (string)null);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Sign((byte[])null, cert2);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Sign(bb, (X509Certificate2)null);
			});



			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Verify((byte[])null, "xx", bb);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Verify(bb, (string)null, bb);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Verify(bb, "xx", (byte[])null);
			});


			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Verify((byte[])null, "xx", cert2);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Verify(bb, (string)null, cert2);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Verify(bb, "xx", (X509Certificate2)null);
			});



			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Encrypt((byte[])null, "xx");
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Encrypt(bb, (string)null);
			});


			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Decrypt((byte[])null, "xx");
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.Decrypt(bb, (string)null);
			});



			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.EncryptText(null, "xx");
			});

			X509Certificate2 x509 = X509Finder.LoadFromPublicKey(X509FinderTest.PublicKeyText);
			Assert.AreEqual(string.Empty, RsaHelper.EncryptText(x509, string.Empty));
			Assert.AreEqual(string.Empty, RsaHelper.EncryptText(x509, null));


			MyAssert.IsError<ArgumentNullException>(() => {
				RsaHelper.DecryptText(null, "xx");
			});
			Assert.AreEqual(string.Empty, RsaHelper.DecryptText(cert2, string.Empty));
			Assert.AreEqual(string.Empty, RsaHelper.DecryptText(cert2, null));

			MyAssert.IsError<ArgumentException>(() => {
				RsaHelper.DecryptText(cert2, "xx");
			});
		}

	}
}
