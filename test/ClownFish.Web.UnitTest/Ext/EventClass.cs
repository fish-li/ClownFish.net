using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Web.UnitTest.Ext
{
	public class EventClass : BaseEventObject
	{
		public event EventHandler<IntEventArgs> BeforeAdd;
		public event EventHandler<IntEventArgs> AfterAdd;

		public int Add(int a, int b)
		{
			EventHandler<IntEventArgs> before = this.BeforeAdd;
			if( before != null ) {
				IntEventArgs e1 = new IntEventArgs { Number = a };
				before(this, e1);
				a = e1.Number;		// 重新从事件参数中取值，因为有可能会在事件中修改
			}

			int c = a + b;

			EventHandler<IntEventArgs> after = this.AfterAdd;
			if( after != null ) {
				IntEventArgs e2 = new IntEventArgs { Number = c };
				after(this, e2);
				c = e2.Number;		// 重新从事件参数中取值，因为有可能会在事件中修改
			}

			return c;
		}
	}

	public class IntEventArgs : EventArgs
	{
		public int Number { get; set; }
	}


	public class EventClassEventSubscriber : EventSubscriber<EventClass>
	{
		public override void SubscribeEvent(EventClass instance)
		{
			instance.BeforeAdd += instance_BeforeAdd;
			instance.AfterAdd += instance_AfterAdd;
		}

		void instance_AfterAdd(object sender, IntEventArgs e)
		{
			e.Number += 20;
		}

		void instance_BeforeAdd(object sender, IntEventArgs e)
		{
			e.Number += 5;
		}
	}



	public class EventClass2 : BaseEventObject
	{
		private EventClass2() { }		// 没有公开的无参构造函数
	}

	public class EventClass2EventSubscriber : EventSubscriber<EventClass2>
	{
		public override void SubscribeEvent(EventClass2 instance)
		{
			throw new NotImplementedException();
		}
	}



	public class EventClass3 : BaseEventObject
	{
		public event EventHandler<IntEventArgs> BeforeAdd;

		public int Add(int a, int b)
		{
			EventHandler<IntEventArgs> before = this.BeforeAdd;
			if( before != null ) {
				IntEventArgs e1 = new IntEventArgs { Number = a };
				before(this, e1);
				a = e1.Number;		// 重新从事件参数中取值，因为有可能会在事件中修改
			}

			return a + b;
		}
	}

}
