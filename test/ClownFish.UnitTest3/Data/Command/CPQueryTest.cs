namespace ClownFish.UnitTest.Data.Command;

[TestClass]
public class CPQueryTest : BaseTest
{
    private static readonly string s_newName = Guid.NewGuid().ToString();


    [TestMethod]
    public void Test_CPQuery的基本CRUD操作()
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {
                db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                var newCustomer = new {
                    CustomerName = s_newName,
                    ContactName = Guid.NewGuid().ToString(),
                    Address = "111111 Address",
                    PostalCode = "111111",
                    Tel = "123456789"
                };

                // 插入一条记录
                db.CPQuery.Create(GetSql("InsertCustomer"), newCustomer).ExecuteNonQuery();

                // 读取刚插入的记录
                var queryArgument = new { CustomerName = s_newName };
                Customer customer = db.CPQuery.Create(GetSql("GetCustomerByName"), queryArgument).ToSingle<Customer>();

                // 验证插入与读取
                Assert.IsNotNull(customer);
                Assert.AreEqual(newCustomer.ContactName, customer.ContactName);





                // 准备更新数据
                Customer updateArgument = new Customer {
                    CustomerID = customer.CustomerID,
                    CustomerName = newCustomer.CustomerName,
                    ContactName = newCustomer.ContactName,
                    Address = Guid.NewGuid().ToString(),
                    PostalCode = newCustomer.PostalCode,
                    Tel = newCustomer.Tel
                };

                // 更新记录
                db.CPQuery.Create(GetSql("UpdateCustomer"), updateArgument).ExecuteNonQuery();

                // 读取刚更新的记录
                var queryArgument2 = new { CustomerID = customer.CustomerID };
                Customer customer2 = db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingle<Customer>();

                // 验证更新与读取
                Assert.IsNotNull(customer2);
                Assert.AreEqual(updateArgument.Address, customer2.Address);


                // 删除记录
                var deleteArgument = new { CustomerID = customer.CustomerID };
                db.CPQuery.Create(GetSql("DeleteCustomer"), deleteArgument).ExecuteNonQuery();

                // 验证删除			
                Customer customer3 = db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingle<Customer>();
                Assert.IsNull(customer3);

                db.Commit();
            }
        }
    }


    [TestMethod]
    public async Task Test_CPQuery的基本CRUD操作_Async()
    {
        ShowCurrentThread();

        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {
                db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                var newCustomer = new {
                    CustomerName = s_newName,
                    ContactName = Guid.NewGuid().ToString(),
                    Address = "111111 Address",
                    PostalCode = "111111",
                    Tel = "123456789"
                };

                ShowCurrentThread();
                // 插入一条记录
                await db.CPQuery.Create(GetSql("InsertCustomer"), newCustomer).ExecuteNonQueryAsync();
                ShowCurrentThread();

                // 读取刚插入的记录
                var queryArgument = new { CustomerName = s_newName };
                Customer customer = await db.CPQuery.Create(GetSql("GetCustomerByName"), queryArgument).ToSingleAsync<Customer>();

                // 验证插入与读取
                Assert.IsNotNull(customer);
                Assert.AreEqual(newCustomer.ContactName, customer.ContactName);





                // 准备更新数据
                Customer updateArgument = new Customer {
                    CustomerID = customer.CustomerID,
                    CustomerName = newCustomer.CustomerName,
                    ContactName = newCustomer.ContactName,
                    Address = Guid.NewGuid().ToString(),
                    PostalCode = newCustomer.PostalCode,
                    Tel = newCustomer.Tel
                };

                // 更新记录
                await db.CPQuery.Create(GetSql("UpdateCustomer"), updateArgument).ExecuteNonQueryAsync();

                // 读取刚更新的记录
                var queryArgument2 = new { CustomerID = customer.CustomerID };
                Customer customer2 = await db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingleAsync<Customer>();

                // 验证更新与读取
                Assert.IsNotNull(customer2);
                Assert.AreEqual(updateArgument.Address, customer2.Address);


                // 删除记录
                var deleteArgument = new { CustomerID = customer.CustomerID };
                await db.CPQuery.Create(GetSql("DeleteCustomer"), deleteArgument).ExecuteNonQueryAsync();

                // 验证删除			
                Customer customer3 = await db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingleAsync<Customer>();
                Assert.IsNull(customer3);

                db.Commit();
            }
        }
    }


    [TestMethod]
    public async Task Test_CPQuery加载实体列表()
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                string sql = GetSql("GetCustomerList");
                var args = new { MaxCustomerID = 100 };

                List<Customer> list1 = db.CPQuery.Create(sql, args).ToList<Customer>();

                Assert.IsNotNull(list1);



                List<Customer> list2 = await db.CPQuery.Create(sql, args).ToListAsync<Customer>();

                Assert.IsNotNull(list2);


                MyAssert.AreEqual(list1, list2);
            }
        }
    }


    [TestMethod]
    public async Task Test_CPQuery_分页加载数据()
    {
        DataTable table1 = null;
        DataTable table2 = null;

        List<Customer> list1 = null;
        List<Customer> list2 = null;

        string sql = GetSql("GetCustomerList");
        var args = new { MaxCustomerID = 100 };

        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                PagingInfo pagingInfo = new PagingInfo() {
                    PageIndex = 0,
                    PageSize = 20
                };


                pagingInfo.TotalRows = -1;
                table1 = db.CPQuery.Create(sql, args).ToPageTable(pagingInfo);

                Assert.IsNotNull(table1);
                Assert.IsTrue(pagingInfo.TotalRows >= 0);



                pagingInfo.TotalRows = -1;
                list1 = db.CPQuery.Create(sql, args).ToPageList<Customer>(pagingInfo);

                Assert.IsNotNull(list1);
                Assert.IsTrue(pagingInfo.TotalRows >= 0);




                pagingInfo.TotalRows = -1;
                table2 = await db.CPQuery.Create(sql, args).ToPageTableAsync(pagingInfo);

                Assert.IsNotNull(table2);
                Assert.IsTrue(pagingInfo.TotalRows >= 0);



                pagingInfo.TotalRows = -1;
                list2 = await db.CPQuery.Create(sql, args).ToPageListAsync<Customer>(pagingInfo);

                Assert.IsNotNull(list2);
                Assert.IsTrue(pagingInfo.TotalRows >= 0);


                MyAssert.AreEqual(table1, table2);
                MyAssert.AreEqual(list1, list2);
            }
        }
    }


    [TestMethod]
    public async Task Test_CPQuery_数据导出()
    {
        DataTable table1 = null;
        DataTable table2 = null;

        List<Customer> list1 = null;
        List<Customer> list2 = null;

        string sql = GetSql("GetCustomerList");
        var args = new { MaxCustomerID = 100 };

        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                PagingInfo pagingInfo = new PagingInfo() {
                    PageIndex = 0,
                    PageSize = 20,
                    NeedCount = false
                };


                pagingInfo.TotalRows = -1;
                table1 = db.CPQuery.Create(sql, args).ToPageTable(pagingInfo);

                Assert.IsNotNull(table1);
                Assert.AreEqual(-1, pagingInfo.TotalRows);  // 确保没有修改过这个属性



                pagingInfo.TotalRows = -1;
                list1 = db.CPQuery.Create(sql, args).ToPageList<Customer>(pagingInfo);

                Assert.IsNotNull(list1);
                Assert.AreEqual(-1, pagingInfo.TotalRows);




                pagingInfo.TotalRows = -1;
                table2 = await db.CPQuery.Create(sql, args).ToPageTableAsync(pagingInfo);

                Assert.IsNotNull(table2);
                Assert.AreEqual(-1, pagingInfo.TotalRows);



                pagingInfo.TotalRows = -1;
                list2 = await db.CPQuery.Create(sql, args).ToPageListAsync<Customer>(pagingInfo);

                Assert.IsNotNull(list2);
                Assert.AreEqual(-1, pagingInfo.TotalRows);


                MyAssert.AreEqual(table1, table2);
                MyAssert.AreEqual(list1, list2);
            }
        }
    }

    [TestMethod]
    public void Test_CPQuery参数支持INT数组()
    {
        string sql = @"select * from dbo.Customers where CustomerID in ({CustomerID})";

        int[] customerIdArray = { 1, 2, 3, 4, 5 };
        var args = new { CustomerID = customerIdArray };

        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                CPQuery query = db.CPQuery.Create(sql, args);

                string commandText = query.ToString();
                Console.WriteLine(commandText);

                // 断言占位符已被替换
                Assert.AreEqual(
                    // 注意：int[] 不会生成命令参数，将直接输出到SQL中
                    "select * from dbo.Customers where CustomerID in (1,2,3,4,5)",
                    commandText
                    );

                // 断言没有参数产生
                Assert.AreEqual(0, query.Command.Parameters.Count);
            }
        }
    }


    [TestMethod]
    public void Test_CPQuery参数支持Guid数组()
    {
        string sql = @"select * from xx where xxGuid in ({guidList})";

        Guid guid1 = new Guid("c7423fc3-fc56-4b0a-9119-dcbdbaac56db");
        Guid guid2 = new Guid("74c5608c-e6b9-40f7-a2c7-999916f3bd40");
        List<Guid> guidArray = new List<Guid> { guid1, guid2 };
        var args = new { guidList = guidArray };

        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                CPQuery query = db.CPQuery.Create(sql, args);

                string commandText = query.ToString();
                Console.WriteLine(commandText);

                // 断言占位符已被替换
                Assert.AreEqual(
                    // 注意：Guid[] 不会生成命令参数，将直接输出到SQL中
                    "select * from xx where xxGuid in ('c7423fc3-fc56-4b0a-9119-dcbdbaac56db','74c5608c-e6b9-40f7-a2c7-999916f3bd40')",
                    commandText
                    );

                // 断言没有参数产生
                Assert.AreEqual(0, query.Command.Parameters.Count);
            }
        }
    }

    [TestMethod]
    public void Test_CPQuery参数支持STRING数组()
    {
        string sql = @"select * from Customers where CustomerID in ({CustomerID})";
        string[] customerIdArray = { "1", "2", "3", "4", "5" };
        var args = new { CustomerID = customerIdArray };

        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                this.ResetCPQueryParamIndex();

                CPQuery query = db.CPQuery.Create(sql, args);

                string commandText = query.ToString();
                Console.WriteLine(commandText);


                // 断言占位符已被替换
                if( db.DatabaseType == DatabaseType.DaMeng ) {
                    Assert.AreEqual("select * from Customers where CustomerID in (:x1,:x2,:x3,:x4,:x5)", commandText);
                }
                else {
                    Assert.AreEqual("select * from Customers where CustomerID in (@x1,@x2,@x3,@x4,@x5)", commandText);
                }

                // 断言参数已产生
                Assert.AreEqual(5, query.Command.Parameters.Count);
            }
        }
    }



    [TestMethod]
    public void Test_CPQuery嵌套使用()
    {
        using( ConnectionScope scope = ConnectionScope.Create() ) {
            CPQuery query1 = "P2 = ".AsCPQuery() + 2;
            CPQuery query2 = "P3 = ".AsCPQuery() + DateTime.Now;
            CPQuery query = CPQuery.Create("select * from t1 where id=@id and {subquery1} and {subquery2}",
                    new { id = 3, subquery1 = query1, subquery2 = query2 }
                );

            string commandText = query.ToString();
            Console.WriteLine(commandText);

            Assert.AreEqual("select * from t1 where id=@id and P2 = @p1 and P3 = @p2", commandText);
            Assert.AreEqual(3, query.Command.Parameters.Count);
        }
    }


    [TestMethod]
    public void Test_CPQuery占位符参数()
    {
        using( ConnectionScope scope = ConnectionScope.Create() ) {
            CPQuery query = CPQuery.Create("select * from {table} where id=@id",
            new { id = 2, table = "t1".AsSql() });

            string commmandText = query.ToString();
            Console.WriteLine(commmandText);

            Assert.AreEqual("select * from t1 where id=@id", commmandText);
        }
    }


    [TestMethod]
    public void Test_CPQuery与CPQuery相加()
    {
        using( ConnectionScope scope = ConnectionScope.Create() ) {
            CPQuery query1 = CPQuery.Create("select * from t1 where id=@id", new { id = 2 });
            CPQuery query2 = CPQuery.Create(";select * from t2 where name=@name", new { name = "abc" });
            CPQuery query3 = query1 + query2;

            string commmandText = query3.ToString();
            Console.WriteLine(commmandText);

            Assert.AreEqual("select * from t1 where id=@id;select * from t2 where name=@name", commmandText);
            Assert.AreEqual(2, query3.Command.Parameters.Count);
        }
    }


    [TestMethod]
    public void Test_CPQuery设置命令超时时间()
    {
        using( ConnectionScope scope = ConnectionScope.Create() ) {
            CPQuery query1 = CPQuery.Create("select * from t1").SetCommand(x => x.CommandTimeout = 2);
            Assert.AreEqual(2, query1.Command.CommandTimeout);


            CPQuery query2 = CPQuery.Create("select * from t1").SetTimeout(2);
            Assert.AreEqual(2, query2.Command.CommandTimeout);
        }
    }

    [TestMethod]
    public async Task Test_CPQuery_ToScalarList()
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                // 结果集的第一列其实是个数字列，这里强制写成 string
                List<string> list1 = db.CPQuery.Create("select * from Categories").ToScalarList<string>();

                List<string> list2 = await db.CPQuery.Create("select * from Categories").ToScalarListAsync<string>();

                MyAssert.AreEqual(list1, list2);
            }
        }
    }


    [TestMethod]
    public async Task Test_CPQuery_ExecuteReader()
    {
        int id1 = -1, id2 = -2;

        using( ConnectionScope scope = ConnectionScope.Create() ) {
            CPQuery query1 = CPQuery.Create("select * from Categories");
            using( DbDataReader reader1 = query1.ExecuteReader() ) {
                if( reader1.Read() )
                    id1 = reader1.GetInt32(0);
            }
        }



        using( DbContext db = DbContext.Create() ) {
            CPQuery query2 = db.CPQuery.Create("select * from Categories");

            using( DbDataReader reader2 = await query2.ExecuteReaderAsync() ) {
                if( reader2.Read() )
                    id2 = reader2.GetInt32(0);
            }
        }

        Assert.AreEqual(id1, id2);
        Assert.IsTrue(id1 > 0);
    }





    [TestMethod]
    public void Test_BaseCommand_CloneParameters()
    {
        var newCustomer = new {
            CustomerName = s_newName,
            ContactName = Guid.NewGuid().ToString(),
            Address = "111111 Address",
            PostalCode = "111111",
            Tel = "123456789"
        };

        using( ConnectionScope scope = ConnectionScope.Create() ) {
            XmlCommand command = XmlCommand.Create("InsertCustomer", newCustomer);

            DbParameter[] parameters1 = command.Command.Parameters.Cast<DbParameter>().ToArray();
            DbParameter[] parameters2 = command.Command.CloneParameters();

            AssertAreEqual_DbParameterArray(parameters1, parameters2);
        }
    }


    private void AssertAreEqual_DbParameterArray(DbParameter[] parameters1, DbParameter[] parameters2)
    {
        Assert.AreEqual(parameters1.Length, parameters2.Length);

        for( int i = 0; i < parameters2.Length; i++ ) {
            DbParameter p1 = parameters1[i];
            DbParameter p2 = parameters2[i];

            Assert.AreEqual(p1.ParameterName, p2.ParameterName);
            Assert.AreEqual(p1.DbType, p2.DbType);
            Assert.AreEqual(p1.Direction, p2.Direction);
            Assert.AreEqual(p1.IsNullable, p2.IsNullable);
            Assert.AreEqual(p1.Size, p2.Size);
            Assert.AreEqual(p1.SourceColumn, p2.SourceColumn);
            Assert.AreEqual(p1.SourceColumnNullMapping, p2.SourceColumnNullMapping);
            Assert.AreEqual(p1.Value.ToString(), p2.Value.ToString());
        }
    }

    [TestMethod]
    public void Test_CPQuery_Init_Dictionary()
    {
        DateTime now = DateTime.Now;
        Guid guid = Guid.NewGuid();

        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict["key1"] = 1;
        dict["key2"] = "abc";
        dict["key3"] = now;
        dict["key4"] = guid;


        Hashtable table = new Hashtable();
        table["key1"] = 1;
        table["key2"] = "abc";
        table["key3"] = now;
        table["key4"] = guid;

        using( ConnectionScope scope = ConnectionScope.Create() ) {
            CPQuery query1 = CPQuery.Create("select ............", dict);
            CPQuery query2 = CPQuery.Create("select ............", table);

            DbParameter[] parameters1 = (from x in query1.Command.Parameters.Cast<DbParameter>()
                                         orderby x.ParameterName
                                         select x).ToArray();

            DbParameter[] parameters2 = (from x in query2.Command.Parameters.Cast<DbParameter>()
                                         orderby x.ParameterName
                                         select x).ToArray();

            AssertAreEqual_DbParameterArray(parameters1, parameters2);
        }
    }


    [TestMethod]
    public void Test_CPQuery_Init_DbParameters()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict["key1"] = 1;
        dict["key2"] = "abc";
        dict["key3"] = DateTime.Now;
        dict["key4"] = Guid.NewGuid();

        using( ConnectionScope scope = ConnectionScope.Create() ) {
            CPQuery query1 = CPQuery.Create("select ............", dict);

            DbParameter[] parameters = query1.Command.CloneParameters();
            CPQuery query2 = CPQuery.Create("select ............", parameters);


            DbParameter[] parameters1 = (from x in query1.Command.Parameters.Cast<DbParameter>()
                                         orderby x.ParameterName
                                         select x).ToArray();

            DbParameter[] parameters2 = (from x in query2.Command.Parameters.Cast<DbParameter>()
                                         orderby x.ParameterName
                                         select x).ToArray();
            AssertAreEqual_DbParameterArray(parameters1, parameters2);
        }
    }


    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = new CPQuery(null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = CPQuery.Create(null, new object());
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = CPQuery.Create(null, new Hashtable());
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = CPQuery.Create(null, new Dictionary<string, object>());
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = CPQuery.Create(null, Empty.Array<DbParameter>());
        });
    }



    [TestMethod]
    public async Task Test_DbExceuteException()
    {
        using( DbContext db = DbContext.Create() ) {

            MyAssert.IsError<DbExceuteException>(() => {
                _ = db.CPQuery.Create("select * from xxx").ToDataTable();
            });


            await MyAssert.IsErrorAsync<DbExceuteException>(async () => {
                _ = await db.CPQuery.Create("select * from xxx").ToDataTableAsync();
            });

        }
    }


    [TestMethod]
    public void Test_Init()
    {
        using( ConnectionScope scope = ConnectionScope.Create() ) {

            CPQuery q1 = CPQuery.Create("select 1", (object)null);
            Assert.AreEqual(0, q1.Command.Parameters.Count);


            CPQuery q2 = CPQuery.Create("select 1", (Hashtable)null);
            Assert.AreEqual(0, q2.Command.Parameters.Count);

            CPQuery q3 = CPQuery.Create("select 1", new Hashtable());
            Assert.AreEqual(0, q3.Command.Parameters.Count);


            CPQuery q4 = CPQuery.Create("select 1", (Dictionary<string, object>)null);
            Assert.AreEqual(0, q4.Command.Parameters.Count);

            CPQuery q5 = CPQuery.Create("select 1", new Dictionary<string, object>());
            Assert.AreEqual(0, q5.Command.Parameters.Count);

            CPQuery q6 = CPQuery.Create("select 1", (DbParameter[])null);
            Assert.AreEqual(0, q6.Command.Parameters.Count);

            CPQuery q7 = CPQuery.Create("select 1", Empty.Array<DbParameter>());
            Assert.AreEqual(0, q7.Command.Parameters.Count);
        }
    }




    [TestMethod]
    public void Test_ToScalar()
    {
        Assert.IsNull(BaseCommand.ToScalar<Product>(null));
        Assert.IsNull(BaseCommand.ToScalar<Product>(DBNull.Value));

        Assert.IsNull(BaseCommand.ToScalar<string>(null));
        Assert.IsNull(BaseCommand.ToScalar<string>(DBNull.Value));

        Assert.AreEqual(0, BaseCommand.ToScalar<int>(null));
        Assert.AreEqual(0, BaseCommand.ToScalar<int>(DBNull.Value));

        Assert.AreEqual(0, BaseCommand.ToScalar<decimal>(null));
        Assert.AreEqual(0, BaseCommand.ToScalar<decimal>(DBNull.Value));

        Assert.AreEqual(DateTime.MinValue, BaseCommand.ToScalar<DateTime>(null));
        Assert.AreEqual(DateTime.MinValue, BaseCommand.ToScalar<DateTime>(DBNull.Value));

        Assert.AreEqual(Guid.Empty, BaseCommand.ToScalar<Guid>(null));
        Assert.AreEqual(Guid.Empty, BaseCommand.ToScalar<Guid>(DBNull.Value));


        Assert.AreEqual(333, BaseCommand.ToScalar<int>((object)333));
        Assert.AreEqual("abc", BaseCommand.ToScalar<string>((object)"abc"));

        Assert.AreEqual(333, BaseCommand.ToScalar<int>((object)"333"));
        Assert.AreEqual("333", BaseCommand.ToScalar<string>((object)333));

    }


    [TestMethod]
    public void Test_InitSQL()
    {
        using( ConnectionScope scope = ConnectionScope.Create() ) {

            CPQuery query = CPQuery.Create("select 1;");

            DbCommand command = query.Command;

            Assert.AreEqual("select 1;", command.CommandText);
            Assert.AreEqual("select 1;", query.ToString());

            Assert.IsFalse((bool)query.GetFieldValue("_sqlChanged"));
            Assert.IsNull(query.GetFieldValue("_sqlBuilder"));
            Assert.IsNotNull(query.GetFieldValue("_initSql"));

            query = query + "select 2;";
            Assert.IsTrue((bool)query.GetFieldValue("_sqlChanged"));
            command = query.Command;
            Assert.IsFalse((bool)query.GetFieldValue("_sqlChanged"));

            Assert.AreEqual("select 1;select 2;", command.CommandText);
            Assert.AreEqual("select 1;select 2;", query.ToString());
            
            Assert.IsNotNull(query.GetFieldValue("_sqlBuilder"));
            Assert.IsNull(query.GetFieldValue("_initSql"));
        }
    }


}
