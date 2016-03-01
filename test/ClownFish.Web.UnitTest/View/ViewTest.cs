using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.View
{
	[TestClass]
	public class ViewTest
	{
		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void Test_MyPageView()
		{
			MyPageView<string> view = new MyPageView<string>();

			int a = 2;
			view.SetModel(a);
		}


		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void Test_MyUserControlView()
		{
			MyUserControlView<string> view = new MyUserControlView<string>();

			int a = 2;
			view.SetModel(a);
		}

	}
}
