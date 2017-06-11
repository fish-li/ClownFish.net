using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class EntityServiceTest : BaseTest
	{
		[TestInitialize]
		public void Init()
		{
			ExtenderManager.RegisterSubscriber(typeof(ClownFish.Data.UnitTest.ClownFishDataEventSubscriber));

			string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Connections.config");

			ClownFish.Data.Initializer.Instance
							.InitConnection(configFile)
							.AllowCreateOneOffDbContext()
							.CompileAllEntityProxy();
		}

		[TestCleanup]
		public void Cleanup()
		{
			ExtenderManager.RemoveSubscriber(typeof(ClownFish.Data.UnitTest.ClownFishDataEventSubscriber));
		}


		[TestMethod]
		public void Test_Action参数接收数据实体代理()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/Entity/Action1.aspx HTTP/1.1

productID=2222&productName=abc&categoryID=3
";
			string result = ExecuteService(requestText);


			string expect = @"
ClownFish.Data.GeneratorCode.ProductX_B30C6A651B9C86C7446742BF3FA9C6E9_Proxy
ProductID=2222
ProductName=abc
CategoryID=3


UPDATE Products SET  ProductName=@p1, CategoryID=@p2 WHERE ProductID = @p3
@p1: (String), abc
@p2: (Int32), 3
@p3: (Int32), 2222
";

			Assert.AreEqual(expect, result);
		}
	}
}
