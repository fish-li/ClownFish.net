namespace ClownFish.UnitTest.Data.Command;

[TestClass]
public class CPQueryFactoryTest
{

    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            CPQueryFactory factory = new CPQueryFactory(null);
        });

    }


    [TestMethod]
    public void Test()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            CPQueryFactory factory = new CPQueryFactory(dbContext);

            var query1 = factory.Create("select * from Products where ProductID = @ProductID");

            var args2 = new { ProductID = 2 };
            var query2 = factory.Create("select * from Products where ProductID = @ProductID", args2);

            Hashtable table = new Hashtable();
            table["ProductID"] = 2;
            var query3 = factory.Create("select * from Products where ProductID = @ProductID", table);

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["ProductID"] = 2;
            var query4 = factory.Create("select * from Products where ProductID = @ProductID", dictionary);

            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@ProductID", 2);
            var query5 = factory.Create("select * from Products where ProductID = @ProductID", parameters);


            string expected = "select * from Products where ProductID = @ProductID";
            Assert.AreEqual(expected, query1.Command.CommandText);
            Assert.AreEqual(expected, query2.Command.CommandText);
            Assert.AreEqual(expected, query3.Command.CommandText);
            Assert.AreEqual(expected, query4.Command.CommandText);
            Assert.AreEqual(expected, query5.Command.CommandText);

            Assert.AreEqual(1, query2.Command.Parameters.Count);
            Assert.AreEqual(1, query3.Command.Parameters.Count);
            Assert.AreEqual(1, query4.Command.Parameters.Count);
            Assert.AreEqual(1, query5.Command.Parameters.Count);

            Assert.AreEqual("@ProductID", query2.Command.Parameters[0].ParameterName);
            Assert.AreEqual("@ProductID", query3.Command.Parameters[0].ParameterName);
            Assert.AreEqual("@ProductID", query4.Command.Parameters[0].ParameterName);
            Assert.AreEqual("@ProductID", query5.Command.Parameters[0].ParameterName);
        }

    }
}
