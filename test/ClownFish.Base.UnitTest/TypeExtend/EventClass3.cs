using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Base.UnitTest.TypeExtend
{
	public class EventClass3 : BaseEventObject
	{
		public event EventHandler<IntEventArgs> BeforeAdd;

		public int Add(int a, int b)
		{
			EventHandler<IntEventArgs> before = this.BeforeAdd;
			if( before != null ) {
				IntEventArgs e1 = new IntEventArgs { Number = a };
				before(this, e1);
				a = e1.Number;      // 重新从事件参数中取值，因为有可能会在事件中修改
			}

			return a + b;
		}
	}
}
