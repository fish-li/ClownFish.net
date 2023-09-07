using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.EntityX;

[TestClass]
public class TableExtensionsTest : BaseTest
{
    [TestMethod]
    public void Test_DataTable_ToList()
    {
        using( ConnectionScope scope = ConnectionScope.Create() ) {
            var queryArgument = new { MaxCustomerID = 10 };
            DataTable table = CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToDataTable();

            List<Customer> list1 = table.ToList<Customer>();
            List<Customer> list2 = CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToList<Customer>();

            string json1 = list1.ToJson();
            string json2 = list2.ToJson();

            Assert.AreEqual(json1, json2);
        }

        MyAssert.IsError<ArgumentNullException>(() => {
            var x = TableExtensions.ToList<Customer>(null);
        });
    }


    [TestMethod]
    public void Test_DataTable_ToSingle()
    {
        using( ConnectionScope scope = ConnectionScope.Create() ) {
            var queryArgument = new { MaxCustomerID = 10 };
            DataTable table = CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToDataTable();

            Customer customer1 = table.Rows[0].ToSingle<Customer>();
            Customer customer2 = CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToSingle<Customer>();

            string json1 = customer1.ToJson();
            string json2 = customer2.ToJson();

            Assert.AreEqual(json1, json2);
        }

        MyAssert.IsError<ArgumentNullException>(() => {
            var x = TableExtensions.ToSingle<Customer>(null);
        });
    }

}
