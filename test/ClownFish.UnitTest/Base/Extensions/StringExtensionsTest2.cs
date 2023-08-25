using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class StringExtensionsTest2
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
        public void Test_NullString()
        {
            string nullstring = null;

            Assert.IsFalse(nullstring.EndsWithIgnoreCase("aa"));
            Assert.IsFalse(nullstring.EndsWith0("aa"));
            Assert.IsFalse(nullstring.StartsWithIgnoreCase("aa"));
            Assert.IsFalse(nullstring.StartsWith0("aa"));
            Assert.AreEqual(-1, nullstring.IndexOfIgnoreCase("aa"));
            Assert.AreEqual(-1,nullstring.IndexOf0("aa"));
        }


        [TestMethod]
        public void Test_EndsWith()
        {
            Assert.IsTrue("abc".EndsWithIgnoreCase("abc"));
            Assert.IsTrue("abc".EndsWithIgnoreCase("aBC"));
            Assert.IsTrue("1abc".EndsWithIgnoreCase("abc"));
            Assert.IsTrue("1abc".EndsWithIgnoreCase("aBC"));

            Assert.IsTrue("abc".EndsWith0("abc"));
            Assert.IsFalse("abc".EndsWith0("aBC"));
            Assert.IsTrue("1abc".EndsWith0("abc"));
            Assert.IsFalse("1abc".EndsWith0("aBC"));
        }

        [TestMethod]
        public void Test_StartsWith()
        {
            Assert.IsTrue("abc".StartsWithIgnoreCase("abc"));
            Assert.IsTrue("abc".StartsWithIgnoreCase("Abc"));
            Assert.IsTrue("abc1".StartsWithIgnoreCase("abc"));
            Assert.IsTrue("abc1".StartsWithIgnoreCase("Abc"));

            Assert.IsTrue("abc".StartsWith0("abc"));
            Assert.IsFalse("abc".StartsWith0("Abc"));
            Assert.IsTrue("abc1".StartsWith0("abc"));
            Assert.IsFalse("abc1".StartsWith0("Abc"));
        }


        [TestMethod]
        public void Test_IndexOf()
        {
            Assert.AreEqual(1, "abcdefg".IndexOfIgnoreCase("bcd"));
            Assert.AreEqual(1, "abcdefg".IndexOfIgnoreCase("BCD"));

            Assert.AreEqual(1, "abcdefg".IndexOf0("bcd"));
            Assert.AreEqual(-1, "abcdefg".IndexOf0("BCD"));
            Assert.AreEqual(-1, "".IndexOf0("BCD"));
        }


        [TestMethod]
        public void Test_SplitTrim()
        {
            string s = " aa " + Environment.NewLine
                        + " bb " + Environment.NewLine;

            string[] s2 = s.ToArray('\r', '\n');
            Assert.AreEqual(2, s2.Length);
            Assert.AreEqual("aa", s2[0]);
            Assert.AreEqual("bb", s2[1]);
        }

        [TestMethod]
        public void Test_SplitString()
        {
            List<NameValue> list = "a=1;b=2;c=3;d=4;".ToKVList(';', '=');
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

        [TestMethod]
        public void Test_ToList()
        {
            string s1 = "111111111,222222222,";
            List<string> list = s1.ToList(',');

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("111111111", list[0]);
            Assert.AreEqual("222222222", list[1]);



            string s2 = "111111111;222222222,333333333;";
            List<string> list2 = s2.ToList2();

            Assert.AreEqual(3, list2.Count);
            Assert.AreEqual("111111111", list2[0]);
            Assert.AreEqual("222222222", list2[1]);
            Assert.AreEqual("333333333", list2[2]);
        }


        [TestMethod]
        public void Test_ToLines()
        {
            string s1 = "111111111\n222222222\r";
            string[] list = s1.ToLines();

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual("111111111", list[0]);
            Assert.AreEqual("222222222", list[1]);



            string s2 = "111111111\n222222222\r\n333333333\r";
            string[] list2 = s2.ToLines();

            Assert.AreEqual(3, list2.Length);
            Assert.AreEqual("111111111", list2[0]);
            Assert.AreEqual("222222222", list2[1]);
            Assert.AreEqual("333333333", list2[2]);
        }

    }
}
