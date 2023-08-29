namespace PerformanceTest.DAL;

public class TestMySqlBatchInsert
{
    private static readonly int s_runCount = 100;
    private static readonly int s_listCount = 100;

    private static readonly string s_insertSQL = @"
insert into products(ProductName, CategoryID, Unit, UnitPrice, Quantity, Remark) 
values (@ProductName, @CategoryID, @Unit, @UnitPrice, @Quantity, @Remark)";

    [MenuMethod(Title = "MySQL测试批量插入--ADO.NET foreach insert")]
    public static void TestBatchInsert1()
    {
        DeleteTestData();
        List<Product> list = CreateProductList(s_listCount);

        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < s_runCount; i++ ) {

            using( DbContext dbContext = DbContext.Create("mysql") ) {
                dbContext.Connection.Open();
                MySqlTransaction transaction = (MySqlTransaction)dbContext.Connection.BeginTransaction();

                foreach( var product in list ) {

                    MySqlCommand command = (MySqlCommand)dbContext.Connection.CreateCommand();
                    command.CommandText = s_insertSQL;
                    command.CommandType = CommandType.Text;
                    command.Transaction = transaction;

                    FillParameters(product, command, command.Parameters);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        watch.Stop();
        Console.WriteLine(watch.Elapsed.ToString());
    }


    [MenuMethod(Title = "MySQL测试批量插入--ClownFish.net/foreach insert1")]
    public static void TestBatchInsert1a()
    {
        DeleteTestData();
        List<Product> list = CreateProductList(s_listCount);

        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < s_runCount; i++ ) {

            using( DbContext dbContext = DbContext.Create("mysql") ) {
                dbContext.BeginTransaction();

                foreach( var product in list )
                    dbContext.Entity.Insert(product);

                dbContext.Commit();
            }
        }

        watch.Stop();
        Console.WriteLine(watch.Elapsed.ToString());
    }

    [MenuMethod(Title = "MySQL测试批量插入--ClownFish.net/foreach insert2")]
    public static void TestBatchInsert1b()
    {
        DeleteTestData();
        List<Product> list = CreateProductList(s_listCount);

        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < s_runCount; i++ ) {

            using( DbContext dbContext = DbContext.Create("mysql") ) {
                dbContext.BeginTransaction();

                foreach( var product in list )
                    dbContext.Entity.Insert(product, InsertOption.AllFields | InsertOption.IgnoreDuplicateError);

                dbContext.Commit();
            }
        }

        watch.Stop();
        Console.WriteLine(watch.Elapsed.ToString());
    }


    [MenuMethod(Title = "MySQL测试批量插入--Data batching API")]
    public static void TestBatchInsert2()
    {
        DeleteTestData();
        List<Product> list = CreateProductList(s_listCount);

        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < s_runCount; i++ ) {

            using( DbContext dbContext = DbContext.Create("mysql") ) {
                dbContext.Connection.Open();
                dbContext.BeginTransaction();

                MySqlConnection connection = (MySqlConnection)dbContext.Connection;
                MySqlCommand command = (MySqlCommand)dbContext.Connection.CreateCommand();

                MySqlBatch batch = connection.CreateBatch();
                batch.Transaction = (MySqlTransaction)dbContext.Transaction;

                foreach( var product in list ) {

                    MySqlBatchCommand batchCommand = (MySqlBatchCommand)batch.CreateBatchCommand();
                    batchCommand.CommandText = s_insertSQL;
                    batchCommand.CommandType = System.Data.CommandType.Text;

                    FillParameters(product, command, batchCommand.Parameters);
                    batch.BatchCommands.Add(batchCommand);
                }

                batch.ExecuteNonQuery();
                dbContext.Commit();
            }
        }

