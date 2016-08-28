using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Base.UnitTest.TypeExtend
{
	public class TestClass 
	{
		public virtual int Add(int a, int b)
		{
			return a + b;
		}
	}


	public class TestClassExt : TestClass
	{
		public override int Add(int a, int b)
		{
			return base.Add(a, b) + 10;
		}
	}
}
