using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Base.UnitTest.TypeExtend
{
	public class EventClass2 : BaseEventObject
	{
		private EventClass2() { }       // 没有公开的无参构造函数
	}

	public class EventClass2EventSubscriber : EventSubscriber<EventClass2>
	{
		public override void SubscribeEvent(EventClass2 instance)
		{
			throw new NotImplementedException();
		}
	}
}
