#if NET45 || NET451 || NET452

using System;
using System.Runtime.Remoting.Messaging;

namespace ClownFish.Base
{
	internal class AsyncLocal<T>
	{
		private readonly string _key = Guid.NewGuid().ToString("N");

		public T Value {
			get => (T)CallContext.LogicalGetData(_key);
			set => CallContext.LogicalSetData(_key, value);
		}
	}
}

#endif
