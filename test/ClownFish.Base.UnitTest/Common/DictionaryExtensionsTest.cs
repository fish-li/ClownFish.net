using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class DictionaryExtensionsTest
	{
		[TestMethod]
		public void Test_Dictionary_Add_SameKey()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			dict.AddValue("abc", 1);



			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				dict.AddValue("abc", 1);
			});

			string expected = "Key=abc";
			Assert.IsTrue(error.IndexOf(expected) > 0);




			Exception exception = null;

			try {
				dict.AddValue("abc", 1);
			}
			catch(Exception ex ) {
				exception = ex;
			}

			Assert.IsNotNull(exception);
			Assert.IsNotNull(exception.InnerException);
			Assert.IsInstanceOfType(exception, typeof(ArgumentException));
			Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));

		}



		[TestMethod]
		public void Test_Hashtable_Add_SameKey()
		{
			Hashtable dict = new Hashtable();
			dict.AddValue("abc", 1);



			string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
				dict.AddValue("abc", 1);
			});

			string expected = "Key=abc";
			Assert.IsTrue(error.IndexOf(expected) > 0);




			Exception exception = null;

			try {
				dict.AddValue("abc", 1);
			}
			catch( Exception ex ) {
				exception = ex;
			}

			Assert.IsNotNull(exception);
			Assert.IsNotNull(exception.InnerException);
			Assert.IsInstanceOfType(exception, typeof(ArgumentException));
			Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));

		}
	}
}
