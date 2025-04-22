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
            Assert.AreEqual("RandGetCustomer", command.Item.CommandName);
        }

        // 【优先查找】开关没有打开，此时得到的结果和 "mysql" 一样
        using( DbContext db = DbContext.Create("sqlserver") ) {
            XmlCommand command = db.XmlCommand.Create("RandGetCustomer");
            Assert.AreEqual("RandGetCustomer", command.Item.CommandName);
        }

        // 开启【优先查找】开关
        ClownFishOptions.XmlCommandSupportMulitDbType = true;

        using( DbContext db = DbContext.Create("sqlserver") ) {
            XmlCommand command = db.XmlCommand.Create("RandGetCustomer");
            Assert.AreEqual("RandGetCustomer.SQLSERVER", command.Item.CommandName);  // 结果有变化！
        }

#if NETCOREAPP
        using( DbContext db = DbContext.Create("kingbase3") ) {   // 使用了自定义的枚举值（数字）
            XmlCommand command = db.XmlCommand.Create("RandGetCustomer");
            Assert.AreEqual("RandGetCustomer.7777", command.Item.CommandName);  // 结果有变化！
        }
#endif

        ClownFishOptions.XmlCommandSupportMulitDbType = false;


        // 关闭【优先查找】开关后，此时得到的结果和 "mysql" 连接的结果一样
        using( DbContext db = DbContext.Create("sqlserver") ) {
            XmlCommand command = db.XmlCommand.Create("RandGetCustomer");
            Assert.AreEqual("RandGetCustomer", command.Item.CommandName);
        }
    }

    [TestMethod]
    public void Test_MulitDbFind2()
    {
        ClownFishOptions.XmlCommandSupportMulitDbType = true;

        string name1, name2;

        using( DbContext db = DbContext.Create("mysql") ) {
            XmlCommand command = db.XmlCommand.Create("GetCustomerById");
            name1 = command.Item.CommandName;
        }

        using( DbContext db = DbContext.Create("sqlserver") ) {
            XmlCommand command = db.XmlCommand.Create("GetCustomerById");
            name2 = command.Item.CommandName;
        }

        // 即使打开了【优先查找】开关，但是并没有针对特定数据库的XmlCommand，所以二次查找结果相同
        Assert.AreEqual(name1, name2);

        ClownFishOptions.XmlCommandSupportMulitDbType = false;
    }
}
