﻿using ClownFish.Data.Linq;

namespace ClownFish.UnitTest.Data.Linq;

[TestClass]
public class LinqTest : BaseTest
{
    public int P5 { get; set; } = 5;
    public int P3 { get; set; } = 3;

    private readonly int _f5 = 5;
    private readonly int _f3 = 3;



    [TestMethod]
    public void Test_LINQ_获取单个实体()
    {
        using( DbContext db = DbContext.Create() ) {
            int a = 5, b = 3;
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == a && t.CategoryID < b
                        select t;

            Product p = query.ToSingle();
            Assert.AreEqual(5, p.ProductID);
        }
        AssertLastExecuteSQL(@"
SELECT *
FROM Products
WHERE ((ProductID = @p1) AND (CategoryID < @p2))
@p1: (Int32), 5
@p2: (Int32), 3
");
    }


    [TestMethod]
    public void Test_LINQ_获取单个实体_追加WHERE条件()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        select t;

            query = query.Where(t => t.ProductID == 5 && t.CategoryID < 3);

            Product p = query.FirstOrDefault();
            Assert.AreEqual(5, p.ProductID);
        }
        AssertLastExecuteSQL(@"
SELECT *
FROM Products
WHERE ((ProductID = @p1) AND (CategoryID < @p2))
@p1: (Int32), 5
@p2: (Int32), 3
");
    }




    [TestMethod]
    public void Test_LINQ_获取实体列表()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == P5 || t.CategoryID < P3
                        select t;

