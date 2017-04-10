using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest
{
	internal static class ExceptionHelper
	{
		public static string ExecuteActionReturnErrorMessage(Action action)
		{
			try {
				action();
			}
			catch( Exception ex ) {
				return ex.Message;
			}


			throw new InternalTestFailureException("异常没有出现！");
		}

		public async static Task<string> ExecuteActionReturnErrorMessage(Func<Task> action)
		{
			try {
				await action();
			}
			catch( Exception ex ) {
				return ex.Message;
			}


			throw new InternalTestFailureException("异常没有出现！");
		}
	}
}