        watch.Stop();
        Console.WriteLine(watch.Elapsed.ToString());
    }


    // 使用 MySqlBulkCopy 的前置条件：
    // 1，MySQL运行参数：local_infile = ON
    // 2，连接字符串增加：AllowLoadLocalInfile=true

    [MenuMethod(Title = "MySQL测试批量插入--MySqlBulkCopy")]
    public static void TestBatchInsert3()
    {
        DeleteTestData();
        List<Product> list = CreateProductList(s_listCount);
        DataTable table = GetTable(list);

        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < s_runCount; i++ ) {

            using( DbContext dbContext = DbContext.Create("mysql") ) {
                dbContext.Connection.Open();

                MySqlConnection connection = (MySqlConnection)dbContext.Connection;
                MySqlTransaction transaction = (MySqlTransaction)dbContext.Connection.BeginTransaction();

                MySqlBulkCopy bulkCopy = new MySqlBulkCopy(connection, transaction);

                bulkCopy.DestinationTableName = "products";
                bulkCopy.WriteToServer(table);
                transaction.Commit();
            }
        }

        watch.Stop();
        Console.WriteLine(watch.Elapsed.ToString());
    }


    [MenuMethod(Title = "MySQL测试批量插入--ClownFish.net/BatchInsert")]
    public static void TestBatchInsert4()
    {
        DeleteTestData();
        List<Product> list = CreateProductList(s_listCount);

        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < s_runCount; i++ ) {

            using( DbContext dbContext = DbContext.Create("mysql") ) {

                dbContext.BeginTransaction();
                dbContext.Batch.Insert(list, -1);
                dbContext.Commit();
            }
        }

        watch.Stop();
        Console.WriteLine(watch.Elapsed.ToString());
    }


    private static void DeleteTestData()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {

            dbContext.CPQuery.Create("delete from products where CategoryID = 10000").ExecuteNonQuery();
        }
    }


    private static DataTable GetTable(List<Product> list)
    {
        DataTable table = null;

        using( DbContext dbContext = DbContext.Create("mysql") ) {

            table = dbContext.CPQuery.Create("select * from products where ProductId = 0").ToDataTable();
        }

        PropertyInfo[] properties = typeof(Product).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach( var product in list ) {

            DataRow row = table.NewRow();
            foreach( PropertyInfo property in properties ) {
                object value = property.GetValue(product, null);
                row[property.Name] = value;
            }
            table.Rows.Add(row);
        }

        //table.AcceptChanges();
        return table;
    }

    private static List<Product> CreateProductList(int count)
    {
        List<Product> list = new List<Product>(count);

        for( int i = 0; i < count; i++ ) {
            Product product = new Product {
                ProductName = "TestBatchInsert_" +  Guid.NewGuid().ToString("N"),
                CategoryID = 10000,
                Quantity = 99999,
                Unit = "TEST",
                UnitPrice = 666.66M,
                Remark = "aaaaaaaaaaaa"
            };
            list.Add(product);
        }

        return list;
    }


    private static void FillParameters(Product product, MySqlCommand command, DbParameterCollection parameters)
    {
        DbParameter parameter1 = command.CreateParameter();
        parameter1.ParameterName = "@ProductName";
        parameter1.Value = product.ProductName;
        parameters.Add(parameter1);


        DbParameter parameter2 = command.CreateParameter();
        parameter2.ParameterName = "@CategoryID";
        parameter2.Value = product.CategoryID;
        parameters.Add(parameter2);

        DbParameter parameter3 = command.CreateParameter();
        parameter3.ParameterName = "@Unit";
        parameter3.Value = product.Unit;
        parameters.Add(parameter3);

        DbParameter parameter4 = command.CreateParameter();
        parameter4.ParameterName = "@UnitPrice";
        parameter4.Value = product.UnitPrice;
        parameters.Add(parameter4);


        DbParameter parameter5 = command.CreateParameter();
        parameter5.ParameterName = "@Quantity";
        parameter5.Value = product.Quantity;
        parameters.Add(parameter5);

        DbParameter parameter6 = command.CreateParameter();
        parameter6.ParameterName = "@Remark";
        parameter6.Value = product.Remark;
        parameters.Add(parameter6);
    }
}
