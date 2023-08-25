using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class BinSerializerTest
    {
		[TestMethod]
		public void Test_BytesIsEqual()
		{
			for( int i = 0; i < 10; i++ ) {
				Product p = Product.CreateByRandomData();
				Product p2 = p.CloneObject();

				byte[] b1 = BinSerializer.Serialize(p);
				byte[] b2 = BinSerializer.Serialize(p2);

				Assert.IsTrue(b1.IsEqual(b2));
			}
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ArgumentNullException1()
		{
			var x = BinSerializer.Serialize(null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ArgumentNullException2()
		{
			var x = BinSerializer.DeserializeObject(null);
		}


		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ArgumentNullException3()
		{
			var x = BinSerializer.DeserializeObject(Array.Empty<byte>());
		}


	}
}
