using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;

namespace ClownFish.Data.PerformanceTest
{
	public class TestMethodAttribute : Attribute
	{
		public string Description { get; private set; }

		public TestMethodAttribute(string description)
		{
			if( string.IsNullOrEmpty(description) )
				throw new ArgumentNullException(nameof(description));

			this.Description = description;
		}
	}



	public interface IPerformanceTest : IDisposable
	{
		object Run();
	}
}
