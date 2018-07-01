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



		private static readonly string s_certSubject = "FishLi-TEST";


		private static readonly string s_publicKey = @"
-----BEGIN CERTIFICATE-----
MIIDAzCCAeugAwIBAgIQtsXW57hUc51DXvrKaqCo8DANBgkqhkiG9w0BAQ0FADAW
MRQwEgYDVQQDEwtGaXNoTGktVEVTVDAgFw0wNDEyMzExNjAwMDBaGA8yMDk5MTIz
MTE2MDAwMFowFjEUMBIGA1UEAxMLRmlzaExpLVRFU1QwggEiMA0GCSqGSIb3DQEB
AQUAA4IBDwAwggEKAoIBAQDL2rhj11xRaBCq6r4pwpuclrU+P5etr57Yn4qsdMP6
OtXMqyA1AgJtQv6s1uDGDSd8B6s9xPxcvTdMftrN6lTnuAFNZOQ1fUfY7kJ4DI95
3ro2rH01j5LXa58qt+bIizLAe9nqFaJBSm12tdFWgclKqqLoKlWMCPCzdD2widQj
nVqIB1OCXA6FNO2OLFj5bHl+HAn2Gt/6yiheCyMM92bsb1o1AWq7hcxXlz15PQjK
VlW7ictQn73wOIQpvvjIPQcPAlyJlp5FGzSbWdC2Ra1g8IfoixR/KuYrCcYf7cGB
hdHL8i2B/j4HXNUYUvwgJCERhxR6DCI4R2yPruBaFQErAgMBAAGjSzBJMEcGA1Ud
AQRAMD6AEJQzFhARMcELtfhoqpaR7VihGDAWMRQwEgYDVQQDEwtGaXNoTGktVEVT
VIIQtsXW57hUc51DXvrKaqCo8DANBgkqhkiG9w0BAQ0FAAOCAQEAmE7KWlQoJJDO
RLsyVaLwxFEiyiB/EQrnaUlwe86rGBx7SnUb5U+wQXXDHPWj3V1iEZddknBb2hGh
KyQs8UOo6LdIz7B6heiAdfQ5O6395Qvr+MAqrHU2TnJ7gNGVkc/QEceOIFE/enBe
uRhUTXoDXYUS0gNa1Yxloq6L2B19UEI3o2I6vWC4wwcIqDf9DPWDl1n15cnF/5+m
gX7P38Ynnn+QPUrqSIjFWTQOKgrFqUVSKJpc37G4kjSCkTT+0NGckP1Gefx/Auvf
9Qv/bhHGaR3SAEttViHrFIeelJZT9jeTiDfzO72r2gfY2fqh2oUoWkZXdKkIGcwK
es3Iz6Lajw==
-----END CERTIFICATE-----
";


		[TestMethod]
		public void Test_RsaHelper_Encrypt_Decrypt()
		{
			string s = @"已评估过类似方案，上海和融25版本做过类似改动，但需求有差异。
和融做此需求时改动量比较大，且与252版本跨系列了，参考价值降低。";
			byte[] bb = Encoding.UTF8.GetBytes(s);

			byte[] b1 = RsaHelper.Encrypt(bb, s_certSubject);
			byte[] b2 = RsaHelper.Decrypt(b1, s_certSubject);

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

			Assert.AreEqual($"不能根据指定的证书主题 abc 找到匹配的证书。", error);
		}

		[TestMethod]
		public void Test_RsaHelper_Decrypt_CertNotFound()
		{
			byte[] bb = Guid.NewGuid().ToByteArray();

			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				RsaHelper.Decrypt(bb, "abc");
			});

			Assert.AreEqual($"不能根据指定的证书主题 abc 找到匹配的证书。", error);
		}


		[TestMethod]
		public void Test_RsaHelper_Sign_CertNotFound()
		{
			byte[] bb = Guid.NewGuid().ToByteArray();

			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				RsaHelper.Sign(bb, "abc");
			});

			Assert.AreEqual($"不能根据指定的证书主题 abc 找到匹配的证书。", error);
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
			string signature = RsaHelper.Sign(data, s_certSubject);
			bool ok = RsaHelper.Verify(data, signature, s_publicKey);
			Assert.IsTrue(ok);

			// 随便改一下签名
			string signX = "abc" + signature.Substring(3);
			ok = RsaHelper.Verify(data, signX, s_publicKey);
			Assert.IsFalse(ok);


			// 随便修改下原始输入数据
			string s2 = "abc" + s;
			byte[] data2 = s2.GetBytes();
			string sign2 = RsaHelper.Sign(data2, s_certSubject);

			// 数据修改了，签名就肯定不一样
			Assert.AreNotEqual(signature, sign2);

		}
	}
}
