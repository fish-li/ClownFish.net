namespace ClownFish.UnitTest.Data.EntityX;

[Obsolete]
[TestClass]
public class CURDTest : BaseTest
{
    [TestMethod]        
    public void Test_Entity_CURD()
    {
        Product product = new Product {
            ProductName = "EntityExtensionsTest_Test_Entity_Insert",
            CategoryID = 2,
            Quantity = 123
        };
        product.LoadDefaultValues();

        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                this.ResetCPQueryParamIndex();
                Console.WriteLine("ConnectionString: " + db.Connection.ConnectionString);
                Console.WriteLine("Database Type: " + db.DatabaseType);

                // insert
                int newId = (int)product.Insert(db, true);

                if( db.DatabaseType == DatabaseType.SQLSERVER)
                    AssertLastQuery(@"INSERT INTO Products(ProductName,CategoryID,Unit,UnitPrice,Remark,Quantity) VALUES (@p1,@p2,@p3,@p4,@p5,@p6); SELECT SCOPE_IDENTITY();");
                else if (db.DatabaseType == DatabaseType.MySQL)
                    AssertLastQuery(@"INSERT INTO Products(ProductName,CategoryID,Unit,UnitPrice,Remark,Quantity) VALUES (@p1,@p2,@p3,@p4,@p5,@p6); SELECT LAST_INSERT_ID();");
                else if( db.DatabaseType == DatabaseType.PostgreSQL)
                    AssertLastQuery(@"INSERT INTO Products(ProductName,CategoryID,Unit,UnitPrice,Remark,Quantity) VALUES (@p1,@p2,@p3,@p4,@p5,@p6); SELECT lastval();");
                else if( db.DatabaseType == DatabaseType.DaMeng)
                    AssertLastQuery(@"INSERT INTO Products(ProductName,CategoryID,Unit,UnitPrice,Remark,Quantity) VALUES (:p1,:p2,:p3,:p4,:p5,:p6); SELECT IDENT_CURRENT('Products');");

                Assert.IsTrue(newId > 0);
                Console.WriteLine("newId: " + newId);


                // query
                Product product2 = db.GetByKey<Product>(newId);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=:p7");
                else
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=@p7");

                Assert.IsNotNull(product2);
                Assert.AreEqual(newId, product2.ProductID);
                Assert.AreEqual("EntityExtensionsTest_Test_Entity_Insert", product2.ProductName);
                Assert.AreEqual(2, product2.CategoryID);
                Assert.AreEqual(123, product2.Quantity);
                // 下面三个值是实体类型上指定的默认值
                Assert.AreEqual("只", product2.Unit);
                Assert.AreEqual("xxx", product2.Remark);
                Assert.AreEqual(147.36m, product2.UnitPrice);


                Product product3 = new Product {
                    ProductID = newId,
                    CategoryID = product2.CategoryID,
                    ProductName = "EntityExtensionsTest_xxxxxxxx",
                    Quantity = 1233,
                    Unit = "AA",
                    UnitPrice = 365.23m
                };

                // update
                int effect = db.Update(product3);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"UPDATE Products SET  ProductName=:p8, CategoryID=:p9, Unit=:p10, UnitPrice=:p11, Quantity=:p12 WHERE ProductID = :p13");
                else
                    AssertLastQuery(@"UPDATE Products SET  ProductName=@p8, CategoryID=@p9, Unit=@p10, UnitPrice=@p11, Quantity=@p12 WHERE ProductID = @p13");

                Assert.AreEqual(1, effect);

                Product product4 = db.GetByKey<Product>(newId);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=:p14");
                else
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=@p14");

                Assert.IsNotNull(product4);
                Assert.AreEqual("EntityExtensionsTest_xxxxxxxx", product4.ProductName);
                Assert.AreEqual(1233, product4.Quantity);
                Assert.AreEqual(365.23m, product4.UnitPrice);
                Assert.AreEqual("AA", product4.Unit);


