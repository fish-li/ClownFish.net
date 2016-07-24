using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Common
{
	[TestClass]
	public class GzipHelperTest
	{
		[TestMethod]
		public void Test_CompressString()
		{
			string s = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
";

			string b = GzipHelper.Compress(s);

			string s2 = GzipHelper.Decompress(b);

			Assert.AreEqual(s, s2);
		}


		[TestMethod]
		public void Test_CompressBytes()
		{
			string s = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
";
			byte[] bb = Encoding.UTF8.GetBytes(s);

			byte[] b1 = GzipHelper.Compress(bb);
			byte[] b2 = GzipHelper.Decompress(b1);

			Assert.AreEqual(bb.Length, b2.Length);
			
			for(int i=0;i<bb.Length; i++ ) {
				Assert.AreEqual((int)bb[i], (int)b2[i]);
			}
		}


	}
}
