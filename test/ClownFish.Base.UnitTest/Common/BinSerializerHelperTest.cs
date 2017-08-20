using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class BinSerializerHelperTest
	{
		[TestMethod]
		public void Test_CloneObject()
		{
			for( int i = 0; i < 10; i++ ) {
				Product p = Product.CreateByRandomData();
				Product p2 = p.CloneObject();

				string json1 = p.ToJson();
				string json2 = p2.ToJson();

				Assert.AreEqual(json1, json2);
			}
		}
	}
}
