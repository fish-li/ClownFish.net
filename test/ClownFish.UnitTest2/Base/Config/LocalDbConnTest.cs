using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Config;
[TestClass]
public class LocalDbConnTest
{
    [TestMethod]
    public void Test_GetAppDbConfig()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = LocalDbConn.GetAppDbConfig("");
        });

        Assert.IsNull(LocalDbConn.GetAppDbConfig("sqlserver"));
        Assert.IsNull(LocalDbConn.GetAppDbConfig("mysql"));

        Assert.IsNotNull(LocalDbConn.GetAppDbConfig("s1"));
        Assert.IsNotNull(LocalDbConn.GetAppDbConfig("s2"));

        Assert.IsNotNull(LocalDbConn.GetAppDbConfig("m1"));
        Assert.IsNotNull(LocalDbConn.GetAppDbConfig("m2"));

        Assert.IsNotNull(LocalDbConn.GetAppDbConfig("pg1"));
        Assert.IsNotNull(LocalDbConn.GetAppDbConfig("dm1"));
    }

    [TestMethod]
    public void Test_GetTntDbConfig()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = LocalDbConn.GetTntDbConfig("xsql", "", "");
        });

        Assert.IsNull(LocalDbConn.GetTntDbConfig("xsql", "t23432412134", ""));

        DbConfig conf1 = LocalDbConn.GetTntDbConfig("xsql", "my57a04574bf635", "");
        DbConfig conf2 = LocalDbConn.GetTntDbConfig("xsql", "my57a197beed7d2", "_readonly");

        Assert.IsNotNull(conf1);
        Assert.IsNotNull(conf2);
        Assert.AreEqual("tenant_xsql_my57a04574bf635", conf1.Name);
        Assert.AreEqual("tenant_xsql_my57a197beed7d2_readonly", conf2.Name);

    }
}
