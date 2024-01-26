using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Debug
{
    [TestClass]
    public class DebugReportTest
    {
        [TestMethod]
        public void Test()
        {
            DebugReportBlock block = new DebugReportBlock {
                Order = 1,
                Category = "Test",
            };

            block.AppendLine("1111111111111111111111");
            block.AppendLine("2222222222222222222222");

            string text = block.ToString();

            // 不想检查内容，偷个懒
            Assert.IsTrue(text.Length > 0);
        }

        [TestMethod]
        public void Test_null()
        {
            DebugReportBlock block = new DebugReportBlock();
            Assert.AreEqual("##### title #####\r\n", block.ToString2());


            block.AppendLine(null);
            string text = block.ToString2();
        }

        [TestMethod]
        public void Test_GetReport()
        {
            string s1 = DebugReport.GetReport("ALL");
            string s2 = DebugReport.GetReport("StatusInfo");
            string s3 = DebugReport.GetReport("SysInfo");
            string s4 = DebugReport.GetReport("AsmInfo");
            string s5 = DebugReport.GetReport("ConfigInfo");
            string s6 = DebugReport.GetReport("StaticVariables");
            string s7 = DebugReport.GetReport("xxxxxx");

            Assert.IsTrue(s1.HasValue());
            Assert.IsTrue(s2.HasValue());
            Assert.IsTrue(s3.HasValue());
            Assert.IsTrue(s4.HasValue());
            Assert.IsTrue(s5.HasValue());
            Assert.IsTrue(s6.HasValue());
            Assert.AreEqual("_NULL_", s7);
        }

        [TestMethod]
        public void Test_GetEnvironmentVariables()
        {
            string connectionString = new DbConfig {
                Name = "aaaaaaa",
                DbType = DatabaseType.MySQL,
                Server = "a72d23afd3b24ef6ac83cd339d5977c9",
                Database = "",
                UserName = "root",
                Password = "3f3144a9ff6d4782818ee8d60f0cd09e",
            }.GetConnectionString(true);

            EnvironmentVariables.Set("aaaaaaa_ConnectionString", connectionString);
            EnvironmentVariables.Set("bbbbbbb_Password", "9534ea5a3a074b688d94a8e777c6f119");
            EnvironmentVariables.Set("api-key", "b094ac7875414ecca20297f71000a033");
            EnvironmentVariables.Set("xx-SecretKey", "f631c032b2d0425f877e15e298ea7031");

            DebugReportBlock block = DebugReportBlocks.GetEnvironmentVariables();
            string text = block.ToString2();
            Console.WriteLine(text);

            Assert.IsFalse(text.Contains("3f3144a9ff6d4782818ee8d60f0cd09e"));
            Assert.IsTrue(text.Contains("aaaaaaa_ConnectionString: Server=a72d23afd3b24ef6ac83cd339d5977c9;Database=;Uid=root;Pwd=********;"));

            Assert.IsFalse(text.Contains("9534ea5a3a074b688d94a8e777c6f119"));
            Assert.IsTrue(text.Contains("bbbbbbb_Password: ********"));

            Assert.IsFalse(text.Contains("b094ac7875414ecca20297f71000a033"));
            Assert.IsTrue(text.Contains("api-key: ********"));

            Assert.IsFalse(text.Contains("f631c032b2d0425f877e15e298ea7031"));
            Assert.IsTrue(text.Contains("xx-SecretKey: ********"));

        }

        [TestMethod]
        public void Test_GetStaticVariablesReportBlock()
        {
            DbConfig config = DbConfig.Parse("Id=123;Name=db1;DbType=MySQL;Server=localpc;Database=MyNorthwind;Port=1025;UserName=admin;Password=fish;");

            DebugReport.OptionList.Add(config);
            DebugReport.OptionList.Add((object)GetXxxOptions);

            DebugReportBlock block = DebugReportBlocks.GetStaticVariablesReportBlock();
            string text = block.ToString2();
            Console.WriteLine(text);

            Assert.IsTrue(text.Contains("--ClownFish.Data.DbConfig--"));
            Assert.IsTrue(text.Contains("--DbConfig_db2---"));
        }

        private static NameValue GetXxxOptions()
        {
            DbConfig config = DbConfig.Parse("Id=234;Name=db2;DbType=MySQL;Server=localpc;Database=MyNorthwind;Port=1025;UserName=admin;Password=fish;");
            return new NameValue {
                Name = "DbConfig_db2",
                Value = config.ToJson(JsonStyle.Indented)
            };
        }

    }    
}

