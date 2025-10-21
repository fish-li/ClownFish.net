using ClownFish.Data.Xml;

namespace AotTestConsoleApp1.TestCase;
internal static class TestDAL
{
    public static readonly string[] ConnNames = [ "sqlserver", "mysql", "postgresql"];

    private static readonly string s_newName = Guid.NewGuid().ToString();

    public static async Task Run()
    {
        await Test_CPQuery的基本CRUD操作_Async();
        await Test_CPQuery加载实体列表();
        await Test_CPQuery_分页加载数据();
        await Test_CPQuery_不分页查询();
        await Test_CPQuery_ExportToNdJson();

        TestEntityCURD();
        await TestLinqQuery();
    }

    


    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    private static string GetSql(string xmlcommandName)
    {
        // 这个测试类为了简单，就直接借用XmlCommand中定义的SQL语句

        XmlCommandItem x1 = XmlCommandManager.Instance.GetCommand(xmlcommandName);
        return x1.CommandText;
    }

    private static async Task Test_CPQuery的基本CRUD操作_Async()
    {
        foreach( var conn in ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {
                db.BeginTransaction(IsolationLevel.ReadCommitted);

                var newCustomer = new {
                    CustomerName = s_newName,
                    ContactName = Guid.NewGuid().ToString(),
                    Address = "111111 Address",
                    PostalCode = "111111",
                    Tel = "123456789"
                };

                // 插入一条记录
                await db.CPQuery.Create(GetSql("InsertCustomer"), newCustomer).ExecuteNonQueryAsync();

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
                var queryArgument2 = new { customer.CustomerID };
                Customer customer2 = await db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingleAsync<Customer>();

                // 验证更新与读取
                Assert.IsNotNull(customer2);
                Assert.AreEqual(updateArgument.Address, customer2.Address);


                // 删除记录
                var deleteArgument = new { customer.CustomerID };
                await db.CPQuery.Create(GetSql("DeleteCustomer"), deleteArgument).ExecuteNonQueryAsync();

                // 验证删除			
                Customer customer3 = await db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingleAsync<Customer>();
                Assert.IsNull(customer3);

                db.Commit();
            }
        }
    }

