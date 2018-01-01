using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class ConvertExtensionsTest : BaseTest
	{
		[TestMethod]
		public void Test_ConvertExtensions_Convert()
		{
			object intValue = 3;
			object strValue = "112";
			object timeValue = DateTime.Now.ToString();

			object r0 = ConvertExtensions.Convert(null, typeof(int));
			Assert.IsNull(r0);

			object r1 = ConvertExtensions.Convert(intValue, typeof(int));
			Assert.IsTrue(r1.GetType() == typeof(int));
			Assert.AreEqual(3, (int)r1);


			object r2 = ConvertExtensions.Convert(intValue, typeof(string));
			Assert.IsTrue(r2.GetType() == typeof(string));
			Assert.AreEqual("3", (string)r2);

			object r3 = ConvertExtensions.Convert(strValue, typeof(string));
			Assert.IsTrue(r3.GetType() == typeof(string));
			Assert.AreEqual("112", (string)r3);

			object r4 = ConvertExtensions.Convert(strValue, typeof(int));
			Assert.IsTrue(r4.GetType() == typeof(int));
			Assert.AreEqual(112, (int)r4);


			object guidString = Guid.NewGuid().ToString();
			object r5 = ConvertExtensions.Convert(guidString, typeof(Guid));
			Assert.IsTrue(r5.GetType() == typeof(Guid));
			Assert.AreEqual((string)guidString, r5.ToString());

			object r6 = ConvertExtensions.Convert(timeValue, typeof(DateTime));
			Assert.IsTrue(r6.GetType() == typeof(DateTime));
			Assert.AreEqual(timeValue, r6.ToString());

			object enumvalue = 5;
			object r7 = ConvertExtensions.Convert(enumvalue, typeof(DayOfWeek));
			Assert.IsTrue(r7.GetType() == typeof(int));
			Assert.AreEqual(5, (int)r7);

		}
	}
}
