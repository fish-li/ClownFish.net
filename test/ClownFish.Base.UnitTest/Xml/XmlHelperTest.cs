using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Xml
{
	public class TestObjectf6b4446ed2f54c6e9508fd2c4f61d4a1
	{
		public int A { get; set; }

		public string B { get; set; }
	}

	[TestClass]
	public class XmlHelperTest
	{
		[TestMethod]
		public void Test_XmlSerializerObject()
		{
			TestObjectf6b4446ed2f54c6e9508fd2c4f61d4a1 obj = new TestObjectf6b4446ed2f54c6e9508fd2c4f61d4a1 {
				A = 5,
				B = "Fish Li"
			};
			string result = ClownFish.Base.Xml.XmlHelper.XmlSerializerObject(obj);
			Console.WriteLine(result);

			string xml = @"
<TestObjectf6b4446ed2f54c6e9508fd2c4f61d4a1>
    <A>5</A>
    <B>Fish Li</B>
</TestObjectf6b4446ed2f54c6e9508fd2c4f61d4a1>";

			Assert.AreEqual(TrimString(xml), TrimString(result));
		}

		private string TrimString(string s)
		{
			if( string.IsNullOrEmpty(s) )
				return s;

			return s.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "");
		}
	}
}
