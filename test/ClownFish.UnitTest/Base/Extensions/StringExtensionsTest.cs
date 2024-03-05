using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;
using System.Collections.Specialized;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void Test_IsNullOrEmpty()
        {
            string s1 = null;
            string s2 = "";
            string s3 = "abc";

            Assert.IsTrue(s1.IsNullOrEmpty());
            Assert.IsTrue(s2.IsNullOrEmpty());
            Assert.IsFalse(s3.IsNullOrEmpty());
        }

        [TestMethod]
        public void Test_EqualsIgnoreCase()
        {
            Assert.IsTrue("aa".EqualsIgnoreCase("Aa"));
            Assert.IsFalse("aa".EqualsIgnoreCase("Aaa"));

            Assert.IsFalse("aa".EqualsIgnoreCase(""));
            Assert.IsFalse("aa".EqualsIgnoreCase(null));

            string nullstring = null;
            Assert.IsFalse(nullstring.EqualsIgnoreCase("xx"));

            string emptystring = string.Empty;
            Assert.IsFalse(emptystring.EqualsIgnoreCase("xx"));
        }

        [TestMethod]
        public void Test_is()
        {
            Assert.IsTrue("aa".Is("Aa"));
            Assert.IsFalse("aa".Is("Aaa"));

            Assert.IsFalse("aa".Is(""));
            Assert.IsFalse("aa".Is(null));

            string nullstring = null;
            Assert.IsFalse(nullstring.Is("xx"));

            string emptystring = string.Empty;
            Assert.IsFalse(emptystring.Is("xx"));
        }

        [TestMethod]
        public void Test_EndsWithIgnoreCase()
        {
            Assert.IsTrue("AAa".EndsWithIgnoreCase("Aa"));
            Assert.IsTrue("AAa".EndsWithIgnoreCase("aa"));

            Assert.IsFalse("AAa".EndsWithIgnoreCase("Ab"));
            Assert.IsFalse("AAa".EndsWithIgnoreCase("ab"));

            string emptystring = string.Empty;
            Assert.IsFalse(emptystring.EndsWithIgnoreCase("xx"));
        }


        [TestMethod]
        public void Test_EndsWith1()
        {
            Assert.IsTrue("AAa".EndsWith1("Aa"));
            Assert.IsTrue("AAa".EndsWith1("aa"));

            Assert.IsFalse("AAa".EndsWith1("Ab"));
            Assert.IsFalse("AAa".EndsWith1("ab"));

            string emptystring = string.Empty;
            Assert.IsFalse(emptystring.EndsWith1("xx"));
        }

        [TestMethod]
        public void Test_StartsWithIgnoreCase()
        {
            Assert.IsTrue("AAa".StartsWithIgnoreCase("Aa"));
            Assert.IsTrue("AAa".StartsWithIgnoreCase("aa"));

            Assert.IsFalse("AAa".StartsWithIgnoreCase("Ba"));
            Assert.IsFalse("AAa".StartsWithIgnoreCase("ba"));

            string emptystring = string.Empty;
            Assert.IsFalse(emptystring.StartsWithIgnoreCase("xx"));
        }

        [TestMethod]
        public void Test_StartsWith1()
        {
            Assert.IsTrue("AAa".StartsWith1("Aa"));
            Assert.IsTrue("AAa".StartsWith1("aa"));

            Assert.IsFalse("AAa".StartsWith1("Ba"));
            Assert.IsFalse("AAa".StartsWith1("ba"));

            string emptystring = string.Empty;
            Assert.IsFalse(emptystring.StartsWith1("xx"));
        }

#if NETFRAMEWORK
        [TestMethod]
        public void Test_StartsWith_char()
        {
            Assert.IsTrue(StringExtensions.StartsWith("a", 'a'));
            Assert.IsFalse(StringExtensions.StartsWith("A", 'a'));

            Assert.IsTrue(StringExtensions.StartsWith("AAa", 'A'));

            Assert.IsFalse(StringExtensions.StartsWith("AAa", 'a'));
            Assert.IsFalse(StringExtensions.StartsWith("AAa", 'b'));
            Assert.IsFalse(StringExtensions.StartsWith("AAa", ' '));


            string nullstring = null;
            Assert.IsFalse(StringExtensions.StartsWith(nullstring, 'c'));

            string emptystring = string.Empty;
            Assert.IsFalse(StringExtensions.EndsWith(emptystring, 'a'));
        }

        [TestMethod]
        public void Test_EndsWith_char()
        {
            Assert.IsTrue(StringExtensions.EndsWith("a", 'a'));
            Assert.IsFalse(StringExtensions.EndsWith("A", 'a'));

            Assert.IsTrue(StringExtensions.EndsWith("AAa", 'a'));

            Assert.IsFalse(StringExtensions.EndsWith("AAa", 'A'));
            Assert.IsFalse(StringExtensions.EndsWith("AAa", 'b'));
            Assert.IsFalse(StringExtensions.EndsWith("AAa", ' '));


            string nullstring = null;
            Assert.IsFalse(StringExtensions.EndsWith(nullstring, 'c'));

            string emptystring = string.Empty;
            Assert.IsFalse(StringExtensions.EndsWith(emptystring, 'a'));
        }
