using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base.Reflection;
using System.Reflection;

namespace ClownFish.Web.UnitTest.Attributes
{
	[TestClass]
	public class AttributeFactoryTest
	{
		[TestMethod]
		public void Test_Type1Attribute()
		{
			Type1Attribute a = typeof(AttributeClass1).GetMyAttribute<Type1Attribute>();
			Assert.AreEqual("20e00d57-fc17-4e90-8b93-8b0d1a51ed3f", a.Flag);
		}

		[TestMethod]
		public void Test_Method1Attribute()
		{
			MethodInfo m = typeof(AttributeClass1).GetMethod("Add");
			Method1Attribute a = m.GetMyAttribute<Method1Attribute>();
			Assert.AreEqual("d16da143-0d90-4e27-a665-83a2639966ea", a.Flag);
		}

		[TestMethod]
		public void Test_Property1Attribute()
		{
			PropertyInfo p = typeof(AttributeClass1).GetProperty("Id");
			Property1Attribute a = p.GetMyAttribute<Property1Attribute>();
			Assert.AreEqual("41b1fa2b-7a8e-43cf-b1a1-f60e74fd498e", a.Flag);
		}

		[TestMethod]
		public void Test_Parameter1Attribute()
		{
			MethodInfo m = typeof(AttributeClass1).GetMethod("Add");
			ParameterInfo p = m.GetParameters()[1];
			Parameter1Attribute a = p.GetMyAttribute<Parameter1Attribute>();
			Assert.AreEqual("bfa101f5-85db-40d9-8260-6727f2fafe55", a.Flag);
		}
	}
}
