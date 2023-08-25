#if TEST_DM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.MultiDB.DaMeng;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.DaMeng;
[TestClass]
public class DaMengClientProviderTest2
{
    [TestMethod]
    public void Test2()
    {
        using DbContext dbContext = DbContext.Create("dm");

        Category c1 = new Category { CategoryName = "手机" };
        bool flag1 = dbContext.Entity.Insert(c1, InsertOption.AllFields | InsertOption.IgnoreDuplicateError) > 0; // c1.Insert2(dbContext);

        Category c2 = new Category { CategoryName = "手机" };
        bool flag2 = dbContext.Entity.Insert(c2, InsertOption.AllFields | InsertOption.IgnoreDuplicateError) > 0; // c2.Insert2(dbContext);

        // CategoryName 字段上有 唯一索引
        // 至少有一个插入语句不能成功执行
        Assert.IsTrue(flag1 == false || flag2 == false);
    }

    [TestMethod]
    public void Test_ChangeDatabase()
    {
        DaMengClientProvider provider = new DaMengClientProvider();
        using DbContext dbContext = DbContext.Create("dm");

        string sql = null;
        try {
            provider.ChangeDatabase(dbContext, "db123");
        }
        catch( DbExceuteException ex ) {
            sql = ex.Command.CommandText;
        }
        Assert.AreEqual("set schema \"db123\"", sql);
    }

}

#endif
