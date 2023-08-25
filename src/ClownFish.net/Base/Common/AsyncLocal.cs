//#if NET45

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Remoting.Messaging;
//using System.Text;
//using System.Threading.Tasks;

//namespace ClownFish.Base
//{
//	internal class AsyncLocal<T>
//	{
//		private readonly string _key = Guid.NewGuid().ToString("N");

//		public T Value {
//			get => (T)CallContext.LogicalGetData(_key);
//			set => CallContext.LogicalSetData(_key, value);
//		}
//	}
//}

//#endif