#endif


        [TestMethod]
        public void Test_IndexOfIgnoreCase()
        {
            Assert.IsTrue("AAa".IndexOfIgnoreCase("Aa") == 0);
            Assert.IsTrue("AAa".IndexOfIgnoreCase("aa") == 0);

            Assert.IsTrue("AAa".IndexOfIgnoreCase("Ba") == -1);
            Assert.IsTrue("AAba".IndexOfIgnoreCase("ba") == 2);


            string emptystring = string.Empty;
            Assert.IsTrue(emptystring.IndexOfIgnoreCase("Aa") == -1);
        }

        [TestMethod]
        public void Test_IndexOf1()
        {
            Assert.IsTrue("AAa".IndexOf1("Aa") == 0);
            Assert.IsTrue("AAa".IndexOf1("aa") == 0);

            Assert.IsTrue("AAa".IndexOf1("Ba") == -1);
            Assert.IsTrue("AAba".IndexOf1("ba") == 2);


            string emptystring = string.Empty;
            Assert.IsTrue(emptystring.IndexOf1("Aa") == -1);
        }


        [TestMethod]
        public void Test_ToArray()
        {
            string s1 = "aaa\r\nbbb\r\nccc\r\n";
            string[] s1Array = s1.ToArray('\r', '\n');

            Assert.AreEqual(3, s1Array.Length);
            Assert.AreEqual("aaa", s1Array[0]);
            Assert.AreEqual("bbb", s1Array[1]);
            Assert.AreEqual("ccc", s1Array[2]);


            string s2 = "aaa\nbbb\nccc\n";
            string[] s2Array = s2.ToArray('\n');

            Assert.AreEqual(3, s2Array.Length);
            Assert.AreEqual("aaa", s2Array[0]);
            Assert.AreEqual("bbb", s2Array[1]);
            Assert.AreEqual("ccc", s2Array[2]);


            string s3 = "aaa,bbb;ccc;";
            string[] s3Array = s3.ToArray2();

            Assert.AreEqual(3, s3Array.Length);
            Assert.AreEqual("aaa", s3Array[0]);
            Assert.AreEqual("bbb", s3Array[1]);
            Assert.AreEqual("ccc", s3Array[2]);


            MyAssert.IsError<ArgumentNullException>(()=> {
                _ = "xx".ToArray();
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = "xx".ToArray(Empty.Array<char>());
            });

            string[] array1 = "".ToArray('c');
            Assert.IsNotNull(array1);
            Assert.AreEqual(0, array1.Length);

            string[] array2 = ((string)null).ToArray('c');
            Assert.IsNotNull(array2);
            Assert.AreEqual(0, array2.Length);
        }


        [TestMethod]
        public void Test_ToLines()
        {
            string s1 = "aaa\r\nbbb\r\nccc\r\n";
            string[] array1 = s1.ToLines();
            Assert.AreEqual(3, array1.Length);
            Assert.AreEqual("aaa", array1[0]);
            Assert.AreEqual("bbb", array1[1]);
            Assert.AreEqual("ccc", array1[2]);

            string[] array2 = "".ToLines();
            Assert.IsNotNull(array2);
            Assert.AreEqual(0, array2.Length);

            string[] array3 = ((string)null).ToLines();
            Assert.IsNotNull(array3);
            Assert.AreEqual(0, array3.Length);
        }

        [TestMethod]
        public void Test_ToList()
        {
            string s1 = "aaa\r\nbbb\r\nccc\r\n";
            List<string> s1Array = s1.ToList('\r', '\n');

            Assert.AreEqual(3, s1Array.Count);
            Assert.AreEqual("aaa", s1Array[0]);
            Assert.AreEqual("bbb", s1Array[1]);
            Assert.AreEqual("ccc", s1Array[2]);


            string s2 = "aaa\nbbb\nccc\n";
            List<string> s2Array = s2.ToList('\n');

            Assert.AreEqual(3, s2Array.Count);
            Assert.AreEqual("aaa", s2Array[0]);
            Assert.AreEqual("bbb", s2Array[1]);
            Assert.AreEqual("ccc", s2Array[2]);


            string s3 = "aaa,bbb;ccc;";
            List<string> s3Array = s3.ToList2();

            Assert.AreEqual(3, s3Array.Count);
            Assert.AreEqual("aaa", s3Array[0]);
            Assert.AreEqual("bbb", s3Array[1]);
            Assert.AreEqual("ccc", s3Array[2]);

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = "xx".ToList();
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = "xx".ToList(Empty.Array<char>());
            });

            List<string> list1 = "".ToList('c');
            Assert.IsNotNull(list1);
            Assert.AreEqual(0, list1.Count);

            List<string> list2 = ((string)null).ToList('c');
            Assert.IsNotNull(list2);
            Assert.AreEqual(0, list2.Count);
        }

        [TestMethod]
        public void Test_ToKVList()
        {
            var list = "a=1;b=2;c=3;d=4;".ToKVList(';', '=');
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("a", list[0].Name);
            Assert.AreEqual("1", list[0].Value);

            Assert.AreEqual("b", list[1].Name);
            Assert.AreEqual("2", list[1].Value);

            Assert.AreEqual("c", list[2].Name);
            Assert.AreEqual("3", list[2].Value);

            Assert.AreEqual("d", list[3].Name);
            Assert.AreEqual("4", list[3].Value);

            var list2 = StringExtensions.ToKVList(null, ',', '=');
            Assert.AreEqual(0, list2.Count);

            var list3 = StringExtensions.ToKVList(string.Empty, ',', '=');
            Assert.AreEqual(0, list3.Count);


            MyAssert.IsError<ArgumentException>(() => {
                _ = StringExtensions.ToKVList("aaa,bbb", ',', '=');
            });

            var list4 = "a=1;b=2;c=3;d=4;".ToKVList(new char[0], '=');
            Assert.AreEqual(0, list4.Count);
        }


        [TestMethod]
        public void Test_ToTitleCase()
        {
            string s1 = "one";
            Assert.AreEqual("One", s1.ToTitleCase());

            string s2 = "ONE";
            Assert.AreEqual(s2, s2.ToTitleCase());

            string s3 = "Aa Bb,Cc";
            Assert.AreEqual(s3, s3.ToTitleCase());

            Assert.IsNull(StringExtensions.ToTitleCase(null));
            Assert.AreEqual("", StringExtensions.ToTitleCase(""));
            Assert.AreEqual("a", StringExtensions.ToTitleCase("a"));
        }

        [TestMethod]
        public void Test_GetBytes()
        {
            string s1 = "将字符串转成byte[]，等效于：Encoding.UTF8.GetBytes(text);";

            byte[] b1 = s1.GetBytes();
            byte[] b2 = Encoding.UTF8.GetBytes(s1);

            Assert.IsTrue(b1.IsEqual(b2));

            Assert.IsNull(((string)null).GetBytes());
        }

        [TestMethod]
        public void Test_SubstringN()
        {
            string s1 = new string('x', 125);
            Assert.AreEqual("xxxxx...125", s1.SubstringN(5));

            Assert.AreEqual(s1, s1.SubstringN(125));
            Assert.AreEqual(s1, s1.SubstringN(135));
        }

        [TestMethod]
        public void Test_ParseLength()
        {
            Assert.AreEqual(0, (null as string).ParseLength());
            Assert.AreEqual(0, "".ParseLength());

            Assert.AreEqual(10L, "10".ParseLength());
            Assert.AreEqual(1248564540L, "1248564540".ParseLength());

            Assert.AreEqual(11L * 1024, "11K".ParseLength());
            Assert.AreEqual(11L * 1024, "11KB".ParseLength());

            Assert.AreEqual(12L * 1024 * 1024, "12M".ParseLength());
            Assert.AreEqual(12L * 1024 * 1024, "12MB".ParseLength());

            Assert.AreEqual(13L * 1024 * 1024 * 1024, "13G".ParseLength());
            Assert.AreEqual(13L * 1024 * 1024 * 1024, "13GB".ParseLength());

            Assert.AreEqual(13L * 1024 * 1024 * 1024 * 1024, "13T".ParseLength());
            Assert.AreEqual(13L * 1024 * 1024 * 1024 * 1024, "13TB".ParseLength());

            Assert.AreEqual(15L * 1024 * 1024 * 1024 * 1024 * 1024, "15P".ParseLength());
            Assert.AreEqual(15L * 1024 * 1024 * 1024 * 1024 * 1024, "15PB".ParseLength());


            MyAssert.IsError<ArgumentException>(() => {
                _ = "xx".ParseLength();
            });
            MyAssert.IsError<ArgumentException>(() => {
                _ = "10xx".ParseLength();
            });
            MyAssert.IsError<ArgumentException>(() => {
                _ = "10MBxx".ParseLength();
            });
            MyAssert.IsError<ArgumentException>(() => {
                _ = "-10MB".ParseLength();
            });
        }


        [TestMethod]
        public void Test_TryToInt()
        {
            Assert.AreEqual(0, "".TryToInt());
            Assert.AreEqual(0, ((string)null).TryToInt());

            Assert.AreEqual(0, "xx".TryToInt());
            Assert.AreEqual(9, "xx".TryToInt(9));
            Assert.AreEqual(0, "99999999999999999999999999999999999999999".TryToInt());
            Assert.AreEqual(9, "99999999999999999999999999999999999999999".TryToInt(9));

            Assert.AreEqual(0, "0".TryToInt());
            Assert.AreEqual(12, "12".TryToInt());
            Assert.AreEqual(-12, "-12".TryToInt());
        }

        [TestMethod]
        public void Test_TryToUInt()
        {
            Assert.AreEqual(0, "".TryToUInt());
            Assert.AreEqual(0, ((string)null).TryToUInt());

            Assert.AreEqual(0, "xx".TryToUInt());
            Assert.AreEqual(9, "xx".TryToUInt(9));
            Assert.AreEqual(0, "99999999999999999999999999999999999999999".TryToUInt());
            Assert.AreEqual(9, "99999999999999999999999999999999999999999".TryToUInt(9));

            Assert.AreEqual(0, "0".TryToUInt());
            Assert.AreEqual(12, "12".TryToUInt());
            Assert.AreEqual(0, "-12".TryToUInt());  // 注意这里的差异
        }

        [TestMethod]
        public void Test_TryToLong()
        {
            Assert.AreEqual(0, "".TryToLong());
            Assert.AreEqual(0, ((string)null).TryToLong());

            Assert.AreEqual(0, "xx".TryToLong());
            Assert.AreEqual(0, "99999999999999999999999999999999999999999999999999999999999999999999999".TryToLong());

            Assert.AreEqual(0, "0".TryToLong());
            Assert.AreEqual(12, "12".TryToLong());
            Assert.AreEqual(-12, "-12".TryToLong());
        }

        [TestMethod]
        public void Test_TryToDouble()
        {
            Assert.AreEqual(0d, "".TryToDouble());
            Assert.AreEqual(0d, ((string)null).TryToDouble());

            Assert.AreEqual(0d, "xx".TryToDouble());

            Assert.AreEqual(0d, "0".TryToDouble());
            Assert.AreEqual(12.34d, "12.34".TryToDouble());
            Assert.AreEqual(-12.34d, "-12.34".TryToDouble());
        }

        [TestMethod]
        public void Test_TryToBool()
        {
            Assert.AreEqual(true, "".TryToBool(true));
            Assert.AreEqual(true, ((string)null).TryToBool(true));

            Assert.AreEqual(true, "1".TryToBool(false));
            Assert.AreEqual(true, "true".TryToBool(false));
            Assert.AreEqual(true, "TRue".TryToBool(false));
            Assert.AreEqual(true, "TRUE".TryToBool(false));


            Assert.AreEqual(false, "".TryToBool(false));
            Assert.AreEqual(false, ((string)null).TryToBool(false));

            Assert.AreEqual(false, "xx".TryToBool(true));
        }

        [TestMethod]
        public void Test_IfEmpty()
        {
            Assert.AreEqual("xx", "".IfEmpty("xx"));
            Assert.AreEqual("xx", ((string)null).IfEmpty("xx"));

            Assert.AreEqual("123", "123".IfEmpty("xx"));
        }


        [TestMethod]
        public void Test_IfEmptyThrow()
        {
            string text = "xx";
            Assert.AreEqual("xx", text.IfEmptyThrow(nameof(text)));

            MyAssert.IsError<ArgumentNullException>(() => {
                string s1 = "";
                _ = s1.IfEmptyThrow(nameof(s1));
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                string s2 = null;
                _ = s2.IfEmptyThrow(nameof(s2));
            });
        }

        [TestMethod]
        public void Test_Merge()
        {
            Assert.AreEqual(string.Empty, StringEnumerableExtensions.Merge(null, "xx"));

            string[] array = new string[] { "11", "22", "33" };
            Assert.AreEqual("11-22-33", array.Merge("-"));
            Assert.AreEqual("112233", array.Merge(""));
            Assert.AreEqual("112233", array.Merge(null));
        }

        [TestMethod]
        public void Test_ToString2()
        {
            Assert.AreEqual(string.Empty, StringExtensions.ToString2(null));

            DateTime dt = DateTime.Now;
            Assert.AreEqual(dt.ToTimeString(), dt.ToString2());

            Assert.AreEqual("123", 123.ToString2());
            Assert.AreEqual("12'3456", 123456.ToString2());

            Assert.AreEqual("12'3456", 123456L.ToString2());

            Assert.AreEqual("true", true.ToString2());
            Assert.AreEqual("false", false.ToString2());
        }


        [TestMethod]
        public void Test_error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _= StringBuilderExtensions.AppendLineRN(null, "filepath");
            });

        }


        [TestMethod]
        public void Test_CheckName()
        {
            string s1 = null;
            string s2 = "";
            string s3 = "abc";
            string s4 = "ABC_dd";
            string s5 = "abc-dd";
            string s6 = "abc;dd";
            string s7 = "abc dd";

            s1.CheckName();
            s2.CheckName();
            s3.CheckName();
            s4.CheckName();

            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                s5.CheckName();
            });
            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                s6.CheckName();
            });
            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                s7.CheckName();
            });
        }

        [TestMethod]
        public void Test_CheckName_Array()
        {
            string s1 = null;
            string s2 = "";
            string s3 = "abc";
            string s4 = "ABC_dd";
            string s5 = "abc-dd";
            string s6 = "abc;dd";
            string s7 = "abc dd";

            string[] names1 = new string[] { s1, s2, s3, s4 };

            string[] names2 = new string[] { s1, s2, s3, s4, s5 };

            List<string> names3 = new List<string> { s7, s6, s1 };

            names1.CheckNames();

            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                names2.CheckNames();
            });
            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                names3.CheckNames();
            });
 
        }


        [TestMethod]
        public void Test_ToHashSet1()
        {
            string text = "aa , bb; cc,;,dd ,aa;bb;;;;cc,,dd,dd";
            var result = text.SplitToHashSet(',', ';', '\n');

            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains("aa"));
            Assert.IsTrue(result.Contains("bb"));
            Assert.IsTrue(result.Contains("cc"));
            Assert.IsTrue(result.Contains("dd"));
        }

        [TestMethod]
        public void Test_ToHashSet2()
        {
            string text = "aa , bb; cc,;,dd ,aa;bb;;;;cc,,dd,dd";
            var result = text.SplitToHashSet();

            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains("aa"));
            Assert.IsTrue(result.Contains("bb"));
            Assert.IsTrue(result.Contains("cc"));
            Assert.IsTrue(result.Contains("dd"));
        }

        [TestMethod]
        public void Test_ToHashSet3()
        {
            var result = "".SplitToHashSet();
            Assert.AreEqual(0, result.Count);

            MyAssert.IsError<ArgumentNullException>(() => {
                char[] separator = null;
                _=  "1;2;3".SplitToHashSet(separator);
            });
        }



        [TestMethod]
        public void Test_ToHeaderCollection()
        {
            string text = @"
x-RequestId: 65a09b23a7644aa7a27a641a43a5c657
x-HostName: 1b5b8b3075a2
x-AppName: XDemo.WebSiteApp
x-dotnet: .NET 6.0.5
Content-Type: text/plain; charset=utf-8
Date: Wed, 15 Jun 2022 11:03:38 GMT
Server: Kestrel
".Trim();

            NameValueCollection headerCollection = text.ToHeaderCollection();
            Assert.AreEqual(7, headerCollection.Count);


            NameValueCollection headerCollection2 = "".ToHeaderCollection();
            Assert.AreEqual(0, headerCollection2.Count);
        }


        [TestMethod]
        public void Test_NotNull()
        {
            string nullstring = null;
            string s2 = nullstring.NotNull();

            Assert.IsNotNull(s2);
            Assert.AreEqual(0, s2.Length);
        }

    }
}
