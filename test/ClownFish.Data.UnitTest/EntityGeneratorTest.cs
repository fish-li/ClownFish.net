using ClownFish.Data.CodeDom;
using ClownFish.Data.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.UnitTest.Models;

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
