#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Sync;
[TestClass]
public class DataImportTest
{
    [TestMethod]
    public void Test_MySql_允许自增主键()
    {
        using DbContext dbContext = DbContext.Create("mysql");
        dbContext.EnableDelimiter = true;
        Create_MySql_Table(dbContext);

        DataImportArgs args = new DataImportArgs {
            DestDbContext = dbContext,
            DestTableName = "TestImport",
            Data = CateDataTable1(),
            AllowAutoIncrement = true,
            WithTranscation = false
        };

        DataImport.Execute(args);

        List<int> list1 = dbContext.CPQuery.Create("select RowId from TestImport order by RowId").ToScalarList<int>();
        Assert.AreEqual("11,12,13,14,15", string.Join(',', list1));

        List<int> list2 = dbContext.CPQuery.Create("select IntValue1 from TestImport order by IntValue1").ToScalarList<int>();
        Assert.AreEqual("1000,1001,1002,1003,1004", string.Join(',', list2));
    }


    [TestMethod]
    public async Task Test_MySql_允许自增主键_Async()
    {
        using DbContext dbContext = DbContext.Create("mysql");
        dbContext.EnableDelimiter = true;
        Create_MySql_Table(dbContext);

        DataImportArgs args = new DataImportArgs {
            DestDbContext = dbContext,
            DestTableName = "TestImport",
            Data = CateDataTable1(),
            AllowAutoIncrement = true,
            WithTranscation = false
        };

        await DataImport.ExecuteAsync(args);

        List<int> list1 = await dbContext.CPQuery.Create("select RowId from TestImport order by RowId").ToScalarListAsync<int>();
        Assert.AreEqual("11,12,13,14,15", string.Join(',', list1));

        List<int> list2 = await dbContext.CPQuery.Create("select IntValue1 from TestImport order by IntValue1").ToScalarListAsync<int>();
        Assert.AreEqual("1000,1001,1002,1003,1004", string.Join(',', list2));
    }


    [TestMethod]
    public void Test_MySql_不允许自增主键()
    {
        using DbContext dbContext = DbContext.Create("mysql");
        dbContext.EnableDelimiter = true;
        Create_MySql_Table(dbContext);

        DataImportArgs args = new DataImportArgs {
            DestDbContext = dbContext,
            DestTableName = "TestImport",
            Data = CateDataTable1(),
            AllowAutoIncrement = false,
            WithTranscation = true
        };

        DataImport.Execute(args);

        List<int> list1 = dbContext.CPQuery.Create("select RowId from TestImport order by RowId").ToScalarList<int>();
        Assert.AreEqual("1,2,3,4,5", string.Join(',', list1));

        List<int> list2 = dbContext.CPQuery.Create("select IntValue1 from TestImport order by IntValue1").ToScalarList<int>();
        Assert.AreEqual("1000,1001,1002,1003,1004", string.Join(',', list2));
    }


    [TestMethod]
    public async Task Test_MySql_不允许自增主键_Async()
    {
        using DbContext dbContext = DbContext.Create("mysql");
        dbContext.EnableDelimiter = true;
        Create_MySql_Table(dbContext);

        DataImportArgs args = new DataImportArgs {
            DestDbContext = dbContext,
            DestTableName = "TestImport",
            Data = CateDataTable1(),
            AllowAutoIncrement = false,
            WithTranscation = true
        };

        await DataImport.ExecuteAsync(args);

        List<int> list1 = await dbContext.CPQuery.Create("select RowId from TestImport order by RowId").ToScalarListAsync<int>();
        Assert.AreEqual("1,2,3,4,5", string.Join(',', list1));

        List<int> list2 = await dbContext.CPQuery.Create("select IntValue1 from TestImport order by IntValue1").ToScalarListAsync<int>();
        Assert.AreEqual("1000,1001,1002,1003,1004", string.Join(',', list2));
    }


    private static DataTable CateDataTable1()
    {
        DataTable table = new DataTable("table1");
        table.Columns.Add("RowId", typeof(int));
        table.Columns.Add("IntValue1", typeof(int));
        table.Columns.Add("TextValue1", typeof(string));

        for( int i = 0; i < 5; i++ ) {
            DataRow dataRow = table.NewRow();
            dataRow["RowId"] = i + 11;
            dataRow["IntValue1"] = i + 1000;
            dataRow["TextValue1"] = i.ToString() + "_" + Guid.NewGuid().ToString("N");
            table.Rows.Add(dataRow);
        }

        return table;
    }



    private void Create_MySql_Table(DbContext dbContext)
    {
        string sql = @"
DROP TABLE IF EXISTS `TestImport`;
CREATE TABLE `TestImport`  (
  `RowId` int NOT NULL AUTO_INCREMENT,
  `IntValue1` int NOT NULL,
  `TextValue1` varchar(255)  NOT NULL,
  `TextValue2` varchar(255)  NULL DEFAULT NULL,
  PRIMARY KEY (`RowId`) USING BTREE
) ENGINE = InnoDB ;
";

        dbContext.CPQuery.Create(sql).ExecuteNonQuery();
    }

    [TestMethod]
    public void Test_ArgumentNullException()
    {
        using DbContext dbContext = DbContext.Create("mysql");

        MyAssert.IsError<ArgumentNullException>(() => {
            DataImport.Execute(null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            DataImport.Execute(new DataImportArgs { });   // DestDbContext is null
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            DataImport.Execute(new DataImportArgs {
                DestDbContext = dbContext,
            });   // DestTableName is null
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            DataImport.Execute(new DataImportArgs {
                DestDbContext = dbContext,
                DestTableName = "TestImport"
            });   // Data is null
        });

        MyAssert.IsError<ArgumentException>(() => {
            DataImport.Execute(new DataImportArgs {
                DestDbContext = dbContext,
                DestTableName = "TestImport",
                Data = new DataTable()
            });   // 数据表没有包含行或者列
        });
    }

    [TestMethod]
    public void Test_NotSupportedException()
    {
        using DbContext dbContext = DbContext.Create("sqlserver");

        MyAssert.IsError<NotSupportedException>(() => {
            DataImport.Execute(new DataImportArgs {
                DestDbContext = dbContext,
                DestTableName = "TestImport",
                Data = CateDataTable1()
            });
        });
    }
}
#endif

