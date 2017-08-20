using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class ByteExtensionsTest
	{
		[TestMethod]
		public void Test_BytesIsEqual()
		{
			for( int i = 0; i < 10; i++ ) {
				Product p = Product.CreateByRandomData();
				Product p2 = p.CloneObject();

				byte[] b1 = BinSerializerHelper.Serialize(p);
				byte[] b2 = BinSerializerHelper.Serialize(p2);

				Assert.IsTrue(b1.IsEqual(b2));
			}
		}
	}
}
