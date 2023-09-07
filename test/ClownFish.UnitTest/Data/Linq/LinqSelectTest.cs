using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.Linq;


[TestClass]
	public class LinqSelectTest : BaseTest
	{

		[TestMethod]
		public void Test_LINQ_Select_2个字段_内嵌WHERE()
		{
			using( DbContext dbContext = DbContext.Create() ) {
				var query = from t in dbContext.Entity.Query<Product>()
							where t.ProductID == 5 || t.CategoryID < 3
							select new Product { ProductID = t.ProductID, ProductName = t.ProductName };

				query = query.Where(x => x.ProductID > 3);

				var list = query.ToList();
			}
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
    public void Test_LINQ_Select_2个字段_内嵌WHERE_2()
    {
        using( DbContext dbContext = DbContext.Create() ) {
            dbContext.EnableDelimiter = true;

            var query = from t in dbContext.Entity.Query<Product>()
                        where t.ProductID == 5 || t.CategoryID < 3
                        select new Product { ProductID = t.ProductID, ProductName = t.ProductName };

            query = query.Where(x => x.ProductID > 3);

            var list = query.ToList();
        }
        AssertLastExecuteSQL(@"
SELECT [ProductID],[ProductName]
FROM [Products]
WHERE ([ProductID] > @p1) AND (([ProductID] = @p2) OR ([CategoryID] < @p3))
@p1: (Int32), 3
@p2: (Int32), 5
@p3: (Int32), 3
");
    }


    [TestMethod]
		public async Task Test_LINQ_Select_2个字段_内嵌WHERE_Async()
		{
			using( DbContext db = DbContext.Create() ) {

				var query = from t in db.Entity.Query<Product>()
							where t.ProductID == 5 || t.CategoryID < 3
                        select new Product { ProductID = 0, ProductName = "" };

            query = query.Where(x => x.ProductID > 3);

				var list = await query.ToListAsync();
			}
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
			using( DbContext db = DbContext.Create() ) {
				var query = from t in db.Entity.Query<Product>()
                        select new Product { ProductID = 0, ProductName = "" };

            query = query.Where(x => x.ProductID > 3);

				var list = query.ToList();
			}
			AssertLastExecuteSQL(@"
SELECT ProductID,ProductName
FROM Products
WHERE (ProductID > @p1)
@p1: (Int32), 3
");
		}


		[TestMethod]
		public async Task Test_LINQ_Select_2个字段_追加WHERE条件_Async()
		{
			using( DbContext db = DbContext.Create() ) {
				var query = from t in db.Entity.Query<Product>()
                        select new Product { ProductID = 100, ProductName = "xx" };

            query = query.Where(x => x.ProductID > 3);

				var list = await query.ToListAsync();
			}
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
			using( DbContext db = DbContext.Create() ) {
				var query = from t in db.Entity.Query<Product>()
							select new { id = t.ProductID, name = t.ProductName };

				query = query.Where(x => x.id > 3);

				var list = query.ToList();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Test_LINQ_Select_与实体类型不匹配()
		{
			using( DbContext db = DbContext.Create() ) {
				var query = from t in db.Entity.Query<Product>()
							where t.ProductID == 5 || t.CategoryID < 3
							select new TestClassIdName { Id = t.ProductID, Name = t.ProductName };

				query = query.Where(x => x.Id > 3);

				var list = query.ToList();
			}
		}


	}


	public class TestClassIdName
	{
		public int Id { get; set; }

		public string Name { get; set; }
	}