                // delete
                db.Delete<Product>(newId);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"DELETE FROM Products WHERE  ProductID=:p15");
                else
                    AssertLastQuery(@"DELETE FROM Products WHERE  ProductID=@p15");

                Product product5 = db.GetByKey<Product>(newId);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=:p16");
                else
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=@p16");

                Assert.IsNull(product5);
            }
        }
    }





    [TestMethod]
    public void Test_Entity_CURD2()
    {
        Product product = new Product {
            ProductName = "EntityExtensionsTest_Test_Entity_Insert",
            CategoryID = 2,
            Quantity = 123
        };
        product.LoadDefaultValues();

        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                this.ResetCPQueryParamIndex();
                Console.WriteLine("ConnectionString: " + db.Connection.ConnectionString);
                Console.WriteLine("Database Type: " + db.DatabaseType);

                // insert
                int newId = (int)product.Insert(db, true);

                if( db.DatabaseType == DatabaseType.SQLSERVER )
                    AssertLastQuery(@"INSERT INTO Products(ProductName,CategoryID,Unit,UnitPrice,Remark,Quantity) VALUES (@p1,@p2,@p3,@p4,@p5,@p6); SELECT SCOPE_IDENTITY();");
                else if( db.DatabaseType == DatabaseType.MySQL )
                    AssertLastQuery(@"INSERT INTO Products(ProductName,CategoryID,Unit,UnitPrice,Remark,Quantity) VALUES (@p1,@p2,@p3,@p4,@p5,@p6); SELECT LAST_INSERT_ID();");
                else if( db.DatabaseType == DatabaseType.PostgreSQL )
                    AssertLastQuery(@"INSERT INTO Products(ProductName,CategoryID,Unit,UnitPrice,Remark,Quantity) VALUES (@p1,@p2,@p3,@p4,@p5,@p6); SELECT lastval();");
                else if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"INSERT INTO Products(ProductName,CategoryID,Unit,UnitPrice,Remark,Quantity) VALUES (:p1,:p2,:p3,:p4,:p5,:p6); SELECT IDENT_CURRENT('Products');");


                Assert.IsTrue(newId > 0);
                Console.WriteLine("newId: " + newId);


                // query
                Product product2 = db.Entity.GetByKey<Product>(newId);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=:p7");
                else
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=@p7");

                Assert.IsNotNull(product2);
                Assert.AreEqual(newId, product2.ProductID);
                Assert.AreEqual("EntityExtensionsTest_Test_Entity_Insert", product2.ProductName);
                Assert.AreEqual(2, product2.CategoryID);
                Assert.AreEqual(123, product2.Quantity);
                // 下面三个值是实体类型上指定的默认值
                Assert.AreEqual("只", product2.Unit);
                Assert.AreEqual("xxx", product2.Remark);
                Assert.AreEqual(147.36m, product2.UnitPrice);


                Product product3 = new Product {
                    ProductID = newId,
                    CategoryID = product2.CategoryID,
                    ProductName = "EntityExtensionsTest_xxxxxxxx",
                    Quantity = 1233,
                    Unit = "AA",
                    UnitPrice = 365.23m
                };

                // update
                int effect = db.Entity.Update(product3);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"UPDATE Products SET  ProductName=:p8, CategoryID=:p9, Unit=:p10, UnitPrice=:p11, Quantity=:p12 WHERE ProductID = :p13");
                else
                    AssertLastQuery(@"UPDATE Products SET  ProductName=@p8, CategoryID=@p9, Unit=@p10, UnitPrice=@p11, Quantity=@p12 WHERE ProductID = @p13");

                Assert.AreEqual(1, effect);

                Product product4 = db.Entity.GetByKey<Product>(newId);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=:p14");
                else
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=@p14");

                Assert.IsNotNull(product4);
                Assert.AreEqual("EntityExtensionsTest_xxxxxxxx", product4.ProductName);
                Assert.AreEqual(1233, product4.Quantity);
                Assert.AreEqual(365.23m, product4.UnitPrice);
                Assert.AreEqual("AA", product4.Unit);


                // delete
                db.Entity.Delete<Product>(newId);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"DELETE FROM Products WHERE  ProductID=:p15");
                else
                    AssertLastQuery(@"DELETE FROM Products WHERE  ProductID=@p15");

                Product product5 = db.Entity.GetByKey<Product>(newId);

                if( db.DatabaseType == DatabaseType.DaMeng )
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=:p16");
                else
                    AssertLastQuery(@"SELECT *  FROM Products WHERE  ProductID=@p16");

                Assert.IsNull(product5);
            }
        }
    }



    [TestMethod]
    public async Task Test_Entity_CURD2_MYSQL_Async()
    {
        Product product = new Product {
            ProductName = "EntityExtensionsTest_Test_Entity_Insert",
            CategoryID = 2,
            Quantity = 123
        };
        product.LoadDefaultValues();

        using( DbContext dbContext = DbContext.Create("mysql") ) {

            dbContext.EnableDelimiter = true;

            // insert
            int newId = (int)await product.InsertAsync(dbContext, true);
            AssertLastQuery(@"INSERT INTO `Products`(`ProductName`,`CategoryID`,`Unit`,`UnitPrice`,`Remark`,`Quantity`) VALUES (@p1,@p2,@p3,@p4,@p5,@p6); SELECT LAST_INSERT_ID();");
            Assert.IsTrue(newId > 0);
            Console.WriteLine(newId);


            // query
            Product product2 = await dbContext.Entity.GetByKeyAsync<Product>(newId);
            AssertLastQuery(@"SELECT *  FROM `Products` WHERE  `ProductID`=@p7");
            Assert.IsNotNull(product2);
            Assert.AreEqual(newId, product2.ProductID);
            Assert.AreEqual("EntityExtensionsTest_Test_Entity_Insert", product2.ProductName);
            Assert.AreEqual(2, product2.CategoryID);
            Assert.AreEqual(123, product2.Quantity);
            // 下面三个值是实体类型上指定的默认值
            Assert.AreEqual("只", product2.Unit);
            Assert.AreEqual("xxx", product2.Remark);
            Assert.AreEqual(147.36m, product2.UnitPrice);


            Product product3 = new Product {
                ProductID = newId,
                CategoryID = product2.CategoryID,
                ProductName = "EntityExtensionsTest_xxxxxxxx",
                Quantity = 1233,
                Unit = "AA",
                UnitPrice = 365.23m
            };

            // update
            int effect = await dbContext.Entity.UpdateAsync(product3);
            AssertLastQuery(@"UPDATE `Products` SET  `ProductName`=@p8, `CategoryID`=@p9, `Unit`=@p10, `UnitPrice`=@p11, `Quantity`=@p12 WHERE `ProductID` = @p13");
            Assert.AreEqual(1, effect);

            Product product4 = await dbContext.Entity.GetByKeyAsync<Product>(newId);
            AssertLastQuery(@"SELECT *  FROM `Products` WHERE  `ProductID`=@p14");
            Assert.IsNotNull(product4);
            Assert.AreEqual("EntityExtensionsTest_xxxxxxxx", product4.ProductName);
            Assert.AreEqual(1233, product4.Quantity);
            Assert.AreEqual(365.23m, product4.UnitPrice);
            Assert.AreEqual("AA", product4.Unit);


            // delete
            await dbContext.Entity.DeleteAsync<Product>(newId);
            AssertLastQuery(@"DELETE FROM `Products` WHERE  `ProductID`=@p15");

            Product product5 = await dbContext.Entity.GetByKeyAsync<Product>(newId);
            AssertLastQuery(@"SELECT *  FROM `Products` WHERE  `ProductID`=@p16");
            Assert.IsNull(product5);
        }
    }

}
