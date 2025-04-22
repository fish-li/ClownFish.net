using ClownFish.Data.Xml;

namespace ClownFish.UnitTest.Data.Xml;

[TestClass]
public class XmlCommandManagerTest
{
    [TestMethod]
    public void Test_LoadFromText()
    {
        XmlCommandManager m1 = new XmlCommandManager();

        m1.LoadFromText("");  // 忽略空操作

        var x1 = m1.GetCommand("DeleteCustomer");
        Assert.IsNull(x1);

        string xml = File.ReadAllText("App_Data/XmlCommand/Test1.config", Encoding.UTF8);
        m1.LoadFromText(xml);

        var x2 = m1.GetCommand("DeleteCustomer");
        Assert.IsNotNull(x2);
    }

    [TestMethod]
    public void Test_LoadFromDirectory()
    {
        XmlCommandManager m1 = new XmlCommandManager();

        MyAssert.IsError<ArgumentNullException>(() => {
            m1.LoadFromDirectory("");
        });

        MyAssert.IsError<DirectoryNotFoundException>(() => {
            m1.LoadFromDirectory("xxxxxxxxxxxxxxxxxx");
        });


        var x1 = m1.GetCommand("DeleteCustomer");
        Assert.IsNull(x1);

        m1.LoadFromDirectory("App_Data/XmlCommand");

        var x2 = m1.GetCommand("DeleteCustomer");
        Assert.IsNotNull(x2);
    }

    [TestMethod]
    public void Test_MulitDbFind()
    {
        using( DbContext db = DbContext.Create("mysql") ) {
            XmlCommand command = db.XmlCommand.Create("RandGetCustomer");
            string sql = command.Item.CommandText.Value.Trim();

            Assert.IsTrue(sql.Contains(" limit 1"));
            Assert.IsFalse(sql.Contains(" top 1"));
        }

        using( DbContext db = DbContext.Create("sqlserver") ) {
            XmlCommand command = db.XmlCommand.Create("RandGetCustomer");
            string sql = command.Item.CommandText.Value.Trim();

            // 此时得到的结果和 "mysql" 连接的结果一样
            Assert.IsTrue(sql.Contains(" limit 1"));
            Assert.IsFalse(sql.Contains(" top 1"));
        }


        ClownFishOptions.XmlCommandSupportMulitDbType = true;

        using( DbContext db = DbContext.Create("sqlserver") ) {
            XmlCommand command = db.XmlCommand.Create("RandGetCustomer");
            string sql = command.Item.CommandText.Value.Trim();

            // 结果有变化！
            Assert.IsTrue(sql.Contains(" top 1"));
            Assert.IsFalse(sql.Contains(" limit 1"));
        }

        ClownFishOptions.XmlCommandSupportMulitDbType = false;

        using( DbContext db = DbContext.Create("sqlserver") ) {
            XmlCommand command = db.XmlCommand.Create("RandGetCustomer");
            string sql = command.Item.CommandText.Value.Trim();

            // 此时得到的结果和 "mysql" 连接的结果一样
            Assert.IsTrue(sql.Contains(" limit 1"));
            Assert.IsFalse(sql.Contains(" top 1"));
        }
    }

    [TestMethod]
    public void Test_MulitDbFind2()
    {
        ClownFishOptions.XmlCommandSupportMulitDbType = true;

        string sql1, sql2;

        using( DbContext db = DbContext.Create("mysql") ) {
            XmlCommand command = db.XmlCommand.Create("GetCustomerById");
            sql1 = command.Item.CommandText.Value.Trim();
        }

        using( DbContext db = DbContext.Create("sqlserver") ) {
            XmlCommand command = db.XmlCommand.Create("GetCustomerById");
            sql2 = command.Item.CommandText.Value.Trim();
        }

        Assert.AreEqual(sql1, sql2);

        ClownFishOptions.XmlCommandSupportMulitDbType = false;
    }
}
