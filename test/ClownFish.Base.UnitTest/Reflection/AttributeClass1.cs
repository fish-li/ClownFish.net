using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.UnitTest.Reflection
{
	[Type1(Flag = "20e00d57-fc17-4e90-8b93-8b0d1a51ed3f")]
	public class AttributeClass1
	{
		[Property1(Flag = "41b1fa2b-7a8e-43cf-b1a1-f60e74fd498e")]
		public int Id { get; set; }

		[Method1(Flag = "d16da143-0d90-4e27-a665-83a2639966ea")]
		public int Add(int a, 
			[Parameter1(Flag ="bfa101f5-85db-40d9-8260-6727f2fafe55")]
			int b)
		{
			return a + b;
		}
	}


	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class Type1Attribute : Attribute
	{
		public string Flag { get; set; }
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class Method1Attribute : Attribute
	{
		public string Flag { get; set; }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class Property1Attribute : Attribute
	{
		public string Flag { get; set; }
	}


	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class Parameter1Attribute : Attribute
	{
		public string Flag { get; set; }
	}
}
