using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class RsaHelperTest
	{
		//注意：
		// 以下测试用例需要使用X509证书，请在测试前导入
		// 测试证书存放路径  [当前项目]\cert



		private static readonly string s_certName = "Myosft-NewDogSign-TEST";


		private static readonly string s_publicKey = @"
-----BEGIN CERTIFICATE-----
MIIDJDCCAgygAwIBAgIQ8U3B/xOEurRBV7ouZQ5QSTANBgkqhkiG9w0BAQ0FADAh
MR8wHQYDVQQDExZNeW9zZnQtTmV3RG9nU2lnbi1URVNUMCAXDTA0MTIzMTE2MDAw
MFoYDzIwOTkxMjMxMTYwMDAwWjAhMR8wHQYDVQQDExZNeW9zZnQtTmV3RG9nU2ln
bi1URVNUMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAuQrcU6LMaLLI
q9PeksiJCjaisyeBM3UcDWH7E95d6HbVx7ySqXICLCdXGcNREpA9EARJRy4Pb+eO
u17YQh0BhMqEihKpuaI/mZUhZsfiZgaB0kECRaPTNWOo76h9B+I/5Qx9atBFnHKx
7iFbXHryPJx2rvfr+GjmlZYPI8KBvkdSNf8B4O/f4BChgKuqKX5U1JYUwjSeSAJ/
QPoRKsCla9D7yqq6ETiT79cpXeY/HN+78RZmlxPv2Ge1670LJDvYzA73G+r4E0l9
VfF2sQSx1koWaK8FIImrJFUxDelvG30ZxSpa2b7tWITMCgIAHoHPVk+y8nJgONax
eLMq80n0uQIDAQABo1YwVDBSBgNVHQEESzBJgBB3HwODmlUThnPneKOsbQLXoSMw
ITEfMB0GA1UEAxMWTXlvc2Z0LU5ld0RvZ1NpZ24tVEVTVIIQ8U3B/xOEurRBV7ou
ZQ5QSTANBgkqhkiG9w0BAQ0FAAOCAQEAf3adOrWJKpW5DjAMJ/+iaNR5ed8RrPH3
FmK3l+9Qe3eKmEySbdbCpHAXXgtXqqx+PA+yNqfoQ+UvhtwEUrNQE3POImv1YTte
3jmw0HYyqc4IXDFRfTtyh7/3kI0WDydKMIfj8b9WraxbFBNXwyc86dN+0GFffooP
0PnJSotcw5y6cqHPEzrm79OUNq3iH+7YADyyMjo9B6FRBIZ2GJVvvCqGD1O7BEWQ
RZGcI51BGWBSO1czd4jtS7xwTpcnbMMrpHmL0xNQ23wE7nPOCT2t8PRlwhUoQ4KR
YiuCv3Mb6+FnGYZtPOo7BK2e/BnamoJBNJERk+xoburzyZxQ1h94XQ==
-----END CERTIFICATE-----
";


		[TestMethod]
		public void Test_RsaHelper_Encrypt_Decrypt()
		{
			string s = @"已评估过类似方案，上海和融25版本做过类似改动，但需求有差异。
和融做此需求时改动量比较大，且与252版本跨系列了，参考价值降低。";
			byte[] bb = Encoding.UTF8.GetBytes(s);

			byte[] b1 = RsaHelper.Encrypt(bb, s_certName);
			byte[] b2 = RsaHelper.Decrypt(b1, s_certName);

			string s2 = Encoding.UTF8.GetString(b2);

			Assert.AreEqual(s, s2);
		}


		[TestMethod]
		public void Test_RsaHelper_Encrypt_CertNotFound()
		{
			byte[] bb = Guid.NewGuid().ToByteArray();

			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				RsaHelper.Encrypt(bb, "abc");
			});

			Assert.AreEqual($"加密证书abc不存在。", error);
		}

		[TestMethod]
		public void Test_RsaHelper_Decrypt_CertNotFound()
		{
			byte[] bb = Guid.NewGuid().ToByteArray();

			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				RsaHelper.Decrypt(bb, "abc");
			});

			Assert.AreEqual($"加密证书abc不存在。", error);
		}


		[TestMethod]
		public void Test_RsaHelper_Sign_CertNotFound()
		{
			byte[] bb = Guid.NewGuid().ToByteArray();

			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				RsaHelper.Sign(bb, "abc");
			});

			Assert.AreEqual($"加密证书abc不存在。", error);
		}


		[TestMethod]
		public void Test_RsaHelper_Sign_Verify()
		{
			string s = @"
大家好：
为进一步规范ERP发版工作，加强研发质量管控，在2017年初，我们对ERP产品发版管理制度V1.1进行了优化，详细调整点如下：
1、结合公司《ERP产品全生命周期管控策略V4.0》，将孵化期产品和成长&成熟期产品的发版活动进行区分细化
2、新增定义《ERP研发质量标准v1.0》
3、结合公司《产品PMO管理机制v1.0》，修订《产品上市评估表》形成2017年版本
4、针对主项目和子项目协作问题，梳理形成《ERP发版-主项目工作清单v1.0》

经ERP353和云ERP1.0SP3发版前工作的试运行、各事业部核心角色的评审，最终经产品PMO的审核，现正式发布ERP产品发版管理制度V2.0，即日生效！
   
如对本管理制度有任何疑问，欢迎咨询各团队QA";

			byte[] data = s.GetBytes();
			string signature = RsaHelper.Sign(data, s_certName);
			bool ok = RsaHelper.Verify(data, signature, s_publicKey);
			Assert.IsTrue(ok);

			// 随便改一下签名
			string signX = "abc" + signature.Substring(3);
			ok = RsaHelper.Verify(data, signX, s_publicKey);
			Assert.IsFalse(ok);


			// 随便修改下原始输入数据
			string s2 = "abc" + s;
			byte[] data2 = s2.GetBytes();
			string sign2 = RsaHelper.Sign(data2, s_certName);

			// 数据修改了，签名就肯定不一样
			Assert.AreNotEqual(signature, sign2);

		}
	}
}
