namespace ClownFish.UnitTest.Data.MultiDB;

[TestClass]
public class DbParameterTest
{
    internal static SqlParameter CreateParameter()
    {
        return new SqlParameter {
            DbType = DbType.String,
            Direction = ParameterDirection.Input,
            IsNullable = false,
            ParameterName = "@id",
            Size = 4,
            Value = 123
        };
    }

    [TestMethod]
    public void Test_CloneParameters()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _= CommandExtensions.CloneParameters(null);
        });


        DbParameter[] parameters = new DbParameter[1];
        SqlParameter p = CreateParameter();
        parameters[0] = p;

        using( DbContext db = DbContext.Create() ) {
            CPQuery query = db.CPQuery.Create("select * from table1 where id = @id", parameters);

            DbParameter[] parameters2 = query.Command.CloneParameters();
            Assert.AreEqual(1, parameters2.Length);
            Assert.IsInstanceOfType(parameters2[0], typeof(SqlParameter));
        }
    }

    [TestMethod]
    public void Test_CloneParameter()
    {
        SqlParameter p = CreateParameter();

        using( DbContext db = DbContext.Create() ) {
            DbCommand command = db.Connection.CreateCommand();

            DbParameter p2 = (DbParameter)typeof(CommandExtensions).InvokeMember("CloneParameter", 
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic,
                null, null, new object[] {p, command });

            Assert.IsNotNull(p2);
            Assert.AreEqual("@id", p2.ParameterName);
        }
    }
}
