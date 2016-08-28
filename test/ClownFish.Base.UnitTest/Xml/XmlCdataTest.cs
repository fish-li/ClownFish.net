using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base.Xml;
using ClownFish.Base;

namespace ClownFish.Base.UnitTest.Serializer
{
	public class Xcdata
	{
		public int A { get; set; }

		public XmlCdata Text { get; set; }
	}


	[TestClass]
	public class XmlCdataTest
	{
		[TestMethod]
		public void Test_XmlCdata_ToXml_FromXml()
		{
			Xcdata m = new Xcdata { A = 2, Text = "123456789" };

			string xml = m.ToXml();

			Xcdata m2 = xml.FromXml<Xcdata>();

			Assert.AreEqual(2, m2.A);
		}


		[TestMethod]
		public void Test_XmlCdata_implicit()
		{
			XmlCdata data = "abc";		// 隐式类型转换
			Assert.AreEqual("abc", data.Value);
			Assert.AreEqual("abc", data.ToString());

			var schema = (data as IXmlSerializable).GetSchema();
			Assert.IsNull(schema);

			string s = data;
			Assert.AreEqual("abc", s);
		}

		[TestMethod]
		public void Test_XmlCdata_Equals()
		{
			XmlCdata data = "abc";      // 隐式类型转换
			XmlCdata data2 = new XmlCdata("abc");

			Assert.IsTrue(data == data2);
			Assert.IsTrue(data.Equals(data2));

			object a = data;
			object b = data2;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(a.GetHashCode() == b.GetHashCode());

			XmlCdata data3 = "ab";
			Assert.IsTrue(data != data3);
		}

	}
}
