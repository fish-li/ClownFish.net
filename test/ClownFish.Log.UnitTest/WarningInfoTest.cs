using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Log.UnitTest
{
    [TestClass]
    public class WarningInfoTest : TestBase
    {
        [TestMethod]
        public void Test_Write_WarningInfo()
        {
            WarningInfo info = WarningInfo.Create("当前发生了XXX，告警告警。");
            LogHelper.SyncWrite(info);

            // 写入成功就认为通过
        }

    }
}
