using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Common
{
	[TestClass]
	public class ByteExtensionsTest
	{
		[TestMethod]
		public void Test_IsEqual()
        {
			byte[] b0 = Empty.Array<byte>();
			byte[] b1 = new byte[] { 11, 22 };
			byte[] b2 = new byte[] { 33, 44 };
			byte[] b3 = new byte[] { 33, 44 };

			Assert.IsTrue(ByteExtensions.IsEqual(null, null));

			Assert.IsFalse(ByteExtensions.IsEqual(null, b0));
			Assert.IsFalse(ByteExtensions.IsEqual(b0, null));

			Assert.IsFalse(b0.IsEqual(b1));
			Assert.IsFalse(b2.IsEqual(b1));
			Assert.IsTrue(b3.IsEqual(b2));
		}


		[TestMethod]
		public void Test_ToBase64()
        {
			byte[] b0 = Empty.Array<byte>();
			byte[] b1 = null;

			Assert.AreEqual(string.Empty, b0.ToBase64());
			Assert.AreEqual(string.Empty, b1.ToBase64());


			byte[] bb = Guid.NewGuid().ToByteArray();

			string base64 = bb.ToBase64();
			Assert.IsTrue(base64.Length > 0);

			byte[] bb2 = Convert.FromBase64String(base64);
			Assert.IsTrue(bb2.IsEqual(bb));
		}

		[TestMethod]
		public void Test_ToHexString()
        {
			byte[] b0 = Empty.Array<byte>();
			byte[] b1 = null;
			byte[] b2 = new byte[] { 33, 44 };

			Assert.AreEqual(string.Empty, b0.ToHexString());
			Assert.AreEqual(string.Empty, b1.ToHexString());

			string hex = b2.ToHexString();
			Assert.AreEqual(4, hex.Length);
		}


	}
}
