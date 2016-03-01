using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log;


namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class SomeExtenstionsTest
	{
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
