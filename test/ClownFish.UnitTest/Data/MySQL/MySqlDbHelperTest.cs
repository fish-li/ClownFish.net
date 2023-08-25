using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.MySQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Data.MySQL
{
    [TestClass]
    public class MySqlDbHelperTest
    {
        [TestMethod]
        public void Test_GetDatabases()
        {
            using( DbContext db = DbContext.Create("mysql") ) {

                List<string> list1 = MySqlDbHelper.GetDatabases(db, true);
                Assert.IsTrue(list1.Contains("information_schema"));

                List<string> list2 = MySqlDbHelper.GetDatabases(db, false);
                Assert.IsFalse(list2.Contains("information_schema"));
            }
        }


        [TestMethod]
        public void Test_GetTables()
        {
            using( DbContext db = DbContext.Create("mysql") ) {

                List<string> list1 = MySqlDbHelper.GetTables(db);
                Assert.IsTrue(list1.FirstOrDefault(x => x.Is("customers")) != null);
                Assert.IsTrue(list1.FirstOrDefault(x => x.Is("products")) != null);
            }
        }

        [TestMethod]
        public void Test_GetFields()
        {
            using( DbContext db = DbContext.Create("mysql") ) {

                List<MySqlDbField> list1 = MySqlDbHelper.GetFields(db, "products");

                MySqlDbField f1 = list1.First(x => x.Name == "ProductID");
                Assert.IsNotNull(f1);
                Console.WriteLine(f1.ToJson(JsonStyle.Indented));

                Assert.AreEqual("ProductID", f1.Name);
                Assert.AreEqual("ProductID", f1.ToString());
                Assert.AreEqual("int", f1.DataType);
                Assert.IsTrue(f1.ColType.StartsWith0("int"));
                Assert.AreEqual("NO", f1.Nullable);                
                Assert.AreEqual(false, f1.IsNull);
                Assert.AreEqual("PRI", f1.Key);
                Assert.AreEqual("产品ID", f1.Comment);
                Assert.AreEqual("产品ID", f1.CommentText);
                Assert.AreEqual("auto_increment", f1.Extra);                
            }
        }
    }
}
