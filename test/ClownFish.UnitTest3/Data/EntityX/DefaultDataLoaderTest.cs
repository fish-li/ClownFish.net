namespace ClownFish.UnitTest.Data.EntityX;

[TestClass]
public class DefaultDataLoaderTest : BaseTest
{
    [TestMethod]
    public void Test_ToSingle_DataRow()
    {
        DefaultDataLoader<Customer> loader = new DefaultDataLoader<Customer>();
        string sql = "select top 1 * from Customers where CustomerID > 0 ";

        using( DbContext db = DbContext.Create() ) {

            DataTable table = db.CPQuery.Create(sql).ToDataTable();
            Assert.AreEqual(1, table.Rows.Count);

            Customer c = loader.ToSingle(table.Rows[0]);
            Assert.IsNotNull(c);
        }
    }



    [TestMethod]
    public void Test_ToSingle_DbDataReader()
    {
        DefaultDataLoader<Customer> loader = new DefaultDataLoader<Customer>();
        string sql = "select * from Customers where CustomerID > 0 ";

        using( DbContext db = DbContext.Create() ) {

            using( DbDataReader reader = db.CPQuery.Create(sql).ExecuteReader() ) {

                Customer c = loader.ToSingle(reader);
                Assert.IsNotNull(c);
            }
        }
    }


    [TestMethod]
    public void Test_ToSingle_DbDataReader_NULL()
    {
        DefaultDataLoader<Customer> loader = new DefaultDataLoader<Customer>();
        string sql = "select * from Customers where CustomerID < 0 ";

        using( DbContext db = DbContext.Create() ) {

            using( DbDataReader reader = db.CPQuery.Create(sql).ExecuteReader() ) {

                Customer c = loader.ToSingle(reader);
                Assert.IsNull(c);
            }
        }
    }

    [TestMethod]
    public void Test_ToSingle()
    {
        DefaultDataLoader<Customer> loader = new DefaultDataLoader<Customer>();
        string sql = "select top 1 * from Customers order by CustomerID ";

        using( DbContext db = DbContext.Create() ) {
            DataTable table = db.CPQuery.Create(sql).ToDataTable();

            Customer customer1 = loader.ToSingle(table.Rows[0]);
            Customer customer2 = db.CPQuery.Create(sql).ToSingle<Customer>();

            MyAssert.AreEqual(customer1, customer2);

            using( DbDataReader reader = db.CPQuery.Create(sql).ExecuteReader() ) {
                Customer customer3 = loader.ToSingle(reader);

                MyAssert.AreEqual(customer1, customer3);
            }
        }

    }


    [TestMethod]
    public void Test_ToList()
    {
        DefaultDataLoader<Customer> loader = new DefaultDataLoader<Customer>();
        string sql = "select top 5 * from Customers where CustomerID > 0 ";

        List<Customer> list1 = null;
        List<Customer> list2 = null;
        List<Customer> list3 = null;

        using( DbContext db = DbContext.Create() ) {

            using( DbDataReader reader = db.CPQuery.Create(sql).ExecuteReader() ) {
                list1 = loader.ToList(reader);
            }

            DataTable table = db.CPQuery.Create(sql).ToDataTable();
            list2 = loader.ToList(table);

            list3 = db.CPQuery.Create(sql).ToList<Customer>();
        }

        Assert.AreEqual(5, list1.Count);
        Assert.AreEqual(5, list2.Count);
        Assert.AreEqual(5, list3.Count);

        MyAssert.AreEqual(list1, list2);
        MyAssert.AreEqual(list1, list3);
    }




    [TestMethod]
    public void Test_SetPropertyValue_Error()
    {
        DefaultDataLoader<Customer> loader = new DefaultDataLoader<Customer>();
        string sql = "select top 1 * from Customers where CustomerID > 0 ";

        using( DbContext db = DbContext.Create() ) {

            DataTable table = db.CPQuery.Create(sql).ToDataTable();

            table.Columns.Remove(table.Columns["CustomerID"]);

            DataColumn col = new DataColumn("CustomerID", typeof(Guid));
            table.Columns.Add(col);

            table.Rows[0]["CustomerID"] = Guid.NewGuid();


            MyAssert.IsError<InvalidCastException>(() => {
                Customer c = loader.ToSingle(table.Rows[0]);
            });

        }
    }



}
