using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Cryptography
{
	[TestClass]
	public class HashHelperTest
	{
        private static readonly string s_filePath = Path.Combine(AppDomain.CurrentDomain.GetTempPath(), "Test_HashHelper.txt");

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            RetryFile.WriteAllText(s_filePath, "禁止使用 ViewState，Session", Encoding.UTF8);
        }


        //[TestMethod]
        //public void Test_FastHash()
        //{
        //    string md5 = null;
        //    md5 = HashHelper.FastHash(@"ClownFish.net.dll");
        //    Console.WriteLine(md5);

        //    md5 = HashHelper.FastHash(@"ClownFish.net.pdb");
        //    Console.WriteLine(md5);

        //    md5 = HashHelper.FastHash(@"ClownFish.net.xml");
        //    Console.WriteLine(md5);

        //    // 这个方法没法断言
        //}

		[TestMethod]
		public void Test_Md5()
		{
			Assert.AreEqual("44D2D9635ED5CDEA2A858CD7A1CC2B0C",
				HashHelper.Md5("Fish Li"));

			Assert.AreEqual("D41D8CD98F00B204E9800998ECF8427E",
				HashHelper.Md5(""));

			Assert.AreEqual("ABCC7373EDFBF009D688B65AC2DBC1CD",
				HashHelper.Md5("如对本管理制度有任何疑问"));

			Assert.AreEqual("DBB688630998654022EEC8C216CD96B0",
				HashHelper.FileMD5(s_filePath));
		}


		[TestMethod]
		public void Test_Sha1()
		{
			Assert.AreEqual("A6DCC78B685D0CEA701CA90A948B9295F3685FDF", 
				HashHelper.Sha1("Fish Li"));

			Assert.AreEqual("DA39A3EE5E6B4B0D3255BFEF95601890AFD80709",
				HashHelper.Sha1(""));

			Assert.AreEqual("3E5C799844C4366172D633004997D18F39C6487A",
				HashHelper.Sha1("如对本管理制度有任何疑问"));

            Assert.AreEqual("3A65DC6A22A9B53E1D79940BEAFB84629C3EE1F9",
                HashHelper.FileSha1(s_filePath));
		}


		[TestMethod]
		public void Test_Sha_256()
		{
			Assert.AreEqual("2ED3899D70C0F8479BB345735242930CDCAC0B79FD8573C3E62A6C511C180293",
				HashHelper.Sha256("Fish Li"));

			Assert.AreEqual("E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855",
				HashHelper.Sha256(""));

			Assert.AreEqual("D7FDA1B7CC33BF2DACB443B4FB67201F0DC866B392DD1AB31754A68FCF12D241",
				HashHelper.Sha256("如对本管理制度有任何疑问"));
		}

		[TestMethod]
		public void Test_Sha_512()
		{
			Assert.AreEqual("59CF151BD2F29475AB5495DFE78722C673626D3F507E31FF9F6B5C5396B9AA8F2E90D5F5C79419940316B7C93077B7B3F6549A22901FB38F1196929B5D74C549",
				HashHelper.Sha512("Fish Li"));

			Assert.AreEqual("CF83E1357EEFB8BDF1542850D66D8007D620E4050B5715DC83F4A921D36CE9CE47D0D13C5D85F2B0FF8318D2877EEC2F63B931BD47417A81A538327AF927DA3E",
				HashHelper.Sha512(""));

			Assert.AreEqual("6C61D6DA178EC59C54B5BC205B754F57F8A407C301DA1FDB430B55E44160370C8E9BCB1B79F23F9BAB787D7AD14A6EE30CFDB4B8895A46393274C420F1B872A4",
				HashHelper.Sha512("如对本管理制度有任何疑问"));
		}


        [TestMethod]
        public void Test_HMACSHA256()
        {
            string key = "xxxxxxxxxxxxxxx";

            Assert.AreEqual("/T9hy6huTWbatP5ze/O898F9zqAsEeCHOK6w84C8t4Y=",
                HashHelper.HMACSHA256(key, "Fish Li"));

            Assert.AreEqual("Fa6zQKnyVlAqgtv8euVQE1MyEd2wzd1JNjhMt+bAcn8=",
                HashHelper.HMACSHA256(key, ""));

            Assert.AreEqual("iNXwQy6IDT8EUFpoclqhXgdfWkwcCC4jpyeNYnafeQ0=",
                HashHelper.HMACSHA256(key, "如对本管理制度有任何疑问"));
        }


        [TestMethod]
        public void Test_HMACSHA256_byte()
        {
            byte[] key = "xxxxxxxxxxxxxxx".ToUtf8Bytes();

            Assert.AreEqual("/T9hy6huTWbatP5ze/O898F9zqAsEeCHOK6w84C8t4Y=",
                HashHelper.HMACSHA256(key, "Fish Li".ToUtf8Bytes()).ToBase64());

            Assert.AreEqual("Fa6zQKnyVlAqgtv8euVQE1MyEd2wzd1JNjhMt+bAcn8=",
                HashHelper.HMACSHA256(key, "".ToUtf8Bytes()).ToBase64());

            Assert.AreEqual("iNXwQy6IDT8EUFpoclqhXgdfWkwcCC4jpyeNYnafeQ0=",
                HashHelper.HMACSHA256(key, "如对本管理制度有任何疑问".ToUtf8Bytes()).ToBase64());
        }


        [TestMethod]
        public void Test_HMACSHA512()
        {
            string key = "xxxxxxxxxxxxxxx";

            Assert.AreEqual("VeCv0LcRsgfhdG7EqBt35DBNQ6Ve9dDJ6ui6v5ALtdNb1p9omrMVcyMRnnAx3rFg0iWp5nOYWTNrpRySOMgo7Q==",
                HashHelper.HMACSHA512(key, "Fish Li"));

            Assert.AreEqual("sPU+26PRD58SOeP1PblYWMuGhAQvw3Ngv9QSr5QOWH1KAVLASUfTIJxXqq0UJsf+bVhNW3490kUX4C89BoBDlg==",
                HashHelper.HMACSHA512(key, ""));

            Assert.AreEqual("pD9hTM+qWMBgjJVQ93043Wz5rem0C0GhGBAVgLRzKXtTSQSjc4xRI88aX7a3COfpnlSlfy/PM9othtVNXHIXVA==",
                HashHelper.HMACSHA512(key, "如对本管理制度有任何疑问"));
        }


        [TestMethod]
        public void Test_HMACSHA512_byte()
        {
            byte[] key = "xxxxxxxxxxxxxxx".ToUtf8Bytes();

            Assert.AreEqual("VeCv0LcRsgfhdG7EqBt35DBNQ6Ve9dDJ6ui6v5ALtdNb1p9omrMVcyMRnnAx3rFg0iWp5nOYWTNrpRySOMgo7Q==",
                HashHelper.HMACSHA512(key, "Fish Li".ToUtf8Bytes()).ToBase64());

            Assert.AreEqual("sPU+26PRD58SOeP1PblYWMuGhAQvw3Ngv9QSr5QOWH1KAVLASUfTIJxXqq0UJsf+bVhNW3490kUX4C89BoBDlg==",
                HashHelper.HMACSHA512(key, "".ToUtf8Bytes()).ToBase64());

            Assert.AreEqual("pD9hTM+qWMBgjJVQ93043Wz5rem0C0GhGBAVgLRzKXtTSQSjc4xRI88aX7a3COfpnlSlfy/PM9othtVNXHIXVA==",
                HashHelper.HMACSHA512(key, "如对本管理制度有任何疑问".ToUtf8Bytes()).ToBase64());
        }



        [TestMethod]
		public void Test_Error()
        {
			MyAssert.IsError<ArgumentNullException>(()=> {
				_ = HashHelper.Sha1(null);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				_ = HashHelper.FileSha1(null);
			});
			MyAssert.IsError<FileNotFoundException>(() => {
				_ = HashHelper.FileSha1("filePath");
			});
		}


	}
}
