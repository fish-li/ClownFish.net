namespace ClownFish.UnitTest.Data.MultiDB;

[TestClass]
public class SequentialGuidTest
{

/*
CREATE TABLE [TestGuid](
    [RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[IntValue] [int] NOT NULL,
	[RowGuid] UniqueIdentifier NOT NULL,     
    CONSTRAINT [PK_TestGuid] PRIMARY KEY CLUSTERED 
    (
        [RowIndex] ASC
    )
) 

检查方法： select * from [TestGuid] order by RowGuid
*/

    private static readonly string s_insertSQL = "insert into TestGuid(RowGuid, IntValue) values(@RowGuid, @IntValue)";

    private void ExecuteTest(DbContext db)
    {
        db.CPQuery.Create("truncate table TestGuid").ExecuteNonQuery();

        int runCount = 100;
        for( int i = 0; i < runCount; i++ ) {

            var args = new {
                RowGuid = db.NewSeqGuid(),
                IntValue = i + 1
            };

            db.CPQuery.Create(s_insertSQL, args).ExecuteNonQuery();
        }

        DataTable table = db.CPQuery.Create("select * from TestGuid  order by RowGuid").ToDataTable();

        Assert.AreEqual(runCount, table.Rows.Count);

        CheckResult(table);
    }


    private void CheckResult(DataTable table)
    {
        int intValue = (int)table.Rows[0]["IntValue"];
        int rowIndex = (int)table.Rows[0]["RowIndex"];

        for( int i = 1; i < table.Rows.Count; ++i ) {

            Assert.AreEqual(rowIndex + i, (int)table.Rows[i]["RowIndex"]);
            Assert.AreEqual(intValue + i, (int)table.Rows[i]["IntValue"]);
        }
    }


    [TestMethod]
    public void Test_SQLSERVER()
    {
        using DbContext db = DbContext.Create("sqlserver");
        ExecuteTest(db);
    }



    [TestMethod]
    public void Test_MySQL()
    {
        using DbContext db = DbContext.Create("mysql");
        ExecuteTest(db);
    }


    [TestMethod]
    public void Test_PostgreSQL()
    {
        using DbContext db = DbContext.Create("postgresql");
        ExecuteTest(db);
    }

#if TEST_DM
    [TestMethod]
    public void Test_DaMeng()
    {
        using DbContext db = DbContext.Create("dm");
        ExecuteTest(db);
    }
#endif

}
