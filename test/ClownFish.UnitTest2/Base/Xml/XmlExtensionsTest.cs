using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Xml
{
	[TestClass]
	public class XmlExtensionsTest
	{
		[TestMethod]
		public void Test_ToXml_FromXml()
		{
			Product p = Product.CreateByRandomData();

			string xml = p.ToXml();
			Product p2 = xml.FromXml<Product>();

			Assert.IsTrue(p.IsEqual(p2));
		}



		[TestMethod]
		public void Test_FromXml_ObjectType()
		{
			Product p = Product.CreateByRandomData();

			string xml = p.ToXml();
			Product p2 = xml.FromXml(typeof(Product)) as Product;

			Assert.IsTrue(p.IsEqual(p2));
		}
	}
}
