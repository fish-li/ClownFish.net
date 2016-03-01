using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClownFish.Web;

namespace Fish.AA
{
	public class TestService
	{
		[Action]
		public int Add(int a, int b)
		{
			return a + b;
		}
	}
}

namespace Fish.BB
{
	public class AddInfo
	{
		public int A;
		public int B;
	}

	public class TestService
	{
		[Action]
		public int Add(AddInfo info)
		{
			return info.A + info.B + 10;	// 故意写错。
		}
	}
}

