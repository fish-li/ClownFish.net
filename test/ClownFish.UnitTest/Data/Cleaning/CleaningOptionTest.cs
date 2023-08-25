using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.Cleaning;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Cleaning;
[TestClass]
public class CleaningOptionTest
{
    [TestMethod]
    public void Test1()
    {
        CleaningOption option = new CleaningOption();

        option.TestInstanceGetSet();

        MyAssert.IsError<ArgumentNullException>(() => {
            option.Validate();  // throw new ArgumentNullException(nameof(DbConfig));
        });

        option.DbConfig = new DbConfig();
        MyAssert.IsError<ArgumentNullException>(() => {
            option.Validate();  // throw new ArgumentNullException(nameof(TableName));
        });

        option.TableName = "test";
        MyAssert.IsError<ArgumentNullException>(() => {
            option.Validate();  // throw new ArgumentNullException(nameof(TimeFieldName));
        });

        option.TimeFieldName = "createtime";
        option.Validate();  // 可以成功调用！

        Assert.AreEqual(24, option.HoursAgo);
        Assert.AreEqual(500, option.BatchRows);
        Assert.AreEqual(20, option.RetryCount);
        Assert.AreEqual(200, option.DbTimeout);
    }


}
