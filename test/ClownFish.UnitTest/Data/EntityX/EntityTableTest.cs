using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Data;
using System.Linq;

namespace ClownFish.UnitTest.Data.EntityX
{
	[TestClass]
    public class EntityTableTest : BaseTest
	{
        private static readonly string s_name = "temp_" + Guid.NewGuid().ToString("N");

        [TestMethod]
        public void Test_实体CUD_匿名委托风格()
        {
            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext db = DbContext.Create(conn) ) {

                    // 先删除之前测试可能遗留下来的数据
                    db.Entity.From<Product>().Where(x => x.ProductName = s_name).Delete();

                    // 插入一条记录
                    db.Entity.From<Product>()
                        .Set(p => { 
                                p.CategoryID = 1; 
                                p.ProductName = s_name;
                                p.Quantity = 100;
                                p.Unit = "x";
                                p.UnitPrice = 112;
                                p.Remark = "abcd";
                        })
                        .Insert();

                    // 检验刚才插入的数据行
                    Product p2 = db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingle();
                    Assert.IsNotNull(p2);
                    Assert.AreEqual(s_name, p2.ProductName);

                    // 更新数据行
                    db.Entity.From<Product>()
                        .Set(p => { p.Unit = "x2"; p.UnitPrice = 222; })
                        .Where(x => x.ProductName = s_name)
                        .Update();

                    // 检验刚才更新的数据行，本次查询只加载部分字段
                    Product p4 = db.Entity.From<Product>()
                        .Where(x => x.ProductName = s_name)
                        .ToSingle();
                    Assert.IsNotNull(p4);
                    Assert.AreEqual("x2", p4.Unit);
                    Assert.AreEqual(222, p4.UnitPrice);


                    // 再次插入一条记录
                    db.Entity.From<Product>()
                        .Set(p => {
                            p.CategoryID = 2;
                            p.ProductName = s_name;
                            p.Quantity = 200;
                            p.Unit = "y";
                            p.UnitPrice = 333;
                            p.Remark = "1qaz";
                        })
                        .Insert();

                    // 查询列表，应该包含本次测试插入的二条记录
                    List<Product> list = db.Entity.From<Product>()
                        .Where(x => x.ProductName = s_name)
                        .ToList()
                        .OrderBy(x => x.ProductID).ToList();

                    Assert.IsNotNull(list);
                    Assert.AreEqual(2, list.Count);
                    Assert.AreEqual(1, list[0].CategoryID);
                    Assert.AreEqual(2, list[1].CategoryID);
                    Assert.AreEqual(100, list[0].Quantity);
                    Assert.AreEqual(200, list[1].Quantity);
                    Assert.AreEqual("x2", list[0].Unit);
                    Assert.AreEqual("y", list[1].Unit);
                    Assert.AreEqual(222, list[0].UnitPrice);
                    Assert.AreEqual(333, list[1].UnitPrice);
                    Assert.AreEqual("abcd", list[0].Remark);
                    Assert.AreEqual("1qaz", list[1].Remark);

                    // 删除记录，应该删除2条记录
                    int rows = db.Entity.From<Product>().Where(x => x.ProductName = s_name).Delete();
                    Assert.AreEqual(2, rows);

                    // 再查询一次，应该是没有记录了
                    Product p6 = db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingle();
                    Assert.IsNull(p6);
                }
            }
        }


		[TestMethod]
		public async Task Test_实体CUD_匿名委托风格_Async()
		{
            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext db = DbContext.Create(conn) ) {

                    ShowCurrentThread();

                    // 先删除之前测试可能遗留下来的数据
                    await db.Entity.From<Product>().Where(x => x.ProductName = s_name).DeleteAsync();
                    ShowCurrentThread();

                    // 插入一条记录
                    await db.Entity.From<Product>()
                        .Set(p => {
                            p.CategoryID = 1;
                            p.ProductName = s_name;
                            p.Quantity = 100;
                            p.Unit = "x";
                            p.UnitPrice = 112;
                            p.Remark = "abcd";
                        })
                        .InsertAsync();
                    ShowCurrentThread();

                    // 检验刚才插入的数据行
                    Product p2 = await db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingleAsync();
                    Assert.IsNotNull(p2);
                    Assert.AreEqual(s_name, p2.ProductName);
                    ShowCurrentThread();

                    // 更新数据行
                    await db.Entity.From<Product>()
                        .Set(p => { p.Unit = "x2"; p.UnitPrice = 222; })
                        .Where(x => x.ProductName = s_name)
                        .UpdateAsync();
                    ShowCurrentThread();

                    // 检验刚才更新的数据行，本次查询只加载部分字段
                    Product p4 = await db.Entity.From<Product>()
                        .Where(x => x.ProductName = s_name)
                        .ToSingleAsync();
                    Assert.IsNotNull(p4);
                    Assert.AreEqual("x2", p4.Unit);
                    Assert.AreEqual(222, p4.UnitPrice);
                    ShowCurrentThread();


                    // 再次插入一条记录
                    await db.Entity.From<Product>()
                        .Set(p => {
                            p.CategoryID = 2;
                            p.ProductName = s_name;
                            p.Quantity = 200;
                            p.Unit = "y";
                            p.UnitPrice = 333;
                            p.Remark = "1qaz";
                        })
                        .InsertAsync();
                    ShowCurrentThread();

                    // 查询列表，应该包含本次测试插入的二条记录
                    // 查询列表，应该包含本次测试插入的二条记录
                    List<Product> list = await db.Entity.From<Product>()
                        .Where(x => x.ProductName = s_name)
                        .ToListAsync();                    
                    ShowCurrentThread();

                    Assert.IsNotNull(list);
                    Assert.AreEqual(2, list.Count);

                    list = list.OrderBy(x=>x.CategoryID).ToList();
                    Assert.AreEqual(1, list[0].CategoryID);
                    Assert.AreEqual(2, list[1].CategoryID);
                    Assert.AreEqual(100, list[0].Quantity);
                    Assert.AreEqual(200, list[1].Quantity);
                    Assert.AreEqual("x2", list[0].Unit);
                    Assert.AreEqual("y", list[1].Unit);
                    Assert.AreEqual(222, list[0].UnitPrice);
                    Assert.AreEqual(333, list[1].UnitPrice);
                    Assert.AreEqual("abcd", list[0].Remark);
                    Assert.AreEqual("1qaz", list[1].Remark);

                    // 根据“IntField = 1978”来删除，应该删除2条记录
                    // 删除记录，应该删除2条记录
                    int rows = await db.Entity.From<Product>().Where(x => x.ProductName = s_name).DeleteAsync();
                    Assert.AreEqual(2, rows);
                    ShowCurrentThread();

                    // 再查询一次，应该是没有记录了
                    Product p6 = await db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingleAsync();
                    Assert.IsNull(p6);
                    ShowCurrentThread();
                }
            }

		}


        [TestMethod]
        public void Test_Ctor_Error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                var x = new EntityTable<Customer>(null);
            });
        }


        [TestMethod]
        public void Test_Set_Error()
        {
            using( DbContext db = DbContext.Create() ) {

                EntityTable<Customer> et = new EntityTable<Customer>(db);

                MyAssert.IsError<ArgumentNullException>(() => {
                    et.Set(null);
                });

                et.Set(x => x.CustomerID = 2);  // 第一次，OK

                MyAssert.IsError<InvalidOperationException>(() => {
                    et.Set(x => x.CustomerID = 22);  // 第二次
                });
            }
        }

        [TestMethod]
        public void Test_Where_Error()
        {
            using( DbContext db = DbContext.Create() ) {

                EntityTable<Customer> et = new EntityTable<Customer>(db);

                MyAssert.IsError<ArgumentNullException>(() => {
                    et.Where(null);
                });

                et.Where(x => x.CustomerID = 2);  // 第一次，OK

                MyAssert.IsError<InvalidOperationException>(() => {
                    et.Where(x => x.CustomerID = 22);  // 第二次
                });
            }
        }


        [TestMethod]
        public void Test_GetInsertQuery_Error()
        {
            using( DbContext db = DbContext.Create() ) {

                EntityTable<Customer> et = new EntityTable<Customer>(db);

                MyAssert.IsError<InvalidOperationException>(() => {
                    et.Insert();
                });
            }
        }

        [TestMethod]
        public void Test_GetDeleteQuery_Error()
        {
            using( DbContext db = DbContext.Create() ) {

                EntityTable<Customer> et = new EntityTable<Customer>(db);

                MyAssert.IsError<InvalidOperationException>(() => {
                    et.Delete();
                });
            }
        }

        [TestMethod]
        public void Test_GeUpdateQuery_Error()
        {
            using( DbContext db = DbContext.Create() ) {

                EntityTable<Customer> et = new EntityTable<Customer>(db);

                MyAssert.IsError<InvalidOperationException>(() => {
                    et.Update();
                });
            }
        }

        [TestMethod]
        public void Test_GetSelectQuery_Error()
        {
            using( DbContext db = DbContext.Create() ) {

                EntityTable<Customer> et = new EntityTable<Customer>(db);

                MyAssert.IsError<InvalidOperationException>(() => {
                    et.ToList();
                });
            }
        }


    }
}
