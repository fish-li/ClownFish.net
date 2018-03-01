using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class StringExtensionsTest
	{
		[TestMethod]
		public void Test_EqualsIgnoreCase()
		{
			string s1 = "abc";
			string s2 = "aBc";
			Assert.IsTrue(s1.EqualsIgnoreCase(s2));
            Assert.IsTrue(s1.Is(s2));
        }


		[TestMethod]
		public void Test_EndsWithIgnoreCase()
		{
			string s1 = "abc";
			string s2 = "aBC";
			Assert.IsTrue(s1.EndsWithIgnoreCase(s2));
		}

		[TestMethod]
		public void Test_StartsWithIgnoreCase()
		{
			string s1 = "abc";
			string s2 = "Abc";
			Assert.IsTrue(s1.StartsWithIgnoreCase(s2));
		}


		[TestMethod]
		public void Test_IndexOfIgnoreCase()
		{
			string s1 = "abcdefg";
			string s2 = "BCD";
			Assert.AreEqual(1, s1.IndexOfIgnoreCase(s2));
		}


		[TestMethod]
		public void Test_SplitTrim()
		{
			string s = " aa " + Environment.NewLine
						+ " bb " + Environment.NewLine;

			string[] s2 = s.SplitTrim('\r', '\n');
			Assert.AreEqual(2, s2.Length);
			Assert.AreEqual("aa", s2[0]);
			Assert.AreEqual("bb", s2[1]);
		}

		[TestMethod]
		public void Test_SplitString()
		{
			List<NameValue> list = ClownFish.Base.StringExtensions.SplitString("a=1;b=2;c=3;d=4;", ';', '=');
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual("a", list[0].Name);
			Assert.AreEqual("1", list[0].Value);
			Assert.AreEqual("d", list[3].Name);
			Assert.AreEqual("4", list[3].Value);
		}


		[TestMethod]
		public void Test_ToTitleCase()
		{
			string s = "abc";
			Assert.AreEqual("Abc", s.ToTitleCase());
		}

		[TestMethod]
		public void Test_KeepLength()
		{
			string s1 = "Microsoft.VisualStudio.TestTools.UnitTesting;";
			Assert.AreEqual("Microsoft.VisualStudio...45", s1.KeepLength(22));

			string s2 = "12345";
			Assert.AreEqual(s2, s2.KeepLength(5));
			Assert.AreEqual(s2, s2.KeepLength(6));
		}

		[TestMethod]
		public void Test_GetBytes()
		{
			string s = "为进一步规范ERP发版工作，加强研发质量管控，在2017年初，我们对ERP产品发版管理制度V1.1进行了优化，详细调整点如下：";
			byte[] b1 = Encoding.UTF8.GetBytes(s);
			byte[] b2 = s.GetBytes();
			Assert.IsTrue(b1.IsEqual(b2));
		}


        [TestMethod]
        public void Test_SubstringN()
        {
            string text = "0123456789";

            string result = text.SubstringN(3);

            Assert.AreEqual("012...10", result);

            string result2 = text.SubstringN(20);
            Assert.AreEqual(text, result2);


            string t2 = null;
            Assert.IsNull(t2.SubstringN(5));

            t2 = "";
            Assert.AreEqual("", t2.SubstringN(3));

        }
    }
}