            List<Product> list = query.ToList();
        }
        AssertLastExecuteSQL(@"
SELECT *
FROM Products
WHERE ((ProductID = @p1) OR (CategoryID < @p2))
@p1: (Int32), 5
@p2: (Int32), 3
");
    }


    [TestMethod]
    public void Test_LINQ_获取实体列表_IN参数()
    {
        string b = "aaa";
        string c = null;

        int[] array = new int[] { 1, 2, 3, 4, 5 };

        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where (t.ProductID == _f5
                                || array.Contains(t.CategoryID)
                                || t.ProductName.StartsWith(b)
                               )
                               && t.Remark != c
                        select t;

            List<Product> list = query.ToList();
        }
        AssertLastExecuteSQL(@"
SELECT *
FROM Products
WHERE ((((ProductID = @p1) OR (CategoryID IN (1,2,3,4,5))) OR (ProductName like @p2)) AND (Remark != @p3))
@p1: (Int32), 5
@p2: (String), aaa%
@p3: (String), NULL
");
    }


    [TestMethod]
    public void Test_LINQ_WHERE分成二段()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == P5 || t.Quantity > 10
                        select t;

            query = query.Where(x => x.CategoryID < _f3);

            List<Product> list = query.ToList();
        }
        AssertLastExecuteSQL(@"
SELECT *
FROM Products
WHERE (CategoryID < @p1) AND ((ProductID = @p2) OR (Quantity > @p3))
@p1: (Int32), 3
@p2: (Int32), 5
@p3: (Int32), 10
");
    }


    [TestMethod]
    public void Test_LINQ_COUNT()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == 5 && t.CategoryID < 3
                        select t;

            int count = query.Count();
        }
        AssertLastExecuteSQL(@"
SELECT count(*)
FROM Products
WHERE ((ProductID = @p1) AND (CategoryID < @p2))
@p1: (Int32), 5
@p2: (Int32), 3
");
    }

    [TestMethod]
    public void Test_LINQ_GetQuery()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == 5 && t.CategoryID < 3
                        select new Product { ProductID = 0, ProductName = "" };

            CPQuery query2 = query.GetQuery();
            Console.WriteLine(query2.Command.ToLoggingText());
        }
    }


    [TestMethod]
    public void Test_LINQ_COUNT_追加WHERE条件()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        select t;

            query = query.Where(t => t.ProductID == 5 || t.CategoryID < 3);

            int count = query.Count();
        }
        AssertLastExecuteSQL(@"
SELECT count(*)
FROM Products
WHERE ((ProductID = @p1) OR (CategoryID < @p2))
@p1: (Int32), 5
@p2: (Int32), 3
");
    }

    [TestMethod]
    public void Test_LINQ_EXIST()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == _f5 && t.CategoryID < 3
                        select t;

            bool exist = query.Any();
        }
        AssertLastExecuteSQL(@"
SELECT 1 WHERE EXISTS ( SELECT 1
FROM Products
WHERE ((ProductID = @p1) AND (CategoryID < @p2)))
@p1: (Int32), 5
@p2: (Int32), 3
");
    }

    [TestMethod]
    public void Test_LINQ_EXIST_追加WHERE条件()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        select t;

            query = query.Where(t => t.ProductID == 5 || t.CategoryID < _f3);

            bool exist = query.Any();
        }
        AssertLastExecuteSQL(@"
SELECT 1 WHERE EXISTS ( SELECT 1
FROM Products
WHERE ((ProductID = @p1) OR (CategoryID < @p2)))
@p1: (Int32), 5
@p2: (Int32), 3
");
    }


    [TestMethod]
    public void Test_LINQ_加载实体只加载个别字段()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == _f5 || t.CategoryID < _f3
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
    public void Test_LINQ_ORDER()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == P5
                        orderby t.ProductID, t.CategoryID descending, t.Quantity, t.UnitPrice descending
                        select t;

            List<Product> list = query.ToList();
        }
        AssertLastExecuteSQL(@"
SELECT *
FROM Products
WHERE (ProductID = @p1)
ORDER BY ProductID,CategoryID DESC,Quantity,UnitPrice DESC
@p1: (Int32), 5
");
    }


    [TestMethod]
    public void Test_LINQ_ORDER_2()
    {
        using( DbContext db = DbContext.Create() ) {
            db.EnableDelimiter = true;

            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == P5
                        orderby t.ProductID, t.CategoryID descending, t.Quantity, t.UnitPrice descending
                        select t;

            List<Product> list = query.ToList();
        }
        AssertLastExecuteSQL(@"
SELECT *
FROM `Products`
WHERE (`ProductID` = @p1)
ORDER BY `ProductID`,`CategoryID` DESC,`Quantity`,`UnitPrice` DESC
@p1: (Int32), 5
");
    }


    [TestMethod]
    public void Test_Expression_ORDER()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == 5
                        select t;

            query = query
                .Where(x => x.CategoryID < 3)
                .OrderBy(x => x.ProductID)
                .OrderByDescending(x => x.ProductName)
                .ThenByDescending(x => x.UnitPrice)
                .ThenBy(x => x.Quantity)
                .Where(x => x.Quantity > 5);

            List<Product> list = query.ToList();
        }
        AssertLastExecuteSQL(@"
SELECT *
FROM Products
WHERE (Quantity > @p1) AND (CategoryID < @p2) AND (ProductID = @p3)
ORDER BY ProductID,ProductName DESC,UnitPrice DESC,Quantity
@p1: (Int32), 5
@p2: (Int32), 3
@p3: (Int32), 5
");
    }


    [TestMethod]
    public void Test_LINQ_WithNoLock()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == P5 || t.CategoryID < P3
                        select t;

            List<Product> list = query.ToList();
        }
        AssertLastExecuteSQL(@"
SELECT *
FROM Products
WHERE ((ProductID = @p1) OR (CategoryID < @p2))
@p1: (Int32), 5
@p2: (Int32), 3
");
    }



    [TestMethod]
    public void Test_Exception()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            var x = new EntityLinqProvider(null, typeof(Product));
        });


        using( DbContext db = DbContext.Create() ) {

            var x = new EntityLinqProvider(db, typeof(Product));

            MyAssert.IsError<NotImplementedException>(() => {
                var x2 = x.CreateQuery(null);
            });

            MyAssert.IsError<NotImplementedException>(() => {
                var x2 = x.Execute(null);
            });
        }
    }


}
