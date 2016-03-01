using System;

namespace DEMO.Common
{
	[Serializable]
	public class MyMessageException : Exception
	{
		// Methods
		public MyMessageException(string message)
			: base(message)
		{
		}

	}
}