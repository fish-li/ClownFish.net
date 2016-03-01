using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.UnitTest
{
	public abstract class TestBase
	{
		protected Exception _lastException = null;

		public TestBase()
		{
			LogHelper.OnError += ex => _lastException = ex;

			WriterFactory.Init();
		}



	}
}
