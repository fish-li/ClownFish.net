namespace ClownFish.UnitTest.Data.Command;

[TestClass]
public class XmlCommandFactoryTest
{
    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            XmlCommandFactory factory = new XmlCommandFactory(null);
        });

    }


    [TestMethod]
    public void Test()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            XmlCommandFactory factory = new XmlCommandFactory(dbContext);

            var query1 = factory.Create("DeleteCustomer");

            var args2 = new { ProductID = 2 };
            var query2 = factory.Create("DeleteCustomer", args2);

            Hashtable table = new Hashtable();
            table["CustomerID"] = 2;
            var query3 = factory.Create("DeleteCustomer", table);

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["CustomerID"] = 2;
            var query4 = factory.Create("DeleteCustomer", dictionary);


            string expected = "delete from Customers where CustomerID = @CustomerID";
            Assert.AreEqual(expected, query1.Command.CommandText.Trim());
            Assert.AreEqual(expected, query2.Command.CommandText.Trim());
            Assert.AreEqual(expected, query3.Command.CommandText.Trim());
            Assert.AreEqual(expected, query4.Command.CommandText.Trim());

            Assert.AreEqual(1, query2.Command.Parameters.Count);
            Assert.AreEqual(1, query3.Command.Parameters.Count);
            Assert.AreEqual(1, query4.Command.Parameters.Count);

            Assert.AreEqual("@CustomerID", query2.Command.Parameters[0].ParameterName);
            Assert.AreEqual("@CustomerID", query3.Command.Parameters[0].ParameterName);
            Assert.AreEqual("@CustomerID", query4.Command.Parameters[0].ParameterName);
        }

    }

}
