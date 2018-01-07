using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class ParaNameBuilderTest : BaseTest
	{
		[TestMethod]
		public void Test_ParaNameBuilderFactory_GetBuilder_ODBC()
		{
			OdbcCommand command1 = new OdbcCommand();
			OleDbCommand command2 = new OleDbCommand();

			ParaNameBuilder builder1 = ParaNameBuilderFactory.Factory.Instance.GetBuilder(command1);
			ParaNameBuilder builder2 = ParaNameBuilderFactory.Factory.Instance.GetBuilder(command2);

			Assert.IsTrue(object.ReferenceEquals(builder1, builder2));

			Assert.AreEqual("abc", builder1.GetParaName("abc"));
			Assert.AreEqual("?", builder1.GetPlaceholder("abc"));
		}
	}


	



}
