﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ClownFish.Data;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Data.EntityX
{
    [Obsolete]
    [TestClass]
    public class EntityExtensionsTest : BaseTest
    {
        [TestMethod]
        public void Test_InsertSync()
        {
            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext db = DbContext.Create(conn) ) {

                    Customer customer = new Customer();
                    customer.CustomerName = "Name_" + Guid.NewGuid().ToString("N");
                    customer.Address = "Address_" + Guid.NewGuid().ToString("N");
                    customer.ContactName = "Contact_" + Guid.NewGuid().ToString("N");
                    customer.PostalCode = "430076";
                    customer.Tel = "123456789";

                    // 确认这个这没有赋值
                    Assert.AreEqual(0, customer.CustomerID);

                    // 插入数据行，并获取自增ID
                    int id = (int)customer.Insert(db, true);
                    customer.CustomerID = id;


                    Assert.IsTrue(id > 0);

                    Customer customer2 = (from x in db.Entity.Query<Customer>()
                                          where x.CustomerID == id
                                          select x
                                          ).FirstOrDefault();

                    // 确认数据插入成功
                    Assert.IsNotNull(customer2);


                    // 确认数据有效
                    MyAssert.AreEqual(customer, customer2);
                }
            }

        }



        [TestMethod]
        public async Task Test_InsertAsync_Error()
        {
           await  MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
               using( DbContext dbContext = DbContext.Create("sqlserver") ) {
                   _ = await EntityExtensions.InsertAsync((Customer)null, dbContext, true);
               }
            });


            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                Customer customer = new Customer();
                _ = await EntityExtensions.InsertAsync(customer, null, true);
            });

            await MyAssert.IsErrorAsync<NotSupportedException>(async () => {
                using( DbContext dbContext = DbContext.Create("sqlserver") ) {
                    Customer customer = dbContext.Entity.BeginEdit<Customer>();
                    _ = await EntityExtensions.InsertAsync(customer, dbContext, true);
                }
            });

            await MyAssert.IsErrorAsync<NotSupportedException>(async () => {
                using( DbContext dbContext = DbContext.Create("sqlserver") ) {
                    Customer customer = dbContext.Entity.BeginEdit<Customer>();
                    long id = await EntityExtensions.InsertAsync(customer, dbContext, true);
                }
            });
        }


        [TestMethod]
        public async Task Test_UpdateAsync_Error()
        {
            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                using( DbContext dbContext = DbContext.Create("sqlserver") ) {
                    _ = await dbContext.Entity.UpdateAsync((Customer)null);
                }
            });

            await MyAssert.IsErrorAsync<NotSupportedException>(async () => {
                using( DbContext dbContext = DbContext.Create("sqlserver") ) {
                    Customer customer = dbContext.Entity.BeginEdit<Customer>();
                    _ = await dbContext.Entity.UpdateAsync(customer);
                }
            });
        }


        [TestMethod]
        public async Task Test_DeleteAsync_Error()
        {
            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                using( DbContext dbContext = DbContext.Create("sqlserver") ) {
                    _ = await dbContext.Entity.DeleteAsync<Customer>((Customer)null);
                }
            });

        }


        [TestMethod]
        public void Test_GetByKey_Error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                using( DbContext dbContext = DbContext.Create("sqlserver") ) {
                    _ = dbContext.Entity.GetByKey<Customer>(null);
                }
            });


            MyAssert.IsError<InvalidOperationException>(() => {
                using( DbContext dbContext = DbContext.Create("sqlserver") ) {
                    _ = dbContext.Entity.GetByKey<OrderDetailX2>(33);
                }
            });

        }


        [TestMethod]
        public async Task Test_GetByKeyAsync_Error()
        {
            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                using( DbContext dbContext = DbContext.Create("sqlserver") ) {
                    _ = await dbContext.Entity.GetByKeyAsync<Customer>(null);
                }
            });
        }


        [TestMethod]
        public void Test_GetInsertSQL()
        {
            ResetCPQueryParamIndex();

            using( DbContext dbContext = DbContext.Create() ) {

                Customer c1 = new Customer();

                string sql = EntityCudUtils.GetInsertSQL(c1, dbContext);
                Console.WriteLine(sql);
                Assert.AreEqual("insert into Customers ( CustomerName, ContactName, Address, PostalCode, Tel ) values ( @CustomerName, @ContactName, @Address, @PostalCode, @Tel )", sql.Trim());
            }
        }



        [TestMethod]
        public void Test_GetInsertSQL_Error()
        {
            ResetCPQueryParamIndex();

            using( DbContext dbContext = DbContext.Create() ) {

                MyAssert.IsError<ArgumentNullException>(() => {
                    Category c1 = null;
                    EntityCudUtils.GetInsertSQL(c1, dbContext);
                });

                MyAssert.IsError<ArgumentNullException>(() => {
                    Category c1 = new Category();
                    EntityCudUtils.GetInsertSQL(c1, null);
                });

                MyAssert.IsError<NotSupportedException>(() => {
                    Category c1 = dbContext.Entity.BeginEdit<Category>();
                    EntityCudUtils.GetInsertSQL(c1, dbContext);
                });

                Category c2 = new Category {
                    CategoryID = 123,
                    CategoryName = "abc"
                };
                string sql2 = EntityCudUtils.GetInsertSQL(c2, dbContext);
                Console.WriteLine(sql2);
                Assert.AreEqual("insert into Categories ( CategoryName ) values ( @CategoryName )", sql2.Trim());


                Category c3 = new Category {
                    CategoryID = 123,
                    CategoryName = null
                };
                string sql3 = EntityCudUtils.GetInsertSQL(c3, dbContext);
                Console.WriteLine(sql3);
                Assert.AreEqual("insert into Categories ( CategoryName ) values ( @CategoryName )", sql3.Trim());
            }
            
        }


        [TestMethod]
        public async Task Test_ArgumentNullException()
        {
            Customer customer = new Customer();

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = customer.Insert(null, false);
            });

            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                _ = await customer.InsertAsync(null, false);
            });


            MyAssert.IsError<ArgumentNullException>(() => {
                _ = customer.Insert2(null);
            });

            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                _ = await customer.Insert2Async(null);
            });


            MyAssert.IsError<ArgumentNullException>(() => {
                _ = customer.Update(null);
            });

            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                _ = await customer.UpdateAsync(null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = ((DbContext)null).Update(customer);
            });

            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                _ = await ((DbContext)null).UpdateAsync(customer);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = ((DbContext)null).Delete<Customer>(3);
            });

            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                _ = await ((DbContext)null).DeleteAsync<Customer>(3);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = ((DbContext)null).GetByKey<Customer>(3);
            });

            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                _ = await ((DbContext)null).GetByKeyAsync<Customer>(3);
            });
        }


    }
}

