using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;


// ##################################################################
//
// ClownFish.Data 性能测试结果解读
//
// http://note.youdao.com/noteshare?id=f45ab5306f6ebdfa6b142322a50f9b32
//
// ##################################################################


namespace ClownFish.Data.PerformanceTest
{
	public class TestCaseAttribute : Attribute
	{
		public string Description { get; private set; }

		public TestCaseAttribute(string description)
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
