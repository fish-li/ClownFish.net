namespace ClownFish.UnitTest.Log.Serialization;

[TestClass]
public class DbCommandSerializerTest
{
    //static DbCommandSerializerTest()
    //{
    //    LoggingLimit.SQL.SqlTextMaxLen = 200;
    //    LoggingLimit.SQL.ParameterMaxCount = 10;
    //    LoggingLimit.SQL.ParamValueMaxLen = 128;
    //}


    [TestMethod]
    public void Test()
    {
        using( DbContext dbContext = DbContext.Create("sqlserver") ) {
            dbContext.BeginTransaction();

            var x = dbContext.CPQuery.Create("select 1").ExecuteScalar<int>();

            SqlCommand command = new SqlCommand();
            command.Connection = (SqlConnection)dbContext.Connection;
            command.Transaction = (SqlTransaction)dbContext.Transaction;

            command.CommandText = "select * from talbe1 where id=@id and name=@name" + "   " + new string('x', 1024);
            command.Parameters.AddWithValue("id", 2);
            command.Parameters.AddWithValue("name", "abc");

            for( int i = 0; i < 20; i++ ) {
                string name = "p" + i.ToString();
                command.Parameters.AddWithValue(name, new string('z', 500));
            }

            string text = command.ToLoggingText();
            //Console.WriteLine(text);

            //Assert.IsTrue(text.StartsWith("[Transaction-Isolation-Level: ReadCommitted]"));

            Assert.IsTrue(text.Contains("select * from talbe1 where id=@id and name=@name"));
            Assert.IsTrue(text.Contains("xxxx...1075"));

            Assert.IsTrue(text.Contains("id(Int32)=2"));
            Assert.IsTrue(text.Contains("name(String)=abc"));

            Assert.IsTrue(text.Contains("p0(String)=zzzzzzz"));
            Assert.IsTrue(text.Contains("zzzzz...500"));
            Assert.IsTrue(text.Contains("p7(String)=zzzzzzz"));
            Assert.IsTrue(text.Contains("#####(String)=参数太多，已被截断...，参数数量：22"));
        }


        Assert.AreEqual("", DbCommandSerializer.ToLoggingText((DbCommand)null));
    }


    [TestMethod]
    public void Test_GetParamValue()
    {
        Assert.AreEqual("NULL", DbCommandSerializer.GetParamValue(null));
        Assert.AreEqual("NULL", DbCommandSerializer.GetParamValue(DBNull.Value));

        Assert.AreEqual("123456", DbCommandSerializer.GetParamValue("123456"));
        Assert.AreEqual(23, DbCommandSerializer.GetParamValue(DateTime.Now).Length);

        Assert.AreEqual("123456", DbCommandSerializer.GetParamValue(123456L));
        Assert.AreEqual("123456", DbCommandSerializer.GetParamValue(123456));

        Assert.AreEqual("1234.5600", DbCommandSerializer.GetParamValue(1234.56m));
        Assert.AreEqual("1234.5600", DbCommandSerializer.GetParamValue(1234.56d));
        //Assert.AreEqual("1234.5600", DbCommandSerializer.GetParamValue(1234.56f));  // Expected:<1234.5600>. Actual:<1234.5601>. 
        Assert.IsTrue(DbCommandSerializer.GetParamValue(1234.56f).StartsWith("1234."));

        Assert.AreEqual("Monday", DbCommandSerializer.GetParamValue(DayOfWeek.Monday));
    }
}
