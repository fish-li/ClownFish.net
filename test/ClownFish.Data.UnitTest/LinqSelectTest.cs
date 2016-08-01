using System;
using System.Linq;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{

	[TestClass]
	public class LinqSelectTest : BaseTestWithConnectionScope
	{

		[TestMethod]
		public void Test_LINQ_Select_2个字段_内嵌WHERE()
		{
			var query = from t in Entity.Query<Product>()
						where t.ProductID == 5 || t.CategoryID < 3
						select new Product { ProductID = t.ProductID, ProductName = t.ProductName };

			query = query.Where(x => x.ProductID > 3);

			var list = query.ToList();

			AssertLastExecuteSQL(@"
SELECT ProductID,ProductName
FROM Products
WHERE (ProductID > @p1) AND ((ProductID = @p2) OR (CategoryID < @p3))
@p1: (Int32), 3
@p2: (Int32), 5
@p3: (Int32), 3
");
		}




		[TestMethod]
		public void Test_LINQ_Select_2个字段_追加WHERE条件()
		{
			var query = from t in Entity.Query<Product>()
						select new Product { ProductID = t.ProductID, ProductName = t.ProductName };

			query = query.Where(x => x.ProductID > 3);

			var list = query.ToList();

			AssertLastExecuteSQL(@"
SELECT ProductID,ProductName
FROM Products
WHERE (ProductID > @p1)
@p1: (Int32), 3
");
		}





		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Test_LINQ_Select_不支持匿名对象结果()
		{
			var query = from t in Entity.Query<Product>()
						select new { id = t.ProductID, name = t.ProductName };

			query = query.Where(x => x.id > 3);

			var list = query.ToList();
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Test_LINQ_Select_与实体类型不匹配()
		{
			var query = from t in Entity.Query<Product>()
						where t.ProductID == 5 || t.CategoryID < 3
						select new TestClass_IdName { id = t.ProductID, name = t.ProductName };

			query = query.Where(x => x.id > 3);

			var list = query.ToList();
		}


	}


	public class TestClass_IdName
	{
		public int id { get; set; }

		public string name { get; set; }
	}
}
