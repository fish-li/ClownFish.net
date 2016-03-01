using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClownFish.Web.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base.Xml;
using ClownFish.Base;

namespace ClownFish.Web.UnitTest.Serializer
{
	[TestClass]
	public class XmlCdataTest
	{
		[TestMethod]
		public void Test()
		{
			CDATAModel m = new CDATAModel { A = 2, Text = "123456789" };

			string xml = m.ToXml();

			CDATAModel m2 = xml.FromXml<CDATAModel>();

			Assert.AreEqual(2, m2.A);
		}


		[TestMethod]
		public void Test2()
		{
			XmlCdata data = "abc";		// 隐式类型转换
			Assert.AreEqual("abc", data.Value);
			Assert.AreEqual("abc", data.ToString());

			var schema = (data as IXmlSerializable).GetSchema();
			Assert.IsNull(schema);

			string s = data;
			Assert.AreEqual("abc", s);
		}
	}
}
