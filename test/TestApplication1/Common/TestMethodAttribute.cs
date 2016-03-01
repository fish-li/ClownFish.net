using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.TestApplication1.Common
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
	public class TestMethodAttribute : System.Attribute
	{
		public TestMethodAttribute(string description)
		{
			if( string.IsNullOrEmpty(description) )
				throw new ArgumentNullException("description");

			this.Description = description;
		}

		public string Description { get; private set; }
	}
}
