using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Common
{
	[TestClass]
	public class HttpValueCollectionTest
	{
		[TestMethod]
		public void Test()
		{
			HttpValueCollection collection = new HttpValueCollection();

			int count = 0;

			collection.MakeReadOnly();

			try {
				collection.Add("k1", "abc");
			}
			catch( NotSupportedException ) {
				count++;
			}

			collection.MakeReadWrite();

			try {
				collection.Add("k1", "abc");
			}
			catch( NotSupportedException ) {
				count++;
			}

			Assert.AreEqual(1, count);
		}
	}
}
