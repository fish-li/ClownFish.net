using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class HashHelperTest
	{
		[TestMethod]
		public void Test_Sha1()
		{
			Assert.AreEqual("A6DCC78B685D0CEA701CA90A948B9295F3685FDF", 
				HashHelper.Sha1("Fish Li"));

			Assert.AreEqual("DA39A3EE5E6B4B0D3255BFEF95601890AFD80709",
				HashHelper.Sha1(""));

			Assert.AreEqual("3E5C799844C4366172D633004997D18F39C6487A",
				HashHelper.Sha1("如对本管理制度有任何疑问"));
		}


		[TestMethod]
		public void Test_Md5()
		{
			Assert.AreEqual("44D2D9635ED5CDEA2A858CD7A1CC2B0C",
				HashHelper.Md5("Fish Li"));

			Assert.AreEqual("D41D8CD98F00B204E9800998ECF8427E",
				HashHelper.Md5(""));

			Assert.AreEqual("ABCC7373EDFBF009D688B65AC2DBC1CD",
				HashHelper.Md5("如对本管理制度有任何疑问"));
		}
	}
}
