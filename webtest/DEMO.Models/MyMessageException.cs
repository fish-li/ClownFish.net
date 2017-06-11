using System;

namespace DEMO.Models
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