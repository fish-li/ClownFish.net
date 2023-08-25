using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class DataTableExtensionsTest
    {
        [TestMethod]
        public void Test1()
        {
            DataTable table = new DataTable("row");
            table.Columns.Add(new DataColumn("id", typeof(int)));
            table.Columns.Add(new DataColumn("name", typeof(string)));

            table.Rows.Add(1, "aa");
            table.Rows.Add(2, "bb");

            string xml = table.TableToXml();
            //Console.WriteLine(xml);

            string xml2 = xml.Replace('\r', ' ').Replace('\n', ' ').Replace(" ", "");
            Assert.IsTrue(xml2.Contains("</xs:schema><row><id>1</id><name>aa</name></row><row><id>2</id><name>bb</name></row></ds>"));


            DataTable table2 = ClownFish.Base.DataTableExtensions.XmlToTable(xml);

            Assert.AreEqual(2, table2.Columns.Count);
            Assert.AreEqual("id", table2.Columns[0].ColumnName);
            Assert.AreEqual(typeof(int), table2.Columns[0].DataType);
            Assert.AreEqual("name", table2.Columns[1].ColumnName);
            Assert.AreEqual(typeof(string), table2.Columns[1].DataType);

            Assert.AreEqual(2, table2.Rows.Count);
        }
    }
}
