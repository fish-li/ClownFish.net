using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.TestApplication1.Common
{
	[Serializable]
	public class AssertFailedException : Exception
	{
		public AssertFailedException(string msg, Exception ex)
			: base(msg, ex)
		{
		}
		public AssertFailedException(string msg)
			: base(msg)
		{
		}
		public AssertFailedException()
		{
		}
		protected AssertFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
