namespace ClownFish.UnitTest.Data.Command;

[TestClass]
public class StoreProcedureFactoryTest
{
    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            StoreProcedureFactory factory = new StoreProcedureFactory(null);
        });

    }


    [TestMethod]
    public void Test()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            StoreProcedureFactory factory = new StoreProcedureFactory(dbContext);

            var sp1 = factory.Create("CreateXXXX");

            var args2 = new { ProductID = 2 };
            var sp2 = factory.Create("CreateXXXX", args2);

            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@ProductID", 2);
            var sp3 = factory.Create("CreateXXXX", parameters);


            string expected = "CreateXXXX";
            Assert.AreEqual(expected, sp1.Command.CommandText);
            Assert.AreEqual(expected, sp2.Command.CommandText);
            Assert.AreEqual(expected, sp3.Command.CommandText);

            Assert.AreEqual(1, sp2.Command.Parameters.Count);
            Assert.AreEqual(1, sp3.Command.Parameters.Count);

            Assert.AreEqual("@ProductID", sp2.Command.Parameters[0].ParameterName);
            Assert.AreEqual("@ProductID", sp3.Command.Parameters[0].ParameterName);
        }
    }
}