    private static async Task Test_CPQuery加载实体列表()
    {
        foreach( var conn in ConnNames ) {
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

    private static async Task Test_CPQuery_分页加载数据()
    {
        DataTable table1 = null;
        DataTable table2 = null;

        List<Customer> list1 = null;
        List<Customer> list2 = null;

        string sql = GetSql("GetCustomerList");
        var args = new { MaxCustomerID = 100 };

        foreach( var conn in ConnNames ) {
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

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    private static async Task Test_CPQuery_不分页查询()
    {
        DataTable table1 = null;
        DataTable table2 = null;

        List<Customer> list1 = null;
        List<Customer> list2 = null;

        string sql = GetSql("GetCustomerList");
        var args = new { MaxCustomerID = 100 };

        foreach( var conn in ConnNames ) {
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

                string xmlFilePath = "temp/Test_CPQuery_不分页查询.xml";

                table2.TableName = "table2";

                table2.WriteXml(xmlFilePath, XmlWriteMode.WriteSchema);
                DataTable table3 = ClownFish.Base.DataTableExtensions.LoadFormXmlFile(xmlFilePath);

                Assert.AreEqual("table2", table3.TableName);
                Assert.AreEqual(table2.Columns.Count, table3.Columns.Count);
                Assert.AreEqual(table2.Rows.Count, table3.Rows.Count);

                for( int i = 0; i < table3.Columns.Count; i++ ) {
                    Assert.AreEqual(table2.Columns[i].ColumnName, table3.Columns[i].ColumnName);
                    Assert.AreEqual(table2.Columns[i].DataType.FullName, table3.Columns[i].DataType.FullName);
                }

                string xml2 = table2.TableToXml();
                string xml3 = table3.TableToXml();
                Assert.AreEqual(xml2, xml3);

                pagingInfo.TotalRows = -1;
                list2 = await db.CPQuery.Create(sql, args).ToPageListAsync<Customer>(pagingInfo);

                Assert.IsNotNull(list2);
                Assert.AreEqual(-1, pagingInfo.TotalRows);


                MyAssert.AreEqual(table1, table2);
                MyAssert.AreEqual(list1, list2);
            }
        }
    }


    private static async Task Test_CPQuery_ExportToNdJson()
    {
        string sql = GetSql("GetCustomerList");
        var args = new { MaxCustomerID = 100 };

        string outFilePath1 = "temp/Test_CPQuery_ExportToNdJson1.txt";
        string outFilePath2 = "temp/Test_CPQuery_ExportToNdJson2.txt";

        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();

        StringWriter writer1 = new StringWriter(sb1);
        StringWriter writer2 = new StringWriter(sb2);

        int count1, count2, count3, count4;
        int maxRows = 100;

        using( DbContext db = DbContext.Create("mysql") ) {
            count1 = db.CPQuery.Create(sql, args).ExportToNdJson(maxRows, outFilePath1);
            count2 = await db.CPQuery.Create(sql, args).ExportToNdJsonAsync(maxRows, outFilePath2);

            count3 = db.CPQuery.Create(sql, args).ExportToNdJson(maxRows, writer1);
            count4 = await db.CPQuery.Create(sql, args).ExportToNdJsonAsync(maxRows, writer2);
        }

        Assert.IsTrue(count1 > 0);
        Assert.AreEqual(count1, count2);
        Assert.AreEqual(count3, count4);
        Assert.AreEqual(count1, count3);

        string text1 = RetryFile.ReadAllText(outFilePath1);
        string text2 = RetryFile.ReadAllText(outFilePath2);
        string text3 = sb1.ToString();
        string text4 = sb2.ToString();

        //Console.WriteLine(text1);

        Assert.IsTrue(text1.HasValue());
        Assert.AreEqual(text1, text2);
        Assert.AreEqual(text3, text4);
        Assert.AreEqual(text1, text3);

        // 下面验证 “空查询” 场景
        int count5 = -1, count6 = -1;
        StringBuilder sb3 = new StringBuilder();
        StringWriter writer3 = new StringWriter(sb3);
        string outFilePath3 = "temp/Test_CPQuery_ExportToNdJson3.txt";

        using( DbContext db = DbContext.Create("mysql") ) {
            count5 = db.CPQuery.Create("select * from products where ProductID < 0").ExportToNdJson(maxRows, writer3);
            count6 = db.CPQuery.Create("select * from products where ProductID < 0").ExportToNdJson(maxRows, outFilePath3);
        }
        Assert.IsTrue(count5 == 0);
        Assert.IsTrue(count6 == 0);
        Assert.IsTrue(sb3.Length == 0);

        string text5 = RetryFile.ReadAllText(outFilePath3);
        Assert.IsTrue(text5.Length == 0);
    }




    private static void TestEntityCURD()
    {
        // 实体代理需要CodeDOM动态生成程序集，目前暂不支持
    }

    private static async Task TestLinqQuery()
    {
        await Test_LINQ_获取单个实体_Async();
        await Test_LINQ_获取单个实体_追加WHERE条件_Async();
        await Test_LINQ_获取实体列表_Async();
        await Test_LINQ_获取实体列表_IN参数_Async();
        await Test_LINQ_获取实体列表_LIKE_Async();
        await Test_LINQ_WHERE分成二段_Async();
        await Test_LINQ_COUNT_Async();
        await Test_LINQ_COUNT_追加WHERE条件_Async();
        await Test_LINQ_EXIST_Async();
        await Test_LINQ_EXIST_追加WHERE条件_Async();
        await Test_LINQ_加载实体只加载个别字段_Async();
        await Test_LINQ_ORDER_Async();
        await Test_Expression_ORDER();
        await Test_LINQ_WithNoLock();
        await Test_UnaryExpression();
        await Test_MemberExpression();
    }


    private static async Task Test_LINQ_获取单个实体_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            int a = 5, b = 3;

            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == a && t.CategoryID < b
                        select t;

            Product p = await query.ToSingleAsync();

            Assert.AreEqual(5, p.ProductID);
        }
    }


    private static async Task Test_LINQ_获取单个实体_追加WHERE条件_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        select t;

            query = query.Where(t => t.ProductID == 5 && t.CategoryID < 3);

            Product p = await query.FirstOrDefaultAsync();

            Assert.AreEqual(5, p.ProductID);
        }
    }


    public static int P5 { get; set; } = 5;
    public static int P3 { get; set; } = 3;

    private static async Task Test_LINQ_获取实体列表_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == P5 || t.CategoryID < P3
                        select t;

            List<Product> list = await query.ToListAsync();
            Assert.IsTrue(list.Count >  0);
        }
    }

