namespace ClownFish.UnitTest.Data.EntityX;

[TestClass]
public class EntityTest : BaseTest
{
    [TestMethod]
    public void Test_DbContext()
    {
        using( DbContext dbContext = DbContext.Create() ) {
            Category c1 = dbContext.Entity.CreateProxy<Category>();
            IEntityProxy proxy = c1 as IEntityProxy;
            Assert.IsNotNull(proxy.DbContext);
            Assert.AreEqual(dbContext, proxy.DbContext);
        }

    }


    [TestMethod]
    public void Test_GetProxy()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            Category c3 = (Category)c2.CreateProxy(dbContext);
            IEntityProxy proxy = c3 as IEntityProxy;
            Assert.AreEqual(dbContext, proxy.DbContext);

            Assert.AreEqual(c2.CategoryID, c3.CategoryID);
            Assert.AreEqual(c2.CategoryName, c3.CategoryName);


            MyAssert.IsError<ArgumentNullException>(() => {
                Category c4 = (Category)c2.CreateProxy(null);
            });
        }
    }

    private class Xx2Entity : Entity { }

    [TestMethod]
    public void Test_GetProxy2()
    {
        using( DbContext dbContext = DbContext.Create() ) {
            Customer c1 = new Customer();

            Customer c2 = c1.CreateProxy(dbContext) as Customer;
            Assert.IsNotNull(c2);

            MyAssert.IsError<ArgumentNullException>(() => {
                Customer c3 = c1.CreateProxy(null) as Customer;
            });

            MyAssert.IsError<NotImplementedException>(() => {
                Xx2Entity x2 = new Xx2Entity();
                var x3 = x2.CreateProxy(dbContext);
            });
        }
    }




    [TestMethod]
    public void Test_GetInsertQuery()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            Category c3 = dbContext.Entity.CreateProxy(c2);
            CPQuery query1 = c3.GetInsertQuery0();
            Assert.IsNull(query1);


            c3.CategoryName = null;
            CPQuery query2 = c3.GetInsertQuery0();
            Assert.IsNotNull(query2);

            string sql = query2.ToString();
            Console.WriteLine(sql);
            MyAssert.SqlAreEqual("INSERT INTO Categories(CategoryName) VALUES (NULL)", sql.Trim());
        }
    }


    [TestMethod]
    public void Test_GetInsertQueryCommand()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            MyAssert.IsError<InvalidOperationException>(() => {
                CPQuery queryx = c2.GetInsertQueryCommand();
            });


            Category c3 = dbContext.Entity.CreateProxy(c2);
            CPQuery query1 = c3.GetInsertQueryCommand();
            Assert.IsNull(query1);


            c3.CategoryName = null;
            CPQuery query2 = c3.GetInsertQueryCommand();
            Assert.IsNotNull(query2);

            // 上次成功调用后，将所有属性变量标记清空了，所以下面再次调用的结果就不一样了

            CPQuery query3 = c3.GetInsertQueryCommand();
            Assert.IsNull(query3);
        }
    }


    [TestMethod]
    public void Test_Insert()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            Category c3 = dbContext.Entity.CreateProxy(c2);
            int result = c3.Insert();
            Assert.AreEqual(-1, result);
        }
    }


    [TestMethod]
    public async Task Test_InsertAsync()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            Category c3 = dbContext.Entity.CreateProxy(c2);
            int result = await c3.InsertAsync();
            Assert.AreEqual(-1, result);
        }
    }



    [TestMethod]
    public void Test_GetWhereQuery()
    {
        this.ResetCPQueryParamIndex();

        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            Category c3 = dbContext.Entity.CreateProxy(c2);
            CPQuery query1 = c3.GetWhereQuery0();
            Assert.IsNull(query1);


            c3.CategoryID = 1000;
            c3.CategoryName = null;
            CPQuery query2 = c3.GetWhereQuery0();
            Assert.IsNotNull(query2);

            string sql = query2.ToString();
            Console.WriteLine(sql);
            Assert.AreEqual("WHERE  CategoryID=@p1 AND  CategoryName=NULL", sql.Trim());
        }
    }


    [TestMethod]
    public void Test_GetDeleteQueryCommand()
    {
        this.ResetCPQueryParamIndex();

        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };


            MyAssert.IsError<InvalidOperationException>(() => {
                c2.GetDeleteQueryCommand();
            });

            Category c3 = dbContext.Entity.CreateProxy(c2);
            CPQuery query1 = c3.GetDeleteQueryCommand();
            Assert.IsNull(query1);


            c3.CategoryID = 1000;
            CPQuery query2 = c3.GetDeleteQueryCommand();
            Assert.IsNotNull(query2);

            string sql = query2.ToString();
            Console.WriteLine(sql);
            MyAssert.SqlAreEqual("DELETE FROM Categories WHERE  CategoryID=@p1", sql.Trim());
        }
    }




    [TestMethod]
    public void Test_Delete()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            MyAssert.IsError<InvalidOperationException>(() => {
                c2.Delete();
            });

            Category c3 = dbContext.Entity.CreateProxy(c2);
            int result = c3.Delete();
            Assert.AreEqual(-1, result);
        }
    }


    [TestMethod]
    public async Task Test_DeleteAsync()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            Category c3 = dbContext.Entity.CreateProxy(c2);
            int result = await c3.DeleteAsync();
            Assert.AreEqual(-1, result);
        }
    }



    [TestMethod]
    public void Test_GetUpdateQuery()
    {
        this.ResetCPQueryParamIndex();

        using( DbContext dbContext = DbContext.Create() ) {

            Customer c2 = new Customer {
                CustomerID = 1234567,
                CustomerName = "aaaaaaaaaa",
                ContactName = "bbbbbbbbbbbb",
                Address = "ccccccccccc",
                PostalCode = "430076",
                Tel = "13812345678"
            };

            Customer c3 = dbContext.Entity.CreateProxy(c2);
            var rowKey = ((IEntityProxy)c3).GetRowKey();

            // 先不做任何属性更新，直接调用 GetUpdateQuery
            CPQuery query1 = c3.GetUpdateQuery0(rowKey);
            Assert.IsNull(query1);


            c3.CustomerID = 1000;  // 只设置了主键字段，这样的更新是无意义的
            CPQuery query2 = c3.GetUpdateQuery0(rowKey);
            Assert.IsNull(query2);

            c3.ContactName = null;
            CPQuery query3 = c3.GetUpdateQuery0(rowKey);
            Assert.IsNotNull(query3);


            string sql = query3.ToString();
            Console.WriteLine(sql);
            MyAssert.SqlAreEqual("UPDATE Customers SET  ContactName=NULL", sql.Trim());
        }
    }



    [TestMethod]
    public void Test_GetUpdateQueryCommand()
    {
        this.ResetCPQueryParamIndex();

        using( DbContext dbContext = DbContext.Create() ) {

            Customer c2 = new Customer {
                CustomerID = 1234567,
                CustomerName = "aaaaaaaaaa",
                ContactName = "bbbbbbbbbbbb",
                Address = "ccccccccccc",
                PostalCode = "430076",
                Tel = "13812345678"
            };


            MyAssert.IsError<InvalidOperationException>(() => {
                c2.GetUpdateQueryCommand();
            });


            Customer c3 = dbContext.Entity.CreateProxy(c2);
            var rowKey = ((IEntityProxy)c3).GetRowKey();

            // 先不做任何属性更新，直接调用 GetUpdateQuery
            CPQuery query1 = c3.GetUpdateQueryCommand();
            Assert.IsNull(query1);


            c3.CustomerID = 1000;  // 只设置了主键字段，这样的更新是无意义的
            CPQuery query2 = c3.GetUpdateQueryCommand();
            Assert.IsNull(query2);

            c3.ContactName = null;
            CPQuery query3 = c3.GetUpdateQueryCommand();
            Assert.IsNotNull(query3);


            string sql = query3.ToString();
            Console.WriteLine(sql);
            MyAssert.SqlAreEqual("UPDATE Customers SET  ContactName=NULL WHERE CustomerID = @p1", sql.Trim());
        }
    }



    [TestMethod]
    public void Test_Update()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            MyAssert.IsError<InvalidOperationException>(() => {
                c2.Update();
            });

            Category c3 = dbContext.Entity.CreateProxy(c2);
            int result = c3.Update();
            Assert.AreEqual(-1, result);
        }
    }


    [TestMethod]
    public async Task Test_UpdateAsync()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category {
                CategoryID = 22,
                CategoryName = "ab c"
            };

            Category c3 = dbContext.Entity.CreateProxy(c2);
            int result = await c3.UpdateAsync();
            Assert.AreEqual(-1, result);
        }
    }

    [TestMethod]
    public void Test_GetTableName()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            Category c2 = new Category();
            Assert.AreEqual("Categories", c2.GetTableName());

            Category c3 = dbContext.Entity.CreateProxy(c2);
            Assert.AreEqual("Categories", c3.GetTableName());
        }
    }


    [TestMethod]
    public void Test_GetIdentity()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            OrderDetailX3 x1 = new OrderDetailX3();
            Assert.AreEqual("NewID", x1.GetIdentity().PropertyInfo.Name);

            OrderDetailX3 x2 = dbContext.Entity.CreateProxy(x1);
            Assert.AreEqual("NewID", x2.GetIdentity().PropertyInfo.Name);
        }
    }


    [TestMethod]
    public void Test_GetPrimaryKey()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            OrderDetailX3 x1 = new OrderDetailX3();
            Assert.AreEqual("RowId", x1.GetPrimaryKey().PropertyInfo.Name);

            OrderDetailX3 x2 = dbContext.Entity.CreateProxy(x1);
            Assert.AreEqual("RowId", x2.GetPrimaryKey().PropertyInfo.Name);
        }
    }


    [TestMethod]
    public void Test_LoadDefaultValues()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            OrderDetailX3 x1 = new OrderDetailX3();
            x1.LoadDefaultValues();
            Assert.AreEqual(2.34m, x1.UnitPrice);
            Assert.AreEqual(123, x1.Quantity);
            Assert.AreEqual("abcd", x1.Remark);

            OrderDetailX3 x2 = dbContext.Entity.CreateProxy(x1);
            x2.LoadDefaultValues();
            Assert.AreEqual(2.34m, x2.UnitPrice);
            Assert.AreEqual(123, x2.Quantity);
            Assert.AreEqual("abcd", x2.Remark);
        }
    }


    [TestMethod]
    public void Test_Update_Delete_where()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            dbContext.Entity.Update<Product>(
               /* set */    x => { x.Unit = "x"; x.Quantity = 9; },
               /* where */  x => x.UnitPrice < 12 && x.CategoryID == 0);

            AssertLastExecuteSQL(@"
UPDATE Products SET  Unit=@p1, Quantity=@p2
WHERE ((UnitPrice < @p3) AND (CategoryID = @p4))
@p1: (String), x
@p2: (Int32), 9
@p3: (Decimal), 12
@p4: (Int32), 0
");


            dbContext.Entity.Delete<Product>(x => x.UnitPrice < 12 && x.CategoryID == 0);

            AssertLastExecuteSQL(@"
DELETE FROM Products
WHERE ((UnitPrice < @p5) AND (CategoryID = @p6))
@p5: (Decimal), 12
@p6: (Int32), 0
");
        }
    }


    [TestMethod]
    public void Test_Update_Delete_where_2()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            // throw new InvalidOperationException("没有任何 set 操作！");
            MyAssert.IsError<InvalidOperationException>(() => {
                dbContext.Entity.Update<Product>(x => { x.Quantity = 9; }, x => true);
            });


            // throw new InvalidOperationException("不允许执行没有任何 查询条件 的 UPDATE ！");
            MyAssert.IsError<InvalidOperationException>(() => {
                dbContext.Entity.Update<Product>(x => { }, x => x.UnitPrice < 12 && x.CategoryID == 0);
            });


            // throw new InvalidOperationException("不允许执行没有任何 查询条件 的 DELETE ！");
            MyAssert.IsError<InvalidOperationException>(() => {
                dbContext.Entity.Delete<Product>(x => true);
            });
        }
    }


    [TestMethod]
    public async Task Test_Update_Delete_where_async()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            await dbContext.Entity.UpdateAsync<Product>(
               /* set */    x => { x.Unit = "x"; x.Quantity = 9; },
               /* where */  x => x.UnitPrice < 12 && x.CategoryID == 0);

            AssertLastExecuteSQL(@"
UPDATE Products SET  Unit=@p1, Quantity=@p2
WHERE ((UnitPrice < @p3) AND (CategoryID = @p4))
@p1: (String), x
@p2: (Int32), 9
@p3: (Decimal), 12
@p4: (Int32), 0
");

            await dbContext.Entity.DeleteAsync<Product>(x => x.UnitPrice < 12 && x.CategoryID == 0);

            AssertLastExecuteSQL(@"
DELETE FROM Products
WHERE ((UnitPrice < @p5) AND (CategoryID = @p6))
@p5: (Decimal), 12
@p6: (Int32), 0
");
        }
    }

}
