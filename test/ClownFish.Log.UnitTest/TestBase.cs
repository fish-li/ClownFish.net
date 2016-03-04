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
			LogHelper.OnError += LogHelper_OnError;

			WriterFactory.Init();
		}

		void LogHelper_OnError(object sender, LogExceptionEventArgs e)
		{
			_lastException = e.Exception;
		}



	}
}
