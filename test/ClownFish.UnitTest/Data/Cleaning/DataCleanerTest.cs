using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.Cleaning;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Cleaning;
[TestClass]
public class DataCleanerTest
{
    [TestMethod]
    public void Test1()
    {
        // 写入 53 条数据
        InsertData(53);

        CleaningOption option = new CleaningOption {
            DbConfig = AppConfig.GetDbConfig("m1"),
            TableName = "xlog",
            TimeFieldName = "createtime",
            BatchRows = 10
        };

        // 按批次数据，分批次 10条，所以要分 6 批次来执行
        DataCleaner cleaner = new DataCleaner(option);
        cleaner.Execute();

        // 检查数据表是否已清空（因为时间超过保留期限）
        int count = GetCount();
        Assert.AreEqual(0, count);

        // 根据执行日志检验执行过程是否符合预期
        string output = cleaner.GetLogs();
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("LOAD: batch=1, load 10 rows"));
        Assert.IsTrue(output.Contains("DELE: batch=1, delete 10 rows"));
        Assert.IsTrue(output.Contains("LOAD: batch=2, load 10 rows"));
        Assert.IsTrue(output.Contains("DELE: batch=2, delete 10 rows"));
        Assert.IsTrue(output.Contains("LOAD: batch=3, load 10 rows"));
        Assert.IsTrue(output.Contains("DELE: batch=3, delete 10 rows"));
        Assert.IsTrue(output.Contains("LOAD: batch=4, load 10 rows"));
        Assert.IsTrue(output.Contains("DELE: batch=4, delete 10 rows"));
        Assert.IsTrue(output.Contains("LOAD: batch=5, load 10 rows"));
        Assert.IsTrue(output.Contains("DELE: batch=5, delete 10 rows"));
        Assert.IsTrue(output.Contains("LOAD: batch=6, load 3 rows"));
        Assert.IsTrue(output.Contains("DELE: batch=6, delete 3 rows"));
        Assert.IsFalse(output.Contains("LOAD: batch=7"));
        Assert.IsFalse(output.Contains("DELE: batch=7"));
    }


    private void InsertData(int count)
    {
        DateTime startTime = new DateTime(2000, 1, 1);
        string sql = "insert into xlog(xtext, createtime) values (@xtext, @createtime)";

        using( DbContext db = DbContext.Create("mysql") ) {

            for( int i = 0; i < count; i++ ) {
                var args = new {
                    xtext = Guid.NewGuid().ToString(),
                    createtime = startTime.AddMinutes(i)
                };

                db.CPQuery.Create(sql, args).ExecuteNonQuery();
            }
        }
    }

    private int GetCount()
    {
        using( DbContext db = DbContext.Create("mysql") ) {

            return db.CPQuery.Create("select count(*) from xlog").ExecuteScalar<int>();
        }
    }


    [TestMethod]
    public void Test2()
    {
        CleaningOption option = new CleaningOption {
            DbConfig = AppConfig.GetDbConfig("m1"),
            TableName = "xlog_xxx",
            TimeFieldName = "createtime",
            BatchRows = 10,
            RetryCount = 1,
        };

        DataCleaner cleaner = new DataCleaner(option);

        MyAssert.IsError<DbExceuteException>(() => {
            cleaner.Execute();
        });

        // 根据执行日志检验执行过程是否符合预期
        string output = cleaner.GetLogs();
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("ERR1: ClownFish.Data.DbExceuteException: Table 'mynorthwind.xlog_xxx' doesn't exist"));
    }

    [TestMethod]
    public void Test3()
    {
        CleaningOption option = new CleaningOption {
            DbConfig = AppConfig.GetDbConfig("m1"),
            TableName = "xlog_xxx",
            TimeFieldName = "createtime",
            BatchRows = 10,
            RetryCount = 1,
        };

        DataCleaner cleaner = new DataCleaner(option);

        DataTable table = new DataTable();
        table.Columns.Add(new DataColumn("createtime", typeof(DateTime)));
        DataRow row = table.NewRow();
        row[0] = DateTime.Now;
        table.Rows.Add(row);

        MyAssert.IsError<DbExceuteException>(() => {
            cleaner.DeleteData(table);
        });


        string output = cleaner.GetLogs();
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("ERR2: ClownFish.Data.DbExceuteException: Table 'mynorthwind.xlog_xxx' doesn't exist"));
    }


    [TestMethod]
    public void Test4()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            CleaningOption option = null;
            DataCleaner cleaner = new DataCleaner(option);
        });
    }
}
