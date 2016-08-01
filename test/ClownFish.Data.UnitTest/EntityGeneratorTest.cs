using System;
using System.IO;
using System.Text;
using ClownFish.Data.CodeDom;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
    public class EntityGeneratorTest
    {
        [TestMethod]
        public void Test1_生成实体代理类型代码()
        {
            EntityGenerator g = new EntityGenerator();
            string code = g.GetCode<Product>();

            code = EntityGenerator.UsingCodeBlock + code;

			//File.WriteAllText("..\\AutoCode1.cs", code, Encoding.UTF8);
			File.WriteAllText("EntityGeneratorTest_code.cs", code, Encoding.UTF8);
		}
    }
}