#pragma warning disable IDE1006 // 命名样式
    private static readonly int _f5 = 5;
    private static readonly int _f3 = 3;
#pragma warning restore IDE1006 // 命名样式

    private static async Task Test_LINQ_获取实体列表_IN参数_Async()
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

            List<Product> list = await query.ToListAsync();
        }
    }


    private static async Task Test_LINQ_获取实体列表_LIKE_Async()
    {
        string b = "aaa";

        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductName.Contains(b)
                        select t;

            List<Product> list = await query.ToListAsync();
        }
    }

    private static async Task Test_LINQ_WHERE分成二段_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == P5 || t.Quantity > 10
                        select t;

            query = query.Where(x => x.CategoryID < _f3);

            List<Product> list = await query.ToListAsync();
        }

    }

    private static async Task Test_LINQ_COUNT_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == 5 && t.CategoryID < 3
                        select t;

            int count = await query.CountAsync();
        }
    }

    private static async Task Test_LINQ_COUNT_追加WHERE条件_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        select t;

            query = query.Where(t => t.ProductID == 5 || t.CategoryID < 3);

            int count = await query.CountAsync();
        }
    }


    private static async Task Test_LINQ_EXIST_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == _f5 && t.CategoryID < 3
                        select t;

            bool exist = await query.AnyAsync();
        }
    }


    private static async Task Test_LINQ_EXIST_追加WHERE条件_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        select t;

            query = query.Where(t => t.ProductID == 5 || t.CategoryID < _f3);

            bool exist = await query.AnyAsync();
        }
    }

    private static async Task Test_LINQ_加载实体只加载个别字段_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == _f5 || t.CategoryID < _f3
                        select new Product { ProductID = t.ProductID, ProductName = t.ProductName };

            query = query.Where(x => x.ProductID > 3);

            var list = await query.ToListAsync();
        }
    }

    private static async Task Test_LINQ_ORDER_Async()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == P5
                        orderby t.ProductID, t.CategoryID descending, t.Quantity, t.UnitPrice descending
                        select t;

            List<Product> list = await query.ToListAsync();
        }
    }

    private static async Task Test_Expression_ORDER()
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

            List<Product> list = await query.ToListAsync();
        }
    }

    private static async Task Test_LINQ_WithNoLock()
    {
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == P5 || t.CategoryID < P3
                        select t;

            List<Product> list = await query.ToListAsync();
        }
    }

    private static async Task Test_UnaryExpression()
    {
        long cid = 5L;
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.CategoryID < (int)cid  // 一元运算符
                        select t;

            List<Product> list = await query.ToListAsync();
        }
    }

    private static async Task Test_MemberExpression()
    {
        var args = new {
            Inner = new {
                id = 3
            }
        };
        using( DbContext db = DbContext.Create() ) {
            var query = from t in db.Entity.Query<Product>()
                        where t.CategoryID == args.Inner.id  // MemberExpression
                        select t;

            List<Product> list = await query.ToListAsync();
        }
    }



}
